using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class BRD {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig(UIHelper.Localize(BuffIds.StraightShotReady), GaugeVisualType.Diamond, new GaugeProcProps {
                进程 = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.StraightShotReady), BuffIds.StraightShotReady, UIColor.Yellow)
                }
            }),
            new GaugeChargesConfig(UIHelper.Localize(ActionIds.BloodLetter), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.Red,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        栏 = true,
                        棱形 = true,
                        CD = 15,
                        MaxCharges = 3,
                        触发 = new[] { new Item(ActionIds.BloodLetter) }
                    }
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.VenomousBite), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 45,
                Color = UIColor.Purple,
                Triggers = new []{
                    new Item(BuffIds.CausticBite),
                    new Item(BuffIds.VenomousBite)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Stormbite), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 45,
                Color = UIColor.LightBlue,
                Triggers = new []{
                    new Item(BuffIds.Windbite),
                    new Item(BuffIds.Stormbite),
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.RagingStrikes), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.RagingStrikes)
                },
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.BattleVoice), new BuffProps {
                CD = 120,
                持续时间 = 15,
                图标 = ActionIds.BattleVoice,
                颜色 = UIColor.Orange,
                触发 = new []{ new Item(ActionIds.BattleVoice) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.RadiantFinale), new BuffProps {
                CD = 110,
                持续时间 = 15,
                图标 = ActionIds.RadiantFinale,
                颜色 = UIColor.DarkBlue,
                触发 = new []{ new Item(ActionIds.RadiantFinale) }
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.Barrage), new BuffProps {
                CD = 120,
                持续时间 = 10,
                图标 = ActionIds.Barrage,
                颜色 = UIColor.Yellow,
                触发 = new []{ new Item(BuffIds.Barrage) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.RagingStrikes), new BuffProps {
                CD = 120,
                持续时间 = 20,
                图标 = ActionIds.RagingStrikes,
                颜色 = UIColor.Yellow,
                触发 = new []{ new Item(ActionIds.RagingStrikes) }
            })
        };

        public static Cursor Cursors => new(JobIds.BRD, CursorType.无, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.Troubadour), new CooldownProps {
                Icon = ActionIds.Troubadour,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Troubadour) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.NaturesMinne), new CooldownProps {
                Icon = ActionIds.NaturesMinne,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.NaturesMinne) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.RagingStrikes), new IconBuffProps {
                Icons = new [] { ActionIds.RagingStrikes },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RagingStrikes), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.BattleVoice), new IconBuffProps {
                Icons = new [] { ActionIds.BattleVoice },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.BattleVoice), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.RadiantFinale), new IconBuffProps {
                Icons = new [] { ActionIds.RadiantFinale },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RadiantFinale), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.VenomousBite), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.CausticBite,
                    ActionIds.VenomousBite,
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.CausticBite), Duration = 45 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.VenomousBite), Duration = 45 },
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Stormbite), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Windbite,
                    ActionIds.Stormbite,
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Windbite), Duration = 45 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Stormbite), Duration = 45 },
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
