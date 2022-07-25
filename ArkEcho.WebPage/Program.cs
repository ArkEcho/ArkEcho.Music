using System;

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
            using (WebPageManager.Instance)
            {
                if (!WebPageManager.Instance.Init())
                {
                    Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
                    return;
                }
                else
                    WebPageManager.Instance.Start(); // Starts Event Cycle and API
            }
        }
    }
}
