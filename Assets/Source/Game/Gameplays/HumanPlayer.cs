using Assets.Source.Game.Checkboards;
using Assets.Source.Game.Misc;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Gameplays {
    class HumanPlayer : IPlayer {
        public event EventHandler SingleMoveMadeEvent;
        public event EventHandler TurnFinishedEvent;

        private readonly GameColor pawnColor;
        private readonly Camera camera;
        private readonly CheckboardDisplay checkboardDisplay;
        private PawnDisplay selectedPawn;

        public HumanPlayer(GameColor pawnColor, CheckboardDisplay checkboardDisplay) {
            camera = Camera.main;
            this.checkboardDisplay = checkboardDisplay;
            this.pawnColor = pawnColor;
        }

        private Checkboard Checkboard => Gameplay.Instance.Checkboard;
        private GameColor CurrentTurnColor => Gameplay.Instance.TurnColor;

        public void MoveUpdate() {
            if (Input.GetButtonUp("Fire1")) {
                if (CheckIfPawnClicked(out var pawnDisplay) && !selectedPawn && pawnColor == pawnDisplay.Pawn.Color) {
                    TrySelectPawn(pawnDisplay);
                } else if (selectedPawn) {
                    DeselectPawn(pawnDisplay);
                }
            }
        }

        private bool CheckIfPawnClicked(out PawnDisplay pawnDisplay) {
            var worldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
            var worldPointInt = new Vector2Int((int)worldPoint.x, (int)worldPoint.y);

            pawnDisplay = checkboardDisplay.GetPawnDisplay(worldPointInt);
            return pawnDisplay;
        }

        private void TrySelectPawn(PawnDisplay pawnDisplay) {
            if (TrySelectMoves(pawnDisplay)) {
                selectedPawn = pawnDisplay;
                selectedPawn.Select();
            }
        }

        private void DeselectPawn(PawnDisplay pawnDisplay) {
            selectedPawn.Deselect();
            selectedPawn = pawnDisplay;
            selectedPawn = null;
        }

        public bool TrySelectMoves(PawnDisplay pawnDisplay) {
            var moves = GetAvailableMoves(pawnDisplay);
            if (moves != null) {
                foreach (var move in moves) {
                    CheckboardDisplay.Instance.GetFieldDisplay(move.NewPos).Select();
                }

                return true;
            }

            return false;
        }

        private IEnumerable<Move> GetAvailableMoves(PawnDisplay pawnDisplay) {
            return pawnDisplay.Pawn.GetAvailableMoves(Checkboard);
        }
    }
}
