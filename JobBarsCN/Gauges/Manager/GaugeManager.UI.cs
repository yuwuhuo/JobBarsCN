using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager {
        public bool LOCKED = true;

        private static readonly GaugePositionType[] ValidGaugePositionType = (GaugePositionType[])Enum.GetValues(typeof(GaugePositionType));

        private readonly InfoBox<GaugeManager> PositionInfoBox = new() {
            Label = "坐标",
            ContentsAction = (GaugeManager manager) => {
                ImGui.Checkbox("坐标锁定" + manager.Id, ref manager.LOCKED);

                if (JobBarsCN.Config.GaugePositionType != GaugePositionType.Split) {
                    if (ImGui.Checkbox("横向排列量谱", ref JobBarsCN.Config.GaugeHorizontal)) {
                        manager.UpdatePositionScale();
                        JobBarsCN.Config.Save();
                    }

                    if (ImGui.Checkbox("置于顶端", ref JobBarsCN.Config.GaugeBottomToTop)) {
                        manager.UpdatePositionScale();
                        JobBarsCN.Config.Save();
                    }

                    if (ImGui.Checkbox("靠右对齐", ref JobBarsCN.Config.GaugeAlignRight)) {
                        manager.UpdatePositionScale();
                        JobBarsCN.Config.Save();
                    }
                }

                if (JobBarsCN.DrawCombo(ValidGaugePositionType, JobBarsCN.Config.GaugePositionType, "量谱排列方式", manager.Id, out var newPosition)) {
                    JobBarsCN.Config.GaugePositionType = newPosition;
                    JobBarsCN.Config.Save();

                    manager.UpdatePositionScale();
                }

                if (JobBarsCN.Config.GaugePositionType == GaugePositionType.Global) { // GLOBAL POSITIONING
                    var pos = JobBarsCN.Config.GaugePositionGlobal;
                    if (ImGui.InputFloat2("坐标" + manager.Id, ref pos)) {
                        SetGaugePositionGlobal(pos);
                    }
                }
                else if (JobBarsCN.Config.GaugePositionType == GaugePositionType.PerJob) { // PER-JOB POSITIONING
                    var pos = manager.GetPerJobPosition();
                    if (ImGui.InputFloat2($"坐标 ({manager.CurrentJob})" + manager.Id, ref pos)) {
                        SetGaugePositionPerJob(manager.CurrentJob, pos);
                    }
                }

                if (ImGui.InputFloat("尺寸" + manager.Id, ref JobBarsCN.Config.GaugeScale)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.Config.Save();
                }
            }
        };

        private readonly InfoBox<GaugeManager> HideWhenInfoBox = new() {
            Label = "隐藏",
            ContentsAction = (GaugeManager manager) => {
                if (ImGui.Checkbox("脱离战斗", ref JobBarsCN.Config.GaugesHideOutOfCombat)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("收起武器", ref JobBarsCN.Config.GaugesHideWeaponSheathed)) JobBarsCN.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("量谱启用" + Id, ref JobBarsCN.Config.GaugesEnabled)) {
                JobBarsCN.Config.Save();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("箭头和棱形闪烁", ref JobBarsCN.Config.GaugePulse)) JobBarsCN.Config.Save();

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("滑步时间 (0 = 关闭)", ref JobBarsCN.Config.GaugeSlidecastTime)) JobBarsCN.Config.Save();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;
            if (JobBarsCN.Config.GaugePositionType == GaugePositionType.Split) {
                foreach (var config in CurrentConfigs) config.DrawPositionBox();
            }
            else if (JobBarsCN.Config.GaugePositionType == GaugePositionType.PerJob) {
                var currentPos = GetPerJobPosition();
                if (JobBarsCN.DrawPositionView($"量谱栏 ({CurrentJob})##量谱坐标", currentPos, out var pos)) {
                    SetGaugePositionPerJob(CurrentJob, pos);
                }
            }
            else { // GLOBAL
                var currentPos = JobBarsCN.Config.GaugePositionGlobal;
                if (JobBarsCN.DrawPositionView("量谱栏##量谱坐标", currentPos, out var pos)) {
                    SetGaugePositionGlobal(pos);
                }
            }
        }

        private static void SetGaugePositionGlobal(Vector2 pos) {
            JobBarsCN.SetWindowPosition("量谱栏##Gauge量谱坐标", pos);
            JobBarsCN.Config.GaugePositionGlobal = pos;
            JobBarsCN.Config.Save();
            JobBarsCN.Builder.SetGaugePosition(pos);
        }

        private static void SetGaugePositionPerJob(JobIds job, Vector2 pos) {
            JobBarsCN.SetWindowPosition($"量谱栏 ({job})##量谱坐标", pos);
            JobBarsCN.Config.GaugePerJobPosition.Set($"{job}", pos);
            JobBarsCN.Config.Save();
            JobBarsCN.Builder.SetGaugePosition(pos);
        }

        // ==========================================

        protected override void DrawItem(GaugeConfig item) {
            ImGui.Indent(5);
            item.Draw(Id, out bool newVisual, out bool reset);
            ImGui.Unindent();

            if (SelectedJob != CurrentJob) return;
            if (newVisual) {
                UpdateVisuals();
                UpdatePositionScale();
            }
            if (reset) Reset();
        }

        protected override string ItemToString(GaugeConfig item) => item.Name;

        protected override bool IsEnabled(GaugeConfig item) => item.Enabled;
    }
}
