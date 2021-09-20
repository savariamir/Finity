using System;
using Finity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Finity.Sample
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Finity.Sample", Version = "v1"});
            });


            services.AddHttpClient("finity")
                .WithBulkhead(a => { a.MaxConcurrentCalls = 12; })
                .WithCache(a => { a.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); })
                .WithRetry(a =>
                {
                    a.RetryCount = 5;
                    a.SleepDurationRetry = TimeSpan.FromSeconds(1);
                })
                .WithCircuitBreaker(a =>
                {
                    a.DurationOfBreak = TimeSpan.Zero;
                    a.ExceptionsAllowedBeforeBreaking = 2;
                    a.SuccessAllowedBeforeClosing = 1;
                });
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finity.Sample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}