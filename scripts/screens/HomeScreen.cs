using Godot;

namespace KheloScan
{
    public partial class HomeScreen : Control
    {
        // -- Color palette ------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorFieldGreen   = new Color(0.086f, 0.639f, 0.290f);
        private static readonly Color ColorGoldMedal    = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorSprintOrange = new Color(0.976f, 0.451f, 0.086f);
        private static readonly Color ColorTrackRed     = new Color(0.863f, 0.149f, 0.149f);
        private static readonly Color ColorBg           = new Color(0.945f, 0.961f, 0.976f);
        private static readonly Color ColorSurface      = new Color(0.973f, 0.980f, 0.988f);
        private static readonly Color ColorTextPrimary  = new Color(0.059f, 0.090f, 0.165f);
        private static readonly Color ColorTextSecondary= new Color(0.278f, 0.333f, 0.412f);
        private static readonly Color ColorTextTertiary = new Color(0.580f, 0.639f, 0.722f);
        private static readonly Color ColorBorder       = new Color(0.886f, 0.910f, 0.941f);
        private static readonly Color ColorWhite        = new Color(1f, 1f, 1f);

        public override void _Ready()
        {
            SetAnchorsPreset(Control.LayoutPreset.FullRect);
            ClipContents = true;

            // -- Root background ------------------------------------------
            var bgRect = new ColorRect();
            bgRect.Color = ColorBg;
            bgRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            bgRect.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            bgRect.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            AddChild(bgRect);

            // -- Root VBox: header + scroll -------------------------------
            var rootVBox = new VBoxContainer();
            rootVBox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            rootVBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            rootVBox.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            rootVBox.AddThemeConstantOverride("separation", 0);
            AddChild(rootVBox);

            // HEADER
            BuildHeader(rootVBox);

            // SCROLL CONTAINER
            var scroll = new ScrollContainer();
            scroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            scroll.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            rootVBox.AddChild(scroll);

            var contentVBox = new VBoxContainer();
            contentVBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            contentVBox.AddThemeConstantOverride("separation", 0);
            scroll.AddChild(contentVBox);

            // -- Athlete Stats Banner -------------------------------------
            BuildStatsBanner(contentVBox);

            // -- Quick Actions --------------------------------------------
            BuildQuickActions(contentVBox);

            // -- Assessment Progress section header -----------------------
            BuildSectionHeader(contentVBox, "Assessment Progress", 20);

            // -- 6 Test progress cards ------------------------------------
            BuildTestProgressCards(contentVBox);

            // -- Sport Recommendations ------------------------------------
            BuildSectionHeader(contentVBox, "Sport Recommendations", 20);
            BuildSportRecommendations(contentVBox);

            // -- Bottom padding -------------------------------------------
            var bottomPad = new Control();
            bottomPad.CustomMinimumSize = new Vector2(0f, 24f);
            contentVBox.AddChild(bottomPad);
        }

        // -- HEADER -------------------------------------------------------
        private void BuildHeader(VBoxContainer parent)
        {
            var header = MakePanel(390f, 108f, ColorChampionBlue, 0);
            header.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            parent.AddChild(header);

            var headerVBox = new VBoxContainer();
            headerVBox.AddThemeConstantOverride("separation", 0);
            header.AddChild(headerVBox);

            // Status bar spacer (32px)
            var statusPad = new Control();
            statusPad.CustomMinimumSize = new Vector2(390f, 32f);
            headerVBox.AddChild(statusPad);

            // Content row
            var rowMargin = new MarginContainer();
            rowMargin.AddThemeConstantOverride("margin_left",  16);
            rowMargin.AddThemeConstantOverride("margin_right", 16);
            rowMargin.AddThemeConstantOverride("margin_top",   0);
            rowMargin.AddThemeConstantOverride("margin_bottom", 0);
            headerVBox.AddChild(rowMargin);

            var row = new HBoxContainer();
            row.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            row.AddThemeConstantOverride("separation", 0);
            rowMargin.AddChild(row);

            // Left VBox: greeting + title
            var leftVBox = new VBoxContainer();
            leftVBox.AddThemeConstantOverride("separation", 2);
            row.AddChild(leftVBox);

            var greetColor = new Color(1f, 1f, 1f, 0.70f);
            var greet = MakeLabel("Namaste, Athlete \U0001F3C5", 11, greetColor);
            leftVBox.AddChild(greet);

            var titleLbl = MakeLabel("My Journey", 20, ColorWhite);
            leftVBox.AddChild(titleLbl);

            // Spacer
            var spacer = new Control();
            spacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            row.AddChild(spacer);

            // Right: bell + avatar
            var rightHBox = new HBoxContainer();
            rightHBox.AddThemeConstantOverride("separation", 8);
            row.AddChild(rightHBox);

            var bellPanel = MakeCirclePanel(32f, new Color(1f, 1f, 1f, 0.20f), 16);
            var bellCenter = new CenterContainer();
            bellCenter.CustomMinimumSize = new Vector2(32f, 32f);
            var bellLbl = MakeLabel("\U0001F514", 14, ColorWhite);
            bellCenter.AddChild(bellLbl);
            bellPanel.AddChild(bellCenter);
            rightHBox.AddChild(bellPanel);

            var avatarPanel = MakeCirclePanel(32f, ColorGoldMedal, 16);
            var avatarCenter = new CenterContainer();
            avatarCenter.CustomMinimumSize = new Vector2(32f, 32f);
            var avatarLbl = MakeLabel("A", 14, ColorWhite);
            avatarCenter.AddChild(avatarLbl);
            avatarPanel.AddChild(avatarCenter);
            rightHBox.AddChild(avatarPanel);
        }

