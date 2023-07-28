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
                ImGui.Checkbox("锁定坐标" + manager.Id, ref manager.LOCKED);

                if (JobBarsCN.设置.量谱位置类型 != GaugePositionType.Split) {
                    if (ImGui.Checkbox("横向排列量谱", ref JobBarsCN.设置.横向量谱)) {
                        manager.UpdatePositionScale();
                        JobBarsCN.设置.Save();
                    }

                    if (ImGui.Checkbox("从下到上", ref JobBarsCN.设置.量谱从下到上)) {
                        manager.UpdatePositionScale();
                        JobBarsCN.设置.Save();
                    }

                    if (ImGui.Checkbox("靠右对齐", ref JobBarsCN.设置.GaugeAlignRight)) {
                        manager.UpdatePositionScale();
                        JobBarsCN.设置.Save();
                    }
                }

                if (JobBarsCN.DrawCombo(ValidGaugePositionType, JobBarsCN.设置.量谱位置类型, "量谱位置类型", manager.Id, out var newPosition)) {
                    JobBarsCN.设置.量谱位置类型 = newPosition;
                    JobBarsCN.设置.Save();

                    manager.UpdatePositionScale();
                }

                if (JobBarsCN.设置.量谱位置类型 == GaugePositionType.Global) { // GLOBAL POSITIONING
                    var pos = JobBarsCN.设置.GaugePositionGlobal;
                    if (ImGui.InputFloat2("坐标" + manager.Id, ref pos)) {
                        SetGaugePositionGlobal(pos);
                    }
                }
                else if (JobBarsCN.设置.量谱位置类型 == GaugePositionType.PerJob) { // PER-JOB POSITIONING
                    var pos = manager.GetPerJobPosition();
                    if (ImGui.InputFloat2($"坐标 ({manager.CurrentJob})" + manager.Id, ref pos)) {
                        SetGaugePositionPerJob(manager.CurrentJob, pos);
                    }
                }

                if (ImGui.InputFloat("尺寸" + manager.Id, ref JobBarsCN.设置.GaugeScale)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.设置.Save();
                }
            }
        };

        private readonly InfoBox<GaugeManager> HideWhenInfoBox = new() {
            Label = "以下情况隐藏",
            ContentsAction = (GaugeManager manager) => {
                if (ImGui.Checkbox("脱离战斗状态", ref JobBarsCN.设置.GaugesHideOutOfCombat)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("武器收起状态", ref JobBarsCN.设置.GaugesHideWeaponSheathed)) JobBarsCN.设置.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用量谱" + Id, ref JobBarsCN.设置.GaugesEnabled)) {
                JobBarsCN.设置.Save();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("量谱闪烁", ref JobBarsCN.设置.GaugePulse)) JobBarsCN.设置.Save();

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("滑步时间 (0 = 关闭)", ref JobBarsCN.设置.GaugeSlidecastTime)) JobBarsCN.设置.Save();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;
            if (JobBarsCN.设置.量谱位置类型 == GaugePositionType.Split) {
                foreach (var config in CurrentConfigs) config.DrawPositionBox();
            }
            else if (JobBarsCN.设置.量谱位置类型 == GaugePositionType.PerJob) {
                var currentPos = GetPerJobPosition();
                if (JobBarsCN.DrawPositionView($"量谱栏 ({CurrentJob})##量谱坐标", currentPos, out var pos)) {
                    SetGaugePositionPerJob(CurrentJob, pos);
                }
            }
            else { // GLOBAL
                var currentPos = JobBarsCN.设置.GaugePositionGlobal;
                if (JobBarsCN.DrawPositionView("量谱栏##量谱坐标", currentPos, out var pos)) {
                    SetGaugePositionGlobal(pos);
                }
            }
        }

        private static void SetGaugePositionGlobal(Vector2 pos) {
            JobBarsCN.SetWindowPosition("量谱栏##量谱坐标", pos);
            JobBarsCN.设置.GaugePositionGlobal = pos;
            JobBarsCN.设置.Save();
            JobBarsCN.Builder.SetGaugePosition(pos);
        }

        private static void SetGaugePositionPerJob(JobIds job, Vector2 pos) {
            JobBarsCN.SetWindowPosition($"量谱栏 ({job})##量谱坐标", pos);
            JobBarsCN.设置.GaugePerJobPosition.Set($"{job}", pos);
            JobBarsCN.设置.Save();
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

        protected override string ItemToString(GaugeConfig item) => item.名称;

        protected override bool IsEnabled(GaugeConfig item) => item.Enabled;
    }
}
