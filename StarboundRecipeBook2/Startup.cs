using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StarboundRecipeBook2.Services;
using System.IO;

namespace WebApplication1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IModRepository, ModRepository>();
            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<IRecipeRepository, RecipeRepository>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (ctx) =>
            { await ctx.Response.WriteAsync("Hi"); });
        }
    }
}
