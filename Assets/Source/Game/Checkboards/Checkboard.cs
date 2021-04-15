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
            if (move.IsATake) {
                this[move.TakePos.Value] = null;
            }
        }

        public Checkboard Simulate(Move[] moves) {
            var checkboard = DeepCopy(this);
            foreach (var move in moves) {
                checkboard.MakeMove(move);
            }

            return checkboard;
        }
    }
}
