using Godot;

namespace KheloScan
{
    public partial class Onboarding : Control
    {
        [Signal] public delegate void OnboardingCompletedEventHandler();

        // -- Color palette ------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorFieldGreen   = new Color(0.086f, 0.639f, 0.290f);
        private static readonly Color ColorGoldMedal    = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorSprintOrange = new Color(0.976f, 0.451f, 0.086f);
        private static readonly Color ColorWhite        = new Color(1f, 1f, 1f);

        // -- Slide data ---------------------------------------------------
        private struct SlideData
        {
            public Color  Bg;
            public string Emoji;
            public string Title;
            public string Subtitle;
            public bool   HasCta;
            public string CtaText;
        }

        private static readonly SlideData[] Slides = new[]
        {
            new SlideData
            {
                Bg       = new Color(0.118f, 0.227f, 0.541f),
                Emoji    = "\U0001F3C3",  // runner
                Title    = "Assess Your Talent",
                Subtitle = "AI-powered fitness assessment from your phone. No expensive lab equipment needed.",
                HasCta   = false,
                CtaText  = ""
            },
            new SlideData
            {
                Bg       = new Color(0.086f, 0.639f, 0.290f),
                Emoji    = "\U0001F4CB",  // clipboard
                Title    = "6 Standard Tests",
                Subtitle = "SAI-approved battery: jump, run, agility & more. Scientifically validated assessments.",
                HasCta   = false,
                CtaText  = ""
            },
            new SlideData
            {
                Bg       = new Color(0.976f, 0.451f, 0.086f),
                Emoji    = "\U0001F3C6",  // trophy
                Title    = "Get Discovered",
                Subtitle = "Compare with national benchmarks, earn badges, get scouted by academies.",
                HasCta   = true,
                CtaText  = "Get Started"
            }
        };

        // -- State --------------------------------------------------------
        private int       _currentSlide = 0;
        private Control[] _slideControls;

        private Control _clipContainer;
        private PanelContainer[] _dots;

        private bool  _dragging   = false;
        private float _dragStartX = 0f;

        // -- _Ready -------------------------------------------------------
        public override void _Ready()
        {
            SetAnchorsPreset(Control.LayoutPreset.FullRect);
            ClipContents = true;

            _clipContainer = new Control();
            _clipContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            _clipContainer.ClipContents = true;
            AddChild(_clipContainer);

            _slideControls = new Control[Slides.Length];

            for (int i = 0; i < Slides.Length; i++)
            {
                var slide = BuildSlide(i);
                slide.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
                slide.OffsetLeft   = i * 390f;
                slide.OffsetRight  = i * 390f + 390f;
                slide.OffsetTop    = 0f;
                slide.OffsetBottom = 844f;
                _clipContainer.AddChild(slide);
                _slideControls[i] = slide;
            }
        }

        // -- Build a single slide -----------------------------------------
        private Control BuildSlide(int index)
        {
            var data = Slides[index];

            var root = new Control();
            root.CustomMinimumSize = new Vector2(390f, 844f);
            root.ClipContents      = true;

            // Background
            var bg = new ColorRect();
            bg.Color = data.Bg;
            bg.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            bg.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            bg.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            root.AddChild(bg);

            // 4px gold top bar
            var topBar = new ColorRect();
            topBar.Color = ColorGoldMedal;
            topBar.SetAnchorsPreset(Control.LayoutPreset.TopWide);
            topBar.OffsetBottom = 4f;
            root.AddChild(topBar);

            // Center VBox via CenterContainer
            var centerContainer = new CenterContainer();
            centerContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            centerContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerContainer.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
            centerContainer.OffsetBottom = -120f;
            root.AddChild(centerContainer);

            var vbox = new VBoxContainer();
            vbox.Alignment = BoxContainer.AlignmentMode.Center;
            vbox.AddThemeConstantOverride("separation", 20);
            centerContainer.AddChild(vbox);

            // Icon panel: 100x100 white 20% opacity, 20px radius, emoji 48px
            var iconPanelColor = new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.20f);
            var iconPanel = MakePanel(100f, 100f, iconPanelColor, 20);
            var iconCenter = new CenterContainer();
            iconCenter.CustomMinimumSize = new Vector2(100f, 100f);
            var iconLabel = MakeLabel(data.Emoji, 48, ColorWhite);
            iconLabel.HorizontalAlignment = HorizontalAlignment.Center;
            iconLabel.VerticalAlignment   = VerticalAlignment.Center;
            iconCenter.AddChild(iconLabel);
            iconPanel.AddChild(iconCenter);

