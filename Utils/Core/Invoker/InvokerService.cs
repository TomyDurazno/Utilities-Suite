using Utility.Core.Commandables;
using Utility.Core.Expressions;
using Utility.Core.Runners;
using Utility.Core.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Utility.Core.Tokens;
using Utility.Tools;
using Utility.Core.Attributes;
using System.Text;

namespace Utility.Core
{
    /*
        This is the class to consume to use the functionalities of 'Dynamic Invoker' solution

    */

    public class InvokerService
    {
        #region Properties

        bool ForceGlobalExit { get; set; }

        bool _force { get; set; }

        string _name;    

        string ServiceName
        {
            get
            {
                return _name;
            }
            set
            {
                _name =  string.Format("{0}_{1}", value, Guid.NewGuid().ToString().Take(4).Project(string.Concat));
            }
        }

        string Status { get; set; }

        StreamProvider StreamProvider { get; set; }

        Func<string> reader { get { return StreamProvider.Reader; }}

        Func<string,string> writer { get { return StreamProvider.Writer; }}

        public Action<string> PostRun { get; }

        #endregion

        #region Constructors

        public InvokerService(string serviceName, StreamProvider provider, bool forceExit = false)
        {
            ServiceName = serviceName;
            StreamProvider = provider;
            _force = forceExit;
        }

        public InvokerService(string serviceName, StreamProvider provider, bool forceExit = false, Action<string> postRun = null) : this (serviceName, provider, forceExit = false)
        {
            ServiceName = serviceName;
            StreamProvider = provider;
            _force = forceExit;
            PostRun = postRun;
        }
        #endregion

        #region Methods

        #region Private

