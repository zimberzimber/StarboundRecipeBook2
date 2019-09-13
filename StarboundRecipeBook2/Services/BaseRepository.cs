using SBRB.Database;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public abstract class BaseRepository<T>
    {
        protected DatabaseConnection _db;
        public BaseRepository()
            => _db = DatabaseConnection.Instance;

        protected IQueryable<T> SkipTake(IQueryable<T> baseQueriable, int skip, int take)
            => baseQueriable.Skip(skip).Take(take);
    }
}
