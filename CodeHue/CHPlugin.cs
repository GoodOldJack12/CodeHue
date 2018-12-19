using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Configuration;
using System.Threading.Tasks;
using LyokoAPI.Plugin;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using static System.String;

namespace CodeHue
{
    public partial class CHPlugin : LyokoAPIPlugin
    {
        public override string Name { get; } = "CHPlugin";
        public override string Author { get; } = "Appryl";
        private ILocalHueClient client;

        protected override bool OnEnable()
        {
            Console.WriteLine("Launching CodeHue...");
            BridgeConnection();
            Listener hueListener = new Listener();
        }

        protected override bool OnDisable()
        {
            throw new NotImplementedException();
        }

        public override void OnGameStart(bool storyMode)
        {
        }

        public override void OnGameEnd(bool failed)
        {
        }

        public async Task BridgeConnection()
        {
            //Finding Bridge
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            Console.WriteLine("Bridge located.");

            //Checking Application Registering
            client = new LocalHueClient(bridgeIPs.First().IpAddress);
            string appKey = "";
            var appKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "appKey.txt");
            Console.WriteLine(appKeyPath);

            if (System.IO.File.Exists(appKeyPath))
            {
                appKey = System.IO.File.ReadAllText(appKeyPath);
            }

            if (appKey.Length == 0)
            {
                Console.WriteLine("Please press your Bridge's link button. Once done, press any key on your computer.");
                Console.ReadKey();
                appKey = await client.RegisterAsync("CodeHue", "Computer");
                System.IO.File.WriteAllText(appKeyPath, appKey);
                Console.WriteLine("Key registered.");
            }

            //Connecting To Bridge
            client.Initialize(appKey);
            Console.WriteLine("Successfully connected to the Bridge.");
        }
    }
}