using System;

namespace Assets.Source.Game.Gameplays {
    internal interface IPlayer {
        public event EventHandler SingleMoveMadeEvent;
        public event EventHandler TurnFinishedEvent;
        public void MoveUpdate();
    }
}