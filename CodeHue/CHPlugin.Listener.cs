using System.Drawing;
using System.Threading.Tasks;
using LyokoAPI.Events;
using LyokoAPI.VirtualStructures;
using LyokoAPI.VirtualStructures.Interfaces;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;

namespace CodeHue
{
    public class Listener
    {
        private bool _listening;

        public Listener()
        {
            _listening = false;
            StartListening();
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

            TowerActivationEvent.Unsubscribe(OnTowerActivation);
            TowerDeactivationEvent.Unsubscribe(OnTowerDeactivation);
        }

        private async void OnTowerActivation(ITower tower)
        {
            string color = "000000"; //color class
            switch (tower.Activator)
            {
                case APIActivator.XANA:
                    color = "e02828";
                    break;
                case APIActivator.JEREMIE:
                    color = "23af38";
                    break;
                case APIActivator.HOPPER:
                    color = "ffffff";
                    break;
            }

            await SendCommand(color);
        }

        private async void OnTowerDeactivation(ITower tower)
        {
            string color;
            color = "000000";
            await SendCommand(color);
        }

        public async Task SendCommand(string color)
        {
            var command = new LightCommand();
            if (color == "000000")
                command.TurnOff();
            else
                command.TurnOn().SetColor(new RGBColor(color));

            await BridgeConnecter.getClient().SendCommandAsync(command);
        }
    }
}