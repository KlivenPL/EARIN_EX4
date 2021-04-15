using Assets.Source.Game.Pawns;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CheckboardDisplay : MonoBehaviour {
    [SerializeField] CheckboardGenerator checkboardGenerator;

    private static CheckboardDisplay instance;
    public static CheckboardDisplay Instance => instance;

    private FieldDisplay[,] fields;
    private IEnumerable<PawnDisplay> pawns;

    public FieldDisplay GetFieldDisplay(Vector2Int position) {
        if (position.x < 8 && position.y < 8 && position.x >= 0 && position.y >= 0) {
            return fields[position.x, position.y];
        }

        return null;
    }

    public PawnDisplay GetPawnDisplay(Vector2Int position) {
        return pawns.SingleOrDefault(p => p.transform.position == new Vector3(position.x, position.y));
    }

    public IEnumerable<Pawn> Init() {
        instance = this;
        var pawns = checkboardGenerator.Generate().ToList();

        fields = new FieldDisplay[8, 8];
        var fieldDisplays = FindObjectsOfType<FieldDisplay>();

        foreach (var fieldDisplay in fieldDisplays) {
            fields[(int)fieldDisplay.transform.position.x, (int)fieldDisplay.transform.position.y] = fieldDisplay;
        }

        this.pawns = FindObjectsOfType<PawnDisplay>().AsEnumerable();

        return pawns.ToList();
    }
}
