using System;

namespace Assets.Source.Game.Gameplays {
    internal interface IPlayer {
        public event EventHandler<Move> SingleMoveMadeEvent;
        public event EventHandler TurnFinishedEvent;
        public void MoveInit();
        public void MoveUpdate();
    }
}