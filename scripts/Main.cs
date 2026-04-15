using Godot;
using Godot.Collections;
using System.Collections.Generic;

namespace KheloScan
{
    /// <summary>
    /// Root scene controller. Owns the full navigation stack and tab bar.
    /// Attach to the root Control node in scenes/main.tscn.
    /// </summary>
    public partial class Main : Control
    {
        // -- Color constants ------------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorFieldGreen   = new Color(0.086f, 0.639f, 0.290f);
        private static readonly Color ColorGoldMedal    = new Color(0.918f, 0.702f, 0.031f);
        private static readonly Color ColorSprintOrange = new Color(0.976f, 0.451f, 0.086f);
        private static readonly Color ColorTrackRed     = new Color(0.863f, 0.149f, 0.149f);
        private static readonly Color ColorSurface      = new Color(0.973f, 0.980f, 0.988f);
        private static readonly Color ColorBg           = new Color(0.945f, 0.961f, 0.976f);
        private static readonly Color ColorTextPrimary  = new Color(0.059f, 0.090f, 0.165f);
        private static readonly Color ColorTextSecondary= new Color(0.278f, 0.333f, 0.412f);
        private static readonly Color ColorTextTertiary = new Color(0.580f, 0.639f, 0.722f);
        private static readonly Color ColorBorder       = new Color(0.886f, 0.910f, 0.941f);
        private static readonly Color ColorWhite        = new Color(1f, 1f, 1f);

        // -- Tab metadata ---------------------------------------------------------
        private static readonly string[]   TabNames  = { "home", "tests", "record", "leaderboard", "profile" };
        private static readonly string[]   TabLabels = { "Home", "Tests", "Record", "Board", "Profile" };
        private static readonly string[]   TabEmoji  = { "\U0001F3E0", "\U0001F4CB", "\U0001F4F9", "\U0001F3C6", "\U0001F464" };

        // -- Scene-tree references ------------------------------------------------
        private Control _bgPanel;
        private Control _mainContainer;
        private Control _contentArea;
        private Control _tabBar;
        private Control _overlayContainer;

        // -- Screen registry and navigation stack ---------------------------------
        private readonly System.Collections.Generic.Dictionary<string, Control> _screens = new();
        private readonly List<Control> _screenStack = new List<Control>();

        // -- Tab button refs for highlight updates --------------------------------
        private readonly List<Button> _tabButtons = new List<Button>();

        // -- Lifecycle ------------------------------------------------------------

        public override void _Ready()
        {
            SetAnchorsPreset(LayoutPreset.FullRect);
            SetupBackground();
            ShowSplash();
        }

        // -- Input: back-navigation -----------------------------------------------

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                PopScreen();
        }

        // -- Background -----------------------------------------------------------

        private void SetupBackground()
        {
            _bgPanel = new Panel();
            _bgPanel.SetAnchorsPreset(LayoutPreset.FullRect);

            var style = new StyleBoxFlat
            {
                BgColor = ColorBg,
                CornerRadiusTopLeft     = 0,
                CornerRadiusTopRight    = 0,
                CornerRadiusBottomLeft  = 0,
                CornerRadiusBottomRight = 0,
            };
            ((Panel)_bgPanel).AddThemeStyleboxOverride("panel", style);
            AddChild(_bgPanel);
        }

        // -- Phase 1: Splash ------------------------------------------------------

        private void ShowSplash()
        {
            var splash = new Splash();
            splash.SetAnchorsPreset(LayoutPreset.FullRect);
            AddChild(splash);

            splash.SplashCompleted += () =>
            {
                splash.QueueFree();
                ShowOnboarding();
            };
        }

        // -- Phase 2: Onboarding --------------------------------------------------

        private void ShowOnboarding()
        {
            var onboarding = new Onboarding();
            onboarding.SetAnchorsPreset(LayoutPreset.FullRect);
            AddChild(onboarding);

            onboarding.OnboardingCompleted += () =>
            {
                onboarding.QueueFree();
                SetupMainUI();
            };
        }

        // -- Phase 3: Main tab UI -------------------------------------------------

        private void SetupMainUI()
        {
            var vbox = new VBoxContainer();
            vbox.SetAnchorsPreset(LayoutPreset.FullRect);
            vbox.AddThemeConstantOverride("separation", 0);
            AddChild(vbox);
            _mainContainer = vbox;

            var contentPanel = new Panel();
            contentPanel.SizeFlagsVertical   = SizeFlags.ExpandFill;
            contentPanel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            var contentStyle = new StyleBoxFlat { BgColor = ColorBg };
            contentPanel.AddThemeStyleboxOverride("panel", contentStyle);
            vbox.AddChild(contentPanel);
            _contentArea = contentPanel;

            _overlayContainer = new Control();
            _overlayContainer.SetAnchorsPreset(LayoutPreset.FullRect);
            _overlayContainer.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(_overlayContainer);

            BuildTabScreens(contentPanel);

            _tabBar = BuildTabBar();
            vbox.AddChild(_tabBar);

            if (AppState.Instance != null)
            {
                AppState.Instance.TestSelected += OnTestSelected;
                AppState.Instance.TabChanged   += OnTabChanged;
            }

            ShowScreen("home");
        }

        // -- Tab screens ----------------------------------------------------------

        private void BuildTabScreens(Control parent)
        {
            _screens["home"]        = new HomeScreen();
            _screens["tests"]       = new TestsScreen();
            _screens["record"]      = new RecordScreen();
            _screens["leaderboard"] = new LeaderboardScreen();
            _screens["profile"]     = new ProfileScreen();

            foreach (var kv in _screens)
            {
                kv.Value.SetAnchorsPreset(LayoutPreset.FullRect);
                kv.Value.Visible = false;
                parent.AddChild(kv.Value);
            }
        }

