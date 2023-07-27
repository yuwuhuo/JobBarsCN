﻿using JobBars.Data;
using System;
using System.Linq;
using System.Numerics;

using JobBars.Gauges.Types;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Diamond;
using JobBars.Gauges.Types.BarDiamondCombo;
using ImGuiNET;

namespace JobBars.Gauges {
    public abstract class GaugeConfig {
        public readonly string Name;
        public GaugeVisualType Type { get; private set; }
        public GaugeTypeConfig TypeConfig { get; private set; }

        public bool Enabled { get; protected set; }
        public int Order { get; private set; }
        public float Scale { get; private set; }
        public bool HideWhenInactive { get; private set; }
        public int SoundEffect { get; private set; }
        public int CompletionSoundEffect { get; private set; }
        public Vector2 Position => JobBarsCN.Config.GaugeSplitPosition.Get(Name);

        public static readonly GaugeCompleteSoundType[] ValidSoundType = (GaugeCompleteSoundType[])Enum.GetValues(typeof(GaugeCompleteSoundType));

        public GaugeConfig(string name, GaugeVisualType type) {
            Name = name;
            Enabled = JobBarsCN.Config.GaugeEnabled.Get(Name);
            Order = JobBarsCN.Config.GaugeOrder.Get(Name);
            Scale = JobBarsCN.Config.GaugeIndividualScale.Get(Name);
            HideWhenInactive = JobBarsCN.Config.GaugeHideInactive.Get(Name);
            SoundEffect = JobBarsCN.Config.GaugeSoundEffect_2.Get(Name);
            CompletionSoundEffect = JobBarsCN.Config.GaugeCompletionSoundEffect_2.Get(Name);
            SetType(JobBarsCN.Config.GaugeType.Get(Name, type));
        }

        public abstract GaugeTracker GetTracker(int idx);

        private void SetType(GaugeVisualType type) {
            var validTypes = GetValidGaugeTypes();
            Type = validTypes.Contains(type) ? type : validTypes[0];
            TypeConfig = Type switch {
                GaugeVisualType.Bar => new GaugeBarConfig(Name),
                GaugeVisualType.Diamond => new GaugeDiamondConfig(Name),
                GaugeVisualType.Arrow => new GaugeArrowConfig(Name),
                GaugeVisualType.BarDiamondCombo => new GaugeBarDiamondComboConfig(Name),
                _ => null
            };
        }

        public void Draw(string id, out bool newVisual, out bool reset) {
            newVisual = reset = false;

            if (JobBarsCN.Config.GaugeEnabled.Draw($"启用{id}", Name, Enabled, out var newEnabled)) {
                Enabled = newEnabled;
                newVisual = true;
            }

            if (JobBarsCN.Config.GaugeHideInactive.Draw($"非战斗状态隐藏{id}", Name, HideWhenInactive, out var newHideWhenInactive)) {
                HideWhenInactive = newHideWhenInactive;
            }

            if (JobBarsCN.Config.GaugeIndividualScale.Draw($"尺寸{id}", Name, out var newScale)) {
                Scale = Math.Max(0.1f, newScale);
                newVisual = true;
            }

            if (JobBarsCN.Config.GaugePositionType == GaugePositionType.Split) {
                if (JobBarsCN.Config.GaugeSplitPosition.Draw($"分体位置{id}", Name, out var newPosition)) {
                    SetSplitPosition(newPosition);
                    newVisual = true;
                }
            }

            if (JobBarsCN.Config.GaugeOrder.Draw($"顺序{id}", Name, Order, out var newOrder)) {
                Order = newOrder;
                newVisual = true;
            }

            var validTypes = GetValidGaugeTypes();
            if (validTypes.Length > 1) {
                if (JobBarsCN.Config.GaugeType.Draw($"Type{id}", Name, validTypes, Type, out var newType)) {
                    SetType(newType);
                    reset = true;
                }
            }

            TypeConfig.Draw(id, ref newVisual, ref reset);

            DrawConfig(id, ref newVisual, ref reset);
        }

        protected void DrawSoundEffect(string label = "进度音效") {
            if (ImGui.Button("测试##音效")) Helper.UIHelper.PlaySoundEffect(SoundEffect);
            ImGui.SameLine();

            ImGui.SetNextItemWidth(200f);
            if (JobBarsCN.Config.GaugeSoundEffect_2.Draw($"{label} (0 = off)", Name, SoundEffect, out var newSoundEffect)) {
                SoundEffect = newSoundEffect;
            }
            ImGui.SameLine();
            HelpMarker("对于宏音效，请添加 36。例如，<se.6> 将是 6+36=42");
        }

        public void PlaySoundEffect() => Helper.UIHelper.PlaySoundEffect(SoundEffect);

        protected void DrawCompletionSoundEffect() {
            if (ImGui.Button("测试##完成音效")) Helper.UIHelper.PlaySoundEffect(CompletionSoundEffect);
            ImGui.SameLine();

            ImGui.SetNextItemWidth(200f);
            if (JobBarsCN.Config.GaugeCompletionSoundEffect_2.Draw($"完成音效 (0 = off)", Name, CompletionSoundEffect, out var newSoundEffect)) {
                CompletionSoundEffect = newSoundEffect;
            }
            ImGui.SameLine();
            HelpMarker("对于宏音效，请添加 36。例如，<se.6> 将是 6+36=42");
        }

        public void PlayCompletionSoundEffect() => Helper.UIHelper.PlaySoundEffect(CompletionSoundEffect);

        public static void HelpMarker(string text) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 5);
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(text);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public void DrawPositionBox() {
            if (JobBarsCN.DrawPositionView(Name + "##量谱位置", Position, out var pos)) {
                JobBarsCN.Config.GaugeSplitPosition.Set(Name, pos);
                SetSplitPosition(pos);
                JobBarsCN.GaugeManager.UpdatePositionScale();
            }
        }

        protected abstract GaugeVisualType[] GetValidGaugeTypes();

        protected abstract void DrawConfig(string id, ref bool newVisual, ref bool reset);

        private void SetSplitPosition(Vector2 pos) {
            JobBarsCN.SetWindowPosition(Name + "##量谱位置", pos);
        }
    }
}