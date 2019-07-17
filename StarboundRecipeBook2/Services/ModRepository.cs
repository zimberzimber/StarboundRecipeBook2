using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Helpers;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    [Flags]
    public enum ModIncludeOptions : byte
    {
        None = 0,
        Items = 1,
        Recipes = 2,
        All = 3
    }

    public interface IModRepository
    {
        /// <summary>Get a list of all the available mods</summary>
        /// <param name="includeOptions">Include options for the query</param>
        List<Mod> GetAllMods(ModIncludeOptions includeOptions = ModIncludeOptions.None);

        /// <summary>Get a specific mod by SteamID, or null if it doesn't exist</summary>
        /// <param name="steamId">Mods Steam ID</param>
        /// <param name="includeOptions">Include options for the query</param>
        Mod GetModById(int steamId, ModIncludeOptions includeOptions = ModIncludeOptions.None);
    }

    public class ModRepository : IModRepository
    {
        DatabaseContext _context;

        public ModRepository(DatabaseContext context)
        { _context = context; }

        IQueryable<Mod> BaseQuery(ModIncludeOptions includeOptions = ModIncludeOptions.None)
        {
            return _context.Mods
                .OrderBy(m => m.SteamId)
                .If(includeOptions.HasFlag(ModIncludeOptions.Items), q => q.Include(m => m.AddedItems))
                .If(includeOptions.HasFlag(ModIncludeOptions.Recipes), q => q.Include(m => m.AddedRecipes));
        }

        public List<Mod> GetAllMods(ModIncludeOptions includeOptions = ModIncludeOptions.None)
            => BaseQuery().ToList();

        public Mod GetModById(int steamId, ModIncludeOptions includeOptions = ModIncludeOptions.None)
            => BaseQuery().FirstOrDefault(m => m.SteamId == steamId);
    }
}
