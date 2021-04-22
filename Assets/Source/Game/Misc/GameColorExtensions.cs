using UnityEngine;

namespace Assets.Source.Game.Misc {
    static class GameColorExtensions {
        public static Color ToBoardColor(this GameColor gameColor) {
            return gameColor == GameColor.Black ? new Color(98 / 255f, 73 / 255f, 62 / 255f, 1) : new Color(214 / 255f, 181 / 255f, 145 / 255f, 1);
        }

        public static Color ToPawnColor(this GameColor gameColor) {
            return gameColor == GameColor.Black ? new Color(34 / 255f, 15 / 255f, 20 / 255f, 1) : new Color(250 / 255f, 209 / 255f, 164 / 255f, 1);
        }
    }
}
