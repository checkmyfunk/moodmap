namespace Logic.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    //using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    //using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class DbAdapter : BusinessObjectAdapter
    {
        public abstract DbContext CurrentDbContext { get; set; }

        public override EntityType GetById<EntityType, IdType>(IdType id)
        {
            return typeof(EntityType).GetCustomAttribute<NoCacheAttribute>() != null
                ? GetFirst<EntityType, IdType>(CompareSelector<EntityType, IdType>(id))
                : CurrentDbContext.Set<EntityType>().Find(id);
        }

        protected override void InternalSave<IdType>(BusinessObjectBase<IdType> obj)
        {
            DbSet set = CurrentDbContext.Set(obj.GetType());
            set.Add(obj);
            CurrentDbContext.SaveChanges();
        }

        protected override void InternalUpdate<IdType>(BusinessObjectBase<IdType> obj)
        {
            DetectChanges(obj);
            CurrentDbContext.SaveChanges();
        }

        public override void Reload<IdType>(BusinessObjectBase<IdType> obj)
        {
            CurrentDbContext.Entry(obj).Reload();
        }

        private void DetectChanges(object entity)
        {
            ObjectContext ctx = ((IObjectContextAdapter)CurrentDbContext).ObjectContext;
            ObjectStateEntry entry;
            if (ctx.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                System.Type listType = typeof(System.Collections.Generic.List<>).MakeGenericType(entry.GetType());
                var list = Activator.CreateInstance(listType);
                MethodInfo mi = list.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                mi.Invoke(list, new object[] { entry });

                var p = ctx.ObjectStateManager.GetType().GetProperty("TransactionManager", BindingFlags.NonPublic | BindingFlags.Instance);
                var pp = p.GetValue(ctx.ObjectStateManager);
                mi = pp.GetType().GetMethod("BeginDetectChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(pp, null);
                try
                {
                    var objs = new object[] { list };
                    mi = ctx.ObjectStateManager.GetType().GetMethod("DetectChangesInNavigationProperties", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(ctx.ObjectStateManager, objs);
                    mi = ctx.ObjectStateManager.GetType().GetMethod("DetectChangesInScalarAndComplexProperties", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(ctx.ObjectStateManager, objs);
                    mi = ctx.ObjectStateManager.GetType().GetMethod("DetectChangesInForeignKeys", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(ctx.ObjectStateManager, objs);

                    mi = pp.GetType().GetMethod("BeginAlignChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(pp, null);
                    mi = ctx.ObjectStateManager.GetType().GetMethod("AlignChangesInRelationships", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(ctx.ObjectStateManager, objs);
                }
                finally
                {
                    mi = pp.GetType().GetMethod("EndAlignChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(pp, null);
                    mi = pp.GetType().GetMethod("EndDetectChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(pp, null);
                }
            }
        }

        protected override void InternalDelete<IdType>(BusinessObjectBase<IdType> obj)
        {
            DbEntityEntry ent = CurrentDbContext.Entry(obj);
            if (ent.State == EntityState.Detached)
            {
                var m = obj.GetType().GetMethod("Attach");
                m.Invoke(obj, null);
            }
            CurrentDbContext.Set(obj.GetType()).Remove(obj);
            CurrentDbContext.SaveChanges();
        }

        public override EntityType GetFirst<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
        {
            return AsQueryable<EntityType, IdType>(selector).FirstOrDefault();
        }

        public override List<EntityType> GetAll<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
        {
            return AsQueryable<EntityType, IdType>(selector).ToList();
        }

        public override IQueryable<EntityType> AsQueryable<EntityType, IdType>(Expression<Func<EntityType, bool>> selector = null)
        {
            IQueryable<EntityType> res = CurrentDbContext.Set<EntityType>();
            if (typeof(EntityType).GetCustomAttribute<NoCacheAttribute>() != null)
                res = res.AsNoTracking<EntityType>();
            return selector != null ? res.Where(selector) : res;
        }

        public override IQueryable<DiscriminatorType> AsQueryable<EntityType, IdType, DiscriminatorType>(Expression<Func<EntityType, bool>> selector = null)
        {
            IQueryable<DiscriminatorType> res = CurrentDbContext.Set<DiscriminatorType>();
            Expression<Func<DiscriminatorType, bool>> exp = selector != null
                ? Expression.Lambda<Func<DiscriminatorType, bool>>(selector.Body, selector.Parameters)
                : null;
            res = exp != null ? res.Where(exp) : res;
            return res;
        }

        public override void Dispose()
        {
            CurrentDbContext.Dispose();
            CurrentDbContext = null;
        }

        public override EntityType Attach<EntityType, IdType>(EntityType entity)
        {
            return CurrentDbContext.Set<EntityType>().Attach(entity);
        }

        public override void Detach<EntityType, IdType>(EntityType entity)
        {
            CurrentDbContext.Entry<EntityType>(entity).State = EntityState.Detached;
        }

        public override Func<string, object> OriginalValues<IdType>(BusinessObjectBase<IdType> entity)
        {
            DbEntityEntry ent = CurrentDbContext.Entry(entity);
            Func<string, object> res = null;
            if (ent != null && ent.State != EntityState.Detached)
                res = (s) => ent.OriginalValues[s];
            return res;
        }

        public static BusinessObjectAdapter CreateDBAdapter()
        {
            string dbAdapter = ConfigurationManager.AppSettings["DbAdapter"];
            if (!string.IsNullOrEmpty(dbAdapter))
            {
                Type t = Type.GetType(dbAdapter);
                if (t != null)
                {
                    return Activator.CreateInstance(t) as DbAdapter;
                }
            }
            return null;
        }

        public static void RemoveConventions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<IdKeyDiscoveryConvention>();
            modelBuilder.Conventions.Remove<NavigationPropertyNameForeignKeyDiscoveryConvention>();
            modelBuilder.Conventions.Remove<PrimaryKeyNameForeignKeyDiscoveryConvention>();
            modelBuilder.Conventions.Remove<TypeNameForeignKeyDiscoveryConvention>();
            modelBuilder.Conventions.Remove<AssociationInverseDiscoveryConvention>();
        }
    }
}
