namespace JobBars.Gauges.Types.Bar {
    public class GaugeBarConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }
        public bool SwapText { get; private set; }
        public bool Vertical { get; private set; }

        public GaugeBarConfig(string name) : base(name) {
            ShowText = JobBarsCN.Config.GaugeShowText.Get(Name);
            SwapText = JobBarsCN.Config.GaugeSwapText.Get(Name);
            Vertical = JobBarsCN.Config.GaugeVertical.Get(Name);
        }

        public override void Draw(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.Config.GaugeShowText.Draw($"显示文本{id}", Name, ShowText, out var newShowText)) {
                ShowText = newShowText;
                newVisual = true;
            }

            if (JobBarsCN.Config.GaugeSwapText.Draw($"调换文本位置{id}", Name, SwapText, out var newSwapText)) {
                SwapText = newSwapText;
                newVisual = true;
            }

            if (JobBarsCN.Config.GaugeVertical.Draw($"竖置{id}", Name, Vertical, out var newVertical)) {
                Vertical = newVertical;
                newVisual = true;
            }
        }
    }
}
