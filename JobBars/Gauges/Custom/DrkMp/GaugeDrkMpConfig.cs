using JobBars.Gauges.MP;
using JobBars.UI;

namespace JobBars.Gauges.Custom {
    public struct GaugeDrkMpProps {
        public float[] Segments;
        public ElementColor Color;
        public ElementColor DarkArtsColor;
    }

    public class GaugeDrkMPConfig : GaugeMPConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.条状, GaugeVisualType.BarDiamondCombo, GaugeVisualType.Diamond, GaugeVisualType.Arrow };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public ElementColor DarkArtsColor { get; private set; }

        private string DarkArtsName => 名称 + "/DarkArts";

        public GaugeDrkMPConfig(string name, GaugeVisualType type, GaugeDrkMpProps props) : base(name, type, props.Segments) {
            DarkArtsColor = JobBarsCN.设置.量谱颜色.Get(DarkArtsName, props.DarkArtsColor);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeDrkMPTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            base.DrawConfig(id, ref newVisual, ref reset);

            if (JobBarsCN.设置.量谱颜色.Draw($"跳蓝颜色{id}", 名称, 颜色, out var newDarkArtsColor)) {
                DarkArtsColor = newDarkArtsColor;
                newVisual = true;
            }
        }
    }
}
