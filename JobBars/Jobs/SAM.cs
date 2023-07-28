using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class SAM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Meikyo), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Meikyo)
                },
                Color = UIColor.BlueGreen
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Fugetsu), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = UIColor.DarkBlue,
                Triggers = new []{
                    new Item(BuffIds.Fugetsu)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Fuka), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.Fuka)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Higanbana), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 60,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.Higanbana)
                }
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.SAM)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.NoColor,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        棱形 = true,
                        MaxCharges = 2,
                        CD = 45,
                        触发 = new []{
                            new Item(ActionIds.TrueNorth)
                        }
                    },
                    new GaugesChargesPartProps {
                        栏 = true,
                        持续时间 = 10,
                        触发 = new []{
                            new Item(BuffIds.TrueNorth)
                        }
                    }
                },
                CompletionSound = GaugeCompleteSoundType.Never
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.OgiNamikiri), new BuffProps {
                CD = 120,
                持续时间 = 30,
                图标 = ActionIds.OgiNamikiri,
                颜色 = UIColor.Red,
                触发 = new []{ new Item(BuffIds.OgiNamikiri) }
            })
        };

        public static Cursor Cursors => new(JobIds.SAM, CursorType.无, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Fugetsu), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Jinpu },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Fugetsu), Duration = 40 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Fuka), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Shifu },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Fuka), Duration = 40 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
