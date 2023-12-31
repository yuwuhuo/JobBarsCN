﻿namespace JobBars.Gauges.Types.BarDiamondCombo {
    public class GaugeBarDiamondComboConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }

        public GaugeBarDiamondComboConfig(string name) : base(name) {
            ShowText = JobBarsCN.设置.量谱显示文本.Get(Name);
        }

        public override void Draw(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.设置.量谱显示文本.Draw($"显示计时{id}", Name, ShowText, out var newShowText)) {
                ShowText = newShowText;
                newVisual = true;
            }
        }
    }
}
