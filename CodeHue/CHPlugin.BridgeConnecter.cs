using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using LyokoAPI.Events;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace CodeHue
{
    public class BridgeConnecter
    {
        private static string appKey = "";
        public static async Task BridgeConnection(ILocalHueClient client)
        {
        
            Console.WriteLine("Test");
            //Finding Bridge
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            LyokoLogger.Log("CodeHue","Bridge located.");

            //Checking Application Registering
            client = new LocalHueClient(bridgeIPs.First().IpAddress);
            var appKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "appKey.txt");

            if (System.IO.File.Exists(appKeyPath))
            {
                appKey = System.IO.File.ReadAllText(appKeyPath);
            }

            if (appKey.Length == 0)
            {
                LyokoLogger.Log("CodeHue", "Please press your Bridge's link button. Once done, press any key on your computer.");
                Timer myTimer = new Timer();
                myTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
                myTimer.Interval = 5000;
                myTimer.Start();
                while (appKey.Length == 0)
                {
                }
                myTimer.Stop();
                System.IO.File.WriteAllText(appKeyPath, appKey);
                LyokoLogger.Log("CodeHue", "Key registered.");
            }

            //Connecting To Bridge
            client.Initialize(appKey);
            LyokoLogger.Log("CodeHue", "Successfully connected to the Bridge.");
        }
        
        public async void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                appKey = await client.RegisterAsync("CodeHue_Test2", "Computer");
            }
            catch (Exception exc)
            {
                Console.WriteLine("Button not pressed. Trying again...");
            }
        }
    }
}