        private async Task InsideRun()
        {            
            bool NotExit = true;

            //Thread safe Heap ??
            // Maybe: is this an 'Object pool"?
            var Heap = new Lazy<ConcurrentDictionary<string, dynamic>>(() => new ConcurrentDictionary<string, dynamic>(), true);

            var binderService = new BinderService();

            #region Eval Loop

            while (NotExit && !ForceGlobalExit)
            {
                var line = reader(); 

                if (!string.IsNullOrEmpty(line))
                {
                    var input = TokenStreamer.MakeTokenInput(line);

                    var stream = input.GetStream();

                    //check logger
                    //stream.GetStringsRepresentation().Project(sr => string.Format("check: {0}", sr)).Project(writer);

                    if (!input.IsCommandInput)
                    {
                        writer("Command inexistent");
                    }
                    else
                    {
                        switch (input.Command)
                        {
                            //Review this functionality, now it writes the result but doesnt handle very good the arguments !
                            case Command.Call:

                                #region Call Implementation

                                var com2 = new CallCommandable(stream, 1);

                                try
                                {
                                    var result = await binderService.DynamicGenInvokeFunc(com2.LookUpName, com2.Arguments);
                                    writer(result);
                                }
                                catch (Exception ex)
                                {
                                    writer(string.Format("Exception when called: ", com2.LookUpName));
                                    writer(ex.ToString());
                                }

                                break;

                            #endregion

                            case Command.Seq:

                                #region Seq Implementation

                                var seqCom = new SeqComandable(stream);

                                var runners = seqCom.PipeCommandables.Select(pc => new PipeRunner() { Pipe = pc, Binder = new BinderService() });

                                var tasks = runners.Select(r => r.Run(lookup => Heap.Value.Where(o => o.Key == lookup.ToLower()).FirstOrDefault().Value));

                                await Task.WhenAll(tasks);

                                break;

                            #endregion

                            case Command.Pipe:

                                #region Pipe Implementation

                                var pipe = new PipeCommandable(stream);

                                var runner = new PipeRunner()
                                {
                                    Pipe = pipe,
                                    Binder = binderService,
                                    OnException = (ex, lookup) => writer(string.Format("Action {0} failed with {1}", lookup, ex.ToString()))
                                };

                                await runner.Run(lookup => Heap.Value.Where(o => o.Key == lookup.ToLower()).FirstOrDefault().Value);

                                break;

                            #endregion

                            case Command.Types:

                                #region DEPRECATED

                                //TODO: ENHANCE THE TO TYPE FOR A DYNAMIC WAY
                                //var callCom = new CallCommandable(stream, 0);

                                //var comodin = callCom.IsComodin();

                                //var invoc = Invocables.Value.Where(inv => comodin || callCom.Arguments.Contains(inv.Name.ToLower())).ToList();

                                //Action<Invocable> writer = i => new[] { "", i.ToString(), i.Binder.TypesToString() }.Enumerates(Console.WriteLine);

                                //invoc.ForEach(writer);

                                #endregion

                                break;

                            case Command.Var:

                                #region Var Implementation

                                var varCom = new VarCommandable(stream.PlainTokens);

                                var name = varCom.Name;

                                if (!varCom.IsVarNameConvention)
                                {
                                    writer(string.Format("var name should start with: {0}", TokenConfigs.VarNameStart));
                                    break;
                                }

                                if (Heap.Value.Any(o => o.Key == name))
                                {
                                    writer(string.Format("'{0}' already defined", name));
                                    break;
                                };

                                var pipesAssign = new PipeCommandable(varCom.Statement);

                                var runnerVar = new PipeRunner()
                                {
                                    Pipe = pipesAssign,
                                    Binder = binderService,
                                    OnException = (lookup, ex) => writer(string.Format("Action {0} failed with {1}", lookup, ex.ToString())),
                                    OnValue = val => Heap.Value.TryAdd(name, val)
                                };

                                await runnerVar.Run(lookup => Heap.Value.Where(o => o.Key == lookup.ToLower()).FirstOrDefault().Value);

                                break;

                            #endregion

                            case Command.Heap:

                                #region Heap Implementation

                                var heapCom = new HeapCommandable(stream.PlainTokens);

                                switch (heapCom.Operator.ToLower())
                                {
                                    case "clear":

                                        Heap.Value.Clear();
                                        writer("Heap cleared");

                                        break;

                                    case "remove":

                                        dynamic outVar;
                                        var dic = new Dictionary<string, bool>();

                                        heapCom.Arguments.Enumerates(obj => {

                                            if (Heap.Value.Any(o => o.Key == obj))
                                                dic.Add(obj, Heap.Value.TryRemove(obj, out outVar));
                                            else
                                                writer(string.Format("'{0}' not defined", obj));
                                        });

                                        dic.Enumerates(e => e.Value ? writer(string.Format("{0} removed", e.Key)) : writer(string.Format("error removing: {0} ", e.Key)));

                                        break;

                                    default:

                                        if (Heap.Value.Any())
                                            Heap.Value.Enumerates(o => writer(o.ToString()));
                                        else
                                            writer("Heap is empty");

                                        break;
                                }

                                break;

                            #endregion

                            case Command.Commands:

                                #region Commands Implementation

                                MyUtils.GetEnums<Command>().Select(e => e.ToString()).Enumerates(writer);

                                break;

                            #endregion

                            case Command.Clear:

                                //PENDING DEFINITION
                                Console.Clear();
                                break;

                            case Command.Exit:

                                #region Exit Command

                                writer("Press Y to exit");

                                if (reader().ToLower() == "y")
                                {
                                    NotExit = false;
                                }

                                break;

                            #endregion

                            case Command.Invocables:

                                #region Implementation

                                var arg = stream.PlainTokens
                                                .Skip(1)
                                                .Where(t => !t.IsEmpty)
                                                .FirstOrDefault();

                                var filter = MyUtils.MakeFunc((string s) => true);

                                if (arg != null)
                                    filter = s => s.IndexOf(arg.Value, StringComparison.InvariantCultureIgnoreCase) >= 0;

                                Reflector.GetAttributesInAssembly<InvokerAttributes.Invoker>()
                                         .OrderBy(a => a.Name)
                                         .Where(a => filter(a.Name))
                                         .Aggregate(new StringBuilder(), (builder, a) => builder.AppendLine(string.Format(a.Description != null ? "{0}: {1}" : "{0}", a.Name, a.Description)))
                                         .ToString()
                                         .Project(writer);

                                #endregion

                                break;    

                            default:

                                #region Default

                                var first = stream.PlainTokens.First();
                                
                                //IsReserved word ?
                                //Is Var instance
                                if (Heap.IsValueCreated)
                                {
                                    var reference = Heap.Value.Where(o => o.Key == first.Value).FirstOrDefault();

                                    if (Heap.Value.Any(o => o.Key == first.Value.ToLower()))
                                    {
                                        writer(MyExtensions.ToNullString(reference)); //val.Value
                                    }
                                    else
                                    {
                                        writer(string.Format("'{0}' is not defined", first.Value));
                                    }
                                }

                               break;

                               #endregion
                        }
                    }
                }

                ForceGlobalExit = _force;
            }

            #endregion
        }

        #endregion

        #region Public

        public async Task Run()
        {
            var watch = new Stopwatch();
            watch.Start();

            await InsideRun().ContinueWith(task =>
            {
                watch.Stop();

                Status = string.Format("InvokerService: {0} run for {1}", ServiceName, watch.Elapsed);

                if (PostRun != null)
                    PostRun(Status);
            });
        }
       
        #endregion

        #endregion
    }
}
