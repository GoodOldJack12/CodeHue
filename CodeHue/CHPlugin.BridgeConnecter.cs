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
        private static ILocalHueClient _client;
        public static async Task BridgeConnection()
        {
            //Checking Connection
            bool check = await BridgeConnecter.getClient().CheckConnection();
            if (check) return;
            
            //Finding Bridge
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            LyokoLogger.Log("CodeHue","Bridge located.");

            //Checking Application Registering
            _client = new LocalHueClient(bridgeIPs.First().IpAddress);
            var appKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "appKey.txt");

            if (System.IO.File.Exists(appKeyPath))
            {
                appKey = System.IO.File.ReadAllText(appKeyPath);
            }

            if (appKey.Length == 0)
            {
                LyokoLogger.Log("CodeHue", "Please press your Bridge's link button.");
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
            _client.Initialize(appKey);
            var command = new LightCommand();
            command.Alert = Alert.Once;
            await _client.SendCommandAsync(command);
            LyokoLogger.Log("CodeHue", "Successfully connected to the Bridge.");
        }
        
        public static async void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                appKey = await _client.RegisterAsync("CodeHue_Test2", "Computer");
            }
            catch (Exception exc)
            {
                LyokoLogger.Log("CodeHue", "Button not pressed. Trying again...");
            }
        }

        public static ILocalHueClient getClient()
        {
            return _client;
        }
    }
}