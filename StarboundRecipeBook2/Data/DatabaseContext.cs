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
        public virtual DbSet<Item> Item { get; set; }
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define primary keys
            builder.Entity<Mod>().HasKey(m => m.SteamId);
            builder.Entity<Item>().HasKey(i => i.ItemId);
            builder.Entity<Rarity>().HasKey(r => r.RarityId);
            builder.Entity<ItemType>().HasKey(it => it.ItemTypeId);
            builder.Entity<ItemCategory>().HasKey(ic => ic.ItemCategoryId);
            builder.Entity<ActiveItemData>().HasKey(aid => aid.ActiveItemDataId);
            builder.Entity<ObjectData>().HasKey(od => od.ObjectDataId);
            builder.Entity<ColonyTag>().HasKey(ct => ct.ColonyTagId);
            builder.Entity<Recipe>().HasKey(r => r.RecipeId);
            builder.Entity<RecipeInput>().HasKey(ri => ri.RecipeInputId);
            builder.Entity<RecipeGroup>().HasKey(rg => rg.RecipeGroupdId);

            builder.Entity<Relationship_ObjectData_ColonyTag>().HasKey(roc => new { roc.ObjectDataId, roc.ColonyTagId });
            builder.Entity<Relationship_Recipe_RecipeGroup>().HasKey(rrr => new { rrr.RecipeId, rrr.RecipeGroupId });

            // Mod relationships
            {
                builder.Entity<Mod>() // Mod - AddedItems (M : 1)
                    .HasMany(mod => mod.AddedItems)
                    .WithOne()
                    .HasForeignKey(item => item.SourceModId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Mod>() // Mod - AddedRecipes (M : 1)
                    .HasMany(mod => mod.AddedRecipes)
                    .WithOne()
                    .HasForeignKey(recipe => recipe.SourceModId)
                    .OnDelete(DeleteBehavior.Restrict);
            }

            // Item Relationships
            {
                builder.Entity<Item>() // Item - Rarity (1 : M)
                    .HasOne(item => item.Rarity)
                    .WithMany(rarity => rarity.Items)
                    .HasForeignKey(item => item.RarityId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ItemType (1 : M)
                    .HasOne(item => item.ItemType)
                    .WithMany(itemType => itemType.Items)
                    .HasForeignKey(item => item.ItemTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ItemCategory (1 : M)
                    .HasOne(item => item.ItemCategory)
                    .WithMany(itemCategory => itemCategory.Items)
                    .HasForeignKey(item => item.ItemCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ObjectData (0/1 : 1)
                    .HasOne(item => item.ObjectData)
                    .WithOne(objectData => objectData.Item)
                    .HasForeignKey<Item>(item => item.ObjectDataId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Item>() // Item - ActiveItemData (0/1 : 1)
                    .HasOne(item => item.ActiveItemData)
                    .WithOne(activeItemData => activeItemData.Item)
                    .HasForeignKey<Item>(item => item.ActiveItemDataId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            }

            // ObjectData-ColonyTag M:M Relationship
            {
                builder.Entity<Relationship_ObjectData_ColonyTag>() // ObjectData - ColonyTag (M : M)
                    .HasOne(roc => roc.ObjectData)
                    .WithMany(objectData => objectData.ColonyTags)
                    .HasForeignKey(roc => roc.ObjectDataId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_ObjectData_ColonyTag>() // ObjectData - ColonyTag (M : M)
                    .HasOne(roc => roc.ColonyTag)
                    .WithMany(colonyTag => colonyTag.ObjectDatas)
                    .HasForeignKey(roc => roc.ColonyTagId)
                    .OnDelete(DeleteBehavior.Restrict);
            }

            // Recipe Relationships
            {
                builder.Entity<Recipe>() // Recipe - RecipeInput (M : 1)
                    .HasMany(recipe => recipe.RecipeInputs)
                    .WithOne(recipeInput => recipeInput.Recipe)
                    .HasForeignKey(recipeInput => recipeInput.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Recipe_RecipeGroup>() // Recipe - RecipeGroups (M : M)
                    .HasOne(rrr => rrr.Recipe)
                    .WithMany(recipe => recipe.RecipeGroups)
                    .HasForeignKey(rrr => rrr.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Relationship_Recipe_RecipeGroup>() // Recipe - RecipeGroups (M : M)
                    .HasOne(rrr => rrr.RecipeGroup)
                    .WithMany(recipeGroups => recipeGroups.Recipes)
                    .HasForeignKey(rrr => rrr.RecipeGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
