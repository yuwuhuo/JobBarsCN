using ImGuiNET;
using JobBars.Data;
using JobBars.UI;

namespace JobBars.Gauges.Procs {
    public struct GaugeProcProps {
        public bool 显示文本;
        public ProcConfig[] 进程;
        public GaugeCompleteSoundType 进程音效;
    }

    public class ProcConfig {
        public readonly string 名称;
        public readonly Item[] 触发;

        public ElementColor 颜色;
        public int 顺序;

        public ProcConfig(string name, BuffIds buff, ElementColor color) : this(name, new Item(buff), color) { }
        public ProcConfig(string name, ActionIds action, ElementColor color) : this(name, new Item(action), color) { }
        public ProcConfig(string name, Item trigger, ElementColor color) : this(name, new[] { trigger }, color) { }
        public ProcConfig(string name, Item[] triggers, ElementColor color) {
            名称 = name;
            触发 = triggers;
            颜色 = JobBarsCN.设置.量谱进程颜色.Get(名称, color);
            顺序 = JobBarsCN.设置.量谱进程顺序.Get(名称);
        }
    }

    public class GaugeProcsConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public ProcConfig[] 进程 { get; private set; }
        public bool 进程显示文本 { get; private set; }
        public GaugeCompleteSoundType 进程音效 { get; private set; }

        public GaugeProcsConfig(string name, GaugeVisualType type, GaugeProcProps props) : base(name, type) {
            进程 = props.进程;
            进程显示文本 = JobBarsCN.设置.量谱显示文本.Get(名称, props.显示文本);
            进程音效 = JobBarsCN.设置.量谱填满音效.Get(名称, props.进程音效);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeProcsTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBarsCN.设置.量谱显示文本.Draw($"显示倒计时{id}", 名称, 进程显示文本, out var newProcsShowText)) {
                进程显示文本 = newProcsShowText;
                newVisual = true;
            }

            if (JobBarsCN.设置.量谱填满音效.Draw($"进程音效{id}", 名称, 生效音效类型, 进程音效, out var newProcSound)) {
                进程音效 = newProcSound;
            }

            DrawSoundEffect("进程音效");

            foreach (var proc in 进程) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

                if (JobBarsCN.设置.量谱进程顺序.Draw($"顺序 ({proc.名称})", proc.名称, proc.顺序, out var newOrder)) {
                    proc.顺序 = newOrder;
                    reset = true;
                }

                if (JobBarsCN.设置.量谱进程颜色.Draw($"颜色 ({proc.名称})", proc.名称, proc.颜色, out var newColor)) {
                    proc.颜色 = newColor;
                    reset = true;
                }
            }
        }
    }
}
