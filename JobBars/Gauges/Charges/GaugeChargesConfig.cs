using JobBars.UI;

namespace JobBars.Gauges.Charges {
    public struct GaugeChargesProps {
        public GaugesChargesPartProps[] Parts;
        public ElementColor BarColor;
        public bool SameColor;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public struct GaugesChargesPartProps {
        public Item[] 触发;
        public float 持续时间;
        public float CD;
        public bool 栏;
        public bool 棱形;
        public int MaxCharges;
        public ElementColor Color;
    }

    public class GaugeChargesConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.条状, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugesChargesPartProps[] Parts { get; private set; }
        public bool SameColor { get; private set; }
        public ElementColor BarColor { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ReverseFill { get; private set; }

        public GaugeChargesConfig(string name, GaugeVisualType type, GaugeChargesProps props) : base(name, type) {
            Parts = props.Parts;
            SameColor = props.SameColor;
            BarColor = JobBarsCN.设置.量谱颜色.Get(名称, props.BarColor);
            CompletionSound = JobBarsCN.设置.量谱填满音效.Get(名称, props.CompletionSound);
            ReverseFill = JobBarsCN.设置.GaugeReverseFill.Get(名称, false);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeChargesTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.设置.量谱颜色.Draw($"颜色{id}", 名称, BarColor, out var newColor)) {
                BarColor = newColor;
                newVisual = true;
            }

            if (JobBarsCN.设置.量谱填满音效.Draw($"完成音效{id}", 名称, 生效音效类型, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            DrawCompletionSoundEffect();
            DrawSoundEffect();

            if (JobBarsCN.设置.GaugeReverseFill.Draw($"相反填充{id}", 名称, ReverseFill, out var newReverseFill)) {
                ReverseFill = newReverseFill;
                newVisual = true;
            }
        }
    }
}
