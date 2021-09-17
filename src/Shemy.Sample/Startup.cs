using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Shemy.Extension;
using Shemy.Extensions;
using Shemy.Prometheus;

// using Polly;
// using Polly.Extensions.Http; 

namespace Shemy.Http
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Shemy.Sample", Version = "v1"});
            });

            services.AddShemyHttpClient("test", a => { })
                .AddBulkhead(a => { a.MaxConcurrentCalls = 12; })
                .AddCache(a => { a.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); })
                .AddRetry(a =>
                {
                    a.RetryCount = 5;
                    a.SleepDurationRetry = TimeSpan.FromSeconds(1);
                })
                .AddPrometheus()
                .AddCircuitBreaker(a =>
                {
                    a.DurationOfBreak = TimeSpan.Zero;
                    a.ExceptionsAllowedBeforeBreaking = 2;
                    a.SuccessAllowedBeforeClosing = 1;
                });
            
            services.AddShemyHttpClient("test1")
                .AddRetry(a =>
                {
                    a.RetryCount = 2;
                    a.SleepDurationRetry = TimeSpan.FromSeconds(200);
                }).AddCache(a => { a.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); });
            // .SetHandlerLifetime(TimeSpan.FromSeconds(100))
            // .AddCircuitBreaker(a =>
            // {
            //     a.DurationOfBreak = TimeSpan.Zero;
            //     a.ExceptionsAllowedBeforeBreaking = 2;
            //     a.SuccessAllowedBeforeClosing = 1;
            // });


            // services.AddHttpClient("csharpcorner")
            //         .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            //         // important step  
            //         .AddPolicyHandler(GetRetryPolicy());

            // services.AddHttpClient("csharpcorner")
            //         .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            //         // important step  
            //         .AddPolicyHandler(GetRetryPolicy());

            // services.AddHttpContextAccessor();
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                    // HttpRequestException, 5XX and 408  
                    .HandleTransientHttpError()
                    // 404  
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    // Retry two times after delay  
                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shemy.Sample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}