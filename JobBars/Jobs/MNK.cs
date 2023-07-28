using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class MNK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.PerfectBalance)
                },
                Color = UIColor.Orange
            }),
            new GaugeProcsConfig(UIHelper.Localize(BuffIds.LeadenFist), GaugeVisualType.Diamond, new GaugeProcProps{
                进程 = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.LeadenFist), BuffIds.LeadenFist, UIColor.Yellow)
                },
                进程音效 = GaugeCompleteSoundType.Never
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.DisciplinedFist), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 15,
                Color = UIColor.PurplePink,
                Triggers = new []{
                    new Item(BuffIds.DisciplinedFist)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Demolish), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 18,
                Color = UIColor.Yellow,
                Triggers = new [] {
                    new Item(BuffIds.Demolish)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.条状, new GaugeSubGCDProps {
                MaxCounter = 11,
                MaxDuration = 20,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.RiddleOfFire)
                }
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.MNK)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.NoColor,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        棱形 = true,
                        MaxCharges = 2,
                        CD = 45,
                        触发 = new []{  new Item(ActionIds.TrueNorth) }
                    },
                    new GaugesChargesPartProps {
                        栏 = true,
                        持续时间 = 10,
                        触发 = new []{ new Item(BuffIds.TrueNorth) }
                    }
                },
                CompletionSound = GaugeCompleteSoundType.Never
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Brotherhood), new BuffProps {
                CD = 120,
                持续时间 = 15,
                图标 = ActionIds.Brotherhood,
                颜色 = UIColor.Orange,
                触发 = new []{ new Item(ActionIds.Brotherhood) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.RiddleOfFire), new BuffProps {
                CD = 60,
                持续时间 = 20,
                图标 = ActionIds.RiddleOfFire,
                颜色 = UIColor.Red,
                触发 = new []{ new Item(ActionIds.RiddleOfFire) }
            })
        };

        public static Cursor Cursors => new(JobIds.MNK, CursorType.无, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Mantra), new CooldownProps {
                Icon = ActionIds.Mantra,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Mantra) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.RiddleOfFire), new IconBuffProps {
                Icons = new [] { ActionIds.RiddleOfFire },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RiddleOfFire), Duration = 20 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.DisciplinedFist), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.TwinSnakes },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.DisciplinedFist), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Demolish), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Demolish },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Demolish), Duration = 18 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
