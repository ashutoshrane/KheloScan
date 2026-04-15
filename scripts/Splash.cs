using Godot;

namespace KheloScan
{
    public partial class Splash : Control
    {
        [Signal] public delegate void SplashCompletedEventHandler();

        // -- Color palette ----------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorGoldMedal    = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorWhite        = new Color(1f, 1f, 1f);

        public override void _Ready()
        {
            // -- Root: full-rect Champion Blue background ---------------------
            SetAnchorsPreset(Control.LayoutPreset.FullRect);
            ClipContents = true;

            var bg = MakePanel(390f, 844f, ColorChampionBlue, 0);
            bg.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            bg.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            bg.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            AddChild(bg);

            // -- Gold top bar (6 px) ------------------------------------------
            var topBar = MakePanel(390f, 6f, ColorGoldMedal, 0);
            topBar.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopWide);
            topBar.CustomMinimumSize = new Vector2(390f, 6f);
            AddChild(topBar);

            // -- Center content via CenterContainer ---------------------------
            var centerContainer = new CenterContainer();
            centerContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            centerContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerContainer.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            AddChild(centerContainer);

            var centerVBox = new VBoxContainer();
            centerVBox.Alignment = BoxContainer.AlignmentMode.Center;
            centerVBox.AddThemeConstantOverride("separation", 16);
            centerContainer.AddChild(centerVBox);

            // Medal icon: 80x80 gold circle with medal emoji
            var logoPanel = MakePanel(80f, 80f, ColorGoldMedal, 40);
            logoPanel.CustomMinimumSize = new Vector2(80f, 80f);
            var logoCenter = new CenterContainer();
            logoCenter.CustomMinimumSize = new Vector2(80f, 80f);
            var logoLabel = MakeLabel("\U0001F3C5", 36, ColorWhite);
            logoLabel.HorizontalAlignment = HorizontalAlignment.Center;
            logoLabel.VerticalAlignment   = VerticalAlignment.Center;
            logoCenter.AddChild(logoLabel);
            logoPanel.AddChild(logoCenter);

            // Wrap logo in its own centering row
            var logoRow = new CenterContainer();
            logoRow.CustomMinimumSize = new Vector2(0f, 80f);
            logoRow.AddChild(logoPanel);
            centerVBox.AddChild(logoRow);

            // Title: "KHELOSCAN" 28px white bold
            var title = MakeLabel("KHELOSCAN", 28, ColorWhite);
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerVBox.AddChild(title);

            // Hindi subtitle: 16px gold
            var hindiLabel = MakeLabel("\u0916\u0947\u0932\u094B \u0938\u094D\u0915\u0948\u0928", 16, ColorGoldMedal);
            hindiLabel.HorizontalAlignment = HorizontalAlignment.Center;
            hindiLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerVBox.AddChild(hindiLabel);

            // Tagline: 14px white 70% opacity
            var taglineColor = new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.70f);
            var tagline = MakeLabel("Every Athlete Deserves to Be Seen", 14, taglineColor);
            tagline.HorizontalAlignment = HorizontalAlignment.Center;
            tagline.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerVBox.AddChild(tagline);

            // Gold divider: 40x2
            var dividerRow = new CenterContainer();
            var divider = MakePanel(40f, 2f, ColorGoldMedal, 1);
            dividerRow.AddChild(divider);
            centerVBox.AddChild(dividerRow);

            // Sub-label: 12px white 50% opacity
            var subColor = new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.50f);
            var subLabel = MakeLabel("AI-Powered Sports Assessment", 12, subColor);
            subLabel.HorizontalAlignment = HorizontalAlignment.Center;
            subLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerVBox.AddChild(subLabel);

            // Fade-in animation for center content
            centerContainer.Modulate = new Color(1f, 1f, 1f, 0f);
            var fadeIn = CreateTween();
            fadeIn.TweenProperty(centerContainer, "modulate:a", 1.0f, 0.8f)
                  .SetEase(Tween.EaseType.Out)
                  .SetTrans(Tween.TransitionType.Sine);

            // -- Bottom area: dots + powered-by text --------------------------
            var bottomAnchor = new Control();
            bottomAnchor.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
            bottomAnchor.CustomMinimumSize = new Vector2(390f, 80f);
            bottomAnchor.OffsetTop   = -80f;
            bottomAnchor.OffsetBottom = 0f;
            AddChild(bottomAnchor);

            var bottomVBox = new VBoxContainer();
            bottomVBox.Alignment = BoxContainer.AlignmentMode.Center;
            bottomVBox.AddThemeConstantOverride("separation", 8);
            bottomVBox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            bottomAnchor.AddChild(bottomVBox);

            // 3 pulsing white dots
            var dotsRow = new CenterContainer();
            var dotsHBox = new HBoxContainer();
            dotsHBox.AddThemeConstantOverride("separation", 8);
            dotsRow.AddChild(dotsHBox);

            for (int i = 0; i < 3; i++)
            {
                var dotPanel = new PanelContainer();
                dotPanel.CustomMinimumSize = new Vector2(8f, 8f);
                var dotStyle = new StyleBoxFlat
                {
                    BgColor = ColorWhite,
                    CornerRadiusTopLeft     = 4,
                    CornerRadiusTopRight    = 4,
                    CornerRadiusBottomLeft  = 4,
                    CornerRadiusBottomRight = 4
                };
                dotPanel.AddThemeStyleboxOverride("panel", dotStyle);
                dotsHBox.AddChild(dotPanel);

                float delay = i * 0.2f;
                var dotTween = CreateTween();
                dotTween.SetLoops();
                dotTween.TweenInterval(delay);
                dotTween.TweenProperty(dotPanel, "modulate:a", 0.3f, 0.4f)
                        .SetEase(Tween.EaseType.InOut)
                        .SetTrans(Tween.TransitionType.Sine);
                dotTween.TweenProperty(dotPanel, "modulate:a", 1.0f, 0.4f)
                        .SetEase(Tween.EaseType.InOut)
                        .SetTrans(Tween.TransitionType.Sine);
            }

            bottomVBox.AddChild(dotsRow);

            // "Powered by SIH25073" label
            var poweredColor = new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.50f);
            var poweredLabel = MakeLabel("Powered by SIH25073", 11, poweredColor);
            poweredLabel.HorizontalAlignment = HorizontalAlignment.Center;
            poweredLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            bottomVBox.AddChild(poweredLabel);

            // -- Timer: emit SplashCompleted after 2.5s -----------------------
            GetTree().CreateTimer(2.5f).Timeout += () =>
            {
                EmitSignal(SignalName.SplashCompleted);
            };
        }

        // -- Helpers ----------------------------------------------------------
        private static PanelContainer MakePanel(float w, float h, Color color, int radius = 0)
        {
            var panel = new PanelContainer();
            panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor                 = color,
                CornerRadiusTopLeft     = radius,
                CornerRadiusTopRight    = radius,
                CornerRadiusBottomLeft  = radius,
                CornerRadiusBottomRight = radius
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
