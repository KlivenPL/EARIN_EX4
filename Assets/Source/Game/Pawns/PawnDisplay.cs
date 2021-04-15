﻿using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using UnityEngine;

class PawnDisplay : MonoBehaviour {
    [SerializeField] private SpriteRenderer sr;

    private GameColor color;
    private bool isKing;

    public bool IsKing => isKing;

    public void Select() {
        sr.color = Color.cyan;
    }

    public void Deselect() {
        sr.color = color.ToPawnColor();
    }

    public Pawn Init(GameColor pawnColor, Vector2 position) {
        color = pawnColor;
        sr.color = pawnColor.ToPawnColor();
        transform.position = position;

        return new Pawn(pawnColor, new Vector2Int((int)position.x, (int)position.y));
    }

    public Pawn Pawn => Gameplay.Instance.Checkboard[(int)transform.position.x, (int)transform.position.y];

    public void SetKing() {
        sr.transform.Find("Crown").gameObject.SetActive(true);
        isKing = true;
    }

    public void TakeDown() {
        Destroy(gameObject);
    }
}