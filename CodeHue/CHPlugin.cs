using System;
using LyokoAPI.Events;
using LyokoAPI.Plugin;

namespace CodeHue
{
    public class CHPlugin : LyokoAPIPlugin
    {
        public override string Name { get; } = "CHPlugin";
        public override string Author { get; } = "Appryl";

        protected override bool OnEnable()
        {
            LyokoLogger.Log("CodeHue", "Launching CodeHue...");
            //listener here
            return true;
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
    }
}