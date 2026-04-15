using Godot;
using Godot.Collections;

namespace KheloScan
{
    public static class FitnessTestData
    {
        // -- All 6 SAI fitness tests --------------------------------------------------

        public static Array<Dictionary> GetAll()
        {
            return new Array<Dictionary>
            {
                // 1. Height
                new Dictionary
                {
                    { "id",               "height"                              },
                    { "name",             "Height Measurement"                  },
                    { "description",      "Anthropometric baseline"             },
                    { "emoji",            "\U0001F4CF" },  // ruler
                    { "instruction",      "Stand straight against wall"         },
                    { "difficulty",       "Basic"                               },
                    { "benchmark_male",   "170 cm"                              },
                    { "benchmark_female", "158 cm"                              },
                    { "color",            new Color(0.086f, 0.639f, 0.290f)     },
                },

                // 2. Weight
                new Dictionary
                {
                    { "id",               "weight"                              },
                    { "name",             "Weight Measurement"                  },
                    { "description",      "Body mass index"                     },
                    { "emoji",            "\u2696" },  // balance scale
                    { "instruction",      "Step on scale, stand still"          },
                    { "difficulty",       "Basic"                               },
                    { "benchmark_male",   "65 kg"                               },
                    { "benchmark_female", "55 kg"                               },
                    { "color",            new Color(0.086f, 0.639f, 0.290f)     },
                },

                // 3. Vertical Jump
                new Dictionary
                {
                    { "id",               "vertical_jump"                       },
                    { "name",             "Vertical Jump"                       },
                    { "description",      "Explosive leg power"                 },
                    { "emoji",            "\U0001F998" },  // kangaroo
                    { "instruction",      "Jump as high as possible from standstill" },
                    { "difficulty",       "Advanced"                            },
                    { "benchmark_male",   "45 cm"                               },
                    { "benchmark_female", "35 cm"                               },
                    { "color",            new Color(0.976f, 0.451f, 0.086f)     },
                },

                // 4. Shuttle Run
                new Dictionary
                {
                    { "id",               "shuttle_run"                         },
                    { "name",             "Shuttle Run"                         },
                    { "description",      "Agility & acceleration"              },
                    { "emoji",            "\U0001F3C3" },  // runner
                    { "instruction",      "Sprint between cones 10m apart"      },
                    { "difficulty",       "Advanced"                            },
                    { "benchmark_male",   "9.5 sec"                             },
                    { "benchmark_female", "10.5 sec"                            },
                    { "color",            new Color(0.863f, 0.149f, 0.149f)     },
                },

                // 5. Sit-ups
                new Dictionary
                {
                    { "id",               "situps"                              },
                    { "name",             "Sit-ups"                             },
                    { "description",      "Core muscular endurance"             },
                    { "emoji",            "\U0001F4AA" },  // flexed biceps
                    { "instruction",      "Maximum reps in 60 seconds"          },
                    { "difficulty",       "Intermediate"                        },
                    { "benchmark_male",   "35 reps"                             },
                    { "benchmark_female", "25 reps"                             },
                    { "color",            new Color(0.918f, 0.702f, 0.031f)     },
                },

                // 6. Endurance Run
                new Dictionary
                {
                    { "id",               "endurance_run"                       },
                    { "name",             "Endurance Run"                       },
                    { "description",      "Cardiovascular fitness"              },
                    { "emoji",            "\U0001FAC1" },  // lungs
                    { "instruction",      "800m/1600m timed run"                },
                    { "difficulty",       "Advanced"                            },
                    { "benchmark_male",   "6:30 min"                            },
                    { "benchmark_female", "8:00 min"                            },
                    { "color",            new Color(0.863f, 0.149f, 0.149f)     },
                },
            };
        }

        public static Dictionary? GetById(string id)
        {
            foreach (var test in GetAll())
            {
                if ((string)test["id"] == id)
                    return test;
            }
            return null;
        }

        // -- Age-group benchmark data -------------------------------------------------

        public static Dictionary GetAgeGroupBenchmarks()
        {
            return new Dictionary
            {
                { "8-10", new Dictionary
                    {
                        { "vertical_jump", "25 cm" },
                        { "shuttle_run",   "12.0 sec" },
                        { "situps",        "20 reps" },
                        { "endurance_run", "9:00 min" },
                    }
                },
                { "11-14", new Dictionary
                    {
                        { "vertical_jump", "35 cm" },
                        { "shuttle_run",   "10.5 sec" },
                        { "situps",        "28 reps" },
                        { "endurance_run", "7:30 min" },
                    }
                },
                { "15-17", new Dictionary
                    {
                        { "vertical_jump", "42 cm" },
                        { "shuttle_run",   "9.8 sec" },
                        { "situps",        "33 reps" },
                        { "endurance_run", "6:45 min" },
                    }
                },
                { "18-21", new Dictionary
                    {
                        { "vertical_jump", "48 cm" },
                        { "shuttle_run",   "9.2 sec" },
                        { "situps",        "38 reps" },
                        { "endurance_run", "6:15 min" },
                    }
                },
            };
        }

        // -- Sport recommendation mappings --------------------------------------------

        public static Array<Dictionary> GetSportRecommendations()
        {
            return new Array<Dictionary>
            {
                new Dictionary
                {
                    { "sport",     "Athletics (Sprint)"                     },
                    { "emoji",     "\U0001F3C3"                             },
                    { "key_tests", new Array<string> { "shuttle_run", "vertical_jump" } },
                    { "desc",      "Strong agility and explosive power"     },
                },
                new Dictionary
                {
                    { "sport",     "Football"                               },
                    { "emoji",     "\u26BD"                                  },
                    { "key_tests", new Array<string> { "shuttle_run", "endurance_run" } },
                    { "desc",      "Agility combined with stamina"          },
                },
                new Dictionary
                {
                    { "sport",     "Basketball"                             },
                    { "emoji",     "\U0001F3C0"                             },
                    { "key_tests", new Array<string> { "vertical_jump", "height" } },
                    { "desc",      "Explosive jump height and stature"      },
                },
                new Dictionary
                {
                    { "sport",     "Wrestling / Kabaddi"                    },
                    { "emoji",     "\U0001F93C"                             },
                    { "key_tests", new Array<string> { "situps", "weight" } },
                    { "desc",      "Core strength and body composition"     },
                },
                new Dictionary
                {
                    { "sport",     "Long Distance Running"                  },
                    { "emoji",     "\U0001F3C5"                             },
                    { "key_tests", new Array<string> { "endurance_run", "situps" } },
                    { "desc",      "Cardiovascular endurance and core"      },
                },
                new Dictionary
                {
                    { "sport",     "Swimming"                               },
                    { "emoji",     "\U0001F3CA"                             },
                    { "key_tests", new Array<string> { "endurance_run", "vertical_jump" } },
                    { "desc",      "Full-body fitness and stamina"          },
                },
            };
        }
    }
}
