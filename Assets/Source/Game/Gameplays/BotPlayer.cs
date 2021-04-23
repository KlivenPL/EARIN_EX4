using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Gameplays {
    // AI Bot player
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
                var alfaBeta = AlfaBeta(state, depth, int.MinValue, int.MaxValue, true, availableMoves);
                Move move = null;

                // Get available moves with heuristic given by AlfaBeta algorithm
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

                // Make chosen move
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

        //Core algorithm: Minimax with alhpa-beta pruning
        int AlfaBeta(State state, int depth, int alfa, int beta, bool maxMove, List<(Move move, int heuristic)> moves) { // regular algorithm parameters plus the list of moves (in the initial depth) with corresponding heuristics
            if (state.Checkboard.IsTerminal(out _) || depth == 0) { //check for terminal state of the checkboard or depth reached
                var heuristic = state.Checkboard.GetHeuristic(pawnColor); //get herustic of the leaf
                return heuristic;
            }
            var currentMovingColor = maxMove ? pawnColor : (pawnColor == GameColor.Black ? GameColor.White : GameColor.Black); // converting current player (min or max) to corresponding pawn color
            var successors = state.Checkboard.GetSuccessors(currentMovingColor, state).ToList(); // get the list of all potential checkboard states after a given player makes move

            if (maxMove) {
                var value = int.MinValue; // assign conventional minus infinity to a variable
                foreach (var successor in successors) {
                    value = Math.Max(value, AlfaBeta(successor, depth - 1, alfa, beta, maxMove: successor.IsInTakeStrike, moves));
                    alfa = Math.Max(alfa, value);

                    if (depth == this.depth) {
                        moves.Add((successor.LastMove, value)); // for the initial depth, so the first (from the root) level we save the moves, so after the algorithm returns the best heuristic, we know which path to choose
                    }

                    if (alfa >= beta) { // pruning part: responsible for avoiding unnecessary iterations
                        break;
                    }
                }

                return value;
            } else {
                var value = int.MaxValue;
                foreach (var successor in successors) {
                    value = Math.Min(value, AlfaBeta(successor, depth - 1, alfa, beta, maxMove: !successor.IsInTakeStrike, moves));
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
