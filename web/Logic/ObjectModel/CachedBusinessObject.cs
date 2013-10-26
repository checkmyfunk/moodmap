using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Logic.ObjectModel
{
    [NoCache]
    public class CachedBusinessObject<EntityType, IdType> : BusinessObjectInternal<EntityType, IdType>
        where EntityType : CachedBusinessObject<EntityType, IdType>, new()
        where IdType : IComparable
    {

        static CachedBusinessObject()
        {
            MemoryAdapter.AddClear(ClearCache);
        }

        internal static void ClearCache()
        {
            if (lazy != null)
                lazy.Clear();
            cache = null;
        }


        internal static object lockObj = new object();
        private static Dictionary<IdType, EntityType> lazy = new Dictionary<IdType, EntityType>();
        internal static bool CacheLoaded { get { lock (lockObj) { return cache != null; } } }

        private static Dictionary<IdType, EntityType> cache;
        protected static Dictionary<IdType, EntityType> Cache
        {
            get
            {
                lock (lockObj)
                {
                    return lazy ?? (cache ?? (cache = ReloadCache()));
                }
            }
        }

        private static Dictionary<IdType, EntityType> ReloadCache()
        {
            return BusinessObjectInternal<EntityType, IdType>.AsQueryable().ToDictionary(obj => obj.Id);
        }

        protected override IBusinessObject InternalSave()
        {
            lock (lockObj)
            {
                MemoryAdapter.AddClearOnAbort(ClearCache);
                base.InternalSave();
                Cache[Id] = this;
                return this;
            }
        }

        protected override IBusinessObject InternalUpdate()
        {
            lock (lockObj)
            {
                MemoryAdapter.AddClearOnAbort(ClearCache);
                base.InternalUpdate();
                (lazy == null ? cache : lazy)[Id] = this;
                return this;
            }
        }

        protected override void InternalDelete()
        {
            lock (lockObj)
            {
                MemoryAdapter.AddClearOnAbort(ClearCache);
                base.InternalDelete();
                Cache.Remove(Id);
            }
        }

        /*
        protected virtual EntityType Clone()
        {
            var t = this.GetType();
            var i = Activator.CreateInstance(t) as EntityType;
            foreach(var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (p.GetCustomAttribute<NotMappedAttribute>() == null && BusinessObjectAdapter.TypeSupported(p.PropertyType) && p.CanWrite && p.CanRead)
                {
                    p.SetValue(i, p.GetValue(this));
                }
            }
            return i;
        }
        */

        public static new EntityType GetById(IdType id)
        {
            lock (lockObj)
            {
                EntityType value;
                if (Cache.TryGetValue(id, out value))
                {
                    //value = value.Attach();
                }
                else
                {
                    value = BusinessObjectAdapter.Current.GetById<EntityType, IdType>(id);
                    if (value != null)
                        Cache[id] = value;
                }
                return value;
            }
        }

        public static new List<EntityType> GetAll(Expression<Func<EntityType, bool>> selector = null)
        {
            lock (lockObj)
            {
                lazy = null;
                return selector == null ? Cache.Values.ToList() : Cache.Values.Where(selector.Compile())
                    //.Select(e => e.Attach())
                    .ToList();
            }
        }

        public static new EntityType GetFirst(Expression<Func<EntityType, bool>> selector = null)
        {
            lock (lockObj)
            {
                var res = selector == null ? Cache.Values.FirstOrDefault() : Cache.Values.Where(selector.Compile()).FirstOrDefault();
                if (res == null && lazy != null)
                {
                    res = BusinessObjectAdapter.Current.GetFirst<EntityType, IdType>(selector);
                    if (res != null)
                    {
                        lazy[res.Id] = res;
                    }
                }
                return res;
                //return res == null ? null : res.Attach();
            }
        }

    }

    public class CachedBusinessObject<EntityType> : CachedBusinessObject<EntityType, int>
        where EntityType : CachedBusinessObject<EntityType, int>, new()
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public static EntityType GetById(int? id)
        {
            return id.HasValue ? CachedBusinessObject<EntityType, int>.GetById(id.Value) : null;
        }
    }

}
