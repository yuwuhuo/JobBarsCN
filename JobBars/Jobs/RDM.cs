﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class RDM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.RDM)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                进程 = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.VerstoneReady), BuffIds.VerstoneReady, UIColor.NoColor),
                    new ProcConfig(UIHelper.Localize(BuffIds.VerfireReady), BuffIds.VerfireReady, UIColor.Red)
                }
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Manafication), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 6,
                Triggers = new []{
                    new Item(BuffIds.Manafication)
                },
                Color = UIColor.DarkBlue
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Manafication), new BuffProps {
                CD = 110,
                持续时间 = 15,
                图标 = ActionIds.Manafication,
                颜色 = UIColor.DarkBlue,
                触发 = new []{ new Item(ActionIds.Manafication) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.Embolden), new BuffProps {
                CD = 120,
                持续时间 = 20,
                图标 = ActionIds.Embolden,
                颜色 = UIColor.White,
                触发 = new []{ new Item(ActionIds.Embolden) }
            })
        };

        public static Cursor Cursors => new(JobIds.RDM, CursorType.无, CursorType.咏唱时间);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.RDM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.MagickBarrier), new CooldownProps {
                Icon = ActionIds.MagickBarrier,
                Duration = 10,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.MagickBarrier) }
            })
        };

        public static IconReplacer[] Icons => Array.Empty<IconReplacer>();

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
