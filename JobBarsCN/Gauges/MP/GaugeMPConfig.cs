using JobBars.UI;

namespace JobBars.Gauges.MP {
    public class GaugeMPConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public float[] Segments { get; private set; }
        public ElementColor Color { get; private set; }
        public bool ShowSegments { get; private set; }

        public GaugeMPConfig(string name, GaugeVisualType type, float[] segments, bool defaultDisabled = false) : base(name, type) {
            Segments = segments;
            if (defaultDisabled)
                Enabled = JobBarsCN.Config.GaugeEnabled.Get(Name, false); // default disabled
            Color = JobBarsCN.Config.GaugeColor.Get(Name, UIColor.MpPink);
            ShowSegments = JobBarsCN.Config.GaugeShowSegments.Get(Name);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeMPTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (Segments != null) {
                if (JobBarsCN.Config.GaugeShowSegments.Draw($"显示分段{id}", Name, ShowSegments, out var newShowSegments)) {
                    ShowSegments = newShowSegments;
                    reset = true;
                }
            }

            if (JobBarsCN.Config.GaugeColor.Draw($"颜色{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }
        }
    }
}
