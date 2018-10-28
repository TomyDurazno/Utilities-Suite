using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Tools;

namespace Dynamic_Invoker.Core.Pool
{
    /*
        Its used to allocate the variables defined by 'var',
        Maybe: can it be used to handle 'Invocables' instances to be reused?  
    */

    public class ObjectPool
    {
        #region Private

        private Lazy<ConcurrentDictionary<string, dynamic>> Heap { get; set; }

        #endregion

        #region Public properties

        public bool IsCreated => Heap.IsValueCreated;

        #endregion

        #region Constructor

        public ObjectPool()
        {
            Heap = new Lazy<ConcurrentDictionary<string, dynamic>>(() => new ConcurrentDictionary<string, dynamic>(), true);
        }

        #endregion

        #region Public Methods

        public dynamic GetAllocated(string lookup)
        {
            return Heap.Value.Where(o => o.Key == lookup.ToLower()).FirstOrDefault().Value;
        }

        public bool IsAny(string name = null)
        {
            var filter = MyUtils.MakeFunc((string s) => true);

            if (name != null)
                filter = key => key == name;

            return Heap.Value.Any(o => filter(o.Key));
        }

        public IEnumerable<KeyValuePair<string,dynamic>> GetVars()
        {
            return Heap.Value;
        }

        public void TryAdd(string name, dynamic val)
        {
            Heap.Value.TryAdd(name, val);
        }

        public bool TryRemove(string key, out dynamic outVar)
        {
            return Heap.Value.TryRemove(key, out outVar);
        }

        public void Clear()
        {
            Heap.Value.Clear();
        }

        #endregion
    }
}
