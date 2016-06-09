using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Database.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {

        /// <summary>
        /// Gets all objects from database
        /// </summary>
        IQueryable<T> GetAll();

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate);

        ///// <summary>
        ///// Gets objects from database with filting and paging.
        ///// </summary>
        //IQueryable<T> Filter<Key>(Expression<Func<T, bool>> filter,
        //    out int total, int index = 0, int size = 50);

        /// <summary>
        /// Returns True if the object exists in database by specified filter.
        /// Flase, otherwise
        /// </summary>
        bool Contains(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add a new object to database.
        /// </summary>
        void Add(T t);

        /// <summary>
        /// Remove objects from database by specified filter expression.
        /// </summary>
        void Remove(Expression<Func<T, bool>> predicate);

        ///// <summary>
        ///// Update object changes and save to database.
        ///// </summary>
        //void Update(T t);
        void SaveChanges();
        /// <summary>
        /// Get the total objects count.
        /// </summary>
        int Count { get; }
    }
}
