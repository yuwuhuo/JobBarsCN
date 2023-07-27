using ImGuiNET;
using JobBars.Data;

namespace JobBars.Icons.Manager {
    public partial class IconManager {
        private readonly InfoBox<IconManager> LargeIconInfoBox = new() {
            Label = "放大",
            ContentsAction = (IconManager manager) => {
                if (ImGui.Checkbox("Buff图标" + manager.Id, ref JobBarsCN.Config.IconBuffLarge)) {
                    JobBarsCN.IconBuilder.RefreshVisuals();
                    JobBarsCN.Config.Save();
                }

                if (ImGui.Checkbox("倒计时图标" + manager.Id, ref JobBarsCN.Config.IconTimerLarge)) {
                    JobBarsCN.IconBuilder.RefreshVisuals();
                    JobBarsCN.Config.Save();
                }
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用图标替换", ref JobBarsCN.Config.IconsEnabled)) {
                JobBarsCN.Config.Save();
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