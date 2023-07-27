using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        public bool LOCKED = true;

        private readonly InfoBox<BuffManager> PositionInfoBox = new() {
            Label = "坐标",
            ContentsAction = (BuffManager manager) => {
                ImGui.Checkbox("坐标锁定" + manager.Id, ref manager.LOCKED);

                ImGui.SetNextItemWidth(25f);
                if (ImGui.InputInt("每行buff数" + manager.Id, ref JobBars.Config.BuffHorizontal, 0)) {
                    JobBars.Config.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("从右到左排列" + manager.Id, ref JobBars.Config.BuffRightToLeft)) {
                    JobBars.Config.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("从下到上排列" + manager.Id, ref JobBars.Config.BuffBottomToTop)) {
                    JobBars.Config.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("方形图标" + manager.Id, ref JobBars.Config.BuffSquare)) {
                    JobBars.Config.Save();
                    JobBars.Builder.UpdateBuffsSize();
                }

                if (ImGui.InputFloat("尺寸" + manager.Id, ref JobBars.Config.BuffScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Config.Save();
                }

                var pos = JobBars.Config.BuffPosition;
                if (ImGui.InputFloat2("坐标" + manager.Id, ref pos)) {
                    SetBuffPosition(pos);
                }
            }
        };

        private readonly InfoBox<BuffManager> PartyListInfoBox = new() {
            Label = "小队列表",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("职业为占星时显示卡牌持续时间" + manager.Id, ref JobBars.Config.BuffPartyListASTText)) JobBars.Config.Save();
            }
        };

        private readonly InfoBox<BuffManager> HideWhenInfoBox = new() {
            Label = "隐藏",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("脱离战斗时", ref JobBars.Config.BuffHideOutOfCombat)) JobBars.Config.Save();
                if (ImGui.Checkbox("武器收起时", ref JobBars.Config.BuffHideWeaponSheathed)) JobBars.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用Buff栏" + Id, ref JobBars.Config.BuffBarEnabled)) {
                JobBars.Config.Save();
                ResetUI();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            PartyListInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("冷却时隐藏BUFF" + Id, ref JobBars.Config.BuffDisplayTimer)) JobBars.Config.Save();

            if (ImGui.Checkbox("显示小队成员的' buffs", ref JobBars.Config.BuffIncludeParty)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.Checkbox("淡黄色边框", ref JobBars.Config.BuffThinBorder)) {
                JobBars.Config.Save();
                JobBars.Builder.UpdateBorderThin();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("冷却时不透明度" + Id, ref JobBars.Config.BuffOnCDOpacity)) JobBars.Config.Save();

            ImGui.SetNextItemWidth(100f);
            if (ImGui.InputInt("倒计时文本尺寸", ref JobBars.Config.BuffTextSize_v2)) {
                if (JobBars.Config.BuffTextSize_v2 <= 0) JobBars.Config.BuffTextSize_v2 = 1;
                if (JobBars.Config.BuffTextSize_v2 > 255) JobBars.Config.BuffTextSize_v2 = 255;
                JobBars.Config.Save();
                JobBars.Builder.UpdateBuffsTextSize();
            }
        }

        protected override void DrawItem(BuffConfig[] item, JobIds _) {
            var reset = false;
            foreach (var buff in item) buff.Draw(Id, ref reset);
            if (reset) ResetUI();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;

            if (JobBars.DrawPositionView("Buff栏##Buff坐标", JobBars.Config.BuffPosition, out var pos)) {
                SetBuffPosition(pos);
            }
        }

        private static void SetBuffPosition(Vector2 pos) {
            JobBars.SetWindowPosition("Buff栏##Buff坐标", pos);
            JobBars.Config.BuffPosition = pos;
            JobBars.Config.Save();
            JobBars.Builder.SetBuffPosition(pos);
        }
    }
}
