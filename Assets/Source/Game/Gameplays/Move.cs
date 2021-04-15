using UnityEngine;

namespace Assets.Source.Game.Gameplays {
    class Move {
        public bool IsATake => TakePos.HasValue;
        public Vector2Int PawnPos { get; set; }
        public Vector2Int NewPos { get; set; }
        public Vector2Int? TakePos { get; set; }
    }
}
