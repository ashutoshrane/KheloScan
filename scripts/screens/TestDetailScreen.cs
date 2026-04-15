using Godot;

namespace KheloScan
{
    public partial class TestDetailScreen : Control
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
            SetAnchorsPreset(LayoutPreset.FullRect);
            ClipContents = true;

            string testId = AppState.Instance?.SelectedTestId ?? "height";
            var testData = FitnessTestData.GetById(testId);

            string testName  = testData != null ? (string)testData["name"]        : "Test";
            string testEmoji = testData != null ? (string)testData["emoji"]       : "";
            string testDesc  = testData != null ? (string)testData["description"] : "";
            string testInstr = testData != null ? (string)testData["instruction"] : "";
            string difficulty= testData != null ? (string)testData["difficulty"]  : "Basic";
            string benchM    = testData != null ? (string)testData["benchmark_male"]   : "";
            string benchF    = testData != null ? (string)testData["benchmark_female"] : "";
            Color  testColor = testData != null ? (Color)testData["color"]        : ColorFieldGreen;

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

            // Hero header
            BuildHero(rootVBox, testName, testEmoji, testColor);

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

            // Description
            BuildInfoSection(contentVBox, "About", testDesc);

            // Instruction
            BuildInfoSection(contentVBox, "Instructions", testInstr);

            // How to Record steps
            BuildHowToRecord(contentVBox);

            // Benchmark comparison
            BuildBenchmarks(contentVBox, benchM, benchF, testColor);

            // CTA button
            BuildCTA(contentVBox);

