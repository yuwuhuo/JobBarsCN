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
                if (ImGui.InputInt("每行Buff数" + manager.Id, ref JobBarsCN.设置.BuffHorizontal, 0)) {
                    JobBarsCN.设置.Save();
                    JobBarsCN.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("从右到左" + manager.Id, ref JobBarsCN.设置.BuffRightToLeft)) {
                    JobBarsCN.设置.Save();
                    JobBarsCN.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("从下到上" + manager.Id, ref JobBarsCN.设置.BuffBottomToTop)) {
                    JobBarsCN.设置.Save();
                    JobBarsCN.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("方形图标" + manager.Id, ref JobBarsCN.设置.BuffSquare)) {
                    JobBarsCN.设置.Save();
                    JobBarsCN.Builder.UpdateBuffsSize();
                }

                if (ImGui.InputFloat("尺寸" + manager.Id, ref JobBarsCN.设置.BuffScale)) {
                    manager.UpdatePositionScale();
                    JobBarsCN.设置.Save();
                }

                var pos = JobBarsCN.设置.BuffPosition;
                if (ImGui.InputFloat2("坐标" + manager.Id, ref pos)) {
                    SetBuffPosition(pos);
                }
            }
        };

        private readonly InfoBox<BuffManager> PartyListInfoBox = new() {
            Label = "小队列表",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("职业为占星时显示卡牌持续时间" + manager.Id, ref JobBarsCN.设置.BuffPartyListASTText)) JobBarsCN.设置.Save();
            }
        };

        private readonly InfoBox<BuffManager> HideWhenInfoBox = new() {
            Label = "隐藏",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("脱离战斗时", ref JobBarsCN.设置.BuffHideOutOfCombat)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("武器收起时", ref JobBarsCN.设置.BuffHideWeaponSheathed)) JobBarsCN.设置.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用Buff栏" + Id, ref JobBarsCN.设置.BuffBarEnabled)) {
                JobBarsCN.设置.Save();
                ResetUI();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            PartyListInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("冷却时隐藏BUFF" + Id, ref JobBarsCN.设置.BuffDisplayTimer)) JobBarsCN.设置.Save();

            if (ImGui.Checkbox("显示小队成员的Buff", ref JobBarsCN.设置.BuffIncludeParty)) {
                JobBarsCN.设置.Save();
                ResetUI();
            }

            if (ImGui.Checkbox("隐藏黄色边框", ref JobBarsCN.设置.BuffThinBorder)) {
                JobBarsCN.设置.Save();
                JobBarsCN.Builder.UpdateBorderThin();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("冷却时不透明度" + Id, ref JobBarsCN.设置.BuffOnCDOpacity)) JobBarsCN.设置.Save();

            ImGui.SetNextItemWidth(100f);
            if (ImGui.InputInt("倒计时文本尺寸", ref JobBarsCN.设置.BuffTextSize_v2)) {
                if (JobBarsCN.设置.BuffTextSize_v2 <= 0) JobBarsCN.设置.BuffTextSize_v2 = 1;
                if (JobBarsCN.设置.BuffTextSize_v2 > 255) JobBarsCN.设置.BuffTextSize_v2 = 255;
                JobBarsCN.设置.Save();
                JobBarsCN.Builder.UpdateBuffsTextSize();
            }
        }

        protected override void DrawItem(BuffConfig[] item, JobIds _) {
            var reset = false;
            foreach (var buff in item) buff.Draw(Id, ref reset);
            if (reset) ResetUI();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;

            if (JobBarsCN.DrawPositionView("Buff栏##Buff坐标", JobBarsCN.设置.BuffPosition, out var pos)) {
                SetBuffPosition(pos);
            }
        }

        private static void SetBuffPosition(Vector2 pos) {
            JobBarsCN.SetWindowPosition("Buff栏##Buff坐标", pos);
            JobBarsCN.设置.BuffPosition = pos;
            JobBarsCN.设置.Save();
            JobBarsCN.Builder.SetBuffPosition(pos);
        }
    }
}
