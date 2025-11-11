using PresetParser.Anno1800;
using System;
using Xunit;

namespace PresetParser.Tests
{
    public class NewOrnamentsGroup1800Tests
    {
        #region test data

        public static TheoryData<string> ParkPathsTestdata => new()
        {
            "Park_1x1_pathstraight",
            "Park_1x1_pathend",
            "Park_1x1_pathangle",
            "Park_1x1_pathcrossing",
            "Park_1x1_pathwall"
        };

        public static TheoryData<string> ParkFencesTestdata => new()
        {
            "Park_1x1_hedgestraight",
            "Park_1x1_hedgeend",
            "Park_1x1_hedgeangle",
            "Park_1x1_hedgecrossing",
            "Park_1x1_hedgewall",
            "Culture_1x1_fencestraight",
            "Culture_1x1_fenceend",
            "Culture_1x1_fenceangle",
            "Culture_1x1_fencecrossing",
            "Culture_1x1_fencewall",
            "Park_1x1_hedgegate",
            "Culture_1x1_fencegate"
        };

        public static TheoryData<string> ParkVegetationTestdata => new()
        {
            "Park_1x1_grass",
            "Park_1x1_bush",
            "Park_1x1_smalltree",
            "Park_1x1_pine",
            "Park_1x1_poplar",
            "Park_1x1_bigtree",
            "Park_1x1_poplarforest",
            "Park_1x1_tropicalforest",
            "Park_1x1_philodendron",
            "Park_1x1_ferns",
            "Park_1x1_floweringshrub",
            "Park_1x1_smallpalmtree",
            "Park_1x1_palmtree",
            "Park_1x1_shrub",
            "Park_1x1_growncypress"
        };

        public static TheoryData<string> ParkFountainsTestdata => new()
        {
            "Uplay_ornament_3x2_large_fountain",
            "Park_2x2_fountain",
            "Park_3x3_fountain"
        };

        public static TheoryData<string> ParkStatuesTestdata => new()
        {
            "Sunken Treasure Ornament 01",
            "Sunken Treasure Ornament 02",
            "Sunken Treasure Ornament 03",
            "Uplay_ornament_2x1_lion_statue",
            "Culture_preorder_statue",
            "Park_2x2_statue",
            "Park_2x2_horsestatue"
        };

        public static TheoryData<string> ParkDecorationsTestdata => new()
        {
            "Park_1x1_benches",
            "Uplay_ornament_2x2_pillar_chess_park",
            "Park_2x2_garden",
            "Park_1x1_stand",
            "Park_2x2_gazebo",
            "Park_3x3_gazebo"
        };

        public static TheoryData<string> CityPathsTestdata => new()
        {
            "Palace Ornament02 Set01 hedge pointy",
            "Palace Ornament01 Set01 banner",
            "Palace Ornament03 Set01 hedge round",
            "Palace Ornament05 Set01 fountain big",
            "Palace Ornament04 Set01 fountain small",
            "Palace Ornament03 Set02 angle",
            "Palace Ornament04 Set02 crossing",
            "Palace Ornament02 Set02 end",
            "Palace Ornament05 Set02 junction",
            "Palace Ornament01 Set02 straight",
            "Palace Ornament06 Set02 straight variation"
        };

        public static TheoryData<string> CityFencesTestdata => new()
        {
            "Culture_prop_system_1x1_03",
            "Culture_prop_system_1x1_04",
            "Culture_prop_system_1x1_05",
            "Culture_prop_system_1x1_06",
            "Culture_prop_system_1x1_07",
            "Culture_prop_system_1x1_08",
            "Culture_prop_system_1x1_09",
            "Culture_prop_system_1x1_11",
            "Culture_prop_system_1x1_12",
            "Culture_prop_system_1x1_13",
            "Culture_prop_system_1x1_14",
            "Culture_1x1_hedgegate",
            "Culture_1x1_hedgestraight"
        };

        public static TheoryData<string> CityStatuesTestdata => new()
        {
            "Park_1x1_statue",
            "City_prop_system_2x2_03",
            "Culture_prop_system_1x1_10"
        };

