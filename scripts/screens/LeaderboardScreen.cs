using Godot;

namespace KheloScan
{
    public partial class LeaderboardScreen : Control
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

        // Podium colors
        private static readonly Color ColorGold   = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorSilver = new Color(0.753f, 0.753f, 0.753f);
        private static readonly Color ColorBronze = new Color(0.804f, 0.498f, 0.196f);

        // Filter state
        private int _activeScope    = 0; // 0=District, 1=State, 2=National
        private int _activeAgeGroup = 0;

        private static readonly string[] ScopeNames    = { "District", "State", "National" };
        private static readonly string[] AgeGroupNames = { "All Ages", "Under 14", "Under 17", "Under 21" };

        // Leaderboard sample data
        private static readonly (string name, int score, float pct, string emoji)[] Leaders =
        {
            ("Vikram S.",   87, 98.2f, "\U0001F947"),
            ("Priya M.",    82, 95.1f, "\U0001F948"),
            ("Arjun K.",    79, 92.4f, "\U0001F949"),
            ("Sneha R.",    76, 88.7f, ""),
            ("Rahul D.",    74, 85.3f, ""),
            ("Ananya P.",   72, 82.1f, ""),
            ("Kiran B.",    69, 78.5f, ""),
            ("Dev T.",      67, 75.0f, ""),
            ("Meera J.",    65, 72.3f, ""),
            ("Aditya G.",   63, 70.1f, ""),
        };

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

            BuildHeader(rootVBox);

            var scroll = new ScrollContainer();
            scroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            scroll.SizeFlagsVertical   = SizeFlags.ExpandFill;
            scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            rootVBox.AddChild(scroll);

            var contentVBox = new VBoxContainer();
            contentVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            contentVBox.AddThemeConstantOverride("separation", 0);
            scroll.AddChild(contentVBox);

            // Filter chips
            BuildFilterChips(contentVBox);

            // Podium display
            BuildPodium(contentVBox);

            // Ranking list
            BuildRankingList(contentVBox);

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

            var titleLbl = MakeLabel("Leaderboard", 22, ColorWhite);
            titleVBox.AddChild(titleLbl);