            var iconRow = new CenterContainer();
            iconRow.AddChild(iconPanel);
            vbox.AddChild(iconRow);

            // Title: 28px bold white
            var title = MakeLabel(data.Title, 28, ColorWhite);
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            vbox.AddChild(title);

            // Subtitle: 15px white 80%, word wrap, max 310px
            var subtitleColor = new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.80f);
            var subtitle = MakeLabel(data.Subtitle, 15, subtitleColor);
            subtitle.HorizontalAlignment  = HorizontalAlignment.Center;
            subtitle.AutowrapMode         = TextServer.AutowrapMode.Word;
            subtitle.CustomMinimumSize    = new Vector2(310f, 0f);
            subtitle.SizeFlagsHorizontal  = Control.SizeFlags.ShrinkCenter;
            vbox.AddChild(subtitle);

            // Gold divider
            var dividerRow = new CenterContainer();
            var divider = MakePanel(60f, 2f, ColorGoldMedal, 1);
            dividerRow.AddChild(divider);
            vbox.AddChild(dividerRow);

            // Bottom navigation
            var bottomNav = BuildBottomNav(index);
            bottomNav.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
            bottomNav.OffsetTop    = -120f;
            bottomNav.OffsetBottom = 0f;
            root.AddChild(bottomNav);

