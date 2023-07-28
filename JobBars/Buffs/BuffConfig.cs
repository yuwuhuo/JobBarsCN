using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System.Numerics;
using System.Security.Cryptography;

namespace JobBars.Buffs {
    public struct BuffProps {
        public float 持续时间;
        public float CD;
        public ActionIds 图标;
        public ElementColor 颜色;
        public Item[] 触发;
        public bool 应用于目标;
        public bool 显示小队文本;
    }

    public class BuffConfig {
        public readonly string 名称;
        public readonly float 持续时间;
        public readonly float CD;

        public readonly ActionIds 图标;
        public readonly ElementColor 颜色;
        public readonly Item[] 触发;
        public readonly bool 应用于目标;
        public readonly bool 显示小队文本;

        public bool 启用 { get; private set; }
        public bool 小队列表Buff高亮 { get; private set; }

        public BuffConfig(string name, BuffProps props) {
            名称 = name;
            持续时间 = props.持续时间;
            CD = props.CD;
            图标 = props.图标;
            颜色 = props.颜色;
            触发 = props.触发;
            应用于目标 = props.应用于目标;
            显示小队文本 = props.显示小队文本;

            启用 = JobBarsCN.设置.启用Buff.Get(名称);
            小队列表Buff高亮 = JobBarsCN.设置.小队列表Buff高亮.Get(名称);
        }

        public void Draw(string _id, ref bool reset) {
            var color = 启用 ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1);

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            if (ImGui.CollapsingHeader($"{名称}{_id}")) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if (JobBarsCN.设置.启用Buff.Draw($"启用{_id}{名称}", 名称, 启用, out var newEnabled)) {
                    启用 = newEnabled;
                    reset = true;
                }

                if (JobBarsCN.设置.小队列表Buff高亮.Draw($"可用时高亮显示小队队员栏{_id}{名称}", 名称, 小队列表Buff高亮, out var newPartyListHighlight)) {
                    小队列表Buff高亮 = newPartyListHighlight;
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }
        }
    }
}