        // -- Tab bar --------------------------------------------------------------

        private Control BuildTabBar()
        {
            var bar = new Panel();
            bar.CustomMinimumSize = new Vector2(0, 72);
            bar.SizeFlagsHorizontal = SizeFlags.ExpandFill;

            var barStyle = new StyleBoxFlat
            {
                BgColor = ColorWhite,
                BorderColor = ColorBorder,
                BorderWidthTop = 1,
            };
            bar.AddThemeStyleboxOverride("panel", barStyle);

            var hbox = new HBoxContainer();
            hbox.SetAnchorsPreset(LayoutPreset.FullRect);
            hbox.AddThemeConstantOverride("separation", 0);
            bar.AddChild(hbox);

            _tabButtons.Clear();
            for (int i = 0; i < TabNames.Length; i++)
            {
                var btn = BuildTabButton(i);
                hbox.AddChild(btn);
                _tabButtons.Add(btn);
            }

            return bar;
        }

        private Button BuildTabButton(int index)
        {
            var btn = new Button();
            btn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            btn.SizeFlagsVertical   = SizeFlags.ExpandFill;
            btn.FocusMode           = FocusModeEnum.None;

            var normalStyle  = new StyleBoxFlat { BgColor = new Color(0, 0, 0, 0) };
            var hoverStyle   = new StyleBoxFlat { BgColor = new Color(0.118f, 0.227f, 0.541f, 0.08f) };
            var pressedStyle = new StyleBoxFlat { BgColor = new Color(0.118f, 0.227f, 0.541f, 0.12f) };
            btn.AddThemeStyleboxOverride("normal",   normalStyle);
            btn.AddThemeStyleboxOverride("hover",    hoverStyle);
            btn.AddThemeStyleboxOverride("pressed",  pressedStyle);
            btn.AddThemeStyleboxOverride("focus",    normalStyle);

            var vbox = new VBoxContainer();
            vbox.SetAnchorsPreset(LayoutPreset.FullRect);
            vbox.AddThemeConstantOverride("separation", 2);
            vbox.Alignment = BoxContainer.AlignmentMode.Center;
            btn.AddChild(vbox);

            var emoji = new Label { Text = TabEmoji[index] };
            emoji.HorizontalAlignment = HorizontalAlignment.Center;
            emoji.AddThemeFontSizeOverride("font_size", 22);
            vbox.AddChild(emoji);

            var label = new Label { Text = TabLabels[index] };
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.AddThemeFontSizeOverride("font_size", 10);
            label.AddThemeColorOverride("font_color", ColorTextSecondary);
            vbox.AddChild(label);

            int capturedIndex = index;
            btn.Pressed += () => OnTabPressed(capturedIndex);

            return btn;
        }

        private void UpdateTabHighlight(int activeIndex)
        {
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                var btn   = _tabButtons[i];
                bool active = (i == activeIndex);

                if (btn.GetChildCount() > 0 && btn.GetChild(0) is VBoxContainer vbox)
                {
                    if (vbox.GetChildCount() >= 2)
                    {
                        var label = vbox.GetChild(1) as Label;
                        if (label != null)
                        {
                            label.AddThemeColorOverride("font_color",
                                active ? ColorChampionBlue : ColorTextSecondary);
                        }
                    }
                }

                var activeStyle = new StyleBoxFlat
                {
                    BgColor = new Color(0, 0, 0, 0),
                };
                if (active)
                {
                    activeStyle.BorderColor      = ColorChampionBlue;
                    activeStyle.BorderWidthBottom = 3;
                }
                btn.AddThemeStyleboxOverride("normal", activeStyle);
            }
        }

        // -- Screen management ----------------------------------------------------

        public void ShowScreen(string screenName)
        {
            foreach (var kv in _screens)
                kv.Value.Visible = false;

            if (_screens.TryGetValue(screenName, out var screen))
                screen.Visible = true;

            if (AppState.Instance != null)
                AppState.Instance.CurrentScreen = screenName;

            for (int i = 0; i < TabNames.Length; i++)
            {
                if (TabNames[i] == screenName)
                {
                    UpdateTabHighlight(i);
                    break;
                }
            }
        }

        public void PushScreen(Control screen)
        {
            screen.SetAnchorsPreset(LayoutPreset.FullRect);
            _overlayContainer.AddChild(screen);
            _screenStack.Add(screen);

            screen.Modulate = new Color(1, 1, 1, 0);
            var tween = CreateTween();
            tween.TweenProperty(screen, "modulate:a", 1.0f, 0.25f)
                 .SetEase(Tween.EaseType.Out);

            if (_tabBar != null)
                _tabBar.Visible = false;
        }

        public void PopScreen()
        {
            if (_screenStack.Count == 0)
                return;

            var top = _screenStack[_screenStack.Count - 1];
            _screenStack.RemoveAt(_screenStack.Count - 1);

            var tween = CreateTween();
            tween.TweenProperty(top, "modulate:a", 0.0f, 0.2f)
                 .SetEase(Tween.EaseType.In);
            tween.TweenCallback(Callable.From(top.QueueFree));

            if (_screenStack.Count == 0 && _tabBar != null)
                _tabBar.Visible = true;
        }

        // -- Signal handlers ------------------------------------------------------

        private void OnTabPressed(int index)
        {
            if (AppState.Instance != null)
                AppState.Instance.SelectTab(index);
            else
                ShowScreen(TabNames[index]);
        }

        private void OnTabChanged(int tabIndex)
        {
            if (tabIndex >= 0 && tabIndex < TabNames.Length)
                ShowScreen(TabNames[tabIndex]);
        }

        private void OnTestSelected(string testId)
        {
            var detailScreen = new TestDetailScreen();
            PushScreen(detailScreen);
        }
    }
}
