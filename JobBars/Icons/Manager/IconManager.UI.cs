using ImGuiNET;
using JobBars.Data;

namespace JobBars.Icons.Manager {
    public partial class IconManager {
        private readonly InfoBox<IconManager> LargeIconInfoBox = new() {
            Label = "放大",
            ContentsAction = (IconManager manager) => {
                if (ImGui.Checkbox("Buff图标" + manager.Id, ref JobBarsCN.设置.IconBuffLarge)) {
                    JobBarsCN.IconBuilder.RefreshVisuals();
                    JobBarsCN.设置.Save();
                }

                if (ImGui.Checkbox("倒计时图标" + manager.Id, ref JobBarsCN.设置.IconTimerLarge)) {
                    JobBarsCN.IconBuilder.RefreshVisuals();
                    JobBarsCN.设置.Save();
                }
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用图标替换", ref JobBarsCN.设置.IconsEnabled)) {
                JobBarsCN.设置.Save();
                Reset();
            }
        }

        protected override void DrawSettings() {
            LargeIconInfoBox.Draw(this);
        }

        protected override void DrawItem(IconReplacer[] item, JobIds _) {
            foreach (var icon in item) {
                icon.Draw(Id, SelectedJob);
            }
        }
    }
}