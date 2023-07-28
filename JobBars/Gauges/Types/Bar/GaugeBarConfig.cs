namespace JobBars.Gauges.Types.Bar {
    public class GaugeBarConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }
        public bool SwapText { get; private set; }
        public bool Vertical { get; private set; }

        public GaugeBarConfig(string name) : base(name) {
            ShowText = JobBarsCN.设置.量谱显示文本.Get(Name);
            SwapText = JobBarsCN.设置.GaugeSwapText.Get(Name);
            Vertical = JobBarsCN.设置.GaugeVertical.Get(Name);
        }

        public override void Draw(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.设置.量谱显示文本.Draw($"显示文本{id}", Name, ShowText, out var newShowText)) {
                ShowText = newShowText;
                newVisual = true;
            }

            if (JobBarsCN.设置.GaugeSwapText.Draw($"调换文本位置{id}", Name, SwapText, out var newSwapText)) {
                SwapText = newSwapText;
                newVisual = true;
            }

            if (JobBarsCN.设置.GaugeVertical.Draw($"竖置{id}", Name, Vertical, out var newVertical)) {
                Vertical = newVertical;
                newVisual = true;
            }
        }
    }
}
