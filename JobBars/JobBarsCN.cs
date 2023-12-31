﻿using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Plugin;
using JobBars.Buffs.Manager;
using JobBars.Cooldowns.Manager;
using JobBars.Cursors.Manager;
using JobBars.Data;
using JobBars.Gauges.Manager;
using JobBars.Helper;
using JobBars.Icons.Manager;
using JobBars.UI;
using System;
using System.Reflection;
using System.Threading;

namespace JobBars {
    public unsafe partial class JobBarsCN : IDalamudPlugin {
        public static DalamudPluginInterface PluginInterface { get; private set; }
        public static ClientState ClientState { get; private set; }
        public static JobGauges JobGauges { get; private set; }
        public static Framework Framework { get; private set; }
        public static Condition Condition { get; private set; }
        public static CommandManager CommandManager { get; private set; }
        public static ObjectTable Objects { get; private set; }
        public static SigScanner SigScanner { get; private set; }
        public static DataManager DataManager { get; private set; }

        public static Configuration 设置 { get; private set; }
        public static UIBuilder Builder { get; private set; }
        public static UIIconManager IconBuilder { get; private set; }

        public static GaugeManager GaugeManager { get; private set; }
        public static BuffManager BuffManager { get; private set; }
        public static CooldownManager CooldownManager { get; private set; }
        public static CursorManager CursorManager { get; private set; }
        public static IconManager IconManager { get; private set; }

        public string Name => "JobBarsCN";
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public static JobIds CurrentJob { get; private set; } = JobIds.OTHER;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> ActorControlSelfHook;

        private delegate IntPtr IconDimmedDelegate(IntPtr iconUnk, byte dimmed);
        private Hook<IconDimmedDelegate> IconDimmedHook;

        private static bool PlayerExists => ClientState?.LocalPlayer != null;
        private static bool RecreateUI => Condition[ConditionFlag.CreatingCharacter]; // getting haircut, etc.
        private bool LoggedOut = true;

        public static AttachAddon AttachAddon { get; private set; } = AttachAddon.Chatbox;
        public static AttachAddon CooldownAttachAddon { get; private set; } = AttachAddon.PartyList;

        private bool IsLoaded = false;

