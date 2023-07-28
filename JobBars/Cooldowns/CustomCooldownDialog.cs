using ImGuiNET;
using JobBars.Cooldowns.Manager;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Numerics;

namespace JobBars.Cooldowns {
    public class CustomCooldownDialog : GenericDialog {
        private enum CustomCooldownType {
            Buff,
            Action
        }

        private static readonly JobIds[] JobOptions = (JobIds[])Enum.GetValues(typeof(JobIds));
        private JobIds SelectedJob = JobIds.OTHER;

        private CustomCooldownType CustomTriggerType = CustomCooldownType.Action;
        private float CustomCD = 30;
        private float CustomDuration = 0;

        private readonly ItemSelector CustomTriggerAction = new("触发", "##自定义技能监控_技能", UIHelper.ActionList);
        private readonly ItemSelector CustomTriggerBuff = new("触发", "##自定义技能监控_Buff", UIHelper.StatusList);
        private readonly ItemSelector CustomIcon = new("图标", "##自定义技能监控_图标", UIHelper.ActionList);

        private CooldownManager Manager => JobBarsCN.CooldownManager;


        public CustomCooldownDialog() : base("自定义技能监控") { }

        public override void DrawBody() {
            var id = "##自定义技能监控";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild(id + "/Child", new Vector2(0, -footerHeight), true);

            if(JobBarsCN.DrawCombo(JobOptions, SelectedJob, "Job", id, out var newSelectedJob)) {
                SelectedJob = newSelectedJob;
            }

            if (ImGui.BeginCombo("##自定义技能监控_类型", $"{CustomTriggerType}", ImGuiComboFlags.HeightLargest)) {
                if (ImGui.Selectable("技能", CustomTriggerType == CustomCooldownType.Action)) CustomTriggerType = CustomCooldownType.Action;
                if (ImGui.Selectable("Buff", CustomTriggerType == CustomCooldownType.Buff)) CustomTriggerType = CustomCooldownType.Buff;
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.Text("触发类型");

            if (CustomTriggerType == CustomCooldownType.Action) CustomTriggerAction.Draw();
            else CustomTriggerBuff.Draw();

            CustomIcon.Draw();

            ImGui.InputFloat($"冷却时间", ref CustomCD);
            ImGui.InputFloat($"持续时间 (0 = 能力技)", ref CustomDuration);

            var selected = CustomTriggerType == CustomCooldownType.Action ? CustomTriggerAction.GetSelected() : CustomTriggerBuff.GetSelected();
            var icon = CustomIcon.GetSelected();

            ImGui.EndChild();

            if (icon.Data.Id != 0 && selected.Data.Id != 0) {
                if (ImGui.Button("添加")) {
                    var newName = $"{selected.Name} - 自定义 ({UIHelper.Localize(SelectedJob)})";
                    var newProps = new CooldownProps {
                        CD = CustomCD,
                        Duration = CustomDuration,
                        Icon = (ActionIds)icon.Data.Id,
                        Triggers = new[] { selected.Data }
                    };
                    Manager.AddCustomCooldown(SelectedJob, newName, newProps);
                    Manager.ResetUi();
                }
            }
            else {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                ImGui.Button("添加");
                ImGui.PopStyleVar();
            }
        }

        public void Show(JobIds job) {
            SelectedJob = job;
            Show();
        }
    }
}
