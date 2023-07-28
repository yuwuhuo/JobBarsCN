using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace JobBars.UI {
    public enum UIIconComboType {
        战斗或生效中,
        战斗且生效中,
        仅战斗中,
        仅生效中,
        从不
    }

    public struct UIIconProps {
        public bool IsTimer;
        public UIIconComboType ComboType;
        public bool ShowRing;
    }

    public unsafe abstract class UIIcon {
        protected enum IconState {
            无,
            倒计时中,
            计时结束,
            Buff生效中
        }

        public readonly uint AdjustedId;
        public readonly uint SlotId;
        public readonly int HotbarIdx;
        public readonly int SlotIdx;
        public AtkComponentNode* Component;
        public AtkComponentIcon* IconComponent;

        protected readonly UIIconComboType ComboType;
        protected readonly bool ShowRing;

        protected IconState State = IconState.无;

        private bool Disposed = false;

        protected uint NodeIdx = 200;

        public UIIcon(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) {
            ComboType = props.ComboType;
            ShowRing = props.ShowRing;

            AdjustedId = adjustedId;
            SlotId = slotId;
            HotbarIdx = hotbarIdx;
            SlotIdx = slotIdx;
            Component = component;
            IconComponent = (AtkComponentIcon*)Component->Component;
        }

        public abstract void SetProgress(float current, float max);

        public abstract void SetDone();

        public abstract void Tick(float dashPercent, bool border);

        public abstract void OnDispose();

        protected bool CalcShowBorder(bool active, bool border) => ComboType switch {
            UIIconComboType.仅战斗中 => border,
            UIIconComboType.仅生效中 => active,
            UIIconComboType.战斗或生效中 => border || active,
            UIIconComboType.战斗且生效中 => border && active,
            UIIconComboType.从不 => false,
            _ => false
        };

        public abstract void RefreshVisuals();

        protected static void SetTextSmall(AtkTextNode* text) {
            text->AtkResNode.Width = 48;
            text->AtkResNode.Height = 12;
            text->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
            text->EdgeColor = new ByteColor { R = 51, G = 51, B = 51, A = 255 };
            text->LineSpacing = 12;
            text->AlignmentFontType = 3;
            text->FontSize = 12;
            text->TextFlags = 8;
        }

        protected static void SetTextLarge(AtkTextNode* text) {
            text->AtkResNode.Width = 40;
            text->AtkResNode.Height = 35;
            text->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
            text->EdgeColor = new ByteColor { R = 0, G = 0, B = 0, A = 255 };
            text->LineSpacing = 12;
            text->AlignmentFontType = 52;
            text->FontSize = 24;
            text->TextFlags = 8;
        }

        public void Dispose() {
            if (Disposed) {
                return;
            }
            Disposed = true;

            OnDispose();

            Component = null;
            IconComponent = null;
        }
    }
}
