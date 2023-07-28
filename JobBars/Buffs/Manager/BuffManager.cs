using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager : PerJobManager<BuffConfig[]> {
        private Dictionary<uint, BuffPartyMember> ObjectIdToMember = new();
        private readonly List<BuffConfig> ApplyToTargetBuffs = new();

        private readonly Dictionary<JobIds, List<BuffConfig>> CustomBuffs = new();
        private List<BuffConfig> ApplyToTargetCustomBuffs => CustomBuffs.Values.SelectMany(x => x.Where(y => y.应用于目标)).ToList();

        public BuffManager() : base("##职业_Buffs") {
            ApplyToTargetBuffs.AddRange(JobToValue.Values.SelectMany(x => x.Where(y => y.应用于目标)).ToList());
            JobBarsCN.Builder.HideAllBuffPartyList();
            JobBarsCN.Builder.HideAllBuffs();
        }

        public BuffConfig[] GetBuffConfigs(JobIds job) {
            List<BuffConfig> configs = new();

            configs.AddRange(ApplyToTargetBuffs);
            if (JobToValue.TryGetValue(job, out var props)) configs.AddRange(props.Where(x => !x.应用于目标)); // avoid double-adding

            configs.AddRange(ApplyToTargetCustomBuffs);
            if (CustomBuffs.TryGetValue(job, out var customProps)) configs.AddRange(customProps.Where(x => !x.应用于目标));

            return configs.ToArray();
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBarsCN.设置.BuffBarEnabled) return;
            if (!JobBarsCN.设置.BuffIncludeParty && objectId != JobBarsCN.ClientState.LocalPlayer.ObjectId) return;

            foreach (var member in ObjectIdToMember.Values) member.ProcessAction(action, objectId);
        }

        public void Tick() {
            if (UIHelper.CalcDoHide(JobBarsCN.设置.BuffBarEnabled, JobBarsCN.设置.BuffHideOutOfCombat, JobBarsCN.设置.BuffHideWeaponSheathed)) {
                JobBarsCN.Builder.HideAllBuffPartyList();
                JobBarsCN.Builder.HideBuffs();
                return;
            }
            else {
                JobBarsCN.Builder.ShowBuffs();
            }

            // ============================

            Dictionary<uint, BuffPartyMember> newObjectIdToMember = new();
            HashSet<BuffTracker> activeBuffs = new();

            if (JobBarsCN.PartyMembers == null) PluginLog.LogError("无小队列表");

            for (int idx = 0; idx < JobBarsCN.PartyMembers.Count; idx++) {
                var partyMember = JobBarsCN.PartyMembers[idx];

                if (partyMember == null || partyMember?.Job == JobIds.OTHER || partyMember?.ObjectId == 0) continue;
                if (!JobBarsCN.设置.BuffIncludeParty && partyMember.ObjectId != JobBarsCN.ClientState.LocalPlayer.ObjectId) continue;

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new BuffPartyMember(partyMember.ObjectId, partyMember.IsPlayer);
                member.Tick(activeBuffs, partyMember, out var highlight, out var partyText);
                JobBarsCN.Builder.SetBuffPartyListVisible(idx, highlight);
                JobBarsCN.Builder.SetBuffPartyListText(idx, (JobBarsCN.设置.BuffPartyListASTText && JobBarsCN.CurrentJob == JobIds.AST) ? partyText : "");
                newObjectIdToMember[partyMember.ObjectId] = member;
            }

            for (int idx = JobBarsCN.PartyMembers.Count; idx < 8; idx++) {
                JobBarsCN.Builder.SetBuffPartyListVisible(idx, false);
                JobBarsCN.Builder.SetBuffPartyListText(idx, "");
            }

            var buffIdx = 0;
            foreach (var buff in JobBarsCN.设置.BuffOrderByActive ?
                activeBuffs.OrderBy(b => b.CurrentState) :
                activeBuffs.OrderBy(b => b.Id)
            ) {
                if (buffIdx >= (UIBuilder.MAX_BUFFS - 1)) break;
                buff.TickUI(JobBarsCN.Builder.Buffs[buffIdx]);
                buffIdx++;
            }
            for (int i = buffIdx; i < UIBuilder.MAX_BUFFS; i++) {
                JobBarsCN.Builder.Buffs[i].Hide(); // hide unused
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public void UpdatePositionScale() {
            JobBarsCN.Builder.SetBuffPosition(JobBarsCN.设置.BuffPosition);
            JobBarsCN.Builder.SetBuffScale(JobBarsCN.设置.BuffScale);
        }

        public void ResetUI() {
            ObjectIdToMember.Clear();
        }

        public void ResetTrackers() {
            foreach (var item in ObjectIdToMember.Values) item.Reset();
        }
    }
}
