using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;

namespace Logic.ObjectModel
{
    public abstract class BusinessObjectBase<IdType> : IBusinessObject where IdType : IComparable
    {
        public virtual IdType Id { get; set; }
        [NotMapped]
        public virtual bool UseUpdateBehavior { get { return HasIdentityId; } }

        protected virtual IBusinessObject InternalSave()
        {
            BusinessObjectAdapter.Current.Save(this);
            return this;
        }

        public IBusinessObject Save()
        {
            using (TransactionScope s = new TransactionScope())
            {
                var res = UseUpdateBehavior && !Id.Equals(default(IdType))
                    ? InternalUpdate()
                    : InternalSave();
                s.Complete();
                return res;
            }
        }
        protected virtual IBusinessObject InternalUpdate()
        {
            BusinessObjectAdapter.Current.Update(this);
            return this;
        }

        public IBusinessObject Update()
        {
            using (TransactionScope s = new TransactionScope())
            {
                InternalUpdate();
                s.Complete();
                return this;
            }
        }

        public void Reload()
        {
            BusinessObjectAdapter.Current.Reload(this);
        }

        protected virtual void InternalDelete()
        {
            BusinessObjectAdapter.Current.Delete(this);
        }

        public void Delete()
        {
            using (TransactionScope s = new TransactionScope())
            {
                InternalDelete();
                s.Complete();
            }
        }

        internal virtual bool HasIdentityId
        {
            get { return false; }
        }

        public List<PropertyTrack> GetChanges()
        {
            return BusinessObjectAdapter.Current.GetChanges(this);
        }

        internal abstract Type BaseType { get; }

        string ITracking.Id
        {
            get { return Id.ToString(); }
        }

        string ITracking.Name
        {
            get { return BaseType.Name; }
        }

    }

    [Serializable]
    public abstract class BusinessObjectInternal<EntityType, IdType> : BusinessObjectBase<IdType>
        where EntityType : BusinessObjectInternal<EntityType, IdType>, new()
        where IdType : IComparable
    {
        public virtual EntityType Attach()
        {
            return BusinessObjectAdapter.Current.Attach<EntityType, IdType>(this);
        }

        public virtual void Detach()
        {
            BusinessObjectAdapter.Current.Detach<EntityType, IdType>(this);
        }

        public static List<EntityType> GetAll(Expression<Func<EntityType, bool>> selector = null)
        {
            return (BusinessObjectAdapter.Current.GetAll<EntityType, IdType>(selector) ?? new List<EntityType>()).ToList();
        }

        public static IQueryable<EntityType> AsQueryable(Expression<Func<EntityType, bool>> selector = null)
        {
            return BusinessObjectAdapter.Current.AsQueryable<EntityType, IdType>(selector);
        }

        public static EntityType GetFirst(Expression<Func<EntityType, bool>> selector = null)
        {
            return BusinessObjectAdapter.Current.GetFirst<EntityType, IdType>(selector);
        }

        public static void Delete(IdType id)
        {
            BusinessObjectAdapter.Current.Delete<EntityType, IdType>(id);
        }

        public static EntityType GetById(IdType id)
        {
            return id != null ? BusinessObjectAdapter.Current.GetById<EntityType, IdType>(id) : null;
        }

        public static implicit operator EntityType(BusinessObjectInternal<EntityType, IdType> value)
        {
            return (EntityType)value;
        }

        public static List<T> GetAll<T>(Expression<Func<EntityType, bool>> selector = null) where T : EntityType
        {
            return AsQueryable<T>(selector).ToList();
        }

        public static IQueryable<T> AsQueryable<T>(Expression<Func<EntityType, bool>> selector = null) where T : EntityType
        {
            return BusinessObjectAdapter.Current.AsQueryable<EntityType, IdType, T>(selector);
        }

        public static T GetFirst<T>(Expression<Func<EntityType, bool>> selector = null) where T : EntityType
        {
            return AsQueryable<T>(selector).FirstOrDefault();
        }

        internal override Type BaseType
        {
            get { return typeof(EntityType); }
        }

        private bool? hasIdentityId;
        internal override bool HasIdentityId
        {
            get
            {
                if (!hasIdentityId.HasValue)
                {
                    var attrs = GetType().GetProperty("Id").GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true);
                    hasIdentityId = attrs.Length == 1 && ((DatabaseGeneratedAttribute)attrs[0]).DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
                }

                return hasIdentityId.Value;
            }
        }
    }

    public abstract class BusinessObject<EntityType, IdType> : BusinessObjectInternal<EntityType, IdType>
        where EntityType : BusinessObjectInternal<EntityType, IdType>, new()
        where IdType : IComparable
    {
        [Key, Column(Order = 0)]
        public override IdType Id { get; set; }
    }

    [Serializable]
    public abstract class BusinessObject<EntityType> : BusinessObjectInternal<EntityType, int> where EntityType : BusinessObjectInternal<EntityType, int>, new()
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public override int Id { get; set; }

        public static EntityType GetById(int? id)
        {
            return id.HasValue ? BusinessObjectAdapter.Current.GetById<EntityType, int>(id.Value) : null;
        }
    }


    public abstract class BusinessObjectWithComplexKey<EntityType> : BusinessObjectInternal<EntityType, string> where EntityType : BusinessObjectInternal<EntityType, string>, new()
    {
        static List<PropertyInfo> props;

        [NotMapped]
        public override string Id
        {
            get
            {
                props = props ?? GetType().GetProperties().Where(p => p.GetCustomAttribute<ColumnAttribute>() != null)
                    .OrderBy(p => p.GetCustomAttribute<ColumnAttribute>().Order).ToList();
                return string.Join("_", props.Select(p => p.GetValue(this).ToString()));
            }
            set
            { }
        }
    }

}
