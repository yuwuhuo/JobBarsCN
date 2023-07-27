using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using System;

namespace JobBars.Cursors {
    public enum CursorType {
        None,
        GCD,
        CastTime,
        MpTick,
        ActorTick,
        DoT_Tick,
        StaticCircle,
        StaticRing,
        StatusTime,
        Slidecast
    }

    public class Cursor {
        public static readonly CursorType[] ValidCursorType = (CursorType[])Enum.GetValues(typeof(CursorType));

        private readonly string Name;
        private readonly string InnerName;
        private readonly string OuterName;

        private CursorType InnerType;
        private CursorType OuterType;

        private ItemData InnerStatus;
        private ItemData OuterStatus;
        private float InnerStatusDuration;
        private float OuterStatusDuration;

        public Cursor(JobIds job, CursorType inner, CursorType outer) {
            Name = $"{job}";
            InnerName = Name + "_内圈";
            OuterName = Name + "_外圈";

            InnerType = JobBarsCN.Config.CursorType.Get(InnerName, inner);
            OuterType = JobBarsCN.Config.CursorType.Get(OuterName, outer);

            InnerStatus = JobBarsCN.Config.CursorStatus.Get(InnerName, new ItemData {
                Name = "无",
                Data = new Item()
            });
            OuterStatus = JobBarsCN.Config.CursorStatus.Get(OuterName, new ItemData {
                Name = "无",
                Data = new Item()
            });
            InnerStatusDuration = JobBarsCN.Config.CursorStatusDuration.Get(InnerName, 5f);
            OuterStatusDuration = JobBarsCN.Config.CursorStatusDuration.Get(OuterName, 5f);
        }

        public float GetInner() => GetValue(InnerType, InnerStatus, InnerStatusDuration);
        public float GetOuter() => GetValue(OuterType, OuterStatus, OuterStatusDuration);

        private float GetValue(CursorType type, ItemData status, float statusDuration) => type switch {
            CursorType.None => 0,
            CursorType.GCD => UIHelper.GetGCD(out var _, out var _),
            CursorType.CastTime => UIHelper.GetCastTime(out var _, out var _),
            CursorType.MpTick => UIHelper.GetMpTick(),
            CursorType.ActorTick => UIHelper.GetActorTick(),
            CursorType.StaticCircle => 2, // just a placeholder value, doesn't actually matter
            CursorType.StaticRing => 1,
            CursorType.StatusTime => GetStatusTime(status, statusDuration),
            CursorType.DoT_Tick => UIHelper.GetDoTTick(),
            CursorType.Slidecast => GetSlidecastTime(),
            _ => 0
        };

        private static float GetStatusTime(ItemData status, float statusDuration) {
            if (statusDuration == 0) return 0;
            if (status.Data.Id == 0) return 0;
            var ret = (UIHelper.PlayerStatus.TryGetValue(status.Data, out var value) ? (value.RemainingTime > 0 ? value.RemainingTime : value.RemainingTime * -1) : 0) / statusDuration;
            return Math.Min(ret, 1f);
        }

        private static float GetSlidecastTime() {
            if (JobBarsCN.Config.GaugeSlidecastTime <= 0f) return UIHelper.GetCastTime(out var _, out var _);

            var isCasting = UIHelper.GetCurrentCast(out var currentTime, out var totalTime);
            if (!isCasting || totalTime == 0) return 0;
            var slidecastTime = totalTime - JobBarsCN.Config.GaugeSlidecastTime;
            if (currentTime > slidecastTime) return 0;
            return currentTime / slidecastTime;
        }

        public void Draw(string _ID) {
            if (JobBarsCN.Config.CursorType.Draw($"内环类型{_ID}", InnerName, ValidCursorType, InnerType, out var newInnerValue)) {
                InnerType = newInnerValue;
            }

            if (InnerType == CursorType.StatusTime) {
                if (JobBarsCN.Config.CursorStatus.Draw($"内环状态{_ID}", InnerName, UIHelper.StatusList, InnerStatus, out var newInnerStatus)) {
                    InnerStatus = newInnerStatus;
                }
                if (JobBarsCN.Config.CursorStatusDuration.Draw($"内环状态持续时间{_ID}", InnerName, InnerStatusDuration, out var newInnerStatusDuration)) {
                    InnerStatusDuration = newInnerStatusDuration;
                }
            }

            if (JobBarsCN.Config.CursorType.Draw($"外环类型{_ID}", OuterName, ValidCursorType, OuterType, out var newOuterValue)) {
                OuterType = newOuterValue;
            }

            if (OuterType == CursorType.StatusTime) {
                if (JobBarsCN.Config.CursorStatus.Draw($"外环状态{_ID}", OuterName, UIHelper.StatusList, OuterStatus, out var newOuterStatus)) {
                    OuterStatus = newOuterStatus;
                }
                if (JobBarsCN.Config.CursorStatusDuration.Draw($"外环状态持续时间{_ID}", OuterName, OuterStatusDuration, out var newOuterStautsDuration)) {
                    OuterStatusDuration = newOuterStautsDuration;
                }
            }
        }
    }
}
