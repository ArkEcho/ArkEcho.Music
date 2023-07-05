using ArkEcho.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

public class Program
{
    private static async Task Main(string[] args)
    {
        using Server server = new Server();

        if (!await server.Init())
        {
            Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
            Console.ReadLine();
            return;
        }

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(server);
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

        //app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        string port = ":7236";
        app.Urls.Add("http://" + server.GetAddress() + port);
        app.Urls.Add("http://localhost" + port);
        foreach (string url in app.Urls)
            Console.WriteLine($"Listening on: {url}");

        app.RunAsync();

        Console.WriteLine($"server started");
        Console.WriteLine("######################################");

        await cmdMenu(server);

        await app.StopAsync();
    }

    private static async Task cmdMenu(Server server)
    {
        while (true)
        {
            string input = readUserInput("cmdMenu $ ");

            if (input == "stop")
                break;
            else if (input == "user")
            {
                string userMenu = "user menu";
                while (true)
                {
                    string userInput = readUserInput($"{userMenu} $ ");

                    if (userInput == "stop")
                        break;
                    else if (userInput == "ls")
                        Console.WriteLine(await server.CmdListAllUsers());
                    else if (userInput == "create")
                    {
                        string userName = readUserInput($"{userMenu} username $ ");
                        string password = readUserInput($"{userMenu} password $ ");
                        string musiclibarypath = readUserInput($"{userMenu} musiclibarypath $ ");
                        Console.WriteLine(await server.CmdCreateUser(userName, password, musiclibarypath));
                    }
                    else if (userInput == "update")
                    {
                        string id = readUserInput($"{userMenu} id $ ");
                        if (int.TryParse(id, out int userid))
                        {
                            string field = readUserInput($"{userMenu} field $ ");
                            string newValue = readUserInput($"{userMenu} new value $ ");
                            Console.WriteLine(await server.CmdUpdateUser(id, field, newValue));
                        }
                    }
                    else if (userInput == "delete")
                    {
                        string id = readUserInput($"{userMenu} id $ ");
                        if (int.TryParse(id, out int userid))
                            Console.WriteLine(await server.CmdDeleteUser(userid));
                    }
                }
            }
        }
    }

    private static string readUserInput(string message)
    {
        Console.Write(message);
        string userInput = Console.ReadLine();
        return userInput;
    }
}