using JobBars.UI;
using System;

namespace JobBars.Gauges.Rolling {
    public enum GaugeGCDRollingType {
        GCD,
        CastTime
    }

    public class GaugeRollingConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.条状 };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public static readonly GaugeGCDRollingType[] ValidRollingType = (GaugeGCDRollingType[])Enum.GetValues(typeof(GaugeGCDRollingType));

        public ElementColor Color { get; private set; }
        public GaugeGCDRollingType RollingType { get; private set; }

        public GaugeRollingConfig(string name, GaugeVisualType type) : base(name, type) {
            Enabled = JobBarsCN.设置.GaugeEnabled.Get(名称, false); // default disabled
            Color = JobBarsCN.设置.量谱颜色.Get(名称, UIColor.Yellow);
            RollingType = JobBarsCN.设置.GaugeGCDRolling.Get(名称, GaugeGCDRollingType.GCD);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeRollingTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.设置.GaugeGCDRolling.Draw($"数据类型{id}", 名称, ValidRollingType, RollingType, out var newRollingType)) {
                RollingType = newRollingType;
                newVisual = true;
            }

            if (JobBarsCN.设置.量谱颜色.Draw($"颜色{id}", 名称, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }
        }
    }
}
