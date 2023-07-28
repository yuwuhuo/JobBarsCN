using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        private readonly InfoBox<CursorManager> HideWhenInfoBox = new() {
            Label = "隐藏",
            ContentsAction = (CursorManager manager) => {
                if (ImGui.Checkbox("鼠标隐藏", ref JobBarsCN.设置.CursorHideWhenHeld)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("脱离战斗状态", ref JobBarsCN.设置.CursorHideOutOfCombat)) JobBarsCN.设置.Save();
                if (ImGui.Checkbox("武器收回状态", ref JobBarsCN.设置.CursorHideWeaponSheathed)) JobBarsCN.设置.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("光标启用" + Id, ref JobBarsCN.设置.CursorsEnabled)) JobBarsCN.设置.Save();
        }

        protected override void DrawSettings() {
            HideWhenInfoBox.Draw(this);

            if (JobBarsCN.DrawCombo(ValidCursorPositionType, JobBarsCN.设置.CursorPosition, "光标坐标", Id, out var newPosition)) {
                JobBarsCN.设置.CursorPosition = newPosition;
                JobBarsCN.设置.Save();
            }

            if (JobBarsCN.设置.CursorPosition == CursorPositionType.CustomPosition) {
                if (ImGui.InputFloat2("自定义光标位置", ref JobBarsCN.设置.CursorCustomPosition)) {
                    JobBarsCN.设置.Save();
                }
            }

            if (ImGui.InputFloat("A环尺寸" + Id, ref JobBarsCN.设置.CursorInnerScale)) JobBarsCN.设置.Save();
            if (ImGui.InputFloat("B环尺寸" + Id, ref JobBarsCN.设置.CursorOuterScale)) JobBarsCN.设置.Save();

            if (Configuration.DrawColor("A环颜色", InnerColor, out var newColorInner)) {
                InnerColor = newColorInner;
                JobBarsCN.设置.CursorInnerColor = newColorInner.Name;
                JobBarsCN.设置.Save();

                JobBarsCN.Builder.SetCursorInnerColor(InnerColor);
            }

            if (Configuration.DrawColor("B环颜色", OuterColor, out var newColorOuter)) {
                OuterColor = newColorOuter;
                JobBarsCN.设置.CursorOuterColor = newColorOuter.Name;
                JobBarsCN.设置.Save();

                JobBarsCN.Builder.SetCursorOuterColor(OuterColor);
            }
        }

        protected override void DrawItem(Cursor item, JobIds _) {
            item.Draw(Id);
        }
    }
}
