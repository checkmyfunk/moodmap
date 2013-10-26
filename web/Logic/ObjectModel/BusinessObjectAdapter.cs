namespace Logic.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Configuration;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web;



    public struct PropertyTrack
    {
        public object Entity;
        public string Name;
        public bool HasValue;
        public object OldValue;
        public object NewValue;
    }

    public delegate void TrackingDelegate(ITracking entity);


    public abstract class BusinessObjectAdapter : IDisposable
    {
        [ThreadStatic]
        private static BusinessObjectAdapter currentUdapter;
        public const string BusinessObjectAdapterKey = "BOAKey";
        static Dictionary<HttpRequest, BusinessObjectAdapter> adapters = new Dictionary<HttpRequest, BusinessObjectAdapter>();

        public static TrackingDelegate OnSave { get; set; }
        public static TrackingDelegate OnUpdate { get; set; }
        public static TrackingDelegate OnDelete { get; set; }

        public Expression<Func<EntityType, bool>> CompareSelector<EntityType, IdType>(IdType id)
        {
            ParameterExpression par = Expression.Parameter(typeof(EntityType));
            Expression exp = Expression.Equal(Expression.Property(par, "Id"), Expression.Constant(id));
            return Expression.Lambda<Func<EntityType, bool>>(exp, par);
        }

        public abstract EntityType GetById<EntityType, IdType>(IdType id)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new();

        public abstract void Dispose();

        protected abstract void InternalSave<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable;
        protected abstract void InternalUpdate<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable;
        protected abstract void InternalDelete<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable;
        public abstract EntityType Attach<EntityType, IdType>(EntityType entity)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new();
        public abstract void Detach<EntityType, IdType>(EntityType entity)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new();

        public abstract void Reload<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable;

        public void Save<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable
        {
            if (OnSave != null && obj.GetType().GetCustomAttribute<NoTrackAttribute>() == null)
                OnSave(obj);
            InternalSave(obj);
        }

        public void Update<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable
        {
            if (OnUpdate != null && obj.GetType().GetCustomAttribute<NoTrackAttribute>() == null)
                OnUpdate(obj);
            InternalUpdate(obj);
        }

        public void Delete<EntityType, IdType>(IdType id)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new()
        {
            EntityType entity = Activator.CreateInstance(typeof(EntityType)) as EntityType;
            entity.Id = id;
            Delete(entity);
        }

        public void Delete<IdType>(BusinessObjectBase<IdType> obj) where IdType : IComparable
        {
            if (OnDelete != null && obj.GetType().GetCustomAttribute<NoTrackAttribute>() == null)
                OnDelete(obj);
            InternalDelete(obj);
        }

        public abstract EntityType GetFirst<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new();

        public abstract IQueryable<EntityType> AsQueryable<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new();

        public abstract IQueryable<DiscriminatorType> AsQueryable<EntityType, IdType, DiscriminatorType>(Expression<Func<EntityType, bool>> selector = null)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new()
            where DiscriminatorType : EntityType;

        public abstract Func<string, object> OriginalValues<IdType>(BusinessObjectBase<IdType> entity)
            where IdType : IComparable;

        public abstract List<EntityType> GetAll<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
            where IdType : IComparable
            where EntityType : BusinessObjectInternal<EntityType, IdType>, new();

        internal static bool TypeSupported(Type type)
        {
            return (type.IsEnum || type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(decimal) || type == typeof(Guid))
                || type == typeof(byte[])
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && TypeSupported(type.GetGenericArguments()[0]));
        }

        public List<PropertyTrack> GetChanges<IdType>(BusinessObjectBase<IdType> entity)
            where IdType : IComparable
        {
            List<PropertyTrack> res = new List<PropertyTrack>();
            var props = entity.GetType().GetProperties();
            var originalValues = OriginalValues(entity);
            foreach (var p in props)
            {
                if (p.CanRead && p.CanWrite && TypeSupported(p.PropertyType)
                    && p.GetCustomAttribute<NotMappedAttribute>() == null
                    && p.GetCustomAttribute<NoTrackAttribute>() == null)
                {
                    object newValue = p.GetValue(entity);
                    object oldValue = originalValues != null ? originalValues(p.Name) : null;
                    if (originalValues == null || !object.Equals(newValue, oldValue))
                    {
                        res.Add(new PropertyTrack { Name = p.Name, NewValue = newValue, OldValue = oldValue, HasValue = originalValues != null });
                    }
                }
            }
            return res;
        }

        public static BusinessObjectAdapter Current
        {
            get
            {
                if (currentUdapter == null)
                {
                    var key = HttpContext.Current.Request;
                    lock (adapters)
                    {
                        if (adapters.ContainsKey(key))
                        {
                            currentUdapter = adapters[key];
                        }
                        else
                        {
                            lock (adapters)
                            {
                                currentUdapter = CreateAdapter();
                                adapters[key] = currentUdapter;
                            }

                        }
                    }
                }
                return currentUdapter;
            }
            set
            {
                currentUdapter = value;
            }
        }

        public static BusinessObjectAdapter CreateAdapter()
        {
            BusinessObjectAdapter a = ConfigurationManager.AppSettings["UseMemoryDatabase"] != null
                && ConfigurationManager.AppSettings["UseMemoryDatabase"].ToLower() == true.ToString().ToLower()
                ? (BusinessObjectAdapter)new MemoryAdapter()
                : DbAdapter.CreateDBAdapter() ?? new MemoryAdapter();
            return a;
        }

        public static void DisposeCurrent()
        {
            lock (adapters)
            {
                if (adapters.Count > 0)
                {
                    if (HttpContext.Current != null)
                    {
                        var key = HttpContext.Current.Request;
                        if (adapters.ContainsKey(key))
                        {
                            currentUdapter = adapters[key];
                            adapters.Remove(key);
                        }
                    }
                }
            }
            if (currentUdapter != null)
                currentUdapter.Dispose();
            currentUdapter = null;
        }
    }
}
