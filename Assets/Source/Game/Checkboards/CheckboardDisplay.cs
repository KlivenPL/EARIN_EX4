using Assets.Source.Game.Gameplays;
using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CheckboardDisplay : MonoBehaviour {
    [SerializeField] CheckboardGenerator checkboardGenerator;

    private static CheckboardDisplay instance;
    public static CheckboardDisplay Instance => instance;

    private FieldDisplay[,] fields;
    private List<PawnDisplay> pawns;

    public FieldDisplay GetFieldDisplay(Vector2Int position) {
        if (position.x < 8 && position.y < 8 && position.x >= 0 && position.y >= 0) {
            return fields[position.x, position.y];
        }

        return null;
    }

    public PawnDisplay GetPawnDisplay(Vector2Int position, bool tryGet = false) {
        if (tryGet)
            return pawns.SingleOrDefault(p => p.Pawn?.Position == position);

        return pawns.Single(p => p.Pawn.Position == position);
    }

    public void MakeMove(Move move) {
        var pawnDisplay = GetPawnDisplay(move.PawnPos);
        var pawn = pawnDisplay.Pawn;

        pawnDisplay.transform.position = move.NewPos.ToVector2();

        if (move.NewPos.y == 0 && Gameplay.Instance.LowerPawnsColor != pawn.Color ||
                move.NewPos.y == 7 && Gameplay.Instance.LowerPawnsColor == pawn.Color) {

            pawnDisplay.SetKing();
        }

        if (move.IsATake) {
            var takenPawn = GetPawnDisplay(move.TakePos.Value, tryGet: true);
            pawns.Remove(takenPawn);
            takenPawn.TakeDown();
        }
    }

    public IEnumerable<Pawn> Init() {
        instance = this;
        var pawns = checkboardGenerator.Generate().ToList();

        fields = new FieldDisplay[8, 8];
        var fieldDisplays = FindObjectsOfType<FieldDisplay>();

        foreach (var fieldDisplay in fieldDisplays) {
            fields[(int)fieldDisplay.transform.position.x, (int)fieldDisplay.transform.position.y] = fieldDisplay;
        }

        this.pawns = FindObjectsOfType<PawnDisplay>().ToList();

        return pawns.ToList();
    }
}
