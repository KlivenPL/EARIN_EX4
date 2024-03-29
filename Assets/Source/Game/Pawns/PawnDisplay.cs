﻿using Assets.Source.Game.Misc;
using Assets.Source.Game.Pawns;
using UnityEngine;

class PawnDisplay : MonoBehaviour {
    [SerializeField] private SpriteRenderer sr;

    private GameColor pawnColor;

    public Pawn Init(GameColor pawnColor, Vector2 position) {
        this.pawnColor = pawnColor;
        sr.color = pawnColor.ToPawnColor();
        transform.position = position;

        return new Pawn(pawnColor, new Vector2Int((int)position.x, (int)position.y));
    }

    public void Select() {
        sr.color = pawnColor.ToPawnColor() - (Color.blue / 4f);
    }

    public void Deselect() {
        sr.color = pawnColor.ToPawnColor();
    }

    public Pawn Pawn => Gameplay.Instance.Checkboard[(int)transform.position.x, (int)transform.position.y];

    public void SetKing() {
        sr.transform.Find("Crown").gameObject.SetActive(true);
    }

    public void TakeDown() {
        Destroy(gameObject);
    }

    public void MoveToFront() {
        sr.sortingOrder = 10;
        sr.transform.Find("Crown").GetComponent<SpriteRenderer>().sortingOrder = 11;
    }

    public void MoveToBack() {
        sr.sortingOrder = default;
        sr.transform.Find("Crown").GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
}