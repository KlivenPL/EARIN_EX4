using Assets.Source.Game.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Game.Gameplays {
    class BotPlayer : IPlayer {
        public event EventHandler<Move> SingleMoveMadeEvent;
        public event EventHandler TurnFinishedEvent;

        private int Depth;

        private readonly GameColor pawnColor;

        public BotPlayer(GameColor pawnColor, int depth) {
            this.pawnColor = pawnColor;
            Depth = depth;
        }

        public void MoveInit() {
            Gameplay.Instance.StartCoroutine(Move());
        }

        IEnumerator Move() {
            var state = new State { Checkboard = Gameplay.Instance.Checkboard };
            bool isTake;
            do {
                yield return null;// new WaitForSeconds(0.5f);
                var availableMoves = new List<(Move move, int heuristic)>();
                var alfaBeta = AlfaBeta(state, Depth, int.MinValue, int.MaxValue, true, ref availableMoves);

                var move = availableMoves
                    .Where(m => m.heuristic == alfaBeta)
                    .First().move;

                isTake = move.IsATake;
                SingleMoveMadeEvent(this, move);
            } while (isTake && HasAnyTakes());

            TurnFinishedEvent(this, null);
        }

        private bool HasAnyTakes() {
            var myPawnTakeMoves = Gameplay.Instance.Checkboard.GetPawns(pawnColor)
                .SelectMany(p => p.GetAvailableMoves(Gameplay.Instance.Checkboard)?
                    .Where(m => m.IsATake) ?? Enumerable.Empty<Move>())
                .Any();

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

            var successors = state.Checkboard.GetSuccessors(maxMove ? pawnColor : (pawnColor == GameColor.Black ? GameColor.White : GameColor.White), state.LastMove).ToList();

            if (maxMove) {
                var value = int.MinValue;
                foreach (var successor in successors) {
                    value = AlfaBeta(successor, depth - 1, alfa, beta, false, ref moves);
                    if (depth == Depth) {
                        moves.Add((successor.LastMove, value));
                    }
                    alfa = Math.Max(alfa, value);
                    if (alfa >= beta) {
                        break;
                    }
                }

                return value;
            } else {
                var value = int.MaxValue;
                foreach (var successor in successors) {
                    value = AlfaBeta(successor, depth - 1, alfa, beta, true, ref moves);
                    if (depth == Depth) {
                        moves.Add((successor.LastMove, value));
                    }
                    beta = Math.Min(beta, value);
                    if (beta <= alfa) {
                        break;
                    }
                }

                return value;
            }
        }
    }
}
