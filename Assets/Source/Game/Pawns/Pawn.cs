using Assets.Source.Game.Checkboards;
using Assets.Source.Game.Gameplays;
using Assets.Source.Game.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Pawns {
    class Pawn {
        public Pawn(GameColor color, Vector2Int position) {
            Position = position;
            Color = color;
        }

        private Pawn() { }

        public static Pawn DeepCopy(Pawn original) {
            return new Pawn {
                Color = original.Color,
                IsKing = original.IsKing,
                Position = original.Position
            };
        }

        public GameColor Color { get; private set; }
        public bool IsKing { get; set; }
        public Vector2Int Position { get; set; }

        public IEnumerable<Move> GetAvailableMoves(Checkboard checkboard) {
            var availableTakes = GetAvailableTakes(checkboard);
            if (availableTakes?.Any() == true) {
                return availableTakes;
            }

            var normalMoves = GetAvailableNormalMoves(checkboard);
            if (normalMoves?.Any() == true) {
                return normalMoves;
            }

            return null;
        }

        private IEnumerable<Move> GetAvailableTakes(Checkboard checkboard) {
            var rd = Position.MoveRD();
            if (CheckTake(rd, rd.MoveRD(), checkboard, out Move takeRd)) {
                yield return takeRd;
            }

            var ru = Position.MoveRU();
            if (CheckTake(ru, ru.MoveRU(), checkboard, out Move takeRu)) {
                yield return takeRu;
            }

            var ld = Position.MoveLD();
            if (CheckTake(ld, ld.MoveLD(), checkboard, out Move takeLd)) {
                yield return takeLd;
            }

            var lu = Position.MoveLU();
            if (CheckTake(lu, lu.MoveLU(), checkboard, out Move takeLu)) {
                yield return takeLu;
            }
        }

        private bool CheckTake(Vector2Int takePos, Vector2Int posAfterTake, Checkboard checkboard, out Move move) {
            move = null;
            if (checkboard.Exists(takePos) &&
                !checkboard.IsEmptyAndExists(takePos) &&
                checkboard[takePos].Color != Color &&
                checkboard.IsEmptyAndExists(posAfterTake)) {
                move = new Move { NewPos = posAfterTake, PawnPos = Position };
                return true;
            }

            return false;
        }

        private IEnumerable<Move> GetAvailableNormalMoves(Checkboard checkboard) {
            List<Move> availableMoves = new List<Move>();

            if (Gameplay.Instance.LowerPawnsColor == Color) {
                var ru = Position.MoveRU();
                if (CheckNormalMove(ru, checkboard, out Move moveRu)) {
                    availableMoves.Add(moveRu);
                }

                var lu = Position.MoveLU();
                if (CheckNormalMove(lu, checkboard, out Move moveLu)) {
                    availableMoves.Add(moveLu);
                }
            } else {
                var rd = Position.MoveRD();
                if (CheckNormalMove(rd, checkboard, out Move moveRd)) {
                    availableMoves.Add(moveRd);
                }

                var ld = Position.MoveLD();
                if (CheckNormalMove(ld, checkboard, out Move moveLd)) {
                    availableMoves.Add(moveLd);
                }
            }

            return availableMoves.Any() ? availableMoves : null;
        }

        private bool CheckNormalMove(Vector2Int movePos, Checkboard checkboard, out Move move) {
            move = null;
            if (checkboard.IsEmptyAndExists(movePos)) {
                move = new Move { NewPos = movePos, PawnPos = Position };
                return true;
            }

            return false;
        }
    }
}
