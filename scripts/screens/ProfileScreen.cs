using Godot;

namespace KheloScan
{
    public partial class ProfileScreen : Control
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
            BuildUI();
        }

        private void BuildUI()
        {
            var bg = new Panel();
            bg.SetAnchorsPreset(LayoutPreset.FullRect);
            bg.AddThemeStyleboxOverride("panel", new StyleBoxFlat { BgColor = ColorBg });
            AddChild(bg);

            var scroll = new ScrollContainer();
            scroll.SetAnchorsPreset(LayoutPreset.FullRect);
            scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            AddChild(scroll);

            var vbox = new VBoxContainer();
            vbox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            vbox.AddThemeConstantOverride("separation", 0);
            scroll.AddChild(vbox);

            // Avatar + name area
            var avatarMargin = new MarginContainer();
            avatarMargin.AddThemeConstantOverride("margin_top",    48);
            avatarMargin.AddThemeConstantOverride("margin_bottom", 8);
            vbox.AddChild(avatarMargin);

            var avatarCenter = new CenterContainer();
            avatarMargin.AddChild(avatarCenter);

            var avatarVBox = new VBoxContainer();
            avatarVBox.AddThemeConstantOverride("separation", 8);
            avatarCenter.AddChild(avatarVBox);

            // Avatar circle
            var avatarPanel = new Panel();
            avatarPanel.CustomMinimumSize = new Vector2(80, 80);
            avatarPanel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = ColorChampionBlue,
                CornerRadiusTopLeft     = 40,
                CornerRadiusTopRight    = 40,
                CornerRadiusBottomLeft  = 40,
                CornerRadiusBottomRight = 40,
            });
            avatarVBox.AddChild(avatarPanel);

            var avatarCenter2 = new CenterContainer();
            avatarCenter2.SetAnchorsPreset(LayoutPreset.FullRect);
            avatarPanel.AddChild(avatarCenter2);

            var avatarIcon = new Label { Text = "\U0001F3C3" };
            avatarIcon.AddThemeFontSizeOverride("font_size", 36);
            avatarCenter2.AddChild(avatarIcon);

            var nameLabel = new Label { Text = "Aspiring Athlete" };
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            nameLabel.AddThemeFontSizeOverride("font_size", 20);
            nameLabel.AddThemeColorOverride("font_color", ColorTextPrimary);
            avatarVBox.AddChild(nameLabel);

            // Stats row
            var statsMargin = new MarginContainer();
            statsMargin.AddThemeConstantOverride("margin_left",   20);
            statsMargin.AddThemeConstantOverride("margin_right",  20);
            statsMargin.AddThemeConstantOverride("margin_top",    20);
            statsMargin.AddThemeConstantOverride("margin_bottom", 8);
            vbox.AddChild(statsMargin);

            var statsPanel = new Panel();
            statsPanel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = ColorSurface,
                CornerRadiusTopLeft     = 16,
                CornerRadiusTopRight    = 16,
                CornerRadiusBottomLeft  = 16,
                CornerRadiusBottomRight = 16,
            });
            statsMargin.AddChild(statsPanel);

            var statsHBox = new HBoxContainer();
            statsHBox.SetAnchorsPreset(LayoutPreset.FullRect);
            statsHBox.AddThemeConstantOverride("separation", 0);
            statsPanel.AddChild(statsHBox);

            int score      = AppState.Instance?.AthleteScore ?? 0;
            int tests      = AppState.Instance?.CompletedTests.Count ?? 0;
            float pct      = AppState.Instance?.Percentile ?? 0f;
            int badges     = AppState.Instance?.Badges.Count ?? 0;

            AddStatColumn(statsHBox, score.ToString(), "Score");
            AddStatDivider(statsHBox);
            AddStatColumn(statsHBox, $"{tests}/6", "Tests");
            AddStatDivider(statsHBox);
            AddStatColumn(statsHBox, $"{pct:F0}%", "Percentile");
            AddStatDivider(statsHBox);
            AddStatColumn(statsHBox, badges.ToString(), "Badges");

            // Radar chart placeholder
            BuildRadarChart(vbox);

            // Badges section
            var badgesHeaderMargin = new MarginContainer();
            badgesHeaderMargin.AddThemeConstantOverride("margin_left",   20);
            badgesHeaderMargin.AddThemeConstantOverride("margin_right",  20);
            badgesHeaderMargin.AddThemeConstantOverride("margin_top",    24);
            badgesHeaderMargin.AddThemeConstantOverride("margin_bottom", 12);
            vbox.AddChild(badgesHeaderMargin);

            var badgesTitle = new Label { Text = "Athlete Badges" };
            badgesTitle.AddThemeFontSizeOverride("font_size", 18);
            badgesTitle.AddThemeColorOverride("font_color", ColorTextPrimary);
            badgesHeaderMargin.AddChild(badgesTitle);

            var badgesMargin = new MarginContainer();
            badgesMargin.AddThemeConstantOverride("margin_left",   20);
            badgesMargin.AddThemeConstantOverride("margin_right",  20);
            badgesMargin.AddThemeConstantOverride("margin_bottom", 24);
            vbox.AddChild(badgesMargin);

            var badgeGrid = new GridContainer { Columns = 3 };
            badgeGrid.AddThemeConstantOverride("h_separation", 12);
            badgeGrid.AddThemeConstantOverride("v_separation", 12);
            badgesMargin.AddChild(badgeGrid);

            var allBadges = new System.Collections.Generic.Dictionary<string, (string emoji, string name)>
            {
                { "first_assessment", ("\U0001F3AF", "First Assessment") },
                { "top_10_percent",   ("\U0001F4AB", "Top 10%") },
                { "all_tests",        ("\U0001F3C6", "All Tests Complete") },
                { "consistent",       ("\U0001F4C8", "Consistent Improver") },
                { "speed_demon",      ("\u26A1",     "Speed Demon") },
                { "endurance_king",   ("\U0001F451", "Endurance King") },
            };

            var earned = AppState.Instance?.Badges ?? new Godot.Collections.Array<string>();

            foreach (var kv in allBadges)
            {
                badgeGrid.AddChild(BuildBadge(kv.Value.emoji, kv.Value.name, earned.Contains(kv.Key)));
            }

            var bottomPad = new Control { CustomMinimumSize = new Vector2(0, 24) };
            vbox.AddChild(bottomPad);
        }

        private void BuildRadarChart(VBoxContainer parent)
        {
            var headerMargin = new MarginContainer();
            headerMargin.AddThemeConstantOverride("margin_left",   20);
            headerMargin.AddThemeConstantOverride("margin_right",  20);
            headerMargin.AddThemeConstantOverride("margin_top",    24);
            headerMargin.AddThemeConstantOverride("margin_bottom", 8);
            parent.AddChild(headerMargin);

            var headerLbl = new Label { Text = "Performance Overview" };
            headerLbl.AddThemeFontSizeOverride("font_size", 18);
            headerLbl.AddThemeColorOverride("font_color", ColorTextPrimary);
            headerMargin.AddChild(headerLbl);

            var chartMargin = new MarginContainer();
            chartMargin.AddThemeConstantOverride("margin_left",   20);
            chartMargin.AddThemeConstantOverride("margin_right",  20);
            chartMargin.AddThemeConstantOverride("margin_top",    0);
            chartMargin.AddThemeConstantOverride("margin_bottom", 0);
            parent.AddChild(chartMargin);

            // Radar chart placeholder panel
            var chartPanel = new Panel();
            chartPanel.CustomMinimumSize = new Vector2(0, 200);
            chartPanel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            chartPanel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = ColorSurface,
                CornerRadiusTopLeft     = 16,
                CornerRadiusTopRight    = 16,
                CornerRadiusBottomLeft  = 16,
                CornerRadiusBottomRight = 16,
                BorderColor       = ColorBorder,
                BorderWidthTop    = 1,
                BorderWidthBottom = 1,
                BorderWidthLeft   = 1,
                BorderWidthRight  = 1,
            });
            chartMargin.AddChild(chartPanel);

            var center = new CenterContainer();
            center.SetAnchorsPreset(LayoutPreset.FullRect);
            chartPanel.AddChild(center);

            var chartVBox = new VBoxContainer();
            chartVBox.AddThemeConstantOverride("separation", 8);
            center.AddChild(chartVBox);

            // Hexagonal outline placeholder with text labels
            var hexLbl = MakeLabel("\u2B21", 80, new Color(ColorChampionBlue.R, ColorChampionBlue.G, ColorChampionBlue.B, 0.20f));
            hexLbl.HorizontalAlignment = HorizontalAlignment.Center;
            chartVBox.AddChild(hexLbl);

            // 6-axis labels
            var axisLabels = new HBoxContainer();
            axisLabels.AddThemeConstantOverride("separation", 4);
            axisLabels.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
            chartVBox.AddChild(axisLabels);

            string[] axes = { "HT", "WT", "VJ", "SR", "SU", "ER" };
            foreach (var ax in axes)
            {
                var axLbl = MakeLabel(ax, 10, ColorTextTertiary);
                axLbl.CustomMinimumSize = new Vector2(30f, 0f);
                axLbl.HorizontalAlignment = HorizontalAlignment.Center;
                axisLabels.AddChild(axLbl);
            }
        }

        private void AddStatColumn(HBoxContainer parent, string value, string label)
        {
            var margin = new MarginContainer();
            margin.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.AddThemeConstantOverride("margin_top",    14);
            margin.AddThemeConstantOverride("margin_bottom", 14);
            parent.AddChild(margin);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 4);
            margin.AddChild(vbox);

            var valLabel = new Label { Text = value };
            valLabel.HorizontalAlignment = HorizontalAlignment.Center;
            valLabel.AddThemeFontSizeOverride("font_size", 22);
            valLabel.AddThemeColorOverride("font_color", ColorChampionBlue);
            vbox.AddChild(valLabel);

            var nameLabel = new Label { Text = label };
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            nameLabel.AddThemeFontSizeOverride("font_size", 11);
            nameLabel.AddThemeColorOverride("font_color", ColorTextTertiary);
            vbox.AddChild(nameLabel);
        }

        private void AddStatDivider(HBoxContainer parent)
        {
            var sep = new Panel();
            sep.CustomMinimumSize = new Vector2(1, 0);
            sep.SizeFlagsVertical = SizeFlags.ExpandFill;
            sep.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = ColorBorder,
            });
            parent.AddChild(sep);
        }

        private Control BuildBadge(string emoji, string name, bool earned)
        {
            var panel = new Panel();
            panel.CustomMinimumSize = new Vector2(0, 90);
            panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = earned ? new Color(0.118f, 0.227f, 0.541f, 0.12f) : ColorSurface,
                CornerRadiusTopLeft     = 12,
                CornerRadiusTopRight    = 12,
                CornerRadiusBottomLeft  = 12,
                CornerRadiusBottomRight = 12,
                BorderColor = earned ? ColorChampionBlue : ColorBorder,
                BorderWidthTop    = 1,
                BorderWidthBottom = 1,
                BorderWidthLeft   = 1,
                BorderWidthRight  = 1,
            });

            var center = new CenterContainer();
            center.SetAnchorsPreset(LayoutPreset.FullRect);
            panel.AddChild(center);

            var vbox = new VBoxContainer();
            vbox.AddThemeConstantOverride("separation", 6);
            center.AddChild(vbox);

            var emojiLabel = new Label { Text = emoji };
            emojiLabel.HorizontalAlignment = HorizontalAlignment.Center;
            emojiLabel.AddThemeFontSizeOverride("font_size", 28);
            emojiLabel.Modulate = earned ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.35f);
            vbox.AddChild(emojiLabel);

            var nameLabel = new Label { Text = name };
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            nameLabel.AddThemeFontSizeOverride("font_size", 10);
            nameLabel.AddThemeColorOverride("font_color", earned ? ColorChampionBlue : ColorTextTertiary);
            nameLabel.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(nameLabel);

            return panel;
        }

        private static Label MakeLabel(string text, int size, Color color, bool bold = false)
        {
            var label = new Label { Text = text };
            label.AddThemeFontSizeOverride("font_size", size);
            label.AddThemeColorOverride("font_color", color);
            return label;
        }

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
    }
}
