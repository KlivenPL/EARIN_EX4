using Assets.Source.Game.Checkboards;

namespace Assets.Source.Game.Gameplays {
    struct State {
        public bool IsInTakeStrike { get; set; }
        public Move LastMove { get; set; }
        public Checkboard Checkboard { get; set; }
    }
}
