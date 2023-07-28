using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class WAR {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.InnerRelease), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.InnerRelease)
                },
                Color = UIColor.Orange
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.SurgingTempest), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 60,
                DefaultDuration = 30,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.SurgingTempest)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.InnerRelease), new BuffProps {
                持续时间 = 15,
                CD = 60,
                图标 = ActionIds.InnerRelease,
                颜色 = UIColor.Orange,
                触发 = new []{ new Item(BuffIds.InnerRelease) }
            })
        };

        public static Cursor Cursors => new(JobIds.WAR, CursorType.无, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.Holmgang), new CooldownProps {
                Icon = ActionIds.Holmgang,
                Duration = 10,
                CD = 240,
                Triggers = new []{ new Item(ActionIds.Holmgang) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.ShakeItOff), new CooldownProps {
                Icon = ActionIds.ShakeItOff,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.ShakeItOff) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Bloodwhetting), new CooldownProps {
                Icon = ActionIds.Bloodwhetting,
                Duration = 6,
                CD = 25,
                Triggers = new []{
                    new Item(ActionIds.NascentFlash),
                    new Item(ActionIds.RawIntuition),
                    new Item(ActionIds.Bloodwhetting)
                }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.SurgingTempest), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.StormsEye },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.SurgingTempest), Duration = 60 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
