using System;
using System.Net.Http;
using System.Text.Json;
using Finity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHttpClient("finity")
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
    })
    .WithAuthentication(a =>
    {
        a.Endpoint = "https://localhost:5003/connect/token";
        a.ClientId = "back-end-client";
        a.Scope = "m2m";
        a.ClientSecret = "secret";
        a.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    });
var app = builder.Build();
app.MapGet("/", (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("finity");
    var request = new HttpRequestMessage(HttpMethod.Get,
        "https://run.mocky.io/v3/10cb934a-b8be-4b75-8b2f-aef09574bd7e");
    var response = client.SendAsync(request).Result;
    if (!response.IsSuccessStatusCode) throw new Exception();
    var responseStream = response.Content.ReadAsStringAsync().Result;
    var data = JsonSerializer.Deserialize<object>(responseStream);
    return Results.Ok(data);
});
app.Run();