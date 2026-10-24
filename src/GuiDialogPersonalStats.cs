using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PersonalStats
{
    public class GuiDialogPersonalStats : GuiDialog
    {
        readonly PersonalStatsModSystem modSystem;

        public override string ToggleKeyCombinationCode => null;

        public GuiDialogPersonalStats(ICoreClientAPI capi, PersonalStatsModSystem modSystem) : base(capi)
        {
            this.modSystem = modSystem;
            ComposeDialog();
        }

        void ComposeDialog()
        {
            ElementBounds textBounds = ElementBounds.Fixed(0, 30, 260, 25);
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog
                .WithAlignment(EnumDialogArea.CenterMiddle)
                .WithFixedAlignmentOffset(0, 0);

            SingleComposer = capi.Gui.CreateCompo("personalstatsdialog", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Personal Stats", OnTitleBarClose)
                .BeginChildElements(bgBounds)
                    .AddStaticText("Distance walked:", CairoFont.WhiteSmallText(), textBounds = ElementBounds.Fixed(0, 30, 260, 25))
                    .AddDynamicText("", CairoFont.WhiteSmallText().WithFontSize(18), textBounds = textBounds.BelowCopy(0, 5), "distanceText")
                    .AddStaticText("Days survived:", CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 15))
                    .AddDynamicText("", CairoFont.WhiteSmallText().WithFontSize(18), textBounds = textBounds.BelowCopy(0, 5), "daysText")
                    .AddStaticText("Deaths:", CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 15))
                    .AddDynamicText("", CairoFont.WhiteSmallText().WithFontSize(18), textBounds = textBounds.BelowCopy(0, 5), "deathsText")
                    .AddStaticText("Blocks broken:", CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 15))
                    .AddDynamicText("", CairoFont.WhiteSmallText().WithFontSize(18), textBounds = textBounds.BelowCopy(0, 5), "blocksText")
                    .AddStaticText("Mobs killed:", CairoFont.WhiteSmallText(), textBounds = textBounds.BelowCopy(0, 15))
                    .AddDynamicText("", CairoFont.WhiteSmallText().WithFontSize(18), textBounds = textBounds.BelowCopy(0, 5), "mobsText")
                .EndChildElements()
                .Compose();

            UpdateValues();
        }

        void OnTitleBarClose()
        {
            TryClose();
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();
            UpdateValues();
        }

        public override void OnRenderGUI(float deltaTime)
        {
            base.OnRenderGUI(deltaTime);
            UpdateValues();
        }

        void UpdateValues()
        {
            double distance = modSystem.GetDistanceWalked();
            string distanceStr = distance >= 1000
                ? $"{distance / 1000.0:0.00} km"
                : $"{distance:0} m";

            double days = modSystem.GetDaysSurvived();
            int deaths = modSystem.GetDeaths();
            int blocksBroken = modSystem.GetBlocksBroken();
            int mobsKilled = modSystem.GetMobsKilled();

            SingleComposer.GetDynamicText("distanceText")?.SetNewText(distanceStr);
            SingleComposer.GetDynamicText("daysText")?.SetNewText($"{days:0.0}");
            SingleComposer.GetDynamicText("deathsText")?.SetNewText($"{deaths}");
            SingleComposer.GetDynamicText("blocksText")?.SetNewText($"{blocksBroken}");
            SingleComposer.GetDynamicText("mobsText")?.SetNewText($"{mobsKilled}");
        }
    }
}
