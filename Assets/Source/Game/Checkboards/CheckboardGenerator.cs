using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using System.Collections.Generic;
using UnityEngine;

// Generating the checkerboard
class CheckboardGenerator : MonoBehaviour {

    [SerializeField] private FieldDisplay fieldPrefab;
    [SerializeField] private PawnDisplay pawnPrefab;
    [SerializeField] private Transform boardParent;

    public IEnumerable<Pawn> Generate() {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                var squareColor = (x + y) % 2 == 0 ? GameColor.Black : GameColor.White;
                var squarePos = new Vector2(x, y);

                var fieldDisplay = Instantiate(fieldPrefab, boardParent, false);
                fieldDisplay.Init(squareColor, squarePos);

                if (squareColor == GameColor.Black && (y < 3 || y > 4)) {
                    GameColor pawnColor;

                    if (y < 4)
                        pawnColor = Gameplay.Instance.LowerPawnsColor == GameColor.Black ? GameColor.Black : GameColor.White;
                    else
                        pawnColor = Gameplay.Instance.LowerPawnsColor == GameColor.White ? GameColor.Black : GameColor.White;

                    var pawnGo = Instantiate(pawnPrefab, boardParent, false);
                    var pawn = pawnGo.Init(pawnColor, squarePos);

                    yield return pawn;
                }
            }
        }
    }
}