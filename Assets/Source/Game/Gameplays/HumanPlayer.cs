using Assets.Source.Game.Checkboards;
using Assets.Source.Game.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Gameplays {
    class HumanPlayer : IPlayer {
        public event EventHandler<Move> SingleMoveMadeEvent;
        public event EventHandler TurnFinishedEvent;

        private readonly GameColor pawnColor;
        private readonly Camera camera;
        private readonly CheckboardDisplay checkboardDisplay;
        private readonly List<FieldDisplay> selectedFields;

        private PawnDisplay selectedPawn;
        private IEnumerable<Move> startingForcedTakes;

        private bool isInTakeStrike = false;

        public HumanPlayer(GameColor pawnColor, CheckboardDisplay checkboardDisplay) {
            camera = Camera.main;
            this.checkboardDisplay = checkboardDisplay;
            this.pawnColor = pawnColor;
            selectedFields = new List<FieldDisplay>();
        }

        private Checkboard Checkboard => Gameplay.Instance.Checkboard;

        public void MoveInit() {
            startingForcedTakes = GetAllMyTakes();
        }

        public void MoveUpdate() {
            if (Gameplay.Instance.IsPawnInMove)
                return;

            if (Input.GetButtonUp("Fire1")) {
                if (CheckIfPawnClicked(out var pawnDisplay) && pawnColor == pawnDisplay.Pawn.Color && !isInTakeStrike) {
                    bool pawnAlreadySelected = false;
                    if (selectedPawn) {
                        pawnAlreadySelected = selectedPawn == pawnDisplay;
                        DeselectPawnAndFields(pawnDisplay);
                    }

                    if (!pawnAlreadySelected)
                        TrySelectPawn(pawnDisplay);

                } else if (selectedPawn) {
                    var availableMoves = GetAvailableMoves(selectedPawn);
                    var clickWorldPos = GetClickPosition(Input.mousePosition);

                    var selectedPawnCopy = selectedPawn;

                    if (!isInTakeStrike)
                        DeselectPawnAndFields(pawnDisplay);

                    Gameplay.Instance.StartCoroutine(TryMakeMove(availableMoves, clickWorldPos, selectedPawnCopy));
                }
            }
        }

        private IEnumerator TryMakeMove(IEnumerable<Move> availableMoves, Vector2Int clickWorldPos, PawnDisplay selectedPawnCopy) {
            var move = availableMoves.FirstOrDefault(m => m.NewPos == clickWorldPos);
            if (move != null) {

                if (isInTakeStrike)
                    DeselectPawnAndFields(selectedPawnCopy);

                startingForcedTakes = null;
                SingleMoveMadeEvent(this, move);

                while (Gameplay.Instance.IsPawnInMove)
                    yield return null;

                if (move.IsATake) {
                    availableMoves = GetAvailableMoves(selectedPawnCopy);

                    if (availableMoves?.Any(m => m.IsATake) != true) {
                        TurnFinishedEvent(this, null);
                        isInTakeStrike = false;
                    } else {
                        isInTakeStrike = true;
                        TrySelectPawn(selectedPawnCopy);
                    }

                } else {
                    TurnFinishedEvent(this, null);
                    isInTakeStrike = false;
                }
            }
        }

        private IEnumerable<Move> GetAllMyTakes() {
            var myPawnTakeMoves = Checkboard.GetPawns(pawnColor)
                .SelectMany(p => p.GetAvailableMoves(Checkboard)?
                    .Where(m => m.IsATake) ?? Enumerable.Empty<Move>())
                .ToList();

            return myPawnTakeMoves;
        }

        private Vector2Int GetClickPosition(Vector3 mousePosition) {
            var worldPoint = camera.ScreenToWorldPoint(mousePosition);
            return ((Vector2)worldPoint).ToVector2Int();
        }

        private bool CheckIfPawnClicked(out PawnDisplay pawnDisplay) {
            var worldPos = GetClickPosition(Input.mousePosition);
            pawnDisplay = checkboardDisplay.GetPawnDisplay(worldPos, tryGet: true);
            return pawnDisplay;
        }

        private void TrySelectPawn(PawnDisplay pawnDisplay) {
            if (TrySelectMoves(pawnDisplay)) {
                selectedPawn = pawnDisplay;
                selectedPawn.Select();
            }
        }

        private void DeselectPawnAndFields(PawnDisplay pawnDisplay) {
            selectedFields.ForEach(sf => sf.Deselect());
            selectedFields.Clear();

            selectedPawn.Deselect();
            selectedPawn = pawnDisplay;
            selectedPawn = null;
        }

        public bool TrySelectMoves(PawnDisplay pawnDisplay) {
            if (startingForcedTakes?.Any() == true) {
                if (!startingForcedTakes.Any(t => t.PawnPos == pawnDisplay.Pawn.Position)) {
                    return false;
                }
            }

            var moves = GetAvailableMoves(pawnDisplay);
            if (moves != null) {
                selectedFields.Clear();
                foreach (var move in moves) {
                    var fieldDisplay = CheckboardDisplay.Instance.GetFieldDisplay(move.NewPos);
                    fieldDisplay.Select();
                    selectedFields.Add(fieldDisplay);
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
