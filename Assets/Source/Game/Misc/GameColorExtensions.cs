using UnityEngine;

namespace Assets.Source.Game.Misc {
    static class GameColorExtensions {
        public static Color ToBoardColor(this GameColor gameColor) {
            return gameColor == GameColor.Black ? Color.gray : new Color(0.8f, 0.8f, 0.8f, 1f);
        }

        public static Color ToPawnColor(this GameColor gameColor) {
            return gameColor == GameColor.Black ? Color.black : Color.white;
        }
    }
}
