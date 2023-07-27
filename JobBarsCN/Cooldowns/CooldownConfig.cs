using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Cooldowns {
    public struct CooldownProps {
        public ActionIds Icon;
        public Item[] Triggers;
        public float Duration;
        public float CD;
    }

    public class CooldownConfig {
        public readonly string Name;
        public readonly ActionIds Icon;
        public readonly Item[] Triggers;
        public readonly float Duration;
        public readonly float CD;

        public bool Enabled { get; private set; }
        public int Order { get; private set; }
        public bool ShowBorderWhenActive { get; private set; }
        public bool ShowBorderWhenOffCD { get; private set; }

        public CooldownConfig(string name, CooldownProps props) {
            Name = name;
            Icon = props.Icon;
            Triggers = props.Triggers;
            Duration = props.Duration;
            CD = props.CD;
            Enabled = JobBarsCN.Config.CooldownEnabled.Get(Name);
            Order = JobBarsCN.Config.CooldownOrder.Get(Name);
            ShowBorderWhenActive = JobBarsCN.Config.CooldownShowBorderWhenActive.Get(Name);
            ShowBorderWhenOffCD = JobBarsCN.Config.CooldownShowBorderWhenOffCD.Get(Name);
        }

        public bool Draw(string _id, bool isCustom, ref bool reset) {
            var deleteCustom = false;
            var color = Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1);

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            if (ImGui.CollapsingHeader($"{Name}{_id}")) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if (isCustom) {
                    if (JobBarsCN.RemoveButton($"删除{_id}", true)) deleteCustom = true;
                }

                if (JobBarsCN.Config.CooldownEnabled.Draw($"启用{_id}{Name}", Name, Enabled, out var newEnabled)) {
                    Enabled = newEnabled;
                    reset = true;
                }

                if (JobBarsCN.Config.CooldownOrder.Draw($"顺序{_id}{Name}", Name, Order, out var newOrder)) {
                    Order = newOrder;
                    reset = true;
                }

                if (JobBarsCN.Config.CooldownShowBorderWhenActive.Draw($"可用时显示边框{_id}{Name}", Name, ShowBorderWhenActive, out var newShowBorderWhenActive)) {
                    ShowBorderWhenActive = newShowBorderWhenActive;
                }

                if (JobBarsCN.Config.CooldownShowBorderWhenOffCD.Draw($"冷却完毕时显示边框{_id}{Name}", Name, ShowBorderWhenOffCD, out var newShowBorderWhenOffCD)) {
                    ShowBorderWhenOffCD = newShowBorderWhenOffCD;
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }

            return deleteCustom;
        }
    }
}
