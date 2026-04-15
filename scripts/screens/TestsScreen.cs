using Godot;

namespace KheloScan
{
    public partial class TestsScreen : Control
    {
        // -- Color palette ------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorFieldGreen   = new Color(0.086f, 0.639f, 0.290f);
        private static readonly Color ColorGoldMedal    = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorSprintOrange = new Color(0.976f, 0.451f, 0.086f);
        private static readonly Color ColorTrackRed     = new Color(0.863f, 0.149f, 0.149f);
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

            // Scroll
            var scroll = new ScrollContainer();
            scroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            scroll.SizeFlagsVertical   = SizeFlags.ExpandFill;
            scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            rootVBox.AddChild(scroll);

            var contentVBox = new VBoxContainer();
            contentVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            contentVBox.AddThemeConstantOverride("separation", 0);
            scroll.AddChild(contentVBox);

            // Subtitle
            var subtitleMargin = new MarginContainer();
            subtitleMargin.AddThemeConstantOverride("margin_left",   16);
            subtitleMargin.AddThemeConstantOverride("margin_right",  16);
            subtitleMargin.AddThemeConstantOverride("margin_top",    16);
            subtitleMargin.AddThemeConstantOverride("margin_bottom", 12);
            contentVBox.AddChild(subtitleMargin);

            var subtitleLbl = MakeLabel("Complete all 6 SAI-approved fitness tests", 13, ColorTextSecondary);
            subtitleLbl.AutowrapMode = TextServer.AutowrapMode.Word;
            subtitleMargin.AddChild(subtitleLbl);

            // Test cards
            BuildTestCards(contentVBox);

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

            var titleLbl = MakeLabel("Assessment Center", 22, ColorWhite);
            titleVBox.AddChild(titleLbl);

            var subLbl = MakeLabel("\U0001F4CB 6 Tests Available", 12, new Color(1f, 1f, 1f, 0.70f));
            titleVBox.AddChild(subLbl);
        }

