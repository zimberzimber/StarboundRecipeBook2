using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarboundRecipeBook2.Data;
using System.IO;

namespace WebApplication1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            { options.UseSqlServer("Data Source=LEVTOP2;Initial Catalog=SBRB-testing;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework"); });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext context)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvcWithDefaultRoute();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.SaveChanges();

            app.Run(async (ctx) =>
            { await ctx.Response.WriteAsync("Hi"); });
        }
    }
}
