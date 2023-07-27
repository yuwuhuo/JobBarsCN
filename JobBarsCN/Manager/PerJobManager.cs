using ImGuiNET;
using JobBars.Helper;

namespace JobBars.Data {
    public abstract class PerJobManager<T> : PerJobManagerGeneric<T> where T : class {
        public PerJobManager(string id) : base(id) { }

        protected override void DrawLeftColumn() {
            foreach (var job in JobToValue.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(UIHelper.Localize(job) + Id, SelectedJob == job)) {
                    SelectedJob = job;
                }
            }
        }

        protected override void DrawRightColumn() {
            if (SelectedJob == JobIds.OTHER) {
                ImGui.Text("选择一个职业...");
            }
            else {
                ImGui.BeginChild(Id + "已选取");
                DrawItem(JobToValue[SelectedJob], SelectedJob);
                ImGui.EndChild();
            }
        }

        protected abstract void DrawItem(T item, JobIds job);
    }
}