        public JobBarsCN(
                DalamudPluginInterface pluginInterface,
                ClientState clientState,
                CommandManager commandManager,
                Condition condition,
                Framework framework,
                ObjectTable objects,
                SigScanner sigScanner,
                DataManager dataManager,
                JobGauges jobGauges
            ) {
            PluginInterface = pluginInterface;
            ClientState = clientState;
            Framework = framework;
            Condition = condition;
            CommandManager = commandManager;
            Objects = objects;
            SigScanner = sigScanner;
            DataManager = dataManager;
            JobGauges = jobGauges;
#if RELEASE
            if (PluginInterface.IsDev) return;
#endif

            UIHelper.Setup();
            UIColor.SetupColors();

            // Upgrade if config is too old
            try {
                设置 = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            }
            catch (Exception e) {
                PluginLog.LogError("加载失败", e);
                设置 = new Configuration();
                设置.Save();
            }
            if (设置.Version < 1) {
                PluginLog.Log("设置版本过旧");
                设置 = new Configuration();
                设置.Save();
            }

            AttachAddon = 设置.AttachAddon;
            CooldownAttachAddon = 设置.CooldownAttachAddon;
            IconBuilder = new UIIconManager();

            // ==========================

            InitializeUI();

            IntPtr receiveActionEffectFuncPtr = SigScanner.ScanText(Constants.ReceiveActionEffectSig);
            ReceiveActionEffectHook = Hook<ReceiveActionEffectDelegate>.FromAddress(receiveActionEffectFuncPtr, ReceiveActionEffect);
            ReceiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = SigScanner.ScanText(Constants.ActorControlSig);
            ActorControlSelfHook = Hook<ActorControlSelfDelegate>.FromAddress(actorControlSelfPtr, ActorControlSelf);
            ActorControlSelfHook.Enable();

            IntPtr iconDimmedPtr = SigScanner.ScanText(Constants.IconDimmedSig);
            IconDimmedHook = Hook<IconDimmedDelegate>.FromAddress(iconDimmedPtr, IconDimmedDetour);
            IconDimmedHook.Enable();

            PluginInterface.UiBuilder.Draw += BuildSettingsUI;
            PluginInterface.UiBuilder.Draw += Animate;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfig;
            Framework.Update += FrameworkOnUpdate;
            ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        private void InitializeUI() {
            // these are created before the addons are even visible, so they aren't attached yet
            PluginLog.Log("==== INIT ====");
            IconBuilder.Reset();

            Builder = new UIBuilder();
            BuffManager = new BuffManager();
            CooldownManager = new CooldownManager();
            GaugeManager = new GaugeManager();
            CursorManager = new CursorManager();
            IconManager = new IconManager();

            IsLoaded = true;
        }

        public void Dispose() {
            ReceiveActionEffectHook?.Disable();
            ActorControlSelfHook?.Disable();
            IconDimmedHook?.Disable();

            Thread.Sleep(500);

            ReceiveActionEffectHook?.Dispose();
            ActorControlSelfHook?.Dispose();
            IconDimmedHook?.Dispose();

            ReceiveActionEffectHook = null;
            ActorControlSelfHook = null;
            IconDimmedHook = null;

            PluginInterface.UiBuilder.Draw -= BuildSettingsUI;
            PluginInterface.UiBuilder.Draw -= Animate;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfig;
            Framework.Update -= FrameworkOnUpdate;
            ClientState.TerritoryChanged -= ZoneChanged;

            GaugeManager = null;
            BuffManager = null;
            CooldownManager = null;
            CursorManager = null;
            IconManager = null;

            Animation.Dispose();
            IconBuilder?.Dispose();
            Builder?.Dispose();
            IconBuilder = null;
            Builder = null;

            PluginInterface = null;
            设置 = null;

            RemoveCommands();
        }

        private void Animate() {
            if (!IsLoaded) return;
            Animation.Tick();
        }

        private void FrameworkOnUpdate(Framework framework) {
            if (!IsLoaded) return;

            var addon = UIHelper.BuffGaugeAttachAddon;

            if (!LoggedOut && RecreateUI) {
                Logout();
                return;
            }

            if (!PlayerExists) {
                if (!LoggedOut && (addon == null)) Logout();
                return;
            }

            if (addon == null || addon->RootNode == null || RecreateUI) return;

            if (LoggedOut) {
                PluginLog.Log("====== REATTACH =======");
                Builder.Attach(); // re-attach after addons have been created
                LoggedOut = false;
                return;
            }

            UIHelper.TickTextures();
            CheckForJobChange();
            Tick();

            GaugeManager.UpdatePositionScale();
            BuffManager.UpdatePositionScale();
            CooldownManager.UpdatePositionScale();
        }

        private void Logout() {
            PluginLog.Log("==== LOGOUT ====");
            IconBuilder.Reset();
            Animation.Dispose();

            LoggedOut = true;
            CurrentJob = JobIds.OTHER;
        }

        private void CheckForJobChange() {
            var jobId = ClientState.LocalPlayer.ClassJob;
            JobIds job = UIHelper.IdToJob(jobId.Id);

            if (job != CurrentJob) {
                CurrentJob = job;
                PluginLog.Log($"职业切换为 {CurrentJob}");
                GaugeManager.SetJob(CurrentJob);
                CursorManager.SetJob(CurrentJob);
                IconManager.SetJob(CurrentJob);
            }
        }

        private void Tick() {
            if (!IsLoaded) return;

            UIHelper.UpdateMp(ClientState.LocalPlayer.CurrentMp);
            UIHelper.UpdatePlayerStatus();

            UpdatePartyMembers();
            GaugeManager.Tick();
            BuffManager.Tick();
            CooldownManager.Tick();
            CursorManager.Tick();
            IconManager.Tick();

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = (float)(millis % 1000) / 1000;

            Builder.Tick(设置.GaugePulse ? percent : 0f);
        }

        // ======= COMMANDS ============

        private void SetupCommands() {
            CommandManager.AddHandler("/jobbars", new CommandInfo(OnCommand) {
                HelpMessage = $"打开设置窗口 {Name}",
                ShowInHelp = true
            });
        }

        private void OnOpenConfig() {
            if (!IsLoaded) return;
            Visible = true;
        }

        public void OnCommand(object command, object args) {
            Visible = !Visible;
        }

        public void RemoveCommands() {
            CommandManager.RemoveHandler("/jobbars");
        }
    }
}
