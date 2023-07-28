﻿using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.UI {
    public unsafe class UIIconBuff : UIIcon {
        private AtkResNode* OriginalOverlay;
        private AtkImageNode* Combo;
        private AtkTextNode* BigText;

        public UIIconBuff(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) :
            base(adjustedId, slotId, hotbarIdx, slotIdx, component, props) {

            var nodeList = Component->Component->UldManager.NodeList;
            OriginalOverlay = nodeList[1];
            var originalBorder = (AtkImageNode*)nodeList[4];

            Combo = UIHelper.CleanAlloc<AtkImageNode>();
            Combo->Ctor();
            Combo->AtkResNode.NodeID = NodeIdx++;
            Combo->AtkResNode.Type = NodeType.Image;
            Combo->AtkResNode.X = -2;
            Combo->AtkResNode.Width = 48;
            Combo->AtkResNode.Height = 48;
            Combo->AtkResNode.Flags = 8243;
            Combo->AtkResNode.Flags_2 = 1;
            Combo->AtkResNode.Flags_2 |= 4;
            Combo->WrapMode = 1;
            Combo->PartId = 0;
            Combo->PartsList = originalBorder->PartsList;

            BigText = UIHelper.CleanAlloc<AtkTextNode>();
            BigText->Ctor();
            BigText->AtkResNode.NodeID = NodeIdx++;
            BigText->AtkResNode.Type = NodeType.Text;
            BigText->AtkResNode.Flags = 8243;
            BigText->AtkResNode.Flags_2 = 1;
            BigText->AtkResNode.Flags_2 |= 4;
            RefreshVisuals();
            BigText->SetText("");

            var rootNode = (AtkResNode*)Component;
            var macroIcon = nodeList[15];
            Combo->AtkResNode.ParentNode = rootNode;
            BigText->AtkResNode.ParentNode = rootNode;

            UIHelper.Link(OriginalOverlay, (AtkResNode*)Combo);
            UIHelper.Link((AtkResNode*)Combo, (AtkResNode*)BigText);
            UIHelper.Link((AtkResNode*)BigText, macroIcon);

            Component->Component->UldManager.UpdateDrawNodeList();

            UIHelper.Hide(Combo);
            UIHelper.Hide(BigText);
        }

        public override void SetProgress(float current, float max) {
            if (State != IconState.BuffRunning) {
                State = IconState.BuffRunning;
                UIHelper.Hide(OriginalOverlay);
                UIHelper.Show(BigText);
                UIHelper.Show(Combo);
            }
            BigText->SetText(((int)Math.Round(current)).ToString());
        }

        public override void SetDone() {
            if (State == IconState.None) return;
            State = IconState.None;

            UIHelper.Hide(BigText);
            UIHelper.Hide(Combo);
            UIHelper.Show(OriginalOverlay);
        }

        public override void Tick(float dashPercent, bool border) {
            // avoid doubling up on borders if combo_or_active
            var showBorder = CalcShowBorder(State == IconState.BuffRunning, false);
            Combo->PartId = !showBorder ? (ushort)0 : (ushort)(6 + dashPercent * 7);
            UIHelper.SetVisibility(Combo, showBorder);
        }

        public override void OnDispose() {
            UIHelper.Link(OriginalOverlay, BigText->AtkResNode.PrevSiblingNode);
            Component->Component->UldManager.UpdateDrawNodeList();

            if (Combo != null) {
                Combo->AtkResNode.Destroy(true);
                Combo = null;
            }

            if (BigText != null) {
                BigText->AtkResNode.Destroy(true);
                BigText = null;
            }

            UIHelper.Show(OriginalOverlay);
            OriginalOverlay = null;
        }

        public override void RefreshVisuals() {
            if (JobBarsCN.Config.IconBuffLarge) {
                BigText->AtkResNode.X = 2;
                BigText->AtkResNode.Y = 7;
                SetTextLarge(BigText);
            }
            else {
                BigText->AtkResNode.X = 0;
                BigText->AtkResNode.Y = 37;
                SetTextSmall(BigText);
            }
        }
    }
}
