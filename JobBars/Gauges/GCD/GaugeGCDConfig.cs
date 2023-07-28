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

            public readonly string 子名称;
            public readonly float MaxDuration;
            public readonly Item[] Triggers;
            public readonly Item[] Increment;
            public int MaxCounter;
            public ElementColor 颜色;
            public bool Invert;
            public GaugeCompleteSoundType CompletionSound;
            public bool ReverseFill;

            public GaugeSubGCDConfig(string name, GaugeSubGCDProps props) {
                Name = name;

                子名称 = props.SubName;
                MaxDuration = props.MaxDuration;
                Triggers = props.Triggers;
                Increment = props.Increment;
                MaxCounter = JobBarsCN.设置.GaugeMaxGcds.Get(Name, props.MaxCounter);
                颜色 = JobBarsCN.设置.量谱颜色.Get(Name, props.Color);
                Invert = JobBarsCN.设置.GaugeInvert.Get(Name, props.Invert);
                CompletionSound = JobBarsCN.设置.量谱填满音效.Get(Name, props.CompletionSound);
                ReverseFill = JobBarsCN.设置.GaugeReverseFill.Get(Name, false);
            }
        }

        // ===========================

        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.条状, GaugeVisualType.Arrow, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugeSubGCDConfig[] SubGCDs { get; private set; }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeSubGCDProps subConfig) : this(name, type, new GaugeGCDProps {
            SubGCDs = new[] { subConfig }
        }) { }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeGCDProps props) : base(name, type) {
            SubGCDs = new GaugeSubGCDConfig[props.SubGCDs.Length];
            for (int i = 0; i < SubGCDs.Length; i++) {
                var id = string.IsNullOrEmpty(props.SubGCDs[i].SubName) ? 名称 : 名称 + "/" + props.SubGCDs[i].SubName;
                SubGCDs[i] = new GaugeSubGCDConfig(id, props.SubGCDs[i]);
            }
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeGCDTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            DrawSoundEffect();
            DrawCompletionSoundEffect();

            foreach (var subGCD in SubGCDs) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

                var suffix = string.IsNullOrEmpty(subGCD.子名称) ? "" : $" ({subGCD.子名称})";

                if (JobBarsCN.设置.量谱颜色.Draw($"颜色{suffix}{id}", subGCD.Name, subGCD.颜色, out var newColor)) {
                    subGCD.颜色 = newColor;
                    newVisual = true;
                }

                if (JobBarsCN.设置.GaugeMaxGcds.Draw($"最大GCDs{suffix}{id}", subGCD.Name, subGCD.MaxCounter, out var newMax)) {
                    if (newMax <= 0) newMax = 1;
                    if (newMax > UIArrow.MAX ) newMax = UIArrow.MAX;
                    subGCD.MaxCounter = newMax;
                    newVisual = true;
                }

                if (JobBarsCN.设置.GaugeInvert.Draw($"相反填充{suffix}{id}", subGCD.Name, subGCD.Invert, out var newInvert)) {
                    subGCD.Invert = newInvert;
                }

                if (JobBarsCN.设置.量谱填满音效.Draw($"完成音效{suffix}{id}", subGCD.Name, 生效音效类型, subGCD.CompletionSound, out var newCompletionSound)) {
                    subGCD.CompletionSound = newCompletionSound;
                }

                if (JobBarsCN.设置.GaugeReverseFill.Draw($"相反填充{suffix}{id}", subGCD.Name, subGCD.ReverseFill, out var newReverseFill)) {
                    subGCD.ReverseFill = newReverseFill;
                    newVisual = true;
                }
            }
        }
    }
}
