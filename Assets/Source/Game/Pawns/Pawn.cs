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
            var availableTakes = IsKing ? GetAvailableKingTakes(checkboard) : GetAvailableTakes(checkboard);
            if (availableTakes?.Any() == true) {
                return availableTakes.ToList();
            }

            var normalMoves = IsKing ? GetAvailableNormalKingMoves(checkboard) : GetAvailableNormalMoves(checkboard);
            if (normalMoves?.Any() == true) {
                return normalMoves.ToList();
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

        private IEnumerable<Move> GetAvailableKingTakes(Checkboard checkboard) {

            foreach (var move in CheckKingTakes(Position, Vector2IntExtensions.MoveRD, checkboard)) {
                yield return move;
            }

            foreach (var move in CheckKingTakes(Position, Vector2IntExtensions.MoveLD, checkboard)) {
                yield return move;
            }

            foreach (var move in CheckKingTakes(Position, Vector2IntExtensions.MoveRU, checkboard)) {
                yield return move;
            }

            foreach (var move in CheckKingTakes(Position, Vector2IntExtensions.MoveLU, checkboard)) {
                yield return move;
            }
        }

        private bool CheckTake(Vector2Int takePos, Vector2Int posAfterTake, Checkboard checkboard, out Move move) {
            move = null;
            if (checkboard.Exists(takePos) &&
                !checkboard.IsEmptyAndExists(takePos) &&
                checkboard[takePos].Color != Color &&
                checkboard.IsEmptyAndExists(posAfterTake)) {
                move = new Move { NewPos = posAfterTake, PawnPos = Position, TakePos = takePos };
                return true;
            }

            return false;
        }

        private IEnumerable<Move> CheckKingTakes(Vector2Int startPos, System.Func<Vector2Int, Vector2Int> moveFunc, Checkboard checkboard) {
            var checkPos = startPos;
            var prevPos = startPos;
            while (checkboard.Exists(checkPos)) {
                checkPos = moveFunc(checkPos);
                var posAfterTake = moveFunc(checkPos);

                if (!(checkboard.IsEmptyAndExists(prevPos) || checkboard[prevPos].Position == startPos)) {
                    yield break;
                }

                if (CheckTake(checkPos, posAfterTake, checkboard, out var move)) {
                    yield return move;
                    var endPos = moveFunc(move.NewPos);

                    while (checkboard.IsEmptyAndExists(endPos)) {
                        yield return new Move {
                            NewPos = endPos,
                            PawnPos = move.PawnPos,
                            TakePos = move.TakePos,
                        };

                        endPos = moveFunc(endPos);
                    }

                    yield break;
                }
                prevPos = moveFunc(prevPos);
            }
        }

        private IEnumerable<Move> GetAvailableNormalMoves(Checkboard checkboard) {
            if (Gameplay.Instance.LowerPawnsColor == Color) {
                var ru = Position.MoveRU();
                if (CheckNormalMove(ru, checkboard, out Move moveRu)) {
                    yield return moveRu;
                }

                var lu = Position.MoveLU();
                if (CheckNormalMove(lu, checkboard, out Move moveLu)) {
                    yield return moveLu;
                }

            } else {

                var rd = Position.MoveRD();
                if (CheckNormalMove(rd, checkboard, out Move moveRd)) {
                    yield return moveRd;
                }

                var ld = Position.MoveLD();
                if (CheckNormalMove(ld, checkboard, out Move moveLd)) {
                    yield return moveLd;
                }
            }
        }

        private IEnumerable<Move> GetAvailableNormalKingMoves(Checkboard checkboard) {
            foreach (var move in CheckKingMoves(Position, Vector2IntExtensions.MoveRD, checkboard)) {
                yield return move;
            }

            foreach (var move in CheckKingMoves(Position, Vector2IntExtensions.MoveLD, checkboard)) {
                yield return move;
            }

            foreach (var move in CheckKingMoves(Position, Vector2IntExtensions.MoveRU, checkboard)) {
                yield return move;
            }

            foreach (var move in CheckKingMoves(Position, Vector2IntExtensions.MoveLU, checkboard)) {
                yield return move;
            }
        }

        private bool CheckNormalMove(Vector2Int movePos, Checkboard checkboard, out Move move) {
            move = null;
            if (checkboard.IsEmptyAndExists(movePos)) {
                move = new Move { NewPos = movePos, PawnPos = Position };
                return true;
            }

            return false;
        }

        private IEnumerable<Move> CheckKingMoves(Vector2Int startPos, System.Func<Vector2Int, Vector2Int> moveFunc, Checkboard checkboard) {
            var checkPos = moveFunc(startPos);
            while (checkboard.IsEmptyAndExists(checkPos)) {
                yield return new Move {
                    NewPos = checkPos,
                    PawnPos = startPos,
                };

                checkPos = moveFunc(checkPos);
            }
        }
    }
}
