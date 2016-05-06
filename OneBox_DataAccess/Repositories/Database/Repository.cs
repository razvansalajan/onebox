using OneBox_DataAccess.DatabaseContexts;
using OneBox_DataAccess.Repositories.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Database
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext Context = null;

        protected DbSet<T> DbSet
        {
            get
            {
                return Context.Set<T>();
            }
        }

        public Repository()
        {
        }

        public Repository(ApplicationDbContext context)
        {
            Context = context;
        }

        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
        }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).AsQueryable<T>();
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            // first or default. what does default means ? ( till now i found as to be null)
            // be carefull.
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual void Add(T T)
        {
            DbSet.Add(T);
            SaveChanges();
        }
        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }
        public void Remove(T entity)
        {
            DbSet dbSet = Context.Set<T>();
            dbSet.Remove(entity);
            SaveChanges();
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }


    }
}
