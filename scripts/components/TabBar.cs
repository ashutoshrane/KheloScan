using Godot;

namespace KheloScan
{
    /// <summary>
    /// Standalone 5-tab navigation bar component.
    /// Can be used as a scene instance or created programmatically.
    /// The Main.cs controller builds the tab bar inline, but this class
    /// is available for standalone use as well.
    /// </summary>
    public partial class TabBar : Control
    {
        [Signal] public delegate void TabPressedEventHandler(int tabIndex);

        // -- Color palette ------------------------------------------------
        private static readonly Color ColorChampionBlue = new Color(0.118f, 0.227f, 0.541f);
        private static readonly Color ColorTextSecondary= new Color(0.278f, 0.333f, 0.412f);
        private static readonly Color ColorBorder       = new Color(0.886f, 0.910f, 0.941f);
        private static readonly Color ColorWhite        = new Color(1f, 1f, 1f);

        private static readonly string[] TabNames  = { "Home", "Tests", "Record", "Board", "Profile" };
        private static readonly string[] TabEmoji  = { "\U0001F3E0", "\U0001F4CB", "\U0001F4F9", "\U0001F3C6", "\U0001F464" };

        private int _activeTab = 0;

        public override void _Ready()
        {
            CustomMinimumSize = new Vector2(390f, 72f);
            SetAnchorsPreset(LayoutPreset.BottomWide);

            var barPanel = new Panel();
            barPanel.SetAnchorsPreset(LayoutPreset.FullRect);
            barPanel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = ColorWhite,
                BorderColor = ColorBorder,
                BorderWidthTop = 1,
            });
            AddChild(barPanel);

            var hbox = new HBoxContainer();
            hbox.SetAnchorsPreset(LayoutPreset.FullRect);
            hbox.AddThemeConstantOverride("separation", 0);
            barPanel.AddChild(hbox);

            for (int i = 0; i < TabNames.Length; i++)
            {
                int capturedIndex = i;

                var btn = new Button();
                btn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                btn.SizeFlagsVertical   = SizeFlags.ExpandFill;
                btn.FocusMode           = FocusModeEnum.None;

                var normalStyle  = new StyleBoxFlat { BgColor = new Color(0, 0, 0, 0) };
                btn.AddThemeStyleboxOverride("normal",  normalStyle);
                btn.AddThemeStyleboxOverride("hover",   normalStyle);
                btn.AddThemeStyleboxOverride("pressed", normalStyle);
                btn.AddThemeStyleboxOverride("focus",   normalStyle);

                var vbox = new VBoxContainer();
                vbox.SetAnchorsPreset(LayoutPreset.FullRect);
                vbox.AddThemeConstantOverride("separation", 2);
                vbox.Alignment = BoxContainer.AlignmentMode.Center;
                btn.AddChild(vbox);

                var emoji = new Label { Text = TabEmoji[i] };
                emoji.HorizontalAlignment = HorizontalAlignment.Center;
                emoji.AddThemeFontSizeOverride("font_size", 22);
                vbox.AddChild(emoji);

                var label = new Label { Text = TabNames[i] };
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.AddThemeFontSizeOverride("font_size", 10);
                label.AddThemeColorOverride("font_color", ColorTextSecondary);
                vbox.AddChild(label);

                btn.Pressed += () => EmitSignal(SignalName.TabPressed, capturedIndex);
                hbox.AddChild(btn);
            }
        }
    }
}
