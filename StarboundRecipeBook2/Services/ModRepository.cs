using MongoDB.Driver;
using SBRB.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public interface IModRepository : IBaseRepository<Mod>
    {
        /// <summary>
        /// Get a list of all the mods
        /// </summary>
        /// <param name="skip">How many to skip</param>
        /// <param name="take">How many to take</param>
        /// <returns>A list of mods</returns>
        List<Mod> GetAllMods(int skip = 0, int take = 0);

        /// <summary>
        /// Get a mod by its steam ID
        /// </summary>
        /// <param name="steamId">The Steam ID to look by</param>
        /// <returns>A mod with the given Steam ID, or null if its not found</returns>
        Mod GetModById(uint steamId);
    }

    public class ModRepository : BaseRepository<Mod>, IModRepository
    {
        public override IQueryable<Mod> BaseQuery => _db.Mods.AsQueryable();

        public List<Mod> GetAllMods(int skip = 0, int take = 0)
            => SkipTake(BaseQuery, skip, take).ToList();

        public Mod GetModById(uint steamId)
            => BaseQuery.FirstOrDefault(m => m.SteamId == steamId);
    }
}
