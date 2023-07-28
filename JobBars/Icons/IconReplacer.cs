using ImGuiNET;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UIIconComboType = JobBars.UI.UIIconComboType;
using UIIconProps = JobBars.UI.UIIconProps;

namespace JobBars.Icons {
    public abstract class IconReplacer {
        public static readonly UIIconComboType[] ValidComboTypes = (UIIconComboType[])Enum.GetValues(typeof(UIIconComboType));

        public enum IconState {
            Inactive,
            Active
        }

        public bool Enabled;

        public readonly string Name;
        protected readonly bool IsTimer;
        protected readonly List<uint> Icons;
        protected IconState State = IconState.Inactive;
        protected UIIconComboType ComboType;
        protected UIIconProps IconProps;
        protected float Offset;
        protected bool ShowRing;

        public IconReplacer(string name, bool isTimer, ActionIds[] icons) {
            Name = name;
            IsTimer = isTimer;
            Icons = new List<ActionIds>(icons).Select(x => (uint)x).ToList();
            Enabled = JobBarsCN.设置.IconEnabled.Get(Name);
            ComboType = JobBarsCN.设置.IconComboType.Get(Name);
            Offset = JobBarsCN.设置.IconTimerOffset.Get(Name);
            ShowRing = JobBarsCN.设置.IconTimerRing.Get(Name);
            CreateIconProps();
        }

        private void CreateIconProps() {
            IconProps = new UIIconProps {
                IsTimer = IsTimer,
                ComboType = ComboType,
                ShowRing = ShowRing
            };
        }

        public void Setup() {
            State = IconState.Inactive;
            if (!Enabled) return;
            JobBarsCN.IconBuilder.Setup(Icons, IconProps);
        }

        public abstract void Tick();

        public abstract void ProcessAction(Item action);

        protected void SetIcon(float current, float duration) => JobBarsCN.IconBuilder.SetProgress(Icons, current, duration);

        protected void ResetIcon() => JobBarsCN.IconBuilder.SetDone(Icons);

        public void Draw(string id, JobIds _) {
            var _ID = id + Name;
            var type = IsTimer ? "计时器" : "BUFF";
            var color = Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1);

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            if (ImGui.CollapsingHeader($"{Name} [{type}]{_ID}")) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if (JobBarsCN.设置.IconEnabled.Draw($"启用{_ID}", Name, Enabled, out var newEnabled)) {
                    Enabled = newEnabled;
                    JobBarsCN.IconManager.Reset();
                }

                if (JobBarsCN.设置.IconComboType.Draw($"提示条件{_ID}", Name, ValidComboTypes, ComboType, out var newComboType)) {
                    ComboType = newComboType;
                    CreateIconProps();
                    JobBarsCN.IconManager.Reset();
                }

                if (IsTimer) {
                    if (JobBarsCN.设置.IconTimerOffset.Draw($"时间{_ID}", Name, Offset, out var newOffset)) {
                        Offset = newOffset;
                    }

                    if (JobBarsCN.设置.IconTimerRing.Draw($"计时圈{_ID}", Name, Enabled, out var newRing)) {
                        ShowRing = newRing;
                        CreateIconProps();
                        JobBarsCN.IconManager.Reset();
                    }
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }
        }
    }
}
