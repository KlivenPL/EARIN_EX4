using Assets.Source.Game.Checkboards;
using Assets.Source.Game.Gameplays;
using Assets.Source.Game.Misc;
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

    void Start() {
        InitalizeGame();
    }

    private void InitalizeGame() {
        instance = this;
        TurnColor = GameColor.White;
        LowerPawnsColor = Random.value > 0.5f ? GameColor.White : GameColor.Black;
        var pawns = checkboardDisplay.Init();
        Checkboard = new Checkboard(pawns);

        whitePlayer = new HumanPlayer(GameColor.White, checkboardDisplay);
        blackPlayer = new HumanPlayer(GameColor.Black, checkboardDisplay);

        whitePlayer.SingleMoveMadeEvent += OnPlayerSingleMoveMade;
        whitePlayer.TurnFinishedEvent += OnTurnFinished;

        blackPlayer.SingleMoveMadeEvent += OnPlayerSingleMoveMade;
        blackPlayer.TurnFinishedEvent += OnTurnFinished;

        whitePlayer.MoveInit();

        var whiteKing = Checkboard[0, 2];
        whiteKing.IsKing = true;
        checkboardDisplay.GetPawnDisplay(whiteKing.Position).SetKing();

        var blackKing = Checkboard[7, 5];
        blackKing.IsKing = true;
        checkboardDisplay.GetPawnDisplay(blackKing.Position).SetKing();
    }

    private void Update() {
        if (TurnColor == GameColor.White) {
            whitePlayer.MoveUpdate();
        } else {
            blackPlayer.MoveUpdate();
        }
    }

    private void OnPlayerSingleMoveMade(object sender, Move move) {
        checkboardDisplay.MakeMove(move);
        Checkboard.MakeMove(move);
    }

    private void OnTurnFinished(object sender, System.EventArgs e) {
        if (TurnColor == GameColor.White) {
            TurnColor = GameColor.Black;
            blackPlayer.MoveInit();
        } else {
            TurnColor = GameColor.White;
            whitePlayer.MoveInit();
        }
    }
}
