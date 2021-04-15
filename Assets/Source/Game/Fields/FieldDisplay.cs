using Assets.Source.Game.Misc;
using UnityEngine;

class FieldDisplay : MonoBehaviour {
    [SerializeField] private SpriteRenderer sr;
    private GameColor fieldColor;
    public PawnDisplay PawnDisplay => CheckboardDisplay.Instance.GetPawnDisplay(new Vector2Int((int)transform.position.x, (int)transform.position.y));

    public void Init(GameColor fieldColor, Vector2 position) {
        this.fieldColor = fieldColor;
        sr.color = fieldColor.ToBoardColor();
        transform.position = position;
    }

    public void Select() {
        sr.color = Color.yellow;
    }

    public void Deselect() {
        sr.color = fieldColor.ToBoardColor();
    }
}
