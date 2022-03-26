using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ArkEcho.WebPage
{
    /* TODO
	 * Anzeige
			-> Interpreten A-Z -> Doppel & dreifach Interpreten zusammenfassen
			-> Album A-Z
			-> Titel A-Z
			-> Ordner	-> Baumstruktur, ganzen Ordner abspielen
	*/
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Singleton und Port per Config
            WebHost.CreateDefaultBuilder()
                               .UseUrls($"https://*:5001")
                               .UseKestrel()
                               .UseStartup<Startup>()
                               .Build()
                               .Run();
        }
    }
}
