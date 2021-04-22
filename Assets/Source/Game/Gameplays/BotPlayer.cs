using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Gameplays {
    class BotPlayer : IPlayer {
        public event EventHandler<Move> SingleMoveMadeEvent;
        public event EventHandler TurnFinishedEvent;

        private readonly int depth;
        private readonly GameColor pawnColor;

        public BotPlayer(GameColor pawnColor, int depth) {
            this.pawnColor = pawnColor;
            this.depth = depth;
        }

        public void MoveInit() {
            Gameplay.Instance.StartCoroutine(Move());
        }

        IEnumerator Move() {
            var state = new State { Checkboard = Gameplay.Instance.Checkboard };
            bool isTake;
            Pawn movingPawn = null;

            do {
                var availableMoves = new List<(Move move, int heuristic)>();
                yield return new WaitForSeconds(0.25f);
                var alfaBeta = AlfaBeta(state, depth, int.MinValue, int.MaxValue, true, ref availableMoves);
                Move move = null;

                var moves = availableMoves
                    .Where(m => m.heuristic == alfaBeta);

                if (movingPawn != null) {
                    move = moves.First(m => m.move.PawnPos == movingPawn.Position).move ?? moves.First().move;
                } else {
                    move = moves.First().move;
                }

                isTake = move.IsATake;
                movingPawn = Gameplay.Instance.Checkboard[move.PawnPos];

                yield return new WaitForSeconds(0.25f);
                SingleMoveMadeEvent(this, move);

                while (Gameplay.Instance.IsPawnInMove)
                    yield return null;

            } while (isTake && HasAnyTakes(movingPawn));

            TurnFinishedEvent(this, null);
        }

        private bool HasAnyTakes(Pawn pawn) {
            var myPawnTakeMoves = pawn.GetAvailableMoves(Gameplay.Instance.Checkboard)?.Where(m => m != null && m.IsATake)?.Any() == true;
            return myPawnTakeMoves;
        }

        public void MoveUpdate() {
            return;
        }

        int AlfaBeta(State state, int depth, int alfa, int beta, bool maxMove, ref List<(Move move, int heuristic)> moves) {
            if (state.Checkboard.IsTerminal(out _) || depth == 0) {
                var heuristic = state.Checkboard.GetHeuristic(pawnColor);
                return heuristic;
            }
            var currentMovingColor = maxMove ? pawnColor : (pawnColor == GameColor.Black ? GameColor.White : GameColor.Black);
            var successors = state.Checkboard.GetSuccessors(currentMovingColor, state).ToList();

            if (maxMove) {
                var value = int.MinValue;
                foreach (var successor in successors) {
                    value = Math.Max(value, AlfaBeta(successor, depth - 1, alfa, beta, maxMove: successor.IsInTakeStrike, ref moves));
                    alfa = Math.Max(alfa, value);

                    if (depth == this.depth) {
                        moves.Add((successor.LastMove, value));
                    }

                    if (alfa >= beta) {
                        break;
                    }
                }

                return value;
            } else {
                var value = int.MaxValue;
                foreach (var successor in successors) {
                    value = Math.Min(value, AlfaBeta(successor, depth - 1, alfa, beta, maxMove: !successor.IsInTakeStrike, ref moves));
                    beta = Math.Min(beta, value);

                    if (depth == this.depth) {
                        moves.Add((successor.LastMove, value));
                    }

                    if (beta <= alfa) {
                        break;
                    }
                }

                return value;
            }
        }
    }
}