        // -- STATS BANNER -------------------------------------------------
        private void BuildStatsBanner(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    16);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var panel = MakePanel(358f, 0f, ColorChampionBlue, 16);
            panel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            margin.AddChild(panel);

            var innerMargin = new MarginContainer();
            innerMargin.AddThemeConstantOverride("margin_left",   16);
            innerMargin.AddThemeConstantOverride("margin_right",  16);
            innerMargin.AddThemeConstantOverride("margin_top",    16);
            innerMargin.AddThemeConstantOverride("margin_bottom", 16);
            panel.AddChild(innerMargin);

            var statsHBox = new HBoxContainer();
            statsHBox.AddThemeConstantOverride("separation", 0);
            statsHBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            innerMargin.AddChild(statsHBox);

            int completed  = AppState.Instance?.CompletedTests.Count ?? 0;
            int score      = AppState.Instance?.AthleteScore ?? 0;
            float pct      = AppState.Instance?.Percentile ?? 0f;

            AddStatColumn(statsHBox, $"{completed}/6", "Tests Done");
            AddStatDivider(statsHBox);
            AddStatColumn(statsHBox, score.ToString(), "Score");
            AddStatDivider(statsHBox);
            AddStatColumn(statsHBox, $"{pct:F0}%", "Percentile");
        }

        private void AddStatColumn(HBoxContainer parent, string value, string label)
        {
            var margin = new MarginContainer();
            margin.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            margin.AddThemeConstantOverride("margin_top",    8);
            margin.AddThemeConstantOverride("margin_bottom", 8);
            parent.AddChild(margin);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 4);
            margin.AddChild(vbox);

            var valLabel = MakeLabel(value, 22, ColorGoldMedal);
            valLabel.HorizontalAlignment = HorizontalAlignment.Center;
            vbox.AddChild(valLabel);

