using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public unsafe partial class CooldownManager {
        private readonly InfoBox<CooldownManager> PositionInfoBox = new() {
            Label = "坐标",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("左对齐" + manager.Id, ref JobBarsCN.设置.CooldownsLeftAligned)) {
                    JobBarsCN.设置.Save();
                    manager.ResetUi();
                }

                if (ImGui.InputFloat("尺寸" + manager.Id, ref JobBarsCN.设置.CooldownScale)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.设置.Save();
                }

                if (ImGui.InputFloat2("坐标" + manager.Id, ref JobBarsCN.设置.CooldownPosition)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.设置.Save();
                }

                if (ImGui.InputFloat("行距" + manager.Id, ref JobBarsCN.设置.CooldownsSpacing)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.设置.Save();
                }
            }
        };

        private readonly InfoBox<CooldownManager> ShowIconInfoBox = new() {
            Label = "显示场景（最好全勾）",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("默认" + manager.Id, ref JobBarsCN.设置.CooldownsStateShowDefault)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("生效中" + manager.Id, ref JobBarsCN.设置.CooldownsStateShowRunning)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("冷却中" + manager.Id, ref JobBarsCN.设置.CooldownsStateShowOnCD)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("可发动" + manager.Id, ref JobBarsCN.设置.CooldownsStateShowOffCD)) JobBarsCN.设置.Save();
            }
        };

        private readonly InfoBox<CooldownManager> HideWhenInfoBox = new() {
            Label = "以下情况隐藏",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("脱离战斗状态", ref JobBarsCN.设置.CooldownsHideOutOfCombat)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("武器收起状态", ref JobBarsCN.设置.CooldownsHideWeaponSheathed)) JobBarsCN.设置.Save();
            }
        };

        private readonly CustomCooldownDialog CustomCooldownDialog = new();

        protected override void DrawHeader() {
            CustomCooldownDialog.Draw();

            if (ImGui.Checkbox("启用技能监控" + Id, ref JobBarsCN.设置.CooldownsEnabled)) {
                JobBarsCN.设置.Save();
                ResetUi();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            ShowIconInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("隐藏滚动黄边" + Id, ref JobBarsCN.设置.CooldownsHideActiveBuffDuration)) JobBarsCN.设置.Save();

            if (ImGui.Checkbox("显示队友的技能监控" + Id, ref JobBarsCN.设置.CooldownsShowPartyMembers)) {
                JobBarsCN.设置.Save();
                ResetUi();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("技能冷却时监控透明度" + Id, ref JobBarsCN.设置.CooldownsOnCDOpacity)) JobBarsCN.设置.Save();
        }

        protected override void DrawItem(CooldownConfig[] item, JobIds job) {
            var reset = false;
            foreach (var cooldown in item) cooldown.Draw(Id, false, ref reset);

            // Delete custom
            if (CustomCooldowns.TryGetValue(job, out var customCooldowns)) {
                foreach (var custom in customCooldowns) {
                    if (custom.Draw(Id, true, ref reset)) {
                        DeleteCustomCooldown(job, custom);
                        reset = true;
                        break;
                    }
                }
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            if (ImGui.Button($"添加自定义技能监控{Id}")) CustomCooldownDialog.Show(job);

            if (reset) ResetUi();
        }
    }
}