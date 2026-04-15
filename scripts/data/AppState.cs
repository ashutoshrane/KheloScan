using Godot;
using Godot.Collections;

namespace KheloScan
{
    /// <summary>
    /// Global application state autoload singleton.
    /// Register this as an autoload in Project Settings -> Autoloads.
    /// </summary>
    public partial class AppState : Node
    {
        // -- Singleton ----------------------------------------------------------------

        public static AppState Instance { get; private set; }

        // -- Navigation state ---------------------------------------------------------

        public string CurrentScreen     { get; set; } = "splash";
        public int    CurrentTab        { get; set; } = 0;
        public string SelectedTestId    { get; set; } = "";

        // -- Athlete / gamification state ---------------------------------------------

        public int   AthleteScore { get; set; } = 0;
        public float Percentile   { get; set; } = 0f;

        public Array<string> CompletedTests { get; } = new Array<string>();
        public Array<string> Badges         { get; } = new Array<string>();

        // -- Signals ------------------------------------------------------------------

        [Signal] public delegate void ScreenChangedEventHandler(string screenName);
        [Signal] public delegate void TabChangedEventHandler(int tabIndex);
        [Signal] public delegate void TestSelectedEventHandler(string testId);
        [Signal] public delegate void ScoreChangedEventHandler(int newTotal);
        [Signal] public delegate void BadgeEarnedEventHandler(string badgeId);

        // -- Lifecycle ----------------------------------------------------------------

        public override void _Ready()
        {
            Instance = this;
        }

        // -- Navigation methods -------------------------------------------------------

        /// <summary>Navigate to a named screen and emit ScreenChanged signal.</summary>
        public void NavigateTo(string screenName)
        {
            CurrentScreen = screenName;
            EmitSignal(SignalName.ScreenChanged, screenName);
        }

        /// <summary>Change the active bottom-tab index and emit TabChanged signal.</summary>
        public void SelectTab(int tabIndex)
        {
            CurrentTab = tabIndex;
            EmitSignal(SignalName.TabChanged, tabIndex);
        }

        // -- Test selection -----------------------------------------------------------

        /// <summary>Set the selected fitness test and emit TestSelected signal.</summary>
        public void SelectTest(string testId)
        {
            SelectedTestId = testId;
            EmitSignal(SignalName.TestSelected, testId);
        }

        // -- Test completion ----------------------------------------------------------

        /// <summary>
        /// Mark a fitness test as completed. Awards score points and checks badges.
        /// </summary>
        public void CompleteTest(string testId)
        {
            if (!CompletedTests.Contains(testId))
            {
                CompletedTests.Add(testId);
                AddScore(15);
            }
        }

        // -- Score management ---------------------------------------------------------

        /// <summary>Add score points and re-check badge conditions.</summary>
        public void AddScore(int points)
        {
            AthleteScore += points;
            Percentile = Mathf.Clamp(AthleteScore / 90f * 100f, 0f, 99.9f);
            EmitSignal(SignalName.ScoreChanged, AthleteScore);
            CheckBadges();
        }

        // -- Badge management ---------------------------------------------------------

        private void CheckBadges()
        {
            TryAwardBadge("first_assessment", CompletedTests.Count >= 1);
            TryAwardBadge("top_10_percent",   Percentile >= 90f);
            TryAwardBadge("all_tests",        CompletedTests.Count >= 6);
            TryAwardBadge("consistent",       AthleteScore >= 40);
            TryAwardBadge("speed_demon",      CompletedTests.Contains("shuttle_run"));
            TryAwardBadge("endurance_king",   CompletedTests.Contains("endurance_run"));
        }

        private void TryAwardBadge(string badgeId, bool condition)
        {
            if (condition && !Badges.Contains(badgeId))
            {
                Badges.Add(badgeId);
                EmitSignal(SignalName.BadgeEarned, badgeId);
            }
        }

        // -- Persistence helpers ------------------------------------------------------

        /// <summary>Returns a plain dictionary snapshot suitable for JSON serialisation.</summary>
        public Dictionary GetSaveData()
        {
            var badgesArr = new Array<string>();
            foreach (var b in Badges)
                badgesArr.Add(b);

            var testsArr = new Array<string>();
            foreach (var t in CompletedTests)
                testsArr.Add(t);

            return new Dictionary
            {
                { "score",      AthleteScore },
                { "percentile", Percentile   },
                { "tests",      testsArr     },
                { "badges",     badgesArr    },
            };
        }

        /// <summary>Restores state from a previously saved dictionary.</summary>
        public void LoadSaveData(Dictionary data)
        {
            if (data.ContainsKey("score"))
                AthleteScore = (int)data["score"];

            if (data.ContainsKey("percentile"))
                Percentile = (float)data["percentile"];

            if (data.ContainsKey("tests"))
            {
                CompletedTests.Clear();
                var arr = (Array<string>)data["tests"];
                foreach (var s in arr)
                    CompletedTests.Add(s);
            }

            if (data.ContainsKey("badges"))
            {
                Badges.Clear();
                var arr = (Array<string>)data["badges"];
                foreach (var s in arr)
                    Badges.Add(s);
            }
        }
    }
}
