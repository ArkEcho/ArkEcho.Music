using ArkEcho.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

using (Server.Instance)
{
    if (!await Server.Instance.Init())
    {
        Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
        Console.ReadLine();
        return;
    }

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSingleton(Server.Instance);
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");

    // TODO: Better Solution
    string port = ":7236";
    app.Urls.Add("https://" + Server.Instance.GetAddress() + port);
    app.Urls.Add("https://localhost" + port);
    foreach (string url in app.Urls)
        Console.WriteLine($"Listening on: {url}");

    app.Run();
}