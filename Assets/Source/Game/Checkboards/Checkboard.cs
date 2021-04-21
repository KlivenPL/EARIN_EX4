using Assets.Source.Game.Gameplays;
using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Checkboards {
    class Checkboard {
        private readonly Pawn[,] pawns;

        public Checkboard(IEnumerable<Pawn> pawns) {
            this.pawns = new Pawn[8, 8];

            foreach (var pawn in pawns) {
                this.pawns[pawn.Position.x, pawn.Position.y] = pawn;
            }
        }

        public Pawn this[int x, int y] {
            get => x < 8 && y < 8 && x >= 0 && y >= 0 ? pawns[x, y] : null;
            private set => pawns[x, y] = value;
        }

        public Pawn this[Vector2Int pos] {
            get => this[pos.x, pos.y];
            private set => this[pos.x, pos.y] = value;
        }

        public bool Exists(int x, int y) {
            return x < 8 && y < 8 && x >= 0 && y >= 0;
        }

        public bool Exists(Vector2Int pos) {
            return Exists(pos.x, pos.y);
        }

        public bool IsEmptyAndExists(int x, int y) {
            return x < 8 && y < 8 && x >= 0 && y >= 0 && pawns[x, y] == null;
        }

        public bool IsEmptyAndExists(Vector2Int pos) {
            return IsEmptyAndExists(pos.x, pos.y);
        }

        public IEnumerable<Pawn> GetPawns(GameColor color) {
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    if (!IsEmptyAndExists(x, y) && this[x, y].Color == color) {
                        yield return this[x, y];
                    }
                }
            }
        }

        public IEnumerable<Pawn> GetAllPawns() {
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    if (!IsEmptyAndExists(x, y)) {
                        yield return this[x, y];
                    }
                }
            }
        }

        public static Checkboard DeepCopy(Checkboard original) {
            var pawns = original.GetAllPawns().Select(p => Pawn.DeepCopy(p)).ToList();
            return new Checkboard(pawns);
        }

        public void MakeMove(Move move) {
            var pawn = this[move.PawnPos];
            pawn.Position = move.NewPos;
            this[move.NewPos] = pawn;
            this[move.PawnPos] = null;

            if (!pawn.IsKing) {
                if (pawn.Position.y == 0 && Gameplay.Instance.LowerPawnsColor != pawn.Color ||
                    pawn.Position.y == 7 && Gameplay.Instance.LowerPawnsColor == pawn.Color) {

                    pawn.IsKing = true;
                }
            }

            if (move.IsATake) {
                this[move.TakePos.Value] = null;
            }
        }

        public IEnumerable<State> GetSuccessors(GameColor playerColor/*, Move previousMove*/) {
            var moves = GetPawns(playerColor).SelectMany(p => p.GetAvailableMoves(this) ?? Enumerable.Empty<Move>());

            /* if (previousMove?.IsATake == true) {
                 moves = moves.Where(m => m.PawnPos == previousMove.NewPos);
             }*/

            if (moves.Any(m => m.IsATake)) {
                moves = moves.Where(m => m.IsATake);
            }

            foreach (var move in moves) {
                while (true) {
                    var copiedCheckboard = DeepCopy(this);
                    copiedCheckboard.MakeMove(move);

                    var myPawn = copiedCheckboard[move.NewPos];

                    var availableTakes = GetAvailableTakes(myPawn, copiedCheckboard);

                    if (availableTakes?.Any() == true) {

                    }
                }
                // jezeli tu jest bicie dalej, to wykonuj
                yield return new State { Checkboard = copiedCheckboard, LastMove = move };
            }


        }

        private IEnumerable<Move> FindAllTakes(Checkboard checkboard, Pawn pawn) {
            var availableTakes = GetAvailableTakes(checkboard, pawn);
            if (availableTakes.Any()) {
                foreach (var availableTake in availableTakes) {
                    var copiedCheckboard = DeepCopy(this);

                }
            } else {

            }


            IEnumerable<Move> GetAvailableTakes(Checkboard checkboard, Pawn pawn) => pawn.GetAvailableMoves(checkboard)?.Where(m => m.IsATake) ?? Enumerable.Empty<Move>();
        }

        public bool IsTerminal(out GameColor winColor) {
            winColor = default;

            if (GetPawns(GameColor.Black)?.Any() != true) {
                winColor = GameColor.White;
                return true;
            }

            if (GetPawns(GameColor.White)?.Any() != true) {
                winColor = GameColor.Black;
                return true;
            }

            return false;
        }

        public int GetHeuristic(GameColor playerColor) {
            var oppositeColor = playerColor == GameColor.Black ? GameColor.White : GameColor.Black;
            var myPawns = GetPawns(playerColor);
            var pawnsDiff = myPawns.Count() - GetPawns(oppositeColor).Count();
            var kings = myPawns.Where(p => p.IsKing).Count();

            return pawnsDiff + kings * 4;
        }
    }
}
