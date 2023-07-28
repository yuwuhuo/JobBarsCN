using JobBars.Data;
using JobBars.Helper;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager : PerJobManagerNested<GaugeConfig> {
        public JobIds CurrentJob = JobIds.OTHER;
        private GaugeConfig[] CurrentConfigs => JobToValue.TryGetValue(CurrentJob, out var configs) ? configs : JobToValue[JobIds.OTHER];
        private readonly List<GaugeTracker> CurrentGauges = new();

        private static readonly List<BuffIds> GaugeBuffsOnPartyMembers = new(new[] { BuffIds.Excog }); // which buffs on party members do we care about?

        public GaugeManager() : base("##职业量谱") {
            JobBarsCN.Builder.HideAllGauges();
        }

        public void SetJob(JobIds job) {
            foreach (var gauge in CurrentGauges) gauge.Cleanup();
            CurrentGauges.Clear();
            JobBarsCN.Builder.HideAllGauges();


            CurrentJob = job;
            for (var idx = 0; idx < CurrentConfigs.Length; idx++) {
                CurrentGauges.Add(CurrentConfigs[idx].GetTracker(idx));
            }
            UpdatePositionScale();
        }

        public void PerformAction(Item action) {
            if (!JobBarsCN.Config.GaugesEnabled) return;

            foreach (var gauge in CurrentGauges.Where(g => g.Enabled && !g.Disposed)) gauge.ProcessAction(action);
        }

        public void Tick() {
            if (UIHelper.CalcDoHide(JobBarsCN.Config.GaugesEnabled, JobBarsCN.Config.GaugesHideOutOfCombat, JobBarsCN.Config.GaugesHideWeaponSheathed)) {
                JobBarsCN.Builder.HideGauges();
                return;
            }
            else {
                JobBarsCN.Builder.ShowGauges();
            }

            // ============================

            if (CurrentJob == JobIds.SCH && !UIHelper.OutOfCombat) { // only need this to catch excog for now
                JobBarsCN.SearchForPartyMemberStatus((int)JobBarsCN.ClientState.LocalPlayer.ObjectId, UIHelper.PlayerStatus, GaugeBuffsOnPartyMembers);
            }

            foreach (var gauge in CurrentGauges.Where(g => g.Enabled && !g.Disposed)) gauge.Tick();
        }

        private Vector2 GetPerJobPosition() => JobBarsCN.Config.GaugePerJobPosition.Get($"{CurrentJob}");

        public void UpdatePositionScale() {
            JobBarsCN.Builder.SetGaugePosition(JobBarsCN.Config.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBarsCN.Config.GaugePositionGlobal);
            JobBarsCN.Builder.SetGaugeScale(JobBarsCN.Config.GaugeScale);

            var position = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (JobBarsCN.Config.GaugePositionType == GaugePositionType.Split) {
                    gauge.UpdateSplitPosition();
                }
                else {
                    var x = JobBarsCN.Config.GaugeHorizontal ? position :
                        (JobBarsCN.Config.GaugeAlignRight ? 160 - gauge.Width : 0);

                    var y = JobBarsCN.Config.GaugeHorizontal ? gauge.YOffset :
                        (JobBarsCN.Config.GaugeBottomToTop ? position - gauge.Height : position);

                    var posChange = JobBarsCN.Config.GaugeHorizontal ? gauge.Width :
                        (JobBarsCN.Config.GaugeBottomToTop ? -1 * gauge.Height : gauge.Height);

                    gauge.SetPosition(new Vector2(x, y));
                    position += posChange;
                }
            }
        }

        public void UpdateVisuals() {
            foreach (var gauge in CurrentGauges) gauge.UpdateVisual();
        }

        public void Reset() => SetJob(CurrentJob);
    }
}
