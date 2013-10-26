namespace Logic.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Transactions;



    public class MemoryAdapter : BusinessObjectAdapter
    {
        class Data
        {
            internal Dictionary<string, object> Props;

            public void ReloadObject<IdType>(BusinessObjectBase<IdType> obj)
                where IdType : IComparable
            {
                Type t = obj.BaseType;
                if (!ObjCache.ContainsKey(t))
                    ObjCache[t] = new Dictionary<object, object>();
                ObjCache[t][Props] = ((MemoryAdapter)BusinessObjectAdapter.Current).ReloadObject<IdType>(obj, Props);
            }

            public T GetObject<T, IdType>(T obj = null)
                where T : BusinessObjectInternal<T, IdType>, new()
                where IdType : IComparable
            {
                Type t = typeof(T);
                if (!ObjCache.ContainsKey(t))
                    ObjCache[t] = new Dictionary<object, object>();
                if (!ObjCache[t].ContainsKey(Props) || obj != null)
                {
                    ObjCache[t][Props] = ((MemoryAdapter)BusinessObjectAdapter.Current).LoadObject<T, IdType>(obj, Props);
                }
                return ObjCache[t][Props] as T;
            }
        }

        bool supportTransactions = true;
        public bool SupportTransactions
        {
            get { return supportTransactions; }
            set { supportTransactions = value; }
        }

        [ThreadStatic]
        private static Dictionary<Type, Dictionary<object, object>> objCache;
        static Dictionary<Type, Dictionary<object, object>> ObjCache
        {
            get
            {
                return objCache ?? (objCache = new Dictionary<Type, Dictionary<object, object>>());
            }
        }


        private static ManualResetEvent sem = new ManualResetEvent(true);
        private static readonly Dictionary<Transaction, Dictionary<Type, Dictionary<object, Data>>> cacheT = new Dictionary<Transaction, Dictionary<Type, Dictionary<object, Data>>>();
        private static readonly Dictionary<Type, Dictionary<object, Data>> cache = new Dictionary<Type, Dictionary<object, Data>>();
        private static readonly Dictionary<Type, int> idCache = new Dictionary<Type, int>();

        private static Type BaseType(Type type)
        {
            while (type.BaseType != null && (!type.IsGenericType || !type.BaseType.IsGenericType || type.BaseType.GetGenericTypeDefinition() != typeof(BusinessObjectBase<>)))
            {
                type = type.BaseType;
            }
            return type;
        }

        private Dictionary<object, Data> GetCache(Type type)
        {
            lock (cache)
            {
                type = BaseType(type);
                var cacheForClass = cache[type] = cache.ContainsKey(type) ? cache[type] : new Dictionary<object, Data>();
                if (SupportTransactions && Transaction.Current != null)
                {
                    if (!cacheT.ContainsKey(Transaction.Current))
                    {
                        sem.WaitOne();
                        sem.Reset();
                        Transaction.Current.TransactionCompleted += transactionCompleted;
                        cacheT[Transaction.Current] = Clone(cache);
                    }
                    cacheForClass = cacheT[Transaction.Current].ContainsKey(type)
                        ? cacheT[Transaction.Current][type]
                        : cacheT[Transaction.Current][type] = new Dictionary<object, Data>();
                }
                return cacheForClass;
            }
        }

        private Dictionary<Type, Dictionary<object, Data>> Clone(Dictionary<Type, Dictionary<object, Data>> cache)
        {
            var res = new Dictionary<Type, Dictionary<object, Data>>();
            foreach (var item in cache)
            {
                res[item.Key] = new Dictionary<object, Data>(item.Value);
            }
            return res;
        }

        private void transactionCompleted(object sender, TransactionEventArgs e)
        {
            if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
            {
                cache.Clear();
                foreach (var item in cacheT[e.Transaction])
                {
                    cache[item.Key] = item.Value;
                }
            }
            cacheT.Remove(e.Transaction);
            sem.Set();
        }

        protected override void InternalSave<IdType>(BusinessObjectBase<IdType> obj)
        {
            var cacheForClass = GetCache(obj.BaseType);
            lock (cacheForClass)
            {
                if (obj.HasIdentityId)
                    UpdateId(obj as BusinessObjectBase<int>);
                else
                    if (cacheForClass.ContainsKey(obj.Id))
                        throw new ApplicationException(string.Format("Duplicate Id ({0}) for {1} class", obj.Id, obj.BaseType.Name));
                cacheForClass[obj.Id] = new Data { Props = StoreProperties(obj) };
            }
        }


        private static Dictionary<string, object> StoreProperties(object obj)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            var props = obj.GetType().GetProperties();
            foreach (var p in props)
            {
                if (p.GetCustomAttribute(typeof(NotMappedAttribute), true) == null && p.CanWrite && p.CanRead && TypeSupported(p.PropertyType))
                {
                    res.Add(p.Name, p.GetValue(obj));
                }
            }
            return res;
        }


        Dictionary<Type, Dictionary<object, object>> loadingObjects = new Dictionary<Type, Dictionary<object, object>>();

        public BusinessObjectBase<IdType> LoadObjectProps<IdType>(BusinessObjectBase<IdType> res, Dictionary<string, object> storedProps)
            where IdType : IComparable
        {
            var type = res.BaseType;
            var props = res.GetType().GetProperties();
            foreach (var p in storedProps)
            {
                var prop = res.GetType().GetProperty(p.Key);
                prop.SetValue(res, p.Value);
            }
            if (loadingObjects[type].ContainsKey(res.Id))
            {
                return loadingObjects[type][res.Id] as BusinessObjectBase<IdType>;
            }
            loadingObjects[type][res.Id] = res;

            foreach (var p in props)
            {
                ForeignKeyAttribute a = p.GetCustomAttribute(typeof(ForeignKeyAttribute), true) as ForeignKeyAttribute;
                if (a != null && !p.GetMethod.Attributes.HasFlag(MethodAttributes.Virtual))
                {
                    Type pType = p.PropertyType.IsPrimitive ? p.PropertyType : BaseType(p.PropertyType);
                    if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(BusinessObjectInternal<,>))
                    {
                        var fk_prop = res.GetType().GetProperty(a.Name);
                        object id = fk_prop.GetValue(res);
                        if (id != null)
                        {
                            var meth = typeof(BusinessObjectInternal<,>).MakeGenericType(pType.GetGenericArguments()).GetMethod("GetById");
                            object obj = meth.Invoke(null, new object[] { id });
                            p.SetValue(res, obj);
                        }
                    }
                    else if (p.PropertyType.IsGenericType)
                    {
                        var args = p.PropertyType.GetGenericArguments();
                        if (args.Length == 1)
                        {
                            if (p.PropertyType.IsAssignableFrom(typeof(List<>).MakeGenericType(args)))
                            {
                                var listType = args[0];
                                var pinfo = listType.GetProperty(a.Name);
                                var par = Expression.Parameter(listType);
                                Expression exp = Expression.Equal(Expression.Constant(res.Id), Expression.Property(par, pinfo));
                                var expr = Expression.Lambda(Expression.GetFuncType(listType, typeof(bool)), exp, par);
                                var meth = BusinessObjectAdapter.Current.GetType().GetMethod("GetAll").MakeGenericMethod(BaseType(listType).GetGenericArguments());
                                var result = meth.Invoke(BusinessObjectAdapter.Current, new object[] { expr });
                                p.SetValue(res, result);
                            }
                        }
                    }
                }
            }
            return res;
        }

        public T LoadObject<T, IdType>(T obj, Dictionary<string, object> storedProps)
            where IdType : IComparable
            where T : BusinessObjectInternal<T, IdType>, new()
        {
            lock (loadingObjects)
            {
                int count = loadingObjects.Count;
                if (!loadingObjects.ContainsKey(typeof(T)))
                {
                    loadingObjects[typeof(T)] = new Dictionary<object, object>();
                }
                T res = ProxyFactory.CreateInstance<T, IdType>();
                res = LoadObjectProps<IdType>(res, storedProps) as T;
                if (count == 0)
                    loadingObjects.Clear();
                return res;
            }
        }

        public BusinessObjectBase<IdType> ReloadObject<IdType>(BusinessObjectBase<IdType> obj, Dictionary<string, object> storedProps)
            where IdType : IComparable
        {
            lock (loadingObjects)
            {
                LoadObjectProps<IdType>(obj, storedProps);
                return obj;
            }
        }

        private void UpdateId(BusinessObjectBase<int> obj)
        {
            if (obj.Id == 0)
            {
                Type type = obj.BaseType;
                obj.Id = idCache.ContainsKey(type) ? idCache[type] + 1 : 1;
                idCache[type] = obj.Id;
            }
        }

        protected override void InternalUpdate<IdType>(BusinessObjectBase<IdType> obj)
        {
            var cacheForClass = GetCache(obj.BaseType);
            lock (cacheForClass)
            {
                var copy = StoreProperties(obj);
                cacheForClass[obj.Id] = new Data { Props = copy };
            }
        }

        public override EntityType GetById<EntityType, IdType>(IdType id)
        {
            var cacheForClass = GetCache(typeof(EntityType));
            lock (cacheForClass)
            {
                return (EntityType)(cacheForClass.ContainsKey(id) ? cacheForClass[id].GetObject<EntityType, IdType>() : null);
            }
        }

        public override List<T> GetAll<T, IdType>(Expression<Func<T, bool>> selector = null)
        {
            var cacheForClass = GetCache(typeof(T));
            lock (cacheForClass)
            {
                return GetFilteredCache<T, IdType>(selector, cacheForClass).ToList();
            }
        }

        public override IQueryable<EntityType> AsQueryable<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
        {
            var res = new SafeEnumerableQuery<EntityType>(GetAll<EntityType, IdType>());
            return selector == null ? res : res.Where(selector);
        }

        protected override void InternalDelete<IdType>(BusinessObjectBase<IdType> obj)
        {
            var cacheForClass = GetCache(obj.BaseType);
            lock (cacheForClass)
            {
                cacheForClass.Remove(obj.Id);
            }
        }

        public override EntityType GetFirst<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
        {
            var cacheForClass = GetCache(typeof(EntityType));
            lock (cacheForClass)
            {
                var c = GetFilteredCache<EntityType, IdType>(selector, cacheForClass);
                return c.FirstOrDefault();
            }
        }

        private static IEnumerable<EntityType> GetFilteredCache<EntityType, IdType>(Expression<Func<EntityType, bool>> selector, Dictionary<object, Data> cacheForClass)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new()
        {
            IEnumerable<EntityType> c = cacheForClass.Select(kv => kv.Value.GetObject<EntityType, IdType>());
            if (selector != null)
            {
                c = c.Where(selector.Compile());
            }
            return c;
        }

        public override IQueryable<DiscriminatorType> AsQueryable<EntityType, IdType, DiscriminatorType>(Expression<Func<EntityType, bool>> selector = null)
        {
            IQueryable<EntityType> res = AsQueryable<EntityType, IdType>(selector);
            res = selector != null ? res.Where(selector) : res;
            return res.OfType<DiscriminatorType>();
        }

        public override void Dispose()
        {
            objCache = null;
        }

        public override Func<string, object> OriginalValues<IdType>(BusinessObjectBase<IdType> entity)
        {
            var cacheForClass = GetCache(entity.BaseType);
            Func<string, object> res = null;
            if (cacheForClass.ContainsKey(entity.Id))
            {
                var props = cacheForClass[entity.Id].Props;
                res = (s) => props[s];
            }
            return res;
        }


        public override EntityType Attach<EntityType, IdType>(EntityType entity)
        {
            var t = typeof(EntityType);
            if (!ObjCache.ContainsKey(t))
                ObjCache[t] = new Dictionary<object, object>();
            ObjCache[t][entity.Id] = entity;
            return entity;
        }

        public override void Detach<EntityType, IdType>(EntityType entity)
        {
            ObjCache[typeof(EntityType)].Remove(entity.Id);
        }


        static List<Action> clear = new List<Action>();
        static Dictionary<Action, Action> clearAbort = new Dictionary<Action, Action>();
        internal static void AddClear(Action ClearCache)
        {
            clear.Add(ClearCache);
        }

        public static void AddClearOnAbort(Action ClearCache)
        {
            if (clearAbort.Count == 0)
                Transaction.Current.TransactionCompleted += clearCache;
            clearAbort[ClearCache] = ClearCache;
        }

        private static void clearCache(object sender, TransactionEventArgs e)
        {
            if (e.Transaction.TransactionInformation.Status == TransactionStatus.Aborted)
            {
                foreach (var clearChache in clearAbort.Keys)
                    clearChache();
            }
            clearAbort.Clear();
        }

        public static void ClearCache()
        {
            clear.ForEach(c => c());
            idCache.Clear();
        }

        public override void Reload<IdType>(BusinessObjectBase<IdType> obj)
        {
            var cacheForClass = GetCache(obj.BaseType);
            lock (cacheForClass)
            {
                cacheForClass[obj.Id].ReloadObject<IdType>(obj);
            }
        }

        public void SetIdentitySeed<EntityType>(int seed)
        {
            idCache[typeof(EntityType)] = seed;
        }

    }

}
