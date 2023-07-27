using JobBars.UI;
using System;

namespace JobBars.Gauges.Rolling {
    public enum GaugeGCDRollingType {
        GCD,
        CastTime
    }

    public class GaugeRollingConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public static readonly GaugeGCDRollingType[] ValidRollingType = (GaugeGCDRollingType[])Enum.GetValues(typeof(GaugeGCDRollingType));

        public ElementColor Color { get; private set; }
        public GaugeGCDRollingType RollingType { get; private set; }

        public GaugeRollingConfig(string name, GaugeVisualType type) : base(name, type) {
            Enabled = JobBarsCN.Config.GaugeEnabled.Get(Name, false); // default disabled
            Color = JobBarsCN.Config.GaugeColor.Get(Name, UIColor.Yellow);
            RollingType = JobBarsCN.Config.GaugeGCDRolling.Get(Name, GaugeGCDRollingType.GCD);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeRollingTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.Config.GaugeGCDRolling.Draw($"数据类型{id}", Name, ValidRollingType, RollingType, out var newRollingType)) {
                RollingType = newRollingType;
                newVisual = true;
            }

            if (JobBarsCN.Config.GaugeColor.Draw($"颜色{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }
        }
    }
}
