using JobBars.Data;
using System;
using System.Linq;

namespace JobBars.Icons.Manager {
    public unsafe partial class IconManager : PerJobManager<IconReplacer[]> {
        public JobIds CurrentJob = JobIds.OTHER;
        private IconReplacer[] CurrentIcons => JobToValue.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToValue[JobIds.OTHER];

        public IconManager() : base("##JobBars_Icons") {
        }

        public void SetJob(JobIds job) {
            JobBarsCN.IconBuilder.Reset();
            CurrentJob = job;

            if (!JobBarsCN.设置.IconsEnabled) return;
            foreach (var icon in CurrentIcons) icon.Setup();
        }

        public void Reset() => SetJob(CurrentJob);

        public void ResetJob(JobIds job) {
            if (job == CurrentJob) Reset();
        }

        public void PerformAction(Item action) {
            if (!JobBarsCN.设置.IconsEnabled) return;

            foreach (var icon in CurrentIcons.Where(i => i.Enabled)) icon.ProcessAction(action);
        }

        public void Tick() {
            if (!JobBarsCN.设置.IconsEnabled) return;
            foreach (var icon in CurrentIcons.Where(i => i.Enabled)) {
                icon.Tick();
            }
            JobBarsCN.IconBuilder.Tick();
        }
    }
}
