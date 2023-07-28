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
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.条状, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public int MaxStacks { get; private set; }
        public Item[] Triggers { get; private set; }
        public ElementColor Color { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ReverseFill { get; private set; }

        public GaugeStacksConfig(string name, GaugeVisualType type, GaugeStacksProps props) : base(name, type) {
            MaxStacks = props.MaxStacks;
            Triggers = props.Triggers;
            Color = JobBarsCN.设置.量谱颜色.Get(名称, props.Color);
            CompletionSound = JobBarsCN.设置.量谱填满音效.Get(名称, props.CompletionSound);
            ReverseFill = JobBarsCN.设置.GaugeReverseFill.Get(名称, false);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeStacksTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.设置.量谱颜色.Draw($"颜色{id}", 名称, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }

            if (JobBarsCN.设置.量谱填满音效.Draw($"完成音效{id}", 名称, 生效音效类型, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            DrawCompletionSoundEffect();
            DrawSoundEffect("切换音效");

            if (JobBarsCN.设置.GaugeReverseFill.Draw($"相反填充{id}", 名称, ReverseFill, out var newReverseFill)) {
                ReverseFill = newReverseFill;
                newVisual = true;
            }
        }
    }
}
