using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }

    public unsafe partial class CooldownManager : PerJobManager<CooldownConfig[]> {
        private static readonly int MILLIS_LOOP = 250;
        private Dictionary<uint, CooldownPartyMember> ObjectIdToMember = new();
        private readonly Dictionary<JobIds, List<CooldownConfig>> CustomCooldowns = new();

        public CooldownManager() : base("##职业_冷却时间") {
            JobBarsCN.Builder.SetCooldownPosition(JobBarsCN.Config.CooldownPosition);

            // initialize custom cooldowns
            foreach (var custom in JobBarsCN.Config.CustomCooldown) {
                if (!CustomCooldowns.ContainsKey(custom.Job)) CustomCooldowns[custom.Job] = new();
                CustomCooldowns[custom.Job].Add(new CooldownConfig(custom.Name, custom.Props));
            }
        }

        public CooldownConfig[] GetCooldownConfigs(JobIds job) {
            List<CooldownConfig> configs = new();
            if (JobToValue.TryGetValue(job, out var props)) configs.AddRange(props);
            if (CustomCooldowns.TryGetValue(job, out var customProps)) configs.AddRange(customProps);
            return configs.ToArray();
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBarsCN.Config.CooldownsEnabled) return;

            foreach (var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick() {
            if (UIHelper.CalcDoHide(JobBarsCN.Config.CooldownsEnabled, JobBarsCN.Config.CooldownsHideOutOfCombat, JobBarsCN.Config.CooldownsHideWeaponSheathed)) {
                JobBarsCN.Builder.HideCooldowns();
                return;
            }
            else {
                JobBarsCN.Builder.ShowCooldowns();
            }

            // ============================

            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            if (JobBarsCN.PartyMembers == null) PluginLog.LogError("无小队队员");

            for (int idx = 0; idx < JobBarsCN.PartyMembers.Count; idx++) {
                var partyMember = JobBarsCN.PartyMembers[idx];

                if (partyMember == null || partyMember?.ObjectId == 0 || partyMember?.Job == JobIds.OTHER) {
                    JobBarsCN.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                if (!JobBarsCN.Config.CooldownsShowPartyMembers && partyMember.ObjectId != JobBarsCN.ClientState.LocalPlayer.ObjectId) {
                    JobBarsCN.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new CooldownPartyMember(partyMember.ObjectId);
                member.Tick(JobBarsCN.Builder.Cooldowns[idx], partyMember, percent);
                newObjectIdToMember[partyMember.ObjectId] = member;

                JobBarsCN.Builder.SetCooldownRowVisible(idx, true);
            }

            for (int idx = JobBarsCN.PartyMembers.Count; idx < 8; idx++) { // hide remaining slots
                JobBarsCN.Builder.SetCooldownRowVisible(idx, false);
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public void UpdatePositionScale() {
            JobBarsCN.Builder.SetCooldownPosition(JobBarsCN.Config.CooldownPosition + new Vector2(0, UIHelper.PartyListOffset()));
            JobBarsCN.Builder.SetCooldownScale(JobBarsCN.Config.CooldownScale);
            JobBarsCN.Builder.RefreshCooldownsLayout();
        }

        public void ResetUi() => ObjectIdToMember.Clear();

        public void ResetTrackers() {
            foreach (var item in ObjectIdToMember.Values) item.Reset();
        }

        public void AddCustomCooldown(JobIds job, string name, CooldownProps props) {
            if (!CustomCooldowns.ContainsKey(job)) CustomCooldowns[job] = new();
            CustomCooldowns[job].Add(new CooldownConfig(name, props));
            JobBarsCN.Config.AddCustomCooldown(name, job, props);
        }

        public void DeleteCustomCooldown(JobIds job, CooldownConfig custom) {
            CustomCooldowns[job].Remove(custom);
            JobBarsCN.Config.RemoveCustomCooldown(custom.Name);
        }
    }
}