using Godot;

namespace KheloScan
{
    public partial class RecordScreen : Control
    {
        // -- Color palette ------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorFieldGreen   = new Color(0.086f, 0.639f, 0.290f);
        private static readonly Color ColorGoldMedal    = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorBg           = new Color(0.945f, 0.961f, 0.976f);
        private static readonly Color ColorTextPrimary  = new Color(0.059f, 0.090f, 0.165f);
        private static readonly Color ColorTextSecondary= new Color(0.278f, 0.333f, 0.412f);
        private static readonly Color ColorTextTertiary = new Color(0.580f, 0.639f, 0.722f);
        private static readonly Color ColorBorder       = new Color(0.886f, 0.910f, 0.941f);
        private static readonly Color ColorWhite        = new Color(1f, 1f, 1f);

        public override void _Ready()
        {
            SetAnchorsPreset(LayoutPreset.FullRect);
            ClipContents = true;

            var bgRect = new ColorRect();
            bgRect.Color = ColorBg;
            bgRect.SetAnchorsPreset(LayoutPreset.FullRect);
            bgRect.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            bgRect.SizeFlagsVertical   = SizeFlags.ExpandFill;
            AddChild(bgRect);

            var rootVBox = new VBoxContainer();
            rootVBox.SetAnchorsPreset(LayoutPreset.FullRect);
            rootVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            rootVBox.SizeFlagsVertical   = SizeFlags.ExpandFill;
            rootVBox.AddThemeConstantOverride("separation", 0);
            AddChild(rootVBox);

            // Header
            BuildHeader(rootVBox);

            // Scroll content
            var scroll = new ScrollContainer();
            scroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            scroll.SizeFlagsVertical   = SizeFlags.ExpandFill;
            scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            rootVBox.AddChild(scroll);

            var contentVBox = new VBoxContainer();
            contentVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            contentVBox.AddThemeConstantOverride("separation", 0);
            scroll.AddChild(contentVBox);

            // Camera viewfinder mockup
            BuildViewfinder(contentVBox);

            // Test selector
            BuildTestSelector(contentVBox);

            // Start Recording CTA
            BuildRecordButton(contentVBox);

            // Offline badge
            BuildOfflineBadge(contentVBox);

            var bottomPad = new Control();
            bottomPad.CustomMinimumSize = new Vector2(0f, 24f);
            contentVBox.AddChild(bottomPad);
        }

        private void BuildHeader(VBoxContainer parent)
        {
            var header = MakePanel(390f, 108f, ColorChampionBlue, 0);
            header.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            parent.AddChild(header);

            var headerVBox = new VBoxContainer();
            headerVBox.AddThemeConstantOverride("separation", 0);
            header.AddChild(headerVBox);

            var statusPad = new Control();
            statusPad.CustomMinimumSize = new Vector2(390f, 32f);
            headerVBox.AddChild(statusPad);

            var rowMargin = new MarginContainer();
            rowMargin.AddThemeConstantOverride("margin_left",  16);
            rowMargin.AddThemeConstantOverride("margin_right", 16);
            headerVBox.AddChild(rowMargin);

            var titleVBox = new VBoxContainer();
            titleVBox.AddThemeConstantOverride("separation", 4);
            rowMargin.AddChild(titleVBox);

            var titleLbl = MakeLabel("Record Test", 22, ColorWhite);
            titleVBox.AddChild(titleLbl);

            var subLbl = MakeLabel("\U0001F4F9 AI-Powered Analysis", 12, new Color(1f, 1f, 1f, 0.70f));
            titleVBox.AddChild(subLbl);
        }

        private void BuildViewfinder(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   24);
            margin.AddThemeConstantOverride("margin_right",  24);
            margin.AddThemeConstantOverride("margin_top",    24);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            // Viewfinder: dark rounded rect simulating camera view
            var viewfinderPanel = new PanelContainer();
            viewfinderPanel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            viewfinderPanel.CustomMinimumSize = new Vector2(342f, 340f);
            var viewfinderStyle = new StyleBoxFlat
            {
                BgColor = new Color(0.08f, 0.08f, 0.12f),
                CornerRadiusTopLeft     = 20,
                CornerRadiusTopRight    = 20,
                CornerRadiusBottomLeft  = 20,
                CornerRadiusBottomRight = 20,
                BorderColor       = ColorBorder,
                BorderWidthTop    = 2,
                BorderWidthBottom = 2,
                BorderWidthLeft   = 2,
                BorderWidthRight  = 2,
            };
            viewfinderPanel.AddThemeStyleboxOverride("panel", viewfinderStyle);
            margin.AddChild(viewfinderPanel);

            // Inner content
            var viewCenter = new CenterContainer();
            viewCenter.SetAnchorsPreset(LayoutPreset.FullRect);
            viewfinderPanel.AddChild(viewCenter);

            var viewVBox = new VBoxContainer();
            viewVBox.AddThemeConstantOverride("separation", 16);
            viewCenter.AddChild(viewVBox);

            // Corner brackets (simulated with text)
            var bracketsLbl = MakeLabel("\u250C\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2510", 20, new Color(1f, 1f, 1f, 0.30f));
            bracketsLbl.HorizontalAlignment = HorizontalAlignment.Center;
            viewVBox.AddChild(bracketsLbl);

            // Camera icon
            var camLbl = MakeLabel("\U0001F4F7", 48, new Color(1f, 1f, 1f, 0.50f));
            camLbl.HorizontalAlignment = HorizontalAlignment.Center;
            viewVBox.AddChild(camLbl);