            var bottomPad = new Control();
            bottomPad.CustomMinimumSize = new Vector2(0f, 24f);
            contentVBox.AddChild(bottomPad);
        }

        private void BuildHero(VBoxContainer parent, string name, string emoji, Color color)
        {
            var hero = MakePanel(390f, 160f, color, 0);
            hero.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            parent.AddChild(hero);

            var heroVBox = new VBoxContainer();
            heroVBox.AddThemeConstantOverride("separation", 0);
            hero.AddChild(heroVBox);

            // Status bar pad
            var statusPad = new Control();
            statusPad.CustomMinimumSize = new Vector2(390f, 32f);
            heroVBox.AddChild(statusPad);

            // Back button row
            var backMargin = new MarginContainer();
            backMargin.AddThemeConstantOverride("margin_left",  12);
            backMargin.AddThemeConstantOverride("margin_right", 12);
            heroVBox.AddChild(backMargin);

            var backBtn = new Button { Text = "\u2190 Back" };
            backBtn.Flat = true;
            backBtn.AddThemeFontSizeOverride("font_size", 14);
            backBtn.AddThemeColorOverride("font_color", ColorWhite);
            backBtn.AddThemeColorOverride("font_hover_color",   ColorWhite);
            backBtn.AddThemeColorOverride("font_pressed_color", ColorWhite);
            var flatStyle = new StyleBoxFlat { BgColor = new Color(0f, 0f, 0f, 0f) };
            backBtn.AddThemeStyleboxOverride("normal",  flatStyle);
            backBtn.AddThemeStyleboxOverride("hover",   flatStyle);
            backBtn.AddThemeStyleboxOverride("pressed", flatStyle);
            backBtn.AddThemeStyleboxOverride("focus",   flatStyle);
            backBtn.Pressed += () =>
            {
                // Find Main and pop
                var main = GetTree().Root.GetNode<Main>("Main");
                if (main != null) main.PopScreen();
            };
            backMargin.AddChild(backBtn);

            // Title area
            var titleMargin = new MarginContainer();
            titleMargin.AddThemeConstantOverride("margin_left",  16);
            titleMargin.AddThemeConstantOverride("margin_right", 16);
            titleMargin.AddThemeConstantOverride("margin_top",   8);
            heroVBox.AddChild(titleMargin);

            var titleHBox = new HBoxContainer();
            titleHBox.AddThemeConstantOverride("separation", 12);
            titleMargin.AddChild(titleHBox);

            var emojiLbl = MakeLabel(emoji, 36, ColorWhite);
            titleHBox.AddChild(emojiLbl);

            var nameLbl = MakeLabel(name, 24, ColorWhite);
            nameLbl.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            titleHBox.AddChild(nameLbl);
        }

        private void BuildInfoSection(VBoxContainer parent, string title, string content)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    16);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var card = MakePanel(0f, 0f, ColorWhite, 12, ColorBorder, 1);
            card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.AddChild(card);

            var innerPad = new MarginContainer();
            innerPad.AddThemeConstantOverride("margin_left",   14);
            innerPad.AddThemeConstantOverride("margin_right",  14);
            innerPad.AddThemeConstantOverride("margin_top",    12);
            innerPad.AddThemeConstantOverride("margin_bottom", 12);
            card.AddChild(innerPad);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 6);
            innerPad.AddChild(vbox);

            var titleLbl = MakeLabel(title, 15, ColorTextPrimary);
            vbox.AddChild(titleLbl);

            var contentLbl = MakeLabel(content, 13, ColorTextSecondary);
            contentLbl.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(contentLbl);
        }

        private void BuildHowToRecord(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    16);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var card = MakePanel(0f, 0f, ColorWhite, 12, ColorBorder, 1);
            card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.AddChild(card);

            var innerPad = new MarginContainer();
            innerPad.AddThemeConstantOverride("margin_left",   14);
            innerPad.AddThemeConstantOverride("margin_right",  14);
            innerPad.AddThemeConstantOverride("margin_top",    12);
            innerPad.AddThemeConstantOverride("margin_bottom", 12);
            card.AddChild(innerPad);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 8);
            innerPad.AddChild(vbox);

            var titleLbl = MakeLabel("How to Record", 15, ColorTextPrimary);
            vbox.AddChild(titleLbl);

            string[] steps = {
                "1. Find a well-lit, flat area",
                "2. Position phone at waist height",
                "3. Ensure full body is visible in frame",
                "4. Press Start and perform the test",
                "5. AI will analyze automatically",
            };

            foreach (var step in steps)
            {
                var stepLbl = MakeLabel(step, 13, ColorTextSecondary);
                vbox.AddChild(stepLbl);
            }
        }

        private void BuildBenchmarks(VBoxContainer parent, string benchM, string benchF, Color color)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    16);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var card = MakePanel(0f, 0f, ColorWhite, 12, ColorBorder, 1);
            card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.AddChild(card);

            var innerPad = new MarginContainer();
            innerPad.AddThemeConstantOverride("margin_left",   14);
            innerPad.AddThemeConstantOverride("margin_right",  14);
            innerPad.AddThemeConstantOverride("margin_top",    12);
            innerPad.AddThemeConstantOverride("margin_bottom", 12);
            card.AddChild(innerPad);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 10);
            innerPad.AddChild(vbox);

            var titleLbl = MakeLabel("Benchmark Comparison", 15, ColorTextPrimary);
            vbox.AddChild(titleLbl);

            // Your score (placeholder)
            var yourRow = new HBoxContainer();
            yourRow.AddThemeConstantOverride("separation", 8);
            vbox.AddChild(yourRow);
            var yourLabel = MakeLabel("Your Score:", 13, ColorTextSecondary);
            yourLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            yourRow.AddChild(yourLabel);
            var yourValue = MakeLabel("--", 13, ColorTextTertiary);
            yourRow.AddChild(yourValue);

            // District avg
            BuildBenchmarkRow(vbox, "District Avg", "50th %ile", color, 0.5f);

            // State avg
            BuildBenchmarkRow(vbox, "State Avg", "65th %ile", color, 0.65f);

            // National avg
            BuildBenchmarkRow(vbox, "National Avg", "75th %ile", color, 0.75f);

            // Male benchmark
            var maleRow = new HBoxContainer();
            maleRow.AddThemeConstantOverride("separation", 8);
            vbox.AddChild(maleRow);
            var maleLabel = MakeLabel("Male Benchmark:", 13, ColorTextSecondary);
            maleLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            maleRow.AddChild(maleLabel);
            var maleValue = MakeLabel(benchM, 13, ColorChampionBlue);
            maleRow.AddChild(maleValue);

            // Female benchmark
            var femaleRow = new HBoxContainer();
            femaleRow.AddThemeConstantOverride("separation", 8);
            vbox.AddChild(femaleRow);
            var femaleLabel = MakeLabel("Female Benchmark:", 13, ColorTextSecondary);
            femaleLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            femaleRow.AddChild(femaleLabel);
            var femaleValue = MakeLabel(benchF, 13, ColorSprintOrange);
            femaleRow.AddChild(femaleValue);
        }

        private void BuildBenchmarkRow(VBoxContainer parent, string label, string value, Color barColor, float fillPercent)
        {
            var hbox = new HBoxContainer();
            hbox.AddThemeConstantOverride("separation", 8);
            parent.AddChild(hbox);

            var labelLbl = MakeLabel(label, 12, ColorTextSecondary);
            labelLbl.CustomMinimumSize = new Vector2(90f, 0f);
            hbox.AddChild(labelLbl);

            // Bar background
            var barBg = MakePanel(0f, 12f, new Color(ColorBorder.R, ColorBorder.G, ColorBorder.B, 0.50f), 6);
            barBg.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            hbox.AddChild(barBg);

            // Bar fill
            var barFill = new PanelContainer();
            barFill.SetAnchorsPreset(LayoutPreset.LeftWide);
            barFill.AnchorRight = fillPercent;
            barFill.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = barColor,
                CornerRadiusTopLeft     = 6,
                CornerRadiusTopRight    = 6,
                CornerRadiusBottomLeft  = 6,
                CornerRadiusBottomRight = 6,
            });
            barBg.AddChild(barFill);

            var valueLbl = MakeLabel(value, 12, ColorTextPrimary);
            hbox.AddChild(valueLbl);
        }

        private void BuildCTA(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    20);
            margin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(margin);

            var ctaBtn = MakeButton("Record This Test", ColorFieldGreen, ColorWhite, 16);
            ctaBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            ctaBtn.CustomMinimumSize   = new Vector2(0f, 52f);
            margin.AddChild(ctaBtn);
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
