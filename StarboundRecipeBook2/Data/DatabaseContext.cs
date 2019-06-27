using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;

namespace StarboundRecipeBook2.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        // Tables
        public virtual DbSet<Mod> Mods { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Rarity> Rarities { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<ActiveItemData> ActiveItemDatas { get; set; }
        public virtual DbSet<ObjectData> ObjectDatas { get; set; }
        public virtual DbSet<ColonyTag> ColonyTags { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeInput> RecipeInputs { get; set; }
        public virtual DbSet<RecipeGroup> RecipeGroups { get; set; }

        // Relationships 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LEVTOP2;Initial Catalog=SBRB-testing;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define primary keys
            builder.Entity<Mod>().HasKey(m => m.SteamId);
            builder.Entity<Item>().HasKey(i => i.InternalName);
            builder.Entity<Rarity>().HasKey(r => r.RarityName);
            builder.Entity<ItemType>().HasKey(it => it.FileExtension);
            builder.Entity<ItemCategory>().HasKey(ic => ic.CategoryName);
            builder.Entity<ConsumeableData>().HasKey(cd => cd.ItemName);
            builder.Entity<ActiveItemData>().HasKey(aid => aid.ItemName);
            builder.Entity<ObjectData>().HasKey(od => od.ItemName);
            builder.Entity<ColonyTag>().HasKey(ct => ct.ColonyTagName);
            builder.Entity<Recipe>().HasKey(r => new { r.RecipeId, r.SourceModId });
            builder.Entity<RecipeInput>().HasKey(ri => new { ri.RecipeInputId, ri.SourceModId });
            builder.Entity<RecipeGroup>().HasKey(rg => rg.RecipeGroupName);

            builder.Entity<Relationship_ObjectData_ColonyTag>().HasKey(roc => new { roc.ObjectItemName, roc.ColonyTagName });
            builder.Entity<Relationship_Recipe_RecipeGroup>().HasKey(rrr => new { rrr.RecipeId, rrr.SourceModId, rrr.RecipeGroupName });
            builder.Entity<Relationship_Item_Item>().HasKey(rii => new { rii.UnlockingItemName, rii.UnlockedItemName });

            // Item Relationships
            {
                builder.Entity<Item>() // Item - Mod (1 : M)
                    .HasOne(item => item.SourceMod)
                    .WithMany(mod => mod.AddedItems)
                    .HasForeignKey(item => item.SourceModId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - Rarity (1 : M)
                    .HasOne(item => item.Rarity)
                    .WithMany(rarity => rarity.Items)
                    .HasForeignKey(item => item.RarityName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ItemType (1 : M)
                    .HasOne(item => item.ItemType)
                    .WithMany(itemType => itemType.Items)
                    .HasForeignKey(item => item.FileExtension)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ItemCategory (1 : M)
                    .HasOne(item => item.ItemCategory)
                    .WithMany(itemCategory => itemCategory.Items)
                    .HasForeignKey(item => item.ItemCategoryName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ObjectData (0/1 : 1)
                    .HasOne(item => item.ObjectData)
                    .WithOne(objectData => objectData.Item)
                    .HasForeignKey<Item>(item => item.InternalName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ActiveItemData (0/1 : 1)
                    .HasOne(item => item.ActiveItemData)
                    .WithOne(activeItemData => activeItemData.Item)
                    .HasForeignKey<Item>(item => item.InternalName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ConsumableData (0/1 : 1)
                    .HasOne(item => item.ConsumeableData)
                    .WithOne(consumeableData => consumeableData.Item)
                    .HasForeignKey<Item>(item => item.InternalName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Item_Item>() // UnlockingItem - UnlockedItem (M : M)
                    .HasOne(rii => rii.UnlockingItem)
                    .WithMany(item => item.Unlocks)
                    .HasForeignKey(rii => rii.UnlockingItemName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Item_Item>() // UnlockingItem - UnlockedItem (M : M)
                    .HasOne(rii => rii.UnlockedItem)
                    .WithMany(item => item.UnlockedBy)
                    .HasForeignKey(rii => rii.UnlockedItemName)
                    .OnDelete(DeleteBehavior.Restrict);
            }

            // ObjectData-ColonyTag M:M Relationship
            {
                builder.Entity<Relationship_ObjectData_ColonyTag>() // ObjectData - ColonyTag (M : M)
                    .HasOne(roc => roc.ObjectData)
                    .WithMany(objectData => objectData.ColonyTags)
                    .HasForeignKey(roc => roc.ObjectItemName)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_ObjectData_ColonyTag>() // ObjectData - ColonyTag (M : M)
                    .HasOne(roc => roc.ColonyTag)
                    .WithMany(colonyTag => colonyTag.ObjectDatas)
                    .HasForeignKey(roc => roc.ColonyTagName)
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
                    .HasForeignKey(recipeInput => new { recipeInput.RecipeInputId, recipeInput.SourceModId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Recipe_RecipeGroup>() // Recipe - RecipeGroups (M : M)
                    .HasOne(rrr => rrr.Recipe)
                    .WithMany(recipe => recipe.RecipeGroups)
                    .HasForeignKey(rrr => new { rrr.RecipeId, rrr.SourceModId })
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
