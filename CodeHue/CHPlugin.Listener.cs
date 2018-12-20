using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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
        private static bool _listening;

        public Listener()
        {
            _listening = false;
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

        public async Task SendCommand(Color color)
        {
            var command = new LightCommand();
            if (color == Color.Black)
                command.TurnOff();
            else
                command.TurnOn().SetColor(new RGBColor(color.ToString()));

            await BridgeConnecter.getClient().SendCommandAsync(command);
        }
    }
}