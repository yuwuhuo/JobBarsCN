using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        private readonly InfoBox<CursorManager> HideWhenInfoBox = new() {
            Label = "隐藏",
            ContentsAction = (CursorManager manager) => {
                if (ImGui.Checkbox("鼠标隐藏", ref JobBarsCN.Config.CursorHideWhenHeld)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("脱离战斗状态", ref JobBarsCN.Config.CursorHideOutOfCombat)) JobBarsCN.Config.Save();
                if (ImGui.Checkbox("武器收回状态", ref JobBarsCN.Config.CursorHideWeaponSheathed)) JobBarsCN.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("光标启用" + Id, ref JobBarsCN.Config.CursorsEnabled)) JobBarsCN.Config.Save();
        }

        protected override void DrawSettings() {
            HideWhenInfoBox.Draw(this);

            if (JobBarsCN.DrawCombo(ValidCursorPositionType, JobBarsCN.Config.CursorPosition, "光标坐标", Id, out var newPosition)) {
                JobBarsCN.Config.CursorPosition = newPosition;
                JobBarsCN.Config.Save();
            }

            if (JobBarsCN.Config.CursorPosition == CursorPositionType.CustomPosition) {
                if (ImGui.InputFloat2("自定义光标位置", ref JobBarsCN.Config.CursorCustomPosition)) {
                    JobBarsCN.Config.Save();
                }
            }

            if (ImGui.InputFloat("内环尺寸" + Id, ref JobBarsCN.Config.CursorInnerScale)) JobBarsCN.Config.Save();
            if (ImGui.InputFloat("外环尺寸" + Id, ref JobBarsCN.Config.CursorOuterScale)) JobBarsCN.Config.Save();

            if (Configuration.DrawColor("内环颜色", InnerColor, out var newColorInner)) {
                InnerColor = newColorInner;
                JobBarsCN.Config.CursorInnerColor = newColorInner.Name;
                JobBarsCN.Config.Save();

                JobBarsCN.Builder.SetCursorInnerColor(InnerColor);
            }

            if (Configuration.DrawColor("外环颜色", OuterColor, out var newColorOuter)) {
                OuterColor = newColorOuter;
                JobBarsCN.Config.CursorOuterColor = newColorOuter.Name;
                JobBarsCN.Config.Save();

                JobBarsCN.Builder.SetCursorOuterColor(OuterColor);
            }
        }

        protected override void DrawItem(Cursor item, JobIds _) {
            item.Draw(Id);
        }
    }
}
