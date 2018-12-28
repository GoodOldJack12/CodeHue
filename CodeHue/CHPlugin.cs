using System;
using System.Runtime.Remoting.Lifetime;
using LyokoAPI.Events;
using LyokoAPI.Plugin;

namespace CodeHue
{
    public class CHPlugin : LyokoAPIPlugin
    {
        public override string Name { get; } = "CHPlugin";
        public override string Author { get; } = "Appryl";
        public Listener hueListener;

        protected override bool OnEnable()
        {
            LyokoLogger.Log("CodeHue", "Launching CodeHue...");
            SetUp();
            return true;
        }

        private async void SetUp()
        {
            await BridgeConnecter.BridgeConnection().ConfigureAwait(false);
            if (BridgeConnecter.Connected)
            {
                this.Disable();
            }
            else
            {
                hueListener = new Listener();
            }
        }

        protected override bool OnDisable()
        {
            hueListener.StopListening();
            return true;
        }

        public override void OnGameStart(bool storyMode)
        {
        }

        public override void OnGameEnd(bool failed)
        {
        }
    }
}