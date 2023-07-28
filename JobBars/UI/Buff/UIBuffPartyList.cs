using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe class UIBuffPartyList {
        private AtkNineGridNode* 高亮;
        private AtkTextNode* TextNode;
        private bool Attached = false;

        public UIBuffPartyList() {
            高亮 = UIBuilder.CreateNineNode();
            高亮->AtkResNode.Width = 320;
            高亮->AtkResNode.Height = 48;
            UIHelper.SetupTexture(高亮, "ui/uld/PartyListTargetBase.tex");
            UIHelper.UpdatePart(高亮->PartsList, 0, 112, 0, 48, 48);
            高亮->TopOffset = 20;
            高亮->BottomOffset = 20;
            高亮->RightOffset = 20;
            高亮->LeftOffset = 20;
            高亮->PartsTypeRenderType = 220;
            高亮->AtkResNode.NodeID = 23;
            高亮->AtkResNode.Flags_2 = 0;
            高亮->AtkResNode.DrawFlags = 0;
            高亮->AtkResNode.MultiplyBlue = 50;
            高亮->AtkResNode.MultiplyRed = 150;
            UIHelper.SetPosition(高亮, 47, 21);
            UIHelper.Hide(高亮);

            TextNode = UIBuilder.CreateTextNode();
            TextNode->FontSize = (byte)JobBarsCN.设置.BuffTextSize_v2;
            TextNode->LineSpacing = (byte)JobBarsCN.设置.BuffTextSize_v2;
            TextNode->AlignmentFontType = 20;
            TextNode->FontSize = 14;
            TextNode->TextColor = new ByteColor { R = 232, G = 255, B = 254, A = 255 };
            TextNode->EdgeColor = new ByteColor { R = 8, G = 80, B = 152, A = 255 };
            TextNode->AtkResNode.X = 30;
            TextNode->AtkResNode.Y = 20;
            TextNode->AtkResNode.Flags_2 = 1;
            TextNode->AtkResNode.Priority = 0;
            TextNode->AtkResNode.NodeID = 24;
            TextNode->SetText("");
            UIHelper.Hide(TextNode);
        }

        public void Dispose() {
            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (高亮 != null) {
                UIHelper.UnloadTexture(高亮);
                高亮->AtkResNode.Destroy(true);
                高亮 = null;
            }
        }

        public void AttachTo(AtkResNode* targetGlowContainer, AtkTextNode* iconBottomLeftText) {
            if (Attached) {
                PluginLog.Error("附加完成");
                return;
            }
            if (targetGlowContainer == null || iconBottomLeftText == null) return;

            targetGlowContainer->ChildCount = 4;
            高亮->AtkResNode.ParentNode = targetGlowContainer;
            UIHelper.Link(targetGlowContainer->ChildNode->PrevSiblingNode->PrevSiblingNode, (AtkResNode*)高亮);

            // parent is the component, so don't have to worry about child count
            TextNode->AtkResNode.ParentNode = iconBottomLeftText->AtkResNode.ParentNode;
            UIHelper.Link((AtkResNode*)iconBottomLeftText, (AtkResNode*)TextNode);

            Attached = true;
        }

        public void DetachFrom(AtkResNode* targetGlowContainer, AtkTextNode* iconBottomLeftText) {
            if (!Attached) {
                PluginLog.Error("未附加");
                return;
            }
            if (targetGlowContainer == null || iconBottomLeftText == null) return;

            targetGlowContainer->ChildCount = 3;
            UIHelper.Detach((AtkResNode*)高亮);
            UIHelper.Detach((AtkResNode*)TextNode);

            Attached = false;
        }

        public void SetHighlightVisibility(bool visible) => UIHelper.SetVisibility(高亮, visible);

        public void SetText(string text) {
            if (string.IsNullOrEmpty(text)) {
                UIHelper.Hide(TextNode);
                return;
            }
            UIHelper.Show(TextNode);
            TextNode->SetText(text);
        }
    }
}
