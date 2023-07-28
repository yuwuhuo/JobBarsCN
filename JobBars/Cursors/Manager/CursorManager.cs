using System;
using ImGuiNET;
using Dalamud.Interface;
using JobBars.Data;
using JobBars.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        public CursorManager() : base("##职业__光标") {
            InnerColor = UIColor.GetColor(JobBarsCN.设置.CursorInnerColor, UIColor.法条色);
            OuterColor = UIColor.GetColor(JobBarsCN.设置.CursorOuterColor, UIColor.HealthGreen);

            JobBarsCN.Builder.SetCursorInnerColor(InnerColor);
            JobBarsCN.Builder.SetCursorOuterColor(OuterColor);
        }

        public void SetJob(JobIds job) {
            CurrentCursor = JobToValue.TryGetValue(job, out var cursor) ? cursor : null;
        }

        public void Tick() {
            if (UIHelper.CalcDoHide(JobBarsCN.设置.CursorsEnabled, JobBarsCN.设置.CursorHideOutOfCombat, JobBarsCN.设置.CursorHideWeaponSheathed)) {
                JobBarsCN.Builder.HideCursor();
                return;
            }
            else {
                JobBarsCN.Builder.ShowCursor();
            }

            // ============================

            if (CurrentCursor == null) {
                JobBarsCN.Builder.SetCursorInnerPercent(0, 1f);
                JobBarsCN.Builder.SetCursorOuterPercent(0, 1f);
                return;
            }

            var viewport = ImGuiHelpers.MainViewport;

            if (JobBarsCN.设置.CursorPosition == CursorPositionType.MouseCursor) {
                var pos = ImGui.GetMousePos() - viewport.Pos;
                var atkStage = AtkStage.GetSingleton();

                var dragging = *((byte*)new IntPtr(atkStage) + 0x137);
                if (JobBarsCN.设置.CursorHideWhenHeld && dragging != 1) {
                    JobBarsCN.Builder.HideCursor();
                    return;
                }
                JobBarsCN.Builder.ShowCursor();

                if (pos.X > 0 && pos.Y > 0 && pos.X < viewport.Size.X && pos.Y < viewport.Size.Y && dragging == 1) {
                    JobBarsCN.Builder.SetCursorPosition(pos);
                }
            }
            else {
                JobBarsCN.Builder.ShowCursor();
                JobBarsCN.Builder.SetCursorPosition(JobBarsCN.设置.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBarsCN.设置.CursorCustomPosition);
            }

            var inner = CurrentCursor.GetInner();
            var outer = CurrentCursor.GetOuter();
            JobBarsCN.Builder.SetCursorInnerPercent(inner, JobBarsCN.设置.CursorInnerScale);
            JobBarsCN.Builder.SetCursorOuterPercent(outer, JobBarsCN.设置.CursorOuterScale);
        }
    }
}