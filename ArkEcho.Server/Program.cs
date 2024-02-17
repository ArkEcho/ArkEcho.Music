using ArkEcho;
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

        app.Urls.Add($"http://localhost:{Resources.ARKECHOPORT}");
        app.Urls.Add($"http://{Resources.GetIpAddress()}:{Resources.ARKECHOPORT}");
        foreach (string url in app.Urls)
            Console.WriteLine($"Listening on: {url}");

        app.RunAsync();

        await cmdMenu(server);

        await app.StopAsync();
    }

    private static async Task cmdMenu(Server server)
    {
        Console.WriteLine("# Arkecho Server started #");
        Console.WriteLine("# Type help for commands #");
        Console.WriteLine("######################################");

        while (true)
        {
            string input = readUserInput("menu>");

            if (input == "stop")
                break;
            else if (input == "help")
            {
                Console.WriteLine("user\tUser Menu");
            }
            else if (input == "user")
            {
                string userMenu = "user";
                while (true)
                {
                    string userInput = readUserInput($"{userMenu}>");

                    if (userInput == "exit")
                        break;
                    else if (userInput == "help")
                    {
                        Console.WriteLine("ls\tList all users");
                        Console.WriteLine("create\tCreate new user");
                        Console.WriteLine("update\tUpdate user properties");
                        Console.WriteLine("delete\tDelete a user");
                        Console.WriteLine("exit\tBack to main menu");
                    }
                    else if (userInput == "ls")
                        Console.WriteLine(await server.CmdListAllUsers());
                    else if (userInput == "create")
                    {
                        string userName = readUserInput($"{userMenu} create $ username>");
                        string password = readUserInput($"{userMenu} create $ password>");
                        string musiclibarypath = readUserInput($"{userMenu} update $ musiclibarypath>");
                        Console.WriteLine(await server.CmdCreateUser(userName, password, musiclibarypath));
                    }
                    else if (userInput == "update")
                    {
                        string id = readUserInput($"{userMenu} update $ id>");
                        if (int.TryParse(id, out int userid))
                        {
                            string field = readUserInput($"{userMenu} update $ field>");
                            string newValue = readUserInput($"{userMenu} update $ value>");
                            Console.WriteLine(await server.CmdUpdateUser(userid, field, newValue));
                        }
                    }
                    else if (userInput == "delete")
                    {
                        string id = readUserInput($"{userMenu} delete $ id>");
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