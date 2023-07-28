using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class BLU {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.BLU)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                进程 = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.AstralAttenuation), BuffIds.AstralAttenuation, UIColor.NoColor),
                    new ProcConfig(UIHelper.Localize(BuffIds.UmbralAttenuation), BuffIds.UmbralAttenuation, UIColor.DarkBlue),
                    new ProcConfig(UIHelper.Localize(BuffIds.PhysicalAttenuation), BuffIds.PhysicalAttenuation, UIColor.Orange)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.BluBleed), GaugeVisualType.条状, new GaugeSubTimerProps
            {
                MaxDuration = 60,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.BluBleed)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Poison), GaugeVisualType.条状, new GaugeSubTimerProps
            {
                MaxDuration = 15,
                Color = UIColor.HealthGreen,
                Triggers = new []{
                    new Item(BuffIds.Poison)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.OffGuard), new BuffProps {
                CD = 60,
                持续时间 = 15,
                图标 = ActionIds.OffGuard,
                颜色 = UIColor.BrightGreen,
                触发 = new []{ new Item(ActionIds.OffGuard) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.PeculiarLight), new BuffProps {
                CD = 60,
                持续时间 = 15,
                图标 = ActionIds.PeculiarLight,
                颜色 = UIColor.Red,
                触发 = new []{ new Item(ActionIds.PeculiarLight) }
            })
        };

        public static Cursor Cursors => new(JobIds.BLU, CursorType.无, CursorType.咏唱时间);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.BLU)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.AngelWhisper), new CooldownProps {
                Icon = ActionIds.AngelWhisper,
                CD = 300,
                Triggers = new []{ new Item(ActionIds.AngelWhisper) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.BLU)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.BluBleed), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.SongOfTorment },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.BluBleed), Duration = 60 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Poison), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.BadBreath },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Poison), Duration = 15 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
