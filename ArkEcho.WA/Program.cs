using ArkEcho.WebPage;

namespace ArkEcho.WA
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
        public static async Task Main(string[] args)
        {
            using (WebPageManager.Instance)
            {
                if (!await WebPageManager.Instance.Init())
                {
                    Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
                    return;
                }
                else
                    await WebPageManager.Instance.Start(); // Starts Event Cycle and API
            }
        }
    }
}