        private void BuildTestCards(VBoxContainer parent)
        {
            var tests     = FitnessTestData.GetAll();
            var completed = AppState.Instance?.CompletedTests ?? new Godot.Collections.Array<string>();

            foreach (var test in tests)
            {
                string id          = (string)test["id"];
                string name        = (string)test["name"];
                string desc        = (string)test["description"];
                string emoji       = (string)test["emoji"];
                string difficulty  = (string)test["difficulty"];
                Color  testColor   = (Color)test["color"];
                bool   isDone      = completed.Contains(id);

                var cardMargin = new MarginContainer();
                cardMargin.AddThemeConstantOverride("margin_left",   16);
                cardMargin.AddThemeConstantOverride("margin_right",  16);
                cardMargin.AddThemeConstantOverride("margin_top",    0);
                cardMargin.AddThemeConstantOverride("margin_bottom", 10);
                cardMargin.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                parent.AddChild(cardMargin);

                // Card with left color border
                var card = new PanelContainer();
                card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                var cardStyle = new StyleBoxFlat
                {
                    BgColor = ColorWhite,
                    CornerRadiusTopLeft     = 12,
                    CornerRadiusTopRight    = 12,
                    CornerRadiusBottomLeft  = 12,
                    CornerRadiusBottomRight = 12,
                    BorderColor       = testColor,
                    BorderWidthLeft   = 4,
                    BorderWidthTop    = 0,
                    BorderWidthBottom = 0,
                    BorderWidthRight  = 0,
                };
                card.AddThemeStyleboxOverride("panel", cardStyle);
                cardMargin.AddChild(card);

                var innerPad = new MarginContainer();
                innerPad.AddThemeConstantOverride("margin_left",   14);
                innerPad.AddThemeConstantOverride("margin_right",  12);
                innerPad.AddThemeConstantOverride("margin_top",    12);
                innerPad.AddThemeConstantOverride("margin_bottom", 12);
                card.AddChild(innerPad);

                var hbox = new HBoxContainer();
                hbox.AddThemeConstantOverride("separation", 12);
                innerPad.AddChild(hbox);

                // Emoji circle
                var emojiPanel = MakePanel(48f, 48f, new Color(testColor.R, testColor.G, testColor.B, 0.12f), 24);
                emojiPanel.CustomMinimumSize = new Vector2(48f, 48f);
                var emojiCenter = new CenterContainer();
                emojiCenter.CustomMinimumSize = new Vector2(48f, 48f);
                var emojiLbl = MakeLabel(emoji, 22, testColor);
                emojiCenter.AddChild(emojiLbl);
                emojiPanel.AddChild(emojiCenter);
                hbox.AddChild(emojiPanel);

                // Info VBox
                var infoVBox = new VBoxContainer();
                infoVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                infoVBox.AddThemeConstantOverride("separation", 2);
                hbox.AddChild(infoVBox);

                var nameLbl = MakeLabel(name, 15, ColorTextPrimary);
                infoVBox.AddChild(nameLbl);

                var descLbl = MakeLabel(desc, 12, ColorTextSecondary);
                infoVBox.AddChild(descLbl);

                // Difficulty badge
                Color badgeBg;
                if (difficulty == "Basic")
                    badgeBg = new Color(ColorFieldGreen.R, ColorFieldGreen.G, ColorFieldGreen.B, 0.15f);
                else if (difficulty == "Intermediate")
                    badgeBg = new Color(ColorGoldMedal.R, ColorGoldMedal.G, ColorGoldMedal.B, 0.15f);
                else
                    badgeBg = new Color(ColorTrackRed.R, ColorTrackRed.G, ColorTrackRed.B, 0.15f);

                Color badgeText;
                if (difficulty == "Basic")
                    badgeText = ColorFieldGreen;
                else if (difficulty == "Intermediate")
                    badgeText = ColorGoldMedal;
                else
                    badgeText = ColorTrackRed;

                var badgePanel = MakePanel(0f, 22f, badgeBg, 11);
                badgePanel.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
                var badgePad = new MarginContainer();
                badgePad.AddThemeConstantOverride("margin_left",  8);
                badgePad.AddThemeConstantOverride("margin_right", 8);
                badgePanel.AddChild(badgePad);
                var badgeLbl = MakeLabel(difficulty, 10, badgeText);
                badgePad.AddChild(badgeLbl);
                infoVBox.AddChild(badgePanel);

                // Status indicator
                var statusText = isDone ? "\u2705" : "\u25B6";
                var statusColor = isDone ? ColorFieldGreen : ColorTextTertiary;
                var statusLbl = MakeLabel(statusText, 20, statusColor);
                hbox.AddChild(statusLbl);

                // Tap overlay
                string capturedId = id;
                var tapBtn = new Button();
                tapBtn.Flat = true;
                tapBtn.SetAnchorsPreset(LayoutPreset.FullRect);
                tapBtn.Text = "";
                var invisStyle = new StyleBoxFlat { BgColor = new Color(0f, 0f, 0f, 0f) };
                tapBtn.AddThemeStyleboxOverride("normal",  invisStyle);
                tapBtn.AddThemeStyleboxOverride("hover",   invisStyle);
                tapBtn.AddThemeStyleboxOverride("pressed", invisStyle);
                tapBtn.AddThemeStyleboxOverride("focus",   invisStyle);
                card.AddChild(tapBtn);
                tapBtn.Pressed += () =>
                {
                    if (AppState.Instance != null)
                        AppState.Instance.SelectTest(capturedId);
                };
            }
        }

        // -- HELPERS ------------------------------------------------------
        private static PanelContainer MakePanel(float w, float h, Color color, int radius = 0)
        {
            var panel = new PanelContainer();
            panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = color,
                CornerRadiusTopLeft     = radius,
                CornerRadiusTopRight    = radius,
                CornerRadiusBottomLeft  = radius,
                CornerRadiusBottomRight = radius,
            });
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
    }
}
