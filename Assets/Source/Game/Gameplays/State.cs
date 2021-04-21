using Assets.Source.Game.Checkboards;

namespace Assets.Source.Game.Gameplays {
    struct State {
        public Move LastMove { get; set; }
        public Checkboard Checkboard { get; set; }
    }
}
