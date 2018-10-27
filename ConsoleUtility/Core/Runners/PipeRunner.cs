using Utility.Core.Commandables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Tools;

namespace Utility.Core.Runners
{
    public class PipeRunner
    {
        #region Properties

        public PipeCommandable Pipe { get; set; }

        public BinderService Binder { get; set; }

        public Action<string, Exception> OnException { get; set; }

        public Action<dynamic> OnValue { get; set; }

        #endregion

        #region Constructors

        public PipeRunner()
        {

        }

        #endregion

        #region Public Methods

        public async Task Run(Func<string, dynamic> GetAllocated)
        {
            var startPipe = Pipe.Commandables.First();

            if (startPipe.StartsWithVarName)
            {
                var reference = GetAllocated(startPipe.VarName);

                if (reference != null)
                {
                    await InternalRun(reference);
                }
                else
                {
                    await InternalRun();
                }
            }
            else
            {
                await InternalRun();
            }
        }

        #endregion

        #region Private Method

        private async Task InternalRun(dynamic lastArgument = default(dynamic))
        {
            var pipeCount = 0;

            bool callWithPipeArgs = true;

            bool throwEx = false;

            if (lastArgument != default(dynamic))
                callWithPipeArgs = false;

            dynamic[] args = null;

            foreach (var pipeCom in Pipe.Commandables)
            {
                try
                {
                    args = callWithPipeArgs ? MyExtensions.AsArrayDynamic(pipeCom.Arguments) : MyExtensions.AsArrayDynamic(lastArgument);

                    lastArgument = await Binder.DynamicGenInvokeFunc(pipeCom.LookUpName, args);

                    callWithPipeArgs = false;
                }
                catch (Exception ex)
                {
                    //Exception Stack Trace possible spot, each runner has a exception trace
                    // Is the 'OnException' an IO Mutation / Monad?
                    //First exception leaves the run ?

                    OnException?.Invoke(pipeCom.LookUpName, ex);
                    throwEx = true;
                }

                pipeCount++;
            }

            if(!throwEx)
                OnValue?.Invoke(lastArgument);
        }

        #endregion
    }
}
