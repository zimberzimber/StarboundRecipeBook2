using MongoDB.Driver.Linq;
using SBRB.Database;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Get a clean query to perform operations on.
        /// </summary>
        IQueryable<T> BaseQuery { get; }
    }

    public abstract class BaseRepository<T> : IBaseRepository<T>
    {
        protected DatabaseConnection _db;
        public BaseRepository()
            => _db = DatabaseConnection.Instance;

        public abstract IQueryable<T> BaseQuery { get; }

        protected IQueryable<T> SkipTake(IQueryable<T> baseQueriable, int skip, int take)
            => baseQueriable.Skip(skip).Take(take);
    }
}
