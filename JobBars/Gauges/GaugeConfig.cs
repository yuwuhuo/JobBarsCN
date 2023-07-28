using JobBars.Data;
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
        public readonly string 名称;
        public GaugeVisualType Type { get; private set; }
        public GaugeTypeConfig TypeConfig { get; private set; }

        public bool Enabled { get; protected set; }
        public int Order { get; private set; }
        public float Scale { get; private set; }
        public bool HideWhenInactive { get; private set; }
        public int SoundEffect { get; private set; }
        public int CompletionSoundEffect { get; private set; }
        public Vector2 Position => JobBarsCN.设置.GaugeSplitPosition.Get(名称);

        public static readonly GaugeCompleteSoundType[] 生效音效类型 = (GaugeCompleteSoundType[])Enum.GetValues(typeof(GaugeCompleteSoundType));

        public GaugeConfig(string name, GaugeVisualType type) {
            名称 = name;
            Enabled = JobBarsCN.设置.GaugeEnabled.Get(名称);
            Order = JobBarsCN.设置.GaugeOrder.Get(名称);
            Scale = JobBarsCN.设置.GaugeIndividualScale.Get(名称);
            HideWhenInactive = JobBarsCN.设置.GaugeHideInactive.Get(名称);
            SoundEffect = JobBarsCN.设置.GaugeSoundEffect_2.Get(名称);
            CompletionSoundEffect = JobBarsCN.设置.GaugeCompletionSoundEffect_2.Get(名称);
            SetType(JobBarsCN.设置.GaugeType.Get(名称, type));
        }

        public abstract GaugeTracker GetTracker(int idx);

        private void SetType(GaugeVisualType type) {
            var validTypes = GetValidGaugeTypes();
            Type = validTypes.Contains(type) ? type : validTypes[0];
            TypeConfig = Type switch {
                GaugeVisualType.条状 => new GaugeBarConfig(名称),
                GaugeVisualType.Diamond => new GaugeDiamondConfig(名称),
                GaugeVisualType.Arrow => new GaugeArrowConfig(名称),
                GaugeVisualType.BarDiamondCombo => new GaugeBarDiamondComboConfig(名称),
                _ => null
            };
        }

        public void Draw(string id, out bool newVisual, out bool reset) {
            newVisual = reset = false;

            if (JobBarsCN.设置.GaugeEnabled.Draw($"启用{id}", 名称, Enabled, out var newEnabled)) {
                Enabled = newEnabled;
                newVisual = true;
            }

            if (JobBarsCN.设置.GaugeHideInactive.Draw($"非战斗状态隐藏{id}", 名称, HideWhenInactive, out var newHideWhenInactive)) {
                HideWhenInactive = newHideWhenInactive;
            }

            if (JobBarsCN.设置.GaugeIndividualScale.Draw($"尺寸{id}", 名称, out var newScale)) {
                Scale = Math.Max(0.1f, newScale);
                newVisual = true;
            }

            if (JobBarsCN.设置.量谱位置类型 == GaugePositionType.Split) {
                if (JobBarsCN.设置.GaugeSplitPosition.Draw($"分体位置{id}", 名称, out var newPosition)) {
                    SetSplitPosition(newPosition);
                    newVisual = true;
                }
            }

            if (JobBarsCN.设置.GaugeOrder.Draw($"顺序{id}", 名称, Order, out var newOrder)) {
                Order = newOrder;
                newVisual = true;
            }

            var validTypes = GetValidGaugeTypes();
            if (validTypes.Length > 1) {
                if (JobBarsCN.设置.GaugeType.Draw($"Type{id}", 名称, validTypes, Type, out var newType)) {
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
            if (JobBarsCN.设置.GaugeSoundEffect_2.Draw($"{label} (0 = off)", 名称, SoundEffect, out var newSoundEffect)) {
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
            if (JobBarsCN.设置.GaugeCompletionSoundEffect_2.Draw($"完成音效 (0 = off)", 名称, CompletionSoundEffect, out var newSoundEffect)) {
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
            if (JobBarsCN.DrawPositionView(名称 + "##量谱位置", Position, out var pos)) {
                JobBarsCN.设置.GaugeSplitPosition.Set(名称, pos);
                SetSplitPosition(pos);
                JobBarsCN.GaugeManager.UpdatePositionScale();
            }
        }

        protected abstract GaugeVisualType[] GetValidGaugeTypes();

        protected abstract void DrawConfig(string id, ref bool newVisual, ref bool reset);

        private void SetSplitPosition(Vector2 pos) {
            JobBarsCN.SetWindowPosition(名称 + "##量谱位置", pos);
        }
    }
}
