using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public class BuffTracker {
        public enum BuffState {
            None, // hidden
            Running, // 亮，显示倒计时
            OffCD, // 亮，无文字
            OnCD_Hidden,
            OnCD_Visible // dark, show countdown
        }

        private readonly BuffConfig Config;

        private BuffState State = BuffState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;
        private float Percent;

        private UIBuff UI;

        public BuffState CurrentState => State;
        public uint Id => (uint)Config.图标;
        public bool Enabled => (State == BuffState.Running || State == BuffState.OffCD || State == BuffState.OnCD_Visible);
        public bool Active => State == BuffState.Running;
        public bool Highlight => Active && Config.小队列表Buff高亮;
        public bool 显示小队文本 => Config.显示小队文本;
        public bool ApplyToTarget => Config.应用于目标;
        public string Text => ((int)Math.Round(TimeLeft)).ToString();

        public BuffTracker(BuffConfig config) {
            Config = config;
        }

        public void ProcessAction(Item action) {
            if (Config.触发.Contains(action)) SetActive(action);
        }

        private void SetActive(Item trigger) {
            State = BuffState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void Tick(Dictionary<Item, Status> buffDict) {
            if (State != BuffState.Running && UIHelper.CheckForTriggers(buffDict, Config.触发, out var trigger)) SetActive(trigger);

            if (State == BuffState.Running) {
                TimeLeft = UIHelper.TimeLeft(Config.持续时间, buffDict, LastActiveTrigger, LastActiveTime);
                if (TimeLeft <= 0) { // Buff over
                    Percent = 1f;
                    TimeLeft = 0;

                    State = Config.CD <= 0 ? BuffState.None : // doesn't have a cooldown, just make it invisible
                        Config.CD <= JobBarsCN.设置.BuffDisplayTimer ? BuffState.OnCD_Visible : BuffState.OnCD_Hidden;
                }
                else { // Still running
                    Percent = 1.0f - (float)(TimeLeft / Config.持续时间);
                }
            }
            else if (State == BuffState.OnCD_Hidden || State == BuffState.OnCD_Visible) {
                TimeLeft = (float)(Config.CD - (DateTime.Now - LastActiveTime).TotalSeconds);

                if (TimeLeft <= 0) {
                    State = BuffState.OffCD;
                }
                else if (TimeLeft <= JobBarsCN.设置.BuffDisplayTimer) { // CD low enough to be visible
                    State = BuffState.OnCD_Visible;
                    Percent = TimeLeft / Config.CD;
                }
            }
        }

        public void TickUI(UIBuff ui) {
            if (UI != ui || UI?.IconId != Config.图标) {
                UI = ui;
                SetupUI();
            }

            UI.Show();
            UI.SetColor(Config.颜色);

            if (State == BuffState.Running) {
                UI.SetOffCD();
                UI.SetPercent(Percent);
                UI.SetText(Text);
            }
            else if (State == BuffState.OffCD) {
                UI.SetOffCD();
                UI.SetPercent(0);
                UI.SetText("");
            }
            else if (State == BuffState.OnCD_Visible) {
                UI.SetOnCD(JobBarsCN.设置.BuffOnCDOpacity);
                UI.SetPercent(Percent);
                UI.SetText(Text);
            }
        }

        private void SetupUI() {
            UI.LoadIcon(Config.图标);
        }

        public void Reset() {
            State = BuffState.None;
        }
    }
}
