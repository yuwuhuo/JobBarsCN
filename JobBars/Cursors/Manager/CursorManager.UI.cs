using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        private readonly InfoBox<CursorManager> HideWhenInfoBox = new() {
            Label = "隐藏",
            ContentsAction = (CursorManager manager) => {
                if (ImGui.Checkbox("鼠标隐藏", ref JobBars.Config.CursorHideWhenHeld)) JobBars.Config.Save();
                if (ImGui.Checkbox("脱离战斗状态", ref JobBars.Config.CursorHideOutOfCombat)) JobBars.Config.Save();
                if (ImGui.Checkbox("武器收回状态", ref JobBars.Config.CursorHideWeaponSheathed)) JobBars.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("光标启用" + Id, ref JobBars.Config.CursorsEnabled)) JobBars.Config.Save();
        }

        protected override void DrawSettings() {
            HideWhenInfoBox.Draw(this);

            if (JobBars.DrawCombo(ValidCursorPositionType, JobBars.Config.CursorPosition, "光标坐标", Id, out var newPosition)) {
                JobBars.Config.CursorPosition = newPosition;
                JobBars.Config.Save();
            }

            if (JobBars.Config.CursorPosition == CursorPositionType.CustomPosition) {
                if (ImGui.InputFloat2("自定义光标位置", ref JobBars.Config.CursorCustomPosition)) {
                    JobBars.Config.Save();
                }
            }

            if (ImGui.InputFloat("内环尺寸" + Id, ref JobBars.Config.CursorInnerScale)) JobBars.Config.Save();
            if (ImGui.InputFloat("外环尺寸" + Id, ref JobBars.Config.CursorOuterScale)) JobBars.Config.Save();

            if (Configuration.DrawColor("内环颜色", InnerColor, out var newColorInner)) {
                InnerColor = newColorInner;
                JobBars.Config.CursorInnerColor = newColorInner.Name;
                JobBars.Config.Save();

                JobBars.Builder.SetCursorInnerColor(InnerColor);
            }

            if (Configuration.DrawColor("外环颜色", OuterColor, out var newColorOuter)) {
                OuterColor = newColorOuter;
                JobBars.Config.CursorOuterColor = newColorOuter.Name;
                JobBars.Config.Save();

                JobBars.Builder.SetCursorOuterColor(OuterColor);
            }
        }

        protected override void DrawItem(Cursor item, JobIds _) {
            item.Draw(Id);
        }
    }
}
