using JobBars.Data;
using System.Collections.Generic;

namespace JobBars.Buffs {
    public class BuffPartyMember {
        private JobIds 小队队员当前职业 = JobIds.OTHER;
        private readonly List<BuffTracker> Trackers = new();
        private readonly uint ObjectId;
        private readonly bool IsPlayer;

        public BuffPartyMember(uint objectId, bool isPlayer) {
            ObjectId = objectId;
            IsPlayer = isPlayer;
        }

        public void Tick(HashSet<BuffTracker> trackers, CurrentPartyMember partyMember, out bool highlight, out string partyText) {
            highlight = false;
            partyText = "";

            if (小队队员当前职业 != partyMember.Job) {
                小队队员当前职业 = partyMember.Job;
                SetupTrackers();
            }

            foreach (var tracker in Trackers) {
                tracker.Tick(partyMember.BuffDict);
                // add the icon if it's active and either a personal buff or on yourself
                if (tracker.Enabled && (!tracker.ApplyToTarget || IsPlayer)) trackers.Add(tracker);
                if (tracker.Highlight) {
                    highlight = true;
                }
                if (tracker.Active && tracker.显示小队文本) {
                    partyText = tracker.Text;
                }
            }
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach (var tracker in Trackers) tracker.ProcessAction(action);
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBarsCN.BuffManager.GetBuffConfigs(小队队员当前职业);
            foreach (var prop in trackerProps) {
                if (!prop.启用) continue;
                Trackers.Add(new BuffTracker(prop));
            }
        }

        public void Reset() {
            foreach (var item in Trackers) item.Reset();
        }
    }
}
