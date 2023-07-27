using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public unsafe partial class CooldownManager {
        private readonly InfoBox<CooldownManager> PositionInfoBox = new() {
            Label = "坐标",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("Left-aligned" + manager.Id, ref JobBarsCN.Config.CooldownsLeftAligned)) {
                    JobBarsCN.Config.Save();
                    manager.ResetUi();
                }

                if (ImGui.InputFloat("Scale" + manager.Id, ref JobBarsCN.Config.CooldownScale)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.Config.Save();
                }

                if (ImGui.InputFloat2("坐标" + manager.Id, ref JobBarsCN.Config.CooldownPosition)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.Config.Save();
                }

                if (ImGui.InputFloat("Line height" + manager.Id, ref JobBarsCN.Config.CooldownsSpacing)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.Config.Save();
                }
            }
        };

        private readonly InfoBox<CooldownManager> ShowIconInfoBox = new() {
            Label = "Show Icons When",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("Default" + manager.Id, ref JobBarsCN.Config.CooldownsStateShowDefault)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("Active" + manager.Id, ref JobBarsCN.Config.CooldownsStateShowRunning)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("On cooldown" + manager.Id, ref JobBarsCN.Config.CooldownsStateShowOnCD)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("Off cooldown" + manager.Id, ref JobBarsCN.Config.CooldownsStateShowOffCD)) JobBarsCN.Config.Save();
            }
        };

        private readonly InfoBox<CooldownManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("Out of combat", ref JobBarsCN.Config.CooldownsHideOutOfCombat)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("Weapon sheathed", ref JobBarsCN.Config.CooldownsHideWeaponSheathed)) JobBarsCN.Config.Save();
            }
        };

        private readonly CustomCooldownDialog CustomCooldownDialog = new();

        protected override void DrawHeader() {
            CustomCooldownDialog.Draw();

            if (ImGui.Checkbox("Cooldowns enabled" + Id, ref JobBarsCN.Config.CooldownsEnabled)) {
                JobBarsCN.Config.Save();
                ResetUi();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            ShowIconInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("Hide active buff text" + Id, ref JobBarsCN.Config.CooldownsHideActiveBuffDuration)) JobBarsCN.Config.Save();

            if (ImGui.Checkbox("Show party members' cooldowns" + Id, ref JobBarsCN.Config.CooldownsShowPartyMembers)) {
                JobBarsCN.Config.Save();
                ResetUi();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("Opacity when on cooldown" + Id, ref JobBarsCN.Config.CooldownsOnCDOpacity)) JobBarsCN.Config.Save();
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
            if (ImGui.Button($"+ Add Custom Cooldown{Id}")) CustomCooldownDialog.Show(job);

            if (reset) ResetUi();
        }
    }
}