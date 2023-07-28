using JobBars.UI;

namespace JobBars.Gauges.MP {
    public class GaugeMPConfig : GaugeConfig {
        private static readonly GaugeVisualType[] 有效量谱视觉类型 = new[] { GaugeVisualType.条状 };
        protected override GaugeVisualType[] GetValidGaugeTypes() => 有效量谱视觉类型;

        public float[] Segments { get; private set; }
        public ElementColor 颜色 { get; private set; }
        public bool ShowSegments { get; private set; }

        public GaugeMPConfig(string name, GaugeVisualType type, float[] segments, bool defaultDisabled = false) : base(name, type) {
            Segments = segments;
            if (defaultDisabled)
                Enabled = JobBarsCN.设置.GaugeEnabled.Get(名称, false); // default disabled
            颜色 = JobBarsCN.设置.量谱颜色.Get(名称, UIColor.法条色);
            ShowSegments = JobBarsCN.设置.GaugeShowSegments.Get(名称);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeMPTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (Segments != null) {
                if (JobBarsCN.设置.GaugeShowSegments.Draw($"分段显示{id}", 名称, ShowSegments, out var newShowSegments)) {
                    ShowSegments = newShowSegments;
                    reset = true;
                }
            }

            if (JobBarsCN.设置.量谱颜色.Draw($"颜色{id}", 名称, 颜色, out var newColor)) {
                颜色 = newColor;
                newVisual = true;
            }
        }
    }
}