        public static TheoryData<string> CityDecorationsTestdata => new()
        {
            "PropagandaTower Players Version",
            "PropagandaFlag Players Version",
            "Botanica Ornament 01",
            "Botanica Ornament 02",
            "Botanica Ornament 03",
            "Culture_1x1_benches",
            "Culture_1x1_stand"
        };

        public static TheoryData<string> SpecialOrnamentsTestdata => new()
        {
            "Event_ornament_halloween_2019",
            "Event_ornament_christmas_2019",
            "Event_ornament_onemio",
            "Twitchdrops_ornament_billboard_annoholic",
            "Twitchdrops_ornament_billboard_anno_union",
            "Twitchdrops_ornament_billboard_anarchy",
            "Twitchdrops_ornament_billboard_sunken_treasures",
            "Twitchdrops_ornament_botanical_garden",
            "Twitchdrops_ornament_flag_banner_annoholic",
            "Twitchdrops_ornament_flag_banner_anno_union",
            "Twitchdrops_ornament_billboard_the_passage",
            "Twitchdrops_ornament_morris_column_annoholic",
            "Twitchdrops_ornament_flag_seat_of_power",
            "Twitchdrops_ornament_billboard_seat_of_power",
            "Twitchdrops_ornament_flag_bright_harvest",
            "Twitchdrops_ornament_billboard_bright_harvest",
            "Twitchdrops_ornament_flag_land_of_lions",
            "Twitchdrops_ornament_billboard_land_of_lions",
            "Season 2 - Fountain Elephant",
            "Season 2 - Statue Tractor",
            "Season 2 - Pillar"
        };

        public static TheoryData<string> ChristmasDecorationsTestdata => new()
        {
            "Xmas City Tree Small 01",
            "Xmas City Tree Big 01",
            "Xmas Parksystem Ornament Straight",
            "Xmas Parksystem Ornament Corner",
            "Xmas Parksystem Ornament End",
            "Xmas City Snowman Ornament 01",
            "Xmas City Lightpost Ornament 01",
            "Xmas Citysystem Ornament T",
            "Xmas Citysystem Ornament Straight",
            "Xmas Citysystem Ornament Gap",
            "Xmas Citysystem Ornament Cross",
            "Xmas Citysystem Ornament End",
            "Xmas Citysystem Ornament Corner",
            "Xmas Parksystem Ornament Gap",
            "Xmas Market 1",
            "Xmas Parksystem Ornament Cross",
            "Xmas Parksystem Ornament T",
            "Xmas Lightpost Ornament 02",
            "Xmas Market 2",
            "XMas Market 3",
            "Xmas Carousel",
            "Xmas presents",
            "Xmas Santa Chair"
        };

        public static TheoryData<string> WorldsFairRewardsTestdata => new()
        {
            "City_prop_system_1x1_02",
            "City_prop_system_1x1_03",
            "City_prop_system_2x2_02",
            "City_prop_system_2x2_04",
            "City_prop_system_3x3_02",
            "City_prop_system_3x3_03",
            "Culture_prop_system_1x1_02",
            "Culture_1x1_basinbridge",
            "Culture_1x1_secondground"
        };

        public static TheoryData<string> GardensTestdata => new()
        {
            "Light pink flower field - roses",
            "Blue flower field - gentian",
            "Labyrinth",
            "Pink flower field - hibiscus",
            "Purple blue flower field - iris",
            "White flower field - blue heart lily",
            "Orange flower field - plumeria aussi orange",
            "Red white flower field - red white petunia",
            "Trees alley",
            "Sculpted trees",
            "Yellow flower field - miracle daisy"
        };

        public static TheoryData<string> AgriculturalOrnamentsTestdata => new()
        {
            "BH Ornament04 Flatbed Wagon",
            "BH Ornament05 Scarecrow",
            "BH Ornament06 LogPile",
            "BH Ornament07 Outhouse",
            "BH Ornament08 Signpost",
            "BH Ornament09 HayBalePile",
            "BH Ornament10 Swing",
            "BH Ornament23 Clothes Line"
        };

        public static TheoryData<string> AgriculturalFencesTestdata => new()
        {
            "BH Ornament03 Fence Straight",
            "BH Ornament03 Fence End",
            "BH Ornament03 Fence Cross",
            "BH Ornament03 Fence T-Cross",
            "BH Ornament03 Fence Corner",
            "BH Ornament03 Fence Gate"
        };

