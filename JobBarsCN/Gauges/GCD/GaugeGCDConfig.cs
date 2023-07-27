using ImGuiNET;
using JobBars.UI;

namespace JobBars.Gauges.GCD {
    public struct GaugeSubGCDProps {
        public int MaxCounter;
        public float MaxDuration;
        public Item[] Triggers;
        public ElementColor Color;
        public bool Invert;
        public string SubName;
        public Item[] Increment;
        public GaugeCompleteSoundType CompletionSound;
    }

    public struct GaugeGCDProps {
        public GaugeSubGCDProps[] SubGCDs;
    }

    public class GaugeGCDConfig : GaugeConfig {
        public class GaugeSubGCDConfig {
            public readonly string Name;

            public readonly string SubName;
            public readonly float MaxDuration;
            public readonly Item[] Triggers;
            public readonly Item[] Increment;
            public int MaxCounter;
            public ElementColor Color;
            public bool Invert;
            public GaugeCompleteSoundType CompletionSound;
            public bool ReverseFill;

            public GaugeSubGCDConfig(string name, GaugeSubGCDProps props) {
                Name = name;

                SubName = props.SubName;
                MaxDuration = props.MaxDuration;
                Triggers = props.Triggers;
                Increment = props.Increment;
                MaxCounter = JobBarsCN.Config.GaugeMaxGcds.Get(Name, props.MaxCounter);
                Color = JobBarsCN.Config.GaugeColor.Get(Name, props.Color);
                Invert = JobBarsCN.Config.GaugeInvert.Get(Name, props.Invert);
                CompletionSound = JobBarsCN.Config.GaugeCompletionSound.Get(Name, props.CompletionSound);
                ReverseFill = JobBarsCN.Config.GaugeReverseFill.Get(Name, false);
            }
        }

        // ===========================

        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar, GaugeVisualType.Arrow, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugeSubGCDConfig[] SubGCDs { get; private set; }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeSubGCDProps subConfig) : this(name, type, new GaugeGCDProps {
            SubGCDs = new[] { subConfig }
        }) { }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeGCDProps props) : base(name, type) {
            SubGCDs = new GaugeSubGCDConfig[props.SubGCDs.Length];
            for (int i = 0; i < SubGCDs.Length; i++) {
                var id = string.IsNullOrEmpty(props.SubGCDs[i].SubName) ? Name : Name + "/" + props.SubGCDs[i].SubName;
                SubGCDs[i] = new GaugeSubGCDConfig(id, props.SubGCDs[i]);
            }
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeGCDTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            DrawSoundEffect();
            DrawCompletionSoundEffect();

            foreach (var subGCD in SubGCDs) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

                var suffix = string.IsNullOrEmpty(subGCD.SubName) ? "" : $" ({subGCD.SubName})";

                if (JobBarsCN.Config.GaugeColor.Draw($"颜色{suffix}{id}", subGCD.Name, subGCD.Color, out var newColor)) {
                    subGCD.Color = newColor;
                    newVisual = true;
                }

                if (JobBarsCN.Config.GaugeMaxGcds.Draw($"GCD最大值{suffix}{id}", subGCD.Name, subGCD.MaxCounter, out var newMax)) {
                    if (newMax <= 0) newMax = 1;
                    if (newMax > UIArrow.MAX ) newMax = UIArrow.MAX;
                    subGCD.MaxCounter = newMax;
                    newVisual = true;
                }

                if (JobBarsCN.Config.GaugeInvert.Draw($"相反走向{suffix}{id}", subGCD.Name, subGCD.Invert, out var newInvert)) {
                    subGCD.Invert = newInvert;
                }

                if (JobBarsCN.Config.GaugeCompletionSound.Draw($"完成音效{suffix}{id}", subGCD.Name, ValidSoundType, subGCD.CompletionSound, out var newCompletionSound)) {
                    subGCD.CompletionSound = newCompletionSound;
                }

                if (JobBarsCN.Config.GaugeReverseFill.Draw($"相反填充{suffix}{id}", subGCD.Name, subGCD.ReverseFill, out var newReverseFill)) {
                    subGCD.ReverseFill = newReverseFill;
                    newVisual = true;
                }
            }
        }
    }
}
