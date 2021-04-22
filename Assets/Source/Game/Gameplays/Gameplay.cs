using Assets.Source.Game.Checkboards;
using Assets.Source.Game.Gameplays;
using Assets.Source.Game.Misc;
using System.Collections;
using UnityEngine;
class Gameplay : MonoBehaviour {
    [SerializeField] private CheckboardDisplay checkboardDisplay;
    private static Gameplay instance;
    public static Gameplay Instance => instance;
    public Checkboard Checkboard { get; private set; }
    public GameColor TurnColor { get; private set; }
    public GameColor LowerPawnsColor { get; private set; }

    private IPlayer whitePlayer;
    private IPlayer blackPlayer;

    public bool IsPawnInMove { get; set; }

    void Start() {
        InitalizeGame();
    }

    private void InitalizeGame() {
        instance = this;
        TurnColor = GameColor.White;
        LowerPawnsColor = Random.value > 0.5f ? GameColor.White : GameColor.Black;
        var pawns = checkboardDisplay.Init();
        Checkboard = new Checkboard(pawns);

        if (LowerPawnsColor == GameColor.White) {
            whitePlayer = new HumanPlayer(GameColor.White, checkboardDisplay);
            blackPlayer = new BotPlayer(GameColor.Black, Menu.Instance.Depth);
        } else {
            whitePlayer = new BotPlayer(GameColor.White, Menu.Instance.Depth);
            blackPlayer = new HumanPlayer(GameColor.Black, checkboardDisplay);
        }

        whitePlayer.SingleMoveMadeEvent += OnPlayerSingleMoveMade;
        whitePlayer.TurnFinishedEvent += OnTurnFinished;

        blackPlayer.SingleMoveMadeEvent += OnPlayerSingleMoveMade;
        blackPlayer.TurnFinishedEvent += OnTurnFinished;

        whitePlayer.MoveInit();
    }

    private void Update() {
        if (!IsPawnInMove) {
            if (TurnColor == GameColor.White) {
                whitePlayer.MoveUpdate();
            } else {
                blackPlayer.MoveUpdate();
            }
        }
    }

    private void OnPlayerSingleMoveMade(object sender, Move move) {
        IsPawnInMove = true;
        checkboardDisplay.MakeMove(move, callback: () => {
            Checkboard.MakeMove(move);
            IsPawnInMove = false;
        });
    }

    private void OnTurnFinished(object sender, System.EventArgs e) {
        StartCoroutine(OnTurnFinished());
    }

    private IEnumerator OnTurnFinished() {
        while (IsPawnInMove)
            yield return null;

        if (Checkboard.IsTerminal(out var winColor)) {
            Debug.Log($"{winColor} won!");
            Menu.Instance?.DisplayWin(winColor);
            yield break;
        }

        if (TurnColor == GameColor.White) {
            TurnColor = GameColor.Black;
            blackPlayer.MoveInit();
        } else {
            TurnColor = GameColor.White;
            whitePlayer.MoveInit();
        }
    }
}