            return root;
        }

        // -- Build bottom navigation for a slide --------------------------
        private Control BuildBottomNav(int slideIndex)
        {
            var nav = new VBoxContainer();
            nav.Alignment = BoxContainer.AlignmentMode.End;
            nav.AddThemeConstantOverride("separation", 16);

            // Dot indicators row
            var dotsRow = new HBoxContainer();
            dotsRow.Alignment = BoxContainer.AlignmentMode.Center;
            dotsRow.AddThemeConstantOverride("separation", 8);
            dotsRow.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

            var dotsCenter = new CenterContainer();
            dotsCenter.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            dotsCenter.AddChild(dotsRow);

            for (int d = 0; d < Slides.Length; d++)
            {
                bool active = (d == slideIndex);
                var dotColor = active
                    ? ColorWhite
                    : new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.30f);
                var dot = MakePanel(active ? 20f : 8f, 8f, dotColor, 4);
                dotsRow.AddChild(dot);
            }
            nav.AddChild(dotsCenter);

            var data = Slides[slideIndex];
            if (data.HasCta)
            {
                // "Get Started" full-width button
                var ctaBtn = MakeButton(data.CtaText, ColorChampionBlue, ColorWhite, 16);
                ctaBtn.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                ctaBtn.CustomMinimumSize   = new Vector2(310f, 52f);
                ctaBtn.Pressed += () => EmitSignal(SignalName.OnboardingCompleted);

                var btnCenter = new CenterContainer();
                btnCenter.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                btnCenter.AddChild(ctaBtn);
                nav.AddChild(btnCenter);
            }
            else
            {
                // Next + Skip buttons row
                var btnRow = new HBoxContainer();
                btnRow.Alignment = BoxContainer.AlignmentMode.Center;
                btnRow.AddThemeConstantOverride("separation", 16);
                btnRow.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

                var btnRowCenter = new CenterContainer();
                btnRowCenter.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

                var skipBtn = MakeButton("Skip", new Color(ColorWhite.R, ColorWhite.G, ColorWhite.B, 0.15f), ColorWhite, 14);
                skipBtn.CustomMinimumSize = new Vector2(100f, 48f);
                skipBtn.Pressed += () => EmitSignal(SignalName.OnboardingCompleted);

                var nextBtn = MakeButton("Next \u2192", ColorGoldMedal, ColorWhite, 15);
                nextBtn.CustomMinimumSize = new Vector2(160f, 48f);
                nextBtn.Pressed += () => NextSlide();

                btnRow.AddChild(skipBtn);
                btnRow.AddChild(nextBtn);
                btnRowCenter.AddChild(btnRow);
                nav.AddChild(btnRowCenter);
            }

            // Spacer at bottom
            var spacer = new Control();
            spacer.CustomMinimumSize = new Vector2(0f, 24f);
            nav.AddChild(spacer);

            return nav;
        }

        // -- Slide transition ---------------------------------------------
        public void NextSlide()
        {
            if (_currentSlide >= Slides.Length - 1) return;

            int from = _currentSlide;
            int to   = _currentSlide + 1;
            AnimateSlide(from, to, forward: true);
            _currentSlide = to;
        }

        public void PrevSlide()
        {
            if (_currentSlide <= 0) return;

            int from = _currentSlide;
            int to   = _currentSlide - 1;
            AnimateSlide(from, to, forward: false);
            _currentSlide = to;
        }

        private void AnimateSlide(int from, int to, bool forward)
        {
            const float W = 390f;

            var slideFrom = _slideControls[from];
            var slideTo   = _slideControls[to];

            if (forward)
            {
                slideTo.OffsetLeft  = W;
                slideTo.OffsetRight = W * 2f;
            }
            else
            {
                slideTo.OffsetLeft  = -W;
                slideTo.OffsetRight = 0f;
            }

            var tween = CreateTween();
            tween.SetParallel(true);

            float outTarget = forward ? -W : W;
            tween.TweenProperty(slideFrom, "offset_left",  outTarget,      0.35f)
                 .SetEase(Tween.EaseType.InOut)
                 .SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(slideFrom, "offset_right", outTarget + W,  0.35f)
                 .SetEase(Tween.EaseType.InOut)
                 .SetTrans(Tween.TransitionType.Cubic);

            tween.TweenProperty(slideTo, "offset_left",  0f, 0.35f)
                 .SetEase(Tween.EaseType.InOut)
                 .SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(slideTo, "offset_right", W,  0.35f)
                 .SetEase(Tween.EaseType.InOut)
                 .SetTrans(Tween.TransitionType.Cubic);
        }

        // -- Swipe input detection ----------------------------------------
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonIndex == MouseButton.Left)
                {
                    if (mb.Pressed)
                    {
                        _dragging   = true;
                        _dragStartX = mb.Position.X;
                    }
                    else
                    {
                        if (_dragging)
                        {
                            float delta = mb.Position.X - _dragStartX;
                            if (delta < -60f)       NextSlide();
                            else if (delta > 60f)   PrevSlide();
                        }
                        _dragging = false;
                    }
                }
            }
            else if (@event is InputEventScreenTouch touch)
            {
                if (touch.Pressed)
                {
                    _dragging   = true;
                    _dragStartX = touch.Position.X;
                }
                else
                {
                    _dragging = false;
                }
            }
            else if (@event is InputEventScreenDrag drag)
            {
                if (_dragging)
                {
                    float delta = drag.Position.X - _dragStartX;
                    if (delta < -60f)
                    {
                        NextSlide();
                        _dragging = false;
                    }
                    else if (delta > 60f)
                    {
                        PrevSlide();
                        _dragging = false;
                    }
                }
            }
        }

        // -- Helpers ------------------------------------------------------
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
            var hoverStyle = new StyleBoxFlat
            {
                BgColor                 = new Color(bgColor.R + 0.05f, bgColor.G + 0.05f, bgColor.B + 0.05f, bgColor.A),
                CornerRadiusTopLeft     = 12,
                CornerRadiusTopRight    = 12,
                CornerRadiusBottomLeft  = 12,
                CornerRadiusBottomRight = 12
            };
            var pressedStyle = new StyleBoxFlat
            {
                BgColor                 = new Color(bgColor.R - 0.05f, bgColor.G - 0.05f, bgColor.B - 0.05f, bgColor.A),
                CornerRadiusTopLeft     = 12,
                CornerRadiusTopRight    = 12,
                CornerRadiusBottomLeft  = 12,
                CornerRadiusBottomRight = 12
            };
            btn.AddThemeStyleboxOverride("normal",  normalStyle);
            btn.AddThemeStyleboxOverride("hover",   hoverStyle);
            btn.AddThemeStyleboxOverride("pressed", pressedStyle);
            btn.AddThemeStyleboxOverride("focus",   normalStyle);
            return btn;
        }
    }
}
