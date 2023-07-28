using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using System;

namespace JobBars.Cursors {
    public enum CursorType {
        无,
        GCD,
        咏唱时间,
        跳蓝,
        节拍器,
        跳DoT,
        固定圆,
        固定环,
        状态时间,
        滑步咏唱
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
            InnerName = Name + "_A环";
            OuterName = Name + "_B环";

            InnerType = JobBarsCN.设置.CursorType.Get(InnerName, inner);
            OuterType = JobBarsCN.设置.CursorType.Get(OuterName, outer);

            InnerStatus = JobBarsCN.设置.CursorStatus.Get(InnerName, new ItemData {
                Name = "无",
                Data = new Item()
            });
            OuterStatus = JobBarsCN.设置.CursorStatus.Get(OuterName, new ItemData {
                Name = "无",
                Data = new Item()
            });
            InnerStatusDuration = JobBarsCN.设置.CursorStatusDuration.Get(InnerName, 5f);
            OuterStatusDuration = JobBarsCN.设置.CursorStatusDuration.Get(OuterName, 5f);
        }

        public float GetInner() => GetValue(InnerType, InnerStatus, InnerStatusDuration);
        public float GetOuter() => GetValue(OuterType, OuterStatus, OuterStatusDuration);

        private float GetValue(CursorType type, ItemData status, float statusDuration) => type switch {
            CursorType.无 => 0,
            CursorType.GCD => UIHelper.GetGCD(out var _, out var _),
            CursorType.咏唱时间 => UIHelper.GetCastTime(out var _, out var _),
            CursorType.跳蓝 => UIHelper.GetMpTick(),
            CursorType.节拍器 => UIHelper.GetActorTick(),
            CursorType.固定圆 => 2, // just a placeholder value, doesn't actually matter
            CursorType.固定环 => 1,
            CursorType.状态时间 => GetStatusTime(status, statusDuration),
            CursorType.跳DoT => UIHelper.GetDoTTick(),
            CursorType.滑步咏唱 => GetSlidecastTime(),
            _ => 0
        };

        private static float GetStatusTime(ItemData status, float statusDuration) {
            if (statusDuration == 0) return 0;
            if (status.Data.Id == 0) return 0;
            var ret = (UIHelper.PlayerStatus.TryGetValue(status.Data, out var value) ? (value.RemainingTime > 0 ? value.RemainingTime : value.RemainingTime * -1) : 0) / statusDuration;
            return Math.Min(ret, 1f);
        }

        private static float GetSlidecastTime() {
            if (JobBarsCN.设置.GaugeSlidecastTime <= 0f) return UIHelper.GetCastTime(out var _, out var _);

            var isCasting = UIHelper.GetCurrentCast(out var currentTime, out var totalTime);
            if (!isCasting || totalTime == 0) return 0;
            var slidecastTime = totalTime - JobBarsCN.设置.GaugeSlidecastTime;
            if (currentTime > slidecastTime) return 0;
            return currentTime / slidecastTime;
        }

        public void Draw(string _ID) {
            if (JobBarsCN.设置.CursorType.Draw($"A环类型{_ID}", InnerName, ValidCursorType, InnerType, out var newInnerValue)) {
                InnerType = newInnerValue;
            }

            if (InnerType == CursorType.状态时间) {
                if (JobBarsCN.设置.CursorStatus.Draw($"A环状态{_ID}", InnerName, UIHelper.StatusList, InnerStatus, out var newInnerStatus)) {
                    InnerStatus = newInnerStatus;
                }
                if (JobBarsCN.设置.CursorStatusDuration.Draw($"A环状态持续时间{_ID}", InnerName, InnerStatusDuration, out var newInnerStatusDuration)) {
                    InnerStatusDuration = newInnerStatusDuration;
                }
            }

            if (JobBarsCN.设置.CursorType.Draw($"B环类型{_ID}", OuterName, ValidCursorType, OuterType, out var newOuterValue)) {
                OuterType = newOuterValue;
            }

            if (OuterType == CursorType.状态时间) {
                if (JobBarsCN.设置.CursorStatus.Draw($"B环状态{_ID}", OuterName, UIHelper.StatusList, OuterStatus, out var newOuterStatus)) {
                    OuterStatus = newOuterStatus;
                }
                if (JobBarsCN.设置.CursorStatusDuration.Draw($"B环状态持续时间{_ID}", OuterName, OuterStatusDuration, out var newOuterStautsDuration)) {
                    OuterStatusDuration = newOuterStautsDuration;
                }
            }
        }
    }
}
