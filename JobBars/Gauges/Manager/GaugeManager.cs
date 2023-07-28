using JobBars.Data;
using JobBars.Helper;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager : PerJobManagerNested<GaugeConfig> {
        public JobIds CurrentJob = JobIds.OTHER;
        private GaugeConfig[] CurrentConfigs => JobToValue.TryGetValue(CurrentJob, out var configs) ? configs : JobToValue[JobIds.OTHER];
        private readonly List<GaugeTracker> 当前量谱 = new();

        private static readonly List<BuffIds> GaugeBuffsOnPartyMembers = new(new[] { BuffIds.Excog }); // which buffs on party members do we care about?

        public GaugeManager() : base("##职业量谱") {
            JobBarsCN.Builder.HideAllGauges();
        }

        public void SetJob(JobIds job) {
            foreach (var gauge in 当前量谱) gauge.Cleanup();
            当前量谱.Clear();
            JobBarsCN.Builder.HideAllGauges();


            CurrentJob = job;
            for (var idx = 0; idx < CurrentConfigs.Length; idx++) {
                当前量谱.Add(CurrentConfigs[idx].GetTracker(idx));
            }
            UpdatePositionScale();
        }

        public void PerformAction(Item action) {
            if (!JobBarsCN.设置.GaugesEnabled) return;

            foreach (var gauge in 当前量谱.Where(g => g.Enabled && !g.Disposed)) gauge.ProcessAction(action);
        }

        public void Tick() {
            if (UIHelper.CalcDoHide(JobBarsCN.设置.GaugesEnabled, JobBarsCN.设置.GaugesHideOutOfCombat, JobBarsCN.设置.GaugesHideWeaponSheathed)) {
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

            foreach (var gauge in 当前量谱.Where(g => g.Enabled && !g.Disposed)) gauge.Tick();
        }

        private Vector2 GetPerJobPosition() => JobBarsCN.设置.GaugePerJobPosition.Get($"{CurrentJob}");

        public void UpdatePositionScale() {
            JobBarsCN.Builder.SetGaugePosition(JobBarsCN.设置.量谱位置类型 == GaugePositionType.PerJob ? GetPerJobPosition() : JobBarsCN.设置.GaugePositionGlobal);
            JobBarsCN.Builder.SetGaugeScale(JobBarsCN.设置.GaugeScale);

            var position = 0;
            foreach (var gauge in 当前量谱.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (JobBarsCN.设置.量谱位置类型 == GaugePositionType.Split) {
                    gauge.UpdateSplitPosition();
                }
                else {
                    var x = JobBarsCN.设置.横向量谱 ? position :
                        (JobBarsCN.设置.GaugeAlignRight ? 160 - gauge.宽度 : 0);

                    var y = JobBarsCN.设置.横向量谱 ? gauge.YOffset :
                        (JobBarsCN.设置.量谱从下到上 ? position - gauge.高度 : position);

                    var posChange = JobBarsCN.设置.横向量谱 ? gauge.宽度 :
                        (JobBarsCN.设置.量谱从下到上 ? -1 * gauge.高度 : gauge.高度);

                    gauge.SetPosition(new Vector2(x, y));
                    position += posChange;
                }
            }
        }

        public void UpdateVisuals() {
            foreach (var gauge in 当前量谱) gauge.UpdateVisual();
        }

        public void Reset() => SetJob(CurrentJob);
    }
}
