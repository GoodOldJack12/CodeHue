using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LyokoAPI.Events;
using LyokoAPI.VirtualStructures;
using LyokoAPI.VirtualStructures.Interfaces;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace CodeHue
{
    public class Listener
    {
        private ILocalHueClient client;
        private static bool _listening;

        public Listener()
        {
            _listening = false;
            BridgeConnection();
        }

        public void StartListening()
        {
            if (_listening) return;

            TowerActivationEvent.Subscribe(OnTowerActivation);
            TowerDeactivationEvent.Subscribe(OnTowerDeactivation);

            _listening = true;
        }

        public void StopListening()
        {
            if (!_listening) return;

            TowerActivationEvent.UnSubscribe(OnTowerActivation);
            TowerDeactivationEvent.UnSubscribe(OnTowerDeactivation);
        }

        private async void OnTowerActivation(ITower tower)
        {
            Color color; //color class
            switch (tower.Activator)
            {
                case APIActivator.XANA:
                    color = Color.Red;
                    break;
                case APIActivator.JEREMIE:
                    color = Color.Green;
                    break;
                case APIActivator.HOPPER:
                    color = Color.White;
                    break;
            }

            await SendCommand(color);
        }

        private async void OnTowerDeactivation(ITower tower)
        {
            Color color;
            color = Color.Black;
            await SendCommand(color);
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

        public async Task SendCommand(Color color)
        {
            var command = new LightCommand();
            if (color == Color.Black)
                command.TurnOff();
            else
                command.TurnOn().SetColor(new RGBColor(color.ToString()));

            await client.SendCommandAsync(command);
        }
    }
}