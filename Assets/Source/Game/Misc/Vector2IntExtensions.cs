using UnityEngine;

namespace Assets.Source.Game.Misc {
    static class Vector2IntExtensions {
        public static Vector2Int MoveRU(this Vector2Int pos) => pos + new Vector2Int(1, 1);
        public static Vector2Int MoveRD(this Vector2Int pos) => pos + new Vector2Int(1, -1);
        public static Vector2Int MoveLU(this Vector2Int pos) => pos + new Vector2Int(-1, 1);
        public static Vector2Int MoveLD(this Vector2Int pos) => pos + new Vector2Int(-1, -1);
    }
}
