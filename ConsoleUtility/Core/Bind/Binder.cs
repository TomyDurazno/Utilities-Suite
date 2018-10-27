using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Tools;
using static Utility.Core.Attributes.InvokerAttributes;

namespace Utility.Core
{
    public class BinderService
    {
        //Debe haber
        //MakeDelegateByType para
        // -> Action
        // -> Action<T>
        // Func<T>
        // Func<T,K>
        // Task<T>
        // Task<T,K>

        public Delegate MakeDelegateByMethodTypes(object instance, MethodInfo method)
        {
            try
            {
                Type delegateType;
                Type delegateTypeByTypes = null;
                var argType = method.GetParameters().FirstOrDefault()?.ParameterType;
                var returnType = method.ReturnType;

                if (argType == null && returnType == typeof(void))
                {
                    delegateTypeByTypes = typeof(Action);
                }

                if (argType == null && returnType != typeof(void))
                {
                    delegateType = typeof(Func<>);
                    delegateTypeByTypes = delegateType.MakeGenericType(returnType);
                }

                if (argType != null && returnType == typeof(void))
                {
                    delegateType = typeof(Action<>);
                    delegateTypeByTypes = delegateType.MakeGenericType(argType);
                }

                if (argType != null && returnType != typeof(void))
                {
                    delegateType = typeof(Func<,>);
                    delegateTypeByTypes = delegateType.MakeGenericType(argType, returnType);
                }

                //'instance' es la instancia del 'Invoker', 'method' el objeto 'methodInfo' asociados
                return Delegate.CreateDelegate(delegateTypeByTypes, instance, method);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// El Corazón de DynamicGenInvoke
        /// TODO: potenciar el 'DynamicInvoke' para múltiples tipos
        /// MAYBE: Pierde el objeto de retorno
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task DynamicCall(Delegate result, object[] arguments)
        {
            if (result.Method.ReturnType == typeof(Task<>))
            {
                await ((Task<dynamic>)result.DynamicInvoke(arguments));
            }

            if (result.Method.ReturnType == typeof(Task))
            {
                await ((Task)result.DynamicInvoke(arguments));
            }

            if(result.Method.ReturnType == typeof(void))
            {
                await Task.Run(() => result.DynamicInvoke(arguments));            
            }

            if (result.GetType() == typeof(Action<>))
            {
                await Task.Run(() => result.DynamicInvoke(arguments));
            }
        }

        public async Task DynamicGenInvoke(string name, object[] arguments, bool directArguments = false)
        {
            await DynamicCall(DelegateByName(name), !directArguments ? new object[] { arguments } : arguments);
        }

        public Delegate DelegateByName(string name)
        {
            var obj = Reflector.MakeInstancesByAttribute<Invoker>()
                               .Where(o => Reflector.GetAttribute<Invoker>(o).Name.ToLower() == name)
                               .First();

            var method = Reflector.GetMethodsWithAttribute<InvokerCaller>(obj)
                                  .First();

            return MakeDelegateByMethodTypes(obj, method);
        }

        public async Task<dynamic> DynamicGenInvokeFunc(string name, dynamic[] arguments)
        {
            Delegate delegateCast = DelegateByName(name);

            var type = delegateCast.Method.ReturnType;

            if(typeof(IAsyncResult).IsAssignableFrom(type))
            {
                var taskToBeManaged = (IAsyncResult)delegateCast.DynamicInvoke(arguments);

                await taskToBeManaged.AsyncWaitHandle.WaitOneAsync();

                var obj = taskToBeManaged.GetType().GetProperty("Result").GetValue(taskToBeManaged);

                return await Task.FromResult(obj);
            }
            else
            {
                return await Task.Run(() => delegateCast.DynamicInvoke(arguments));
            }
        }
    }
}