        public static TheoryData<string> IndustrialOrnamentsTestdata => new()
        {
            "BH Ornament24 Empty Groundplane",
            "BH Ornament11 Pipes",
            "BH Ornament12 Barrel Pile",
            "BH Ornament13 WoddenBoxes",
            "BH Ornament14 Tanks",
            "BH Ornament15 Water Tower",
            "BH Ornament17 Shed",
            "BH Ornament18 Pile Iron Bars",
            "BH Ornament19 Pile Boxes and Barrels",
            "BH Ornament20 Heap",
            "BH Ornament21 Large Boxes",
            "BH Ornament22 Gangway"
        };

        public static TheoryData<string> IndustrialFencesTestdata => new()
        {
            "BH Ornament01 Wall Straight",
            "BH Ornament01 Wall End",
            "BH Ornament01 Wall Cross",
            "BH Ornament01 Wall T-Cross",
            "BH Ornament01 Wall Corner",
            "BH Ornament01 Wall Gate",
            "BH Ornament01 Wall Gate 02",
            "BH Ornament02 Wall Straight Large",
            "BH Ornament02 Wall End Large",
            "BH Ornament02 Wall Cross Large",
            "BH Ornament02 Wall T-Cross Large",
            "BH Ornament02 Wall Corner Large",
            "BH Ornament02 Wall Gate Large"
        };

        public static TheoryData<string> AmusementParkTestdata => new()
        {
            "",
        };

        #endregion

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void GetNewOrnamentsGroup1800IdentifierIsNullOrWhiteSpaceShouldThrow(string identifier)
        {
            // Arrange/Act
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null));

            // Assert
            Assert.NotNull(ex);
            Assert.Contains("No identifier was given.", ex.Message);
        }

        [Theory]
        [MemberData(nameof(ParkPathsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToParkPathsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("01 Park Paths", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(ParkFencesTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToParkFencesShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("02 Park Fences", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(ParkVegetationTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToParkVegetationShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("03 Park Vegetation", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(ParkFountainsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToParkFountainsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("04 Park Fountains", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(ParkStatuesTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToParkStatuesShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("05 Park Statues", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(ParkDecorationsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToParkDecorationsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("06 Park Decorations", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(CityPathsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToCityPathsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("07 City Paths", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(CityFencesTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToCityFencesShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("08 City Fences", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(CityStatuesTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToCityStatuesShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("09 City Statues", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(CityDecorationsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToCityDecorationsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("10 City Decorations", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(SpecialOrnamentsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToSpecialOrnamentsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("11 Special Ornaments", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(ChristmasDecorationsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToChristmasDecorationsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("12 Christmas Decorations", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(WorldsFairRewardsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToWorldsFairRewardsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("13 World's Fair Rewards", Group);
            Assert.Null(Template);
        }

        [Theory]
        [MemberData(nameof(GardensTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToGardensShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("14 Gardens", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(AgriculturalOrnamentsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToAgriculturalOrnamentsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("15 Agricultural Ornaments", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(AgriculturalFencesTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToAgriculturalFencesShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("16 Agricultural Fences", Group);
            Assert.Equal("OrnamentalBuilding_Park", Template);
        }

        [Theory]
        [MemberData(nameof(IndustrialOrnamentsTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToIndustrialOrnamentsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("17 Industrial Ornaments", Group);
            Assert.Equal("OrnamentalBuilding_Industrial", Template);
        }

        [Theory]
        [MemberData(nameof(IndustrialFencesTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToIndustrialFencesShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("18 IndustrialFences", Group);
            Assert.Equal("OrnamentalBuilding_Industrial", Template);
        }

        [Theory(Skip = "not yet released")]
        [MemberData(nameof(AmusementParkTestdata))]
        public void GetNewOrnamentsGroup1800IdentifierBelongsToAmusementParkShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewOrnamentsGroup1800.GetNewOrnamentsGroup1800(identifier, null, null, null);

            // Assert
            Assert.Equal("Ornaments", Faction);
            Assert.Equal("19 Amusement Park", Group);
            Assert.Null(Template);
        }
    }
}
