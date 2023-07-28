using JobBars.UI;

namespace JobBars.Gauges.Stacks {
    public struct GaugeStacksProps {
        public int MaxStacks;
        public Item[] Triggers;
        public ElementColor Color;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public class GaugeStacksConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public int MaxStacks { get; private set; }
        public Item[] Triggers { get; private set; }
        public ElementColor Color { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ReverseFill { get; private set; }

        public GaugeStacksConfig(string name, GaugeVisualType type, GaugeStacksProps props) : base(name, type) {
            MaxStacks = props.MaxStacks;
            Triggers = props.Triggers;
            Color = JobBarsCN.Config.GaugeColor.Get(Name, props.Color);
            CompletionSound = JobBarsCN.Config.GaugeCompletionSound.Get(Name, props.CompletionSound);
            ReverseFill = JobBarsCN.Config.GaugeReverseFill.Get(Name, false);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeStacksTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.Config.GaugeColor.Draw($"颜色{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }

            if (JobBarsCN.Config.GaugeCompletionSound.Draw($"完成音效{id}", Name, ValidSoundType, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            DrawCompletionSoundEffect();
            DrawSoundEffect("切换音效");

            if (JobBarsCN.Config.GaugeReverseFill.Draw($"相反填充{id}", Name, ReverseFill, out var newReverseFill)) {
                ReverseFill = newReverseFill;
                newVisual = true;
            }
        }
    }
}