            // Position guide overlay text
            var guideLbl = MakeLabel("Position Guide", 16, new Color(1f, 1f, 1f, 0.70f));
            guideLbl.HorizontalAlignment = HorizontalAlignment.Center;
            viewVBox.AddChild(guideLbl);

            var guideDescLbl = MakeLabel("Align your body within the frame", 12, new Color(1f, 1f, 1f, 0.50f));
            guideDescLbl.HorizontalAlignment = HorizontalAlignment.Center;
            viewVBox.AddChild(guideDescLbl);

            // Bottom brackets
            var bottomBracketsLbl = MakeLabel("\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518", 20, new Color(1f, 1f, 1f, 0.30f));
            bottomBracketsLbl.HorizontalAlignment = HorizontalAlignment.Center;
            viewVBox.AddChild(bottomBracketsLbl);
        }

        private void BuildTestSelector(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   24);
            margin.AddThemeConstantOverride("margin_right",  24);
            margin.AddThemeConstantOverride("margin_top",    20);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var selectorVBox = new VBoxContainer();
            selectorVBox.AddThemeConstantOverride("separation", 8);
            margin.AddChild(selectorVBox);

            var selectorLabel = MakeLabel("Select Test", 14, ColorTextPrimary);
            selectorVBox.AddChild(selectorLabel);

            // Dropdown-style panel
            var dropdownPanel = MakePanel(0f, 48f, ColorWhite, 12, ColorBorder, 1);
            dropdownPanel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            selectorVBox.AddChild(dropdownPanel);

            var dropPad = new MarginContainer();
            dropPad.AddThemeConstantOverride("margin_left",  14);
            dropPad.AddThemeConstantOverride("margin_right", 14);
            dropdownPanel.AddChild(dropPad);

            var dropHBox = new HBoxContainer();
            dropHBox.AddThemeConstantOverride("separation", 8);
            dropPad.AddChild(dropHBox);

            var dropLabel = MakeLabel("\U0001F3C3 Shuttle Run", 14, ColorTextPrimary);
            dropLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            dropHBox.AddChild(dropLabel);

            var arrowLbl = MakeLabel("\u25BC", 12, ColorTextTertiary);
            dropHBox.AddChild(arrowLbl);
        }

        private void BuildRecordButton(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   24);
            margin.AddThemeConstantOverride("margin_right",  24);
            margin.AddThemeConstantOverride("margin_top",    20);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var recordBtn = MakeButton("Start Recording", ColorFieldGreen, ColorWhite, 16);
            recordBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            recordBtn.CustomMinimumSize   = new Vector2(0f, 52f);
            margin.AddChild(recordBtn);
        }

        private void BuildOfflineBadge(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   0);
            margin.AddThemeConstantOverride("margin_right",  0);
            margin.AddThemeConstantOverride("margin_top",    16);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var centerContainer = new CenterContainer();
            centerContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.AddChild(centerContainer);

            var badgePanel = MakePanel(0f, 32f, new Color(ColorFieldGreen.R, ColorFieldGreen.G, ColorFieldGreen.B, 0.12f), 16);
            var badgePad = new MarginContainer();
            badgePad.AddThemeConstantOverride("margin_left",  16);
            badgePad.AddThemeConstantOverride("margin_right", 16);
            badgePanel.AddChild(badgePad);

            var badgeLbl = MakeLabel("Offline Available \u2713", 12, ColorFieldGreen);
            badgePad.AddChild(badgeLbl);

            centerContainer.AddChild(badgePanel);
        }

        // -- HELPERS ------------------------------------------------------
        private static PanelContainer MakePanel(float w, float h, Color color, int radius = 0, Color? borderColor = null, int borderWidth = 0)
        {
            var style = new StyleBoxFlat
            {
                BgColor = color,
                CornerRadiusTopLeft     = radius,
                CornerRadiusTopRight    = radius,
                CornerRadiusBottomLeft  = radius,
                CornerRadiusBottomRight = radius,
            };
            if (borderColor.HasValue)
            {
                style.BorderColor       = borderColor.Value;
                style.BorderWidthLeft   = style.BorderWidthRight =
                style.BorderWidthTop    = style.BorderWidthBottom = borderWidth;
            }
            var panel = new PanelContainer();
            panel.AddThemeStyleboxOverride("panel", style);
            panel.CustomMinimumSize = new Vector2(w, h);
            return panel;
        }

        private static Label MakeLabel(string text, int size, Color color, bool bold = false)
        {
            var label = new Label { Text = text };
            label.AddThemeFontSizeOverride("font_size", size);
            label.AddThemeColorOverride("font_color", color);
            return label;
        }

        private static Button MakeButton(string text, Color bgColor, Color textColor, int fontSize)
        {
            var btn = new Button { Text = text };
            btn.AddThemeFontSizeOverride("font_size", fontSize);
            btn.AddThemeColorOverride("font_color", textColor);
            btn.AddThemeColorOverride("font_hover_color",   textColor);
            btn.AddThemeColorOverride("font_pressed_color", textColor);
            btn.AddThemeColorOverride("font_focus_color",   textColor);

            var normalStyle = new StyleBoxFlat
            {
                BgColor                 = bgColor,
                CornerRadiusTopLeft     = 12,
                CornerRadiusTopRight    = 12,
                CornerRadiusBottomLeft  = 12,
                CornerRadiusBottomRight = 12
            };
            btn.AddThemeStyleboxOverride("normal",  normalStyle);
            btn.AddThemeStyleboxOverride("hover",   normalStyle);
            btn.AddThemeStyleboxOverride("pressed", normalStyle);
            btn.AddThemeStyleboxOverride("focus",   normalStyle);
            return btn;
        }
    }
}
