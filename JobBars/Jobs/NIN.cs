using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class NIN {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.RaijuReady), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.RaijuReady)
                },
                Color = UIColor.PurplePink
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Bunshin), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.Bunshin)
                },
                Color = UIColor.Red
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.NIN)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
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
            new BuffConfig(UIHelper.Localize(ActionIds.Mug), new BuffProps {
                CD = 120,
                持续时间 = 20,
                图标 = ActionIds.Mug,
                颜色 = UIColor.LightBlue,
                触发 = new []{ new Item(ActionIds.Mug) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.TrickAttack), new BuffProps {
                CD = 60,
                持续时间 = 15,
                图标 = ActionIds.TrickAttack,
                颜色 = UIColor.Yellow,
                触发 = new []{ new Item(ActionIds.TrickAttack) }
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.Bunshin), new BuffProps {
                CD = 90,
                持续时间 = 30,
                图标 = ActionIds.Bunshin,
                颜色 = UIColor.Orange,
                触发 = new []{ new Item(BuffIds.Bunshin) }
            })
        };

        public static Cursor Cursors => new(JobIds.NIN, CursorType.无, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.TrickAttack), new IconBuffProps {
                Icons = new [] { ActionIds.TrickAttack },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.TrickAttack), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Mug), new IconBuffProps {
                Icons = new [] { ActionIds.Mug },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Mug), Duration = 20 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
