using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.Services;
using ScoreFourServer.OnMemory.Adapter;
using Microsoft.OpenApi.Models;

namespace ScoreFourServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            // DI
            services.AddScoped(sp => new GameManagerFactory(
                sp.GetService<IGameMovementAdapter>()
                ));
            services.AddScoped(sp => new PlayerMatchingService(
                sp.GetService<IGameRoomAdapter>(),
                sp.GetService<IWaitingPlayerAdapter>(),
                sp.GetService<GameManagerFactory>()
                ));
            services.AddScoped<IGameMovementAdapter>(sp => new GameMovementAdapter());
            services.AddScoped<IGameRoomAdapter>(sp => new GameRoomAdapter());
            services.AddScoped<IWaitingPlayerAdapter>(sp => new WaitingPlayerAdapter());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}