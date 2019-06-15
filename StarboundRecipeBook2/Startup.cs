using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Models;

namespace WebApplication1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            { options.UseSqlServer("Data Source=LEVTOP2;Initial Catalog=SBRB-testing;Integrated Security=True"); });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext context)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvcWithDefaultRoute();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            SeedTestingData(context);

            context.SaveChanges();

            app.Run(async (ctx) =>
            { await ctx.Response.WriteAsync("Hello World!"); });
        }

        void SeedTestingData(DatabaseContext context)
        {
            var m1 = new Mod
            {
                SteamId = 1,
                InternalName = "TestMod1",
                Version = "1",
                LastUpdated = DateTime.Now,
                AddedItems = new List<Item>(),
                AddedRecipes = new List<Recipe>(),
                FriendlyName = "Test Mod 1",
            };

            context.Mods.Add(m1);
        }
    }
}