            var subLbl = MakeLabel("\U0001F3C6 Compete & Shine", 12, new Color(1f, 1f, 1f, 0.70f));
            titleVBox.AddChild(subLbl);
        }

        private void BuildFilterChips(VBoxContainer parent)
        {
            // Scope row (District / State / National)
            var scopeMargin = new MarginContainer();
            scopeMargin.AddThemeConstantOverride("margin_left",   16);
            scopeMargin.AddThemeConstantOverride("margin_right",  16);
            scopeMargin.AddThemeConstantOverride("margin_top",    12);
            scopeMargin.AddThemeConstantOverride("margin_bottom", 8);
            parent.AddChild(scopeMargin);

            var scopeHBox = new HBoxContainer();
            scopeHBox.AddThemeConstantOverride("separation", 8);
            scopeMargin.AddChild(scopeHBox);

            for (int i = 0; i < ScopeNames.Length; i++)
            {
                bool isActive = (i == _activeScope);
                Color chipBg   = isActive ? ColorChampionBlue : ColorWhite;
                Color textClr  = isActive ? ColorWhite : ColorTextSecondary;

                var chip = MakePanel(0f, 32f, chipBg, 16, isActive ? null : ColorBorder, isActive ? 0 : 1);
                var chipPad = new MarginContainer();
                chipPad.AddThemeConstantOverride("margin_left",  14);
                chipPad.AddThemeConstantOverride("margin_right", 14);
                chip.AddChild(chipPad);
                var chipLbl = MakeLabel(ScopeNames[i], 12, textClr);
                chipPad.AddChild(chipLbl);
                scopeHBox.AddChild(chip);
            }

            // Age group row
            var ageMargin = new MarginContainer();
            ageMargin.AddThemeConstantOverride("margin_left",   16);
            ageMargin.AddThemeConstantOverride("margin_right",  16);
            ageMargin.AddThemeConstantOverride("margin_top",    0);
            ageMargin.AddThemeConstantOverride("margin_bottom", 8);
            parent.AddChild(ageMargin);

            var ageHBox = new HBoxContainer();
            ageHBox.AddThemeConstantOverride("separation", 8);
            ageMargin.AddChild(ageHBox);

            for (int i = 0; i < AgeGroupNames.Length; i++)
            {
                bool isActive = (i == _activeAgeGroup);
                Color chipBg   = isActive ? ColorFieldGreen : ColorWhite;
                Color textClr  = isActive ? ColorWhite : ColorTextSecondary;

                var chip = MakePanel(0f, 28f, chipBg, 14, isActive ? null : ColorBorder, isActive ? 0 : 1);
                var chipPad = new MarginContainer();
                chipPad.AddThemeConstantOverride("margin_left",  10);
                chipPad.AddThemeConstantOverride("margin_right", 10);
                chip.AddChild(chipPad);
                var chipLbl = MakeLabel(AgeGroupNames[i], 11, textClr);
                chipPad.AddChild(chipLbl);
                ageHBox.AddChild(chip);
            }
        }

        private void BuildPodium(VBoxContainer parent)
        {
            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left",   16);
            margin.AddThemeConstantOverride("margin_right",  16);
            margin.AddThemeConstantOverride("margin_top",    8);
            margin.AddThemeConstantOverride("margin_bottom", 16);
            parent.AddChild(margin);

            var hbox = new HBoxContainer();
            hbox.AddThemeConstantOverride("separation", 10);
            hbox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.AddChild(hbox);

            // Silver (2nd place) - slightly shorter
            hbox.AddChild(BuildPodiumCard(Leaders[1].name, Leaders[1].score, 2, ColorSilver, 120f));
            // Gold (1st place) - tallest
            hbox.AddChild(BuildPodiumCard(Leaders[0].name, Leaders[0].score, 1, ColorGold, 140f));
            // Bronze (3rd place) - shortest
            hbox.AddChild(BuildPodiumCard(Leaders[2].name, Leaders[2].score, 3, ColorBronze, 110f));
        }

        private PanelContainer BuildPodiumCard(string name, int score, int rank, Color color, float height)
        {
            var card = MakePanel(0f, height, color, 16);
            card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            card.ClipContents = true;

            var center = new CenterContainer();
            center.SetAnchorsPreset(LayoutPreset.FullRect);
            card.AddChild(center);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 4);
            center.AddChild(vbox);

            // Medal emoji
            string medalEmoji = rank == 1 ? "\U0001F947" : (rank == 2 ? "\U0001F948" : "\U0001F949");
            var medalLbl = MakeLabel(medalEmoji, 28, ColorWhite);
            medalLbl.HorizontalAlignment = HorizontalAlignment.Center;
            vbox.AddChild(medalLbl);

            // Rank number
            var rankLbl = MakeLabel($"#{rank}", 18, ColorWhite);
            rankLbl.HorizontalAlignment = HorizontalAlignment.Center;
            vbox.AddChild(rankLbl);

            // Name
            var nameLbl = MakeLabel(name, 12, ColorWhite);
            nameLbl.HorizontalAlignment = HorizontalAlignment.Center;
            vbox.AddChild(nameLbl);

            // Score
            var scoreLbl = MakeLabel($"{score} pts", 11, new Color(1f, 1f, 1f, 0.80f));
            scoreLbl.HorizontalAlignment = HorizontalAlignment.Center;
            vbox.AddChild(scoreLbl);

            return card;
        }

        private void BuildRankingList(VBoxContainer parent)
        {
            var headerMargin = new MarginContainer();
            headerMargin.AddThemeConstantOverride("margin_left",   16);
            headerMargin.AddThemeConstantOverride("margin_right",  16);
            headerMargin.AddThemeConstantOverride("margin_top",    0);
            headerMargin.AddThemeConstantOverride("margin_bottom", 8);
            parent.AddChild(headerMargin);

            var headerLbl = MakeLabel("Full Rankings", 16, ColorTextPrimary);
            headerMargin.AddChild(headerLbl);

            for (int i = 3; i < Leaders.Length; i++)
            {
                var entry = Leaders[i];
                int rank  = i + 1;

                var cardMargin = new MarginContainer();
                cardMargin.AddThemeConstantOverride("margin_left",   16);
                cardMargin.AddThemeConstantOverride("margin_right",  16);
                cardMargin.AddThemeConstantOverride("margin_top",    0);
                cardMargin.AddThemeConstantOverride("margin_bottom", 6);
                cardMargin.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                parent.AddChild(cardMargin);

                var card = MakePanel(358f, 56f, ColorWhite, 10, ColorBorder, 1);
                card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                cardMargin.AddChild(card);

                var innerPad = new MarginContainer();
                innerPad.AddThemeConstantOverride("margin_left",   12);
                innerPad.AddThemeConstantOverride("margin_right",  12);
                innerPad.AddThemeConstantOverride("margin_top",    8);
                innerPad.AddThemeConstantOverride("margin_bottom", 8);
                card.AddChild(innerPad);

                var hbox = new HBoxContainer();
                hbox.AddThemeConstantOverride("separation", 12);
                innerPad.AddChild(hbox);

                // Rank number
                var rankLbl = MakeLabel($"#{rank}", 16, ColorChampionBlue);
                rankLbl.CustomMinimumSize = new Vector2(32f, 0f);
                hbox.AddChild(rankLbl);

                // Name
                var nameLbl = MakeLabel(entry.name, 14, ColorTextPrimary);
                nameLbl.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                hbox.AddChild(nameLbl);

                // Score
                var scoreLbl = MakeLabel($"{entry.score}", 14, ColorChampionBlue);
                hbox.AddChild(scoreLbl);

                // Percentile bar
                var barBg = MakePanel(60f, 8f, ColorBorder, 4);
                barBg.SizeFlagsVertical = SizeFlags.ShrinkCenter;
                hbox.AddChild(barBg);

                float barWidth = entry.pct / 100f * 60f;
                var barFill = MakePanel(barWidth, 8f, ColorFieldGreen, 4);
                barBg.AddChild(barFill);
            }
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
    }
}
