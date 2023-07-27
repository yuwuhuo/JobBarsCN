﻿using Dalamud.Interface;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Numerics;
using System.Security.Cryptography;

namespace JobBars {
    public unsafe partial class JobBarsCN {
        public bool Visible = false;

        public static readonly AttachAddon[] ValidAttachTypes = (AttachAddon[])Enum.GetValues(typeof(AttachAddon));
        public static readonly Vector4 RED_COLOR = new(0.85098039216f, 0.32549019608f, 0.30980392157f, 1.0f);
        public static readonly Vector4 GREEN_COLOR = new(0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f);

        private readonly InfoBox<JobBarsCN> RequiresRestartInfoBox = new() {
            Label = "重启游戏起效",
            ContentsAction = (JobBarsCN item) => {
                if (ImGui.Checkbox("使用 4K 纹理##职业_Settings", ref Config.Use4K)) {
                    Config.Save();
                }

                ImGui.SetNextItemWidth(200f);
                if (DrawCombo(ValidAttachTypes, Config.AttachAddon, "量谱/Buff计时/光标 UI 元素", "##职业_设置", out var newAttach)) {
                    Config.AttachAddon = newAttach;
                    Config.Save();
                }

                ImGui.SetNextItemWidth(200f);
                if (DrawCombo(ValidAttachTypes, Config.CooldownAttachAddon, "冷却 UI 元素", "##职业_设置", out var newCDAttach)) {
                    Config.CooldownAttachAddon = newCDAttach;
                    Config.Save();
                }
            }
        };

        private static readonly string Text = "选择 UI 元素不洽合于隐藏它们的插件（例如 Chatbox 的 Chat2、PartyList的 Delv UI）。另外，在选择量谱作为小队列表元素时，请确保在“角色设置（K）”>“界面设置”>“小队列表”中关闭“单人时隐藏队伍列表”。";

        protected static void DisplayWarning() {
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1, 0, 0, 0.3f));
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(1, 0, 0, 0.1f));

            var textSize = ImGui.CalcTextSize(Text, ImGui.GetContentRegionMax().X - 40);

            ImGui.BeginChild("##动画警告", new Vector2(-1,
                textSize.Y +
                ImGui.GetStyle().ItemSpacing.Y * 2 +
                ImGui.GetStyle().FramePadding.Y * 2 + 5
            ), true);

            ImGui.TextWrapped(Text);

            ImGui.EndChild();
            ImGui.PopStyleColor(2);
        }

        private void BuildSettingsUI() {
            if (!IsLoaded) return;
            if (!PlayerExists) return;
            if (!Visible) return;

            string _ID = "##职业_设置";
            ImGui.SetNextWindowSize(new Vector2(600, 1000), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("技能栏设置", ref Visible)) {
                RequiresRestartInfoBox.Draw(this);

                DisplayWarning();

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);

                // ==========================

                ImGui.BeginTabBar("选项卡" + _ID);
                if (ImGui.BeginTabItem("量谱" + _ID)) {
                    GaugeManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("图标" + _ID)) {
                    IconManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Buffs" + _ID)) {
                    BuffManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("冷却时间" + _ID)) {
                    CooldownManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("光标" + _ID)) {
                    CursorManager?.Draw();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
            ImGui.End();

            GaugeManager?.DrawPositionBox();
            BuffManager?.DrawPositionBox();
        }

        public static void SetWindowPosition(string Id, Vector2 position) {
            var minPosition = ImGuiHelpers.MainViewport.Pos;
            ImGui.SetWindowPos(Id, position + minPosition);
        }

        public static bool DrawPositionView(string Id, Vector2 position, out Vector2 newPosition) {
            ImGuiHelpers.ForceNextWindowMainViewport();
            var minPosition = ImGuiHelpers.MainViewport.Pos;
            ImGui.SetNextWindowPos(position + minPosition, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.7f);
            ImGui.Begin(Id, ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize);
            newPosition = ImGui.GetWindowPos() - minPosition;
            ImGui.PopStyleVar(1);
            ImGui.End();
            return newPosition != position;
        }

        public static void Separator() {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
            ImGui.Separator();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
        }

        public static bool RemoveButton(string label, bool small = false) => ColorButton(label, RED_COLOR, small);

        public static bool OkButton(string label, bool small = false) => ColorButton(label, GREEN_COLOR, small);

        public static bool ColorButton(string label, Vector4 color, bool small) {
            var ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            if (small) {
                if (ImGui.SmallButton(label)) {
                    ret = true;
                }
            }
            else {
                if (ImGui.Button(label)) {
                    ret = true;
                }
            }
            ImGui.PopStyleColor();
            return ret;
        }

        public static bool DrawCombo<T>(T[] validOptions, T currentValue, string label, string _ID, out T newValue) {
            newValue = currentValue;
            var ret = false;
            if (ImGui.BeginCombo(label + _ID, $"{currentValue}")) {
                foreach (var value in validOptions) {
                    if (ImGui.Selectable($"{value}" + _ID, value.Equals(currentValue))) {
                        ret = true;
                        newValue = value;
                    }
                }
                ImGui.EndCombo();
            }
            return ret;
        }
    }
}