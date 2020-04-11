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
using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.Services;
using Microsoft.OpenApi.Models;

namespace ScoreFourServer.WebApi
{
    public class Startup
    {
        readonly string allowSpecificOriginsForDev = "_allowSpecificOriginsDev";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();


            // Cors
            services.AddCors(options =>
            {
                options.AddPolicy(allowSpecificOriginsForDev,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScoreFourServer API", Version = "v1" });
            });

            // DI
            services.AddScoped(sp => new GameManagerFactory(
                sp.GetService<IGameRoomAdapter>(),
                sp.GetService<IGameMovementAdapter>()
                ));
            services.AddScoped(sp => new PlayerMatchingService(
                sp.GetService<IGameRoomAdapter>(),
                sp.GetService<IWaitingPlayerAdapter>(),
                sp.GetService<GameManagerFactory>()
                ));
            services.AddScoped<IGameMovementAdapter>(sp => new Adapters.OnMemory.GameMovementAdapter());
            services.AddScoped<IGameRoomAdapter>(sp => new Adapters.Azure.GameRoomAdapter(Configuration.GetConnectionString("StorageConnectionString")));
            services.AddScoped<IWaitingPlayerAdapter>(sp => new Adapters.OnMemory.WaitingPlayerAdapter());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScoreFourServer API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                app.UseCors(allowSpecificOriginsForDev);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
