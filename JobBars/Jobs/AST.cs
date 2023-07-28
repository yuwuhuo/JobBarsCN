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
    public static class AST {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.AST)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps {
                进程 = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, UIColor.LightBlue)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Combust3), GaugeVisualType.条状, new GaugeTimerProps {
                SubTimers = new [] {
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Combust3),
                        Triggers = new []{
                            new Item(BuffIds.Combust2),
                            new Item(BuffIds.Combust3),
                            new Item(BuffIds.Combust)
                        }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.TheBalance), new BuffProps {
                持续时间 = 15,
                图标 = ActionIds.TheBalance,
                颜色 = UIColor.Orange,
                触发 = new []{ new Item(BuffIds.TheBalance) },
                应用于目标 = true,
                显示小队文本 = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheBole), new BuffProps {
                持续时间 = 15,
                图标 = ActionIds.TheBole,
                颜色 = UIColor.BrightGreen,
                触发 = new []{ new Item(BuffIds.TheBole) },
                应用于目标 = true,
                显示小队文本 = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheSpear), new BuffProps {
                持续时间 = 15,
                图标 = ActionIds.TheSpear,
                颜色 = UIColor.DarkBlue,
                触发 = new []{ new Item(BuffIds.TheSpear) },
                应用于目标 = true,
                显示小队文本 = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheSpire), new BuffProps {
                持续时间 = 15,
                图标 = ActionIds.TheSpire,
                颜色 = UIColor.Yellow,
                触发 = new []{ new Item(BuffIds.TheSpire) },
                应用于目标 = true,
                显示小队文本 = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheArrow), new BuffProps {
                持续时间 = 15,
                图标 = ActionIds.TheArrow,
                颜色 = UIColor.LightBlue,
                触发 = new []{ new Item(BuffIds.TheArrow) },
                应用于目标 = true,
                显示小队文本 = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheEwer), new BuffProps {
                持续时间 = 15,
                图标 = ActionIds.TheEwer,
                颜色 = UIColor.LightBlue,
                触发 = new []{ new Item(BuffIds.TheEwer) },
                应用于目标 = true,
                显示小队文本 = true
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.Divination), new BuffProps {
                CD = 120,
                持续时间 = 15,
                图标 = ActionIds.Divination,
                颜色 = UIColor.Yellow,
                触发 = new []{ new Item(ActionIds.Divination) }
            })
        };

        public static Cursor Cursors => new(JobIds.AST, CursorType.无, CursorType.咏唱时间);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.NeutralSect), new CooldownProps {
                Icon = ActionIds.NeutralSect,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.NeutralSect) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Macrocosmos), new CooldownProps {
                Icon = ActionIds.Macrocosmos,
                CD = 180,
                Duration = 15,
                Triggers = new []{ new Item(ActionIds.Macrocosmos) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.EarthlyStar), new CooldownProps {
                Icon = ActionIds.EarthlyStar,
                Duration = 20,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.EarthlyStar) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Combust3), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Combust1,
                    ActionIds.Combust2,
                    ActionIds.Combust3
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust3), Duration = 30 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Lightspeed), new IconBuffProps {
                Icons = new [] { ActionIds.Lightspeed },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Lightspeed), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(ActionIds.EarthlyStar), new IconBuffProps {
                Icons = new [] {
                    ActionIds.EarthlyStar,
                    ActionIds.StellarDetonation
                },
                Triggers = new [] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.GiantDominance), Duration = 10 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(ActionIds.Astrodyne), new IconBuffProps {
                Icons = new [] {
                    ActionIds.Astrodyne
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HarmonyOfSpirit), Duration = 15 }
                }
            }),
            new IconCooldownReplacer(UIHelper.Localize(ActionIds.MinorArcana), new IconCooldownProps {
                Icons = new[] {
                    ActionIds.LordOfCrowns,
                    ActionIds.LadyOfCrowns
                },
                Triggers = new[] {
                    new Item(ActionIds.MinorArcana)
                },
                Cooldown = 60
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
