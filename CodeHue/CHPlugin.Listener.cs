using System.Drawing;
using LyokoAPI.Events;
using LyokoAPI.VirtualStructures;
using LyokoAPI.VirtualStructures.Interfaces;

namespace CodeHue
{
    public partial class CHPlugin
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
                if (_listening) 
                {
                    return;
                }
                
                TowerActivationEvent.Subscribe(OnTowerActivation);
                TowerDeactivationEvent.Subscribe(OnTowerDeactivation);

                _listening = true;
            }

            public void StopListening()
            {
                if (!_listening)
                {
                    return;
                }
                
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

        }
    }
}