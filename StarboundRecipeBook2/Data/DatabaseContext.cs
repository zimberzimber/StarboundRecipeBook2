using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;

namespace StarboundRecipeBook2.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public const string CONNECTION_STRING = "Data Source=LEVTOP2;Initial Catalog=SBRB-testing;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        // Tables
        public virtual DbSet<Mod> Mods { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ActiveItemData> ActiveItemDatas { get; set; }
        public virtual DbSet<consumableData> ConsumableDatas { get; set; }
        public virtual DbSet<ObjectData> ObjectDatas { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeInput> RecipeInputs { get; set; }
        public virtual DbSet<RecipeGroup> RecipeGroups { get; set; }
        public virtual DbSet<RecipeUnlock> RecipeUnlocks { get; set; }

        // Relationships 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { optionsBuilder.UseSqlServer(CONNECTION_STRING); }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define primary keys
            builder.Entity<Mod>().HasKey(m => m.SteamId);
            builder.Entity<Item>().HasKey(i => new { i.SourceModId, i.ItemId });

            builder.Entity<consumableData>().HasKey(cd => new { cd.SourceModId, cd.consumableDataId });
            builder.Entity<ActiveItemData>().HasKey(aid => new { aid.SourceModId, aid.ActiveItemDataId });
            builder.Entity<ObjectData>().HasKey(od => new { od.SourceModId, od.ObjectDataId });

            builder.Entity<Recipe>().HasKey(r => new { r.SourceModId, r.RecipeId });
            builder.Entity<RecipeInput>().HasKey(ri => new { ri.SourceModId, ri.RecipeInputId });
            builder.Entity<RecipeGroup>().HasKey(rg => rg.RecipeGroupName);

            builder.Entity<Relationship_Recipe_RecipeGroup>().HasKey(rrr => new { rrr.SourceModId, rrr.RecipeId, rrr.RecipeGroupName });
            builder.Entity<RecipeUnlock>().HasKey(ru => new { ru.UnlockingItemSourceModId, ru.UnlockingItemId, ru.UnlockedItemName });

            // Item Relationships
            {
                builder.Entity<Item>() // Item - Mod (1 : M)
                    .HasOne(item => item.SourceMod)
                    .WithMany(mod => mod.AddedItems)
                    .HasForeignKey(item => item.SourceModId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - RecipeUnlock (M : 1)
                    .HasMany(item => item.Unlocks)
                    .WithOne(ru => ru.UnlockingItem)
                    .HasForeignKey(ru => new { ru.UnlockingItemSourceModId, ru.UnlockingItemId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - Object Data (1 : 1)
                    .HasOne(item => item.ObjectData)
                    .WithOne(obj => obj.Item)
                    .HasForeignKey<Item>(item => new { item.SourceModId, item.ObjectDataId })
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - Active Item Data (1 : 1)
                    .HasOne(item => item.ActiveItemData)
                    .WithOne(obj => obj.Item)
                    .HasForeignKey<Item>(item => new { item.SourceModId, item.ActiveItemDataId })
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - consumable Data (1 : 1)
                    .HasOne(item => item.consumableData)
                    .WithOne(obj => obj.Item)
                    .HasForeignKey<Item>(item => new { item.SourceModId, item.consumableDataId })
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            }

            // Recipe Relationships
            {
                builder.Entity<Recipe>() // Recipe - Mod (1 : M)
                    .HasOne(recipe => recipe.SourceMod)
                    .WithMany(mod => mod.AddedRecipes)
                    .HasForeignKey(recipe => recipe.SourceModId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Recipe>() // Recipe - RecipeInput (M : 1)
                    .HasMany(recipe => recipe.RecipeInputs)
                    .WithOne(recipeInput => recipeInput.Recipe)
                    .HasForeignKey(recipeInput => new { recipeInput.SourceModId, recipeInput.RecipeInputId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Recipe_RecipeGroup>() // Recipe - RecipeGroups (M : M)
                    .HasOne(rrr => rrr.Recipe)
                    .WithMany(recipe => recipe.RecipeGroups)
                    .HasForeignKey(rrr => new { rrr.SourceModId, rrr.RecipeId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Recipe_RecipeGroup>() // Recipe - RecipeGroups (M : M)
                    .HasOne(rrr => rrr.RecipeGroup)
                    .WithMany(recipeGroups => recipeGroups.Recipes)
                    .HasForeignKey(rrr => rrr.RecipeGroupName)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
