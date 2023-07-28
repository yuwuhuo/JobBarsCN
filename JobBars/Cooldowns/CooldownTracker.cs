using JobBars.Data;
using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Cooldowns {
    public class CooldownTracker {
        public enum TrackerState {
            无,
            生效中,
            冷却中,
            可用时
        }

        private readonly CooldownConfig Config;

        private TrackerState State = TrackerState.无;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;

        private UICooldownItem UI;

        public TrackerState CurrentState => State;
        public ActionIds Icon => Config.Icon;

        public CooldownTracker(CooldownConfig config) {
            Config = config;
        }

        public void ProcessAction(Item action) {
            if (Config.Triggers.Contains(action)) SetActive(action);
        }

        private void SetActive(Item trigger) {
            State = Config.Duration == 0 ? TrackerState.冷却中 : TrackerState.生效中;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void Tick(Dictionary<Item, Status> buffDict) {
            if (State != TrackerState.生效中 && UIHelper.CheckForTriggers(buffDict, Config.Triggers, out var trigger)) SetActive(trigger);

            if (State == TrackerState.生效中) {
                TimeLeft = UIHelper.TimeLeft(JobBarsCN.设置.CooldownsHideActiveBuffDuration ? 0 : Config.Duration, buffDict, LastActiveTrigger, LastActiveTime);
                if(TimeLeft <= 0) {
                    TimeLeft = 0;
                    State = TrackerState.冷却中; // mitigation needs to have a CD
                }
            }
            else if (State == TrackerState.冷却中) {
                TimeLeft = (float)(Config.CD - (DateTime.Now - LastActiveTime).TotalSeconds);

                if (TimeLeft <= 0) {
                    State = TrackerState.可用时;
                }
            }
        }

        public void TickUI(UICooldownItem ui, float percent) {
            if (UI != ui || UI?.IconId != Config.Icon) {
                UI = ui;
                SetupUI();
            }

            UI.Show();

            if (State == TrackerState.无) {
                ui.SetOffCD();
                ui.SetText("");
                ui.SetNoDash();
            }
            else if (State == TrackerState.生效中) {
                ui.SetOffCD();
                ui.SetText(((int)Math.Round(TimeLeft)).ToString());
                if (Config.ShowBorderWhenActive) {
                    ui.SetDash(percent);
                }
                else {
                    ui.SetNoDash();
                }
            }
            else if (State == TrackerState.冷却中) {
                ui.SetOnCD(JobBarsCN.设置.CooldownsOnCDOpacity);
                ui.SetText(((int)Math.Round(TimeLeft)).ToString());
                ui.SetNoDash();
            }
            else if (State == TrackerState.可用时) {
                ui.SetOffCD();
                ui.SetText("");
                if (Config.ShowBorderWhenOffCD) {
                    ui.SetDash(percent);
                }
                else {
                    ui.SetNoDash();
                }
            }
        }

        private void SetupUI() {
            UI.LoadIcon(Config.Icon);
        }

        public void Reset() {
            State = TrackerState.无;
        }
    }
}