            var nameLabel = MakeLabel(label, 11, new Color(1f, 1f, 1f, 0.70f));
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            vbox.AddChild(nameLabel);
        }

        private void AddStatDivider(HBoxContainer parent)
        {
            var sep = new Panel();
            sep.CustomMinimumSize = new Vector2(1, 0);
            sep.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            sep.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = new Color(1f, 1f, 1f, 0.20f),
            });
            parent.AddChild(sep);
        }

        // -- QUICK ACTIONS ------------------------------------------------
        private void BuildQuickActions(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    12);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var hbox = new HBoxContainer();
            hbox.AddThemeConstantOverride("separation", 12);
            hbox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            margin.AddChild(hbox);

            // Start Assessment button
            var startBtn = MakeButton("Start Assessment", ColorFieldGreen, ColorWhite, 14);
            startBtn.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            startBtn.CustomMinimumSize   = new Vector2(0f, 48f);
            startBtn.Pressed += () =>
            {
                if (AppState.Instance != null) AppState.Instance.SelectTab(1);
            };
            hbox.AddChild(startBtn);

            // View Results button
            var resultsBtn = MakeButton("View Results", ColorWhite, ColorChampionBlue, 14);
            resultsBtn.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            resultsBtn.CustomMinimumSize   = new Vector2(0f, 48f);
            resultsBtn.Pressed += () =>
            {
                if (AppState.Instance != null) AppState.Instance.SelectTab(4);
            };

            // Add border to results button
            var resultsStyle = new StyleBoxFlat
            {
                BgColor = ColorWhite,
                CornerRadiusTopLeft     = 12,
                CornerRadiusTopRight    = 12,
                CornerRadiusBottomLeft  = 12,
                CornerRadiusBottomRight = 12,
                BorderColor       = ColorBorder,
                BorderWidthTop    = 1,
                BorderWidthBottom = 1,
                BorderWidthLeft   = 1,
                BorderWidthRight  = 1,
            };
            resultsBtn.AddThemeStyleboxOverride("normal", resultsStyle);
            hbox.AddChild(resultsBtn);
        }

        // -- SECTION HEADER -----------------------------------------------
        private void BuildSectionHeader(VBoxContainer parent, string title, int topMargin)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    topMargin);
            margin.AddThemeConstantOverride("margin_bottom", 8);
            parent.AddChild(margin);

            var row = new HBoxContainer();
            row.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            margin.AddChild(row);

            var titleLbl = MakeLabel(title, 16, ColorTextPrimary);
            row.AddChild(titleLbl);

            var spacer = new Control();
            spacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            row.AddChild(spacer);
        }

        // -- TEST PROGRESS CARDS ------------------------------------------
        private void BuildTestProgressCards(VBoxContainer parent)
        {
            var tests = FitnessTestData.GetAll();
            var completed = AppState.Instance?.CompletedTests ?? new Godot.Collections.Array<string>();

            foreach (var test in tests)
            {
                string id          = (string)test["id"];
                string name        = (string)test["name"];
                string emoji       = (string)test["emoji"];
                string difficulty  = (string)test["difficulty"];
                bool isDone        = completed.Contains(id);

                var cardMargin = new MarginContainer();
                cardMargin.AddThemeConstantOverride("margin_left",   16);
                cardMargin.AddThemeConstantOverride("margin_right",  16);
                cardMargin.AddThemeConstantOverride("margin_top",    0);
                cardMargin.AddThemeConstantOverride("margin_bottom", 8);
                cardMargin.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                parent.AddChild(cardMargin);

                var card = MakePanel(358f, 56f, ColorWhite, 12);
                card.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                cardMargin.AddChild(card);

                var innerPad = new MarginContainer();
                innerPad.AddThemeConstantOverride("margin_left",   12);
                innerPad.AddThemeConstantOverride("margin_right",  12);
                innerPad.AddThemeConstantOverride("margin_top",    8);
                innerPad.AddThemeConstantOverride("margin_bottom", 8);
                card.AddChild(innerPad);

                var hbox = new HBoxContainer();
                hbox.AddThemeConstantOverride("separation", 10);
                innerPad.AddChild(hbox);

                // Emoji
                var emojiLbl = MakeLabel(emoji, 22, ColorTextPrimary);
                hbox.AddChild(emojiLbl);

                // Name
                var nameLbl = MakeLabel(name, 14, ColorTextPrimary);
                nameLbl.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                hbox.AddChild(nameLbl);

                // Status
                var statusText = isDone ? "\u2705" : "\u23F3";
                var statusLbl  = MakeLabel(statusText, 16, isDone ? ColorFieldGreen : ColorTextTertiary);
                hbox.AddChild(statusLbl);
            }
        }

        // -- SPORT RECOMMENDATIONS ----------------------------------------
        private void BuildSportRecommendations(VBoxContainer parent)
        {
            var sports = FitnessTestData.GetSportRecommendations();

            var hScroll = new ScrollContainer();
            hScroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            hScroll.HorizontalScrollMode = ScrollContainer.ScrollMode.ShowNever;
            hScroll.VerticalScrollMode   = ScrollContainer.ScrollMode.Disabled;
            hScroll.CustomMinimumSize    = new Vector2(390f, 110f);

            var leftPad = new MarginContainer();
            leftPad.AddThemeConstantOverride("margin_left",   16);
            leftPad.AddThemeConstantOverride("margin_right",  16);
            leftPad.AddThemeConstantOverride("margin_top",    0);
            leftPad.AddThemeConstantOverride("margin_bottom", 0);
            hScroll.AddChild(leftPad);

            var cardsHBox = new HBoxContainer();
            cardsHBox.AddThemeConstantOverride("separation", 12);
            leftPad.AddChild(cardsHBox);

            Color[] sportColors = { ColorChampionBlue, ColorFieldGreen, ColorSprintOrange, ColorTrackRed, ColorGoldMedal, ColorFieldGreen };
            int colorIdx = 0;

            foreach (var sport in sports)
            {
                string sName  = (string)sport["sport"];
                string sEmoji = (string)sport["emoji"];
                string sDesc  = (string)sport["desc"];
                Color sColor  = sportColors[colorIdx % sportColors.Length];
                colorIdx++;

                var sportCard = MakePanel(140f, 100f, sColor, 12);
                sportCard.ClipContents = true;

                var pad = new MarginContainer();
                pad.AddThemeConstantOverride("margin_left",   10);
                pad.AddThemeConstantOverride("margin_right",  10);
                pad.AddThemeConstantOverride("margin_top",    10);
                pad.AddThemeConstantOverride("margin_bottom", 10);
                sportCard.AddChild(pad);

                var vbox = new VBoxContainer();
                vbox.AddThemeConstantOverride("separation", 4);
                pad.AddChild(vbox);

                var emoLbl = MakeLabel(sEmoji, 24, ColorWhite);
                vbox.AddChild(emoLbl);

                var nLbl = MakeLabel(sName, 12, ColorWhite);
                nLbl.AutowrapMode = TextServer.AutowrapMode.Word;
                vbox.AddChild(nLbl);

                var dLbl = MakeLabel(sDesc, 9, new Color(1f, 1f, 1f, 0.70f));
                dLbl.AutowrapMode = TextServer.AutowrapMode.Word;
                vbox.AddChild(dLbl);

                cardsHBox.AddChild(sportCard);
            }

            parent.AddChild(hScroll);
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

        private static PanelContainer MakeCirclePanel(float size, Color color, int radius)
        {
            var panel = MakePanel(size, size, color, radius);
            panel.CustomMinimumSize = new Vector2(size, size);
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
