using PresetParser.Anno1800;
using System;
using Xunit;

namespace PresetParser.Tests
{
    public class NewFactionAndGroup1800Tests
    {
        #region testdata

        public static TheoryData<string> FarmersPublicBuildingsTestdata => new()
        {
            "Logistic_01 (Marketplace)",
            "Service_01 (Pub)",
            "Institution_02 (Fire Department)"
        };

        public static TheoryData<string> FarmersProductionBuildingsTestdata => new()
        {
            "Coastal_01 (Fish Coast Building)",
            "Processing_04 (Weavery)",
            "Food_06 (Schnapps Maker)",
            "Factory_03 (Timber Factory)",
            "Agriculture_05 (Timber Yard)"
        };

        public static TheoryData<string> FarmersFarmBuildingsTestdata => new()
        {
            "Agriculture_04 (Potato Farm)",
            "Agriculture_06 (Sheep Farm)"
        };

        public static TheoryData<string> WorkersPublicBuildingsTestdata => new()
        {
            "Institution_01 (Police)",
            "Service_04 (Church)",
            "Service_02 (School)"
        };

        public static TheoryData<string> WorkersProductionBuildingsTestdata => new()
        {
            "Factory_09 (Sailcloth Factory)",
            "Heavy_01 (Beams Heavy Industry)",
            "Heavy_04 (Weapons Heavy Industry)",
            "Heavy_02 (Steel Heavy Industry)",
            "Heavy_03 (Coal Heavy Industry)",
            "Processing_01 (Tallow Processing)",
            "Food_07 (Sausage Maker)",
            "Processing_02 (Flour Processing)",
            "Factory_02 (Soap Factory)",
            "Processing_03 (Malt Processing)",
            "Food_02 (Beer Maker)",
            "Factory_04 (Brick Factory)",
            "Food_01 (Bread Maker)"
        };

        public static TheoryData<string> WorkersFarmBuildingsTestdata => new()
        {
            "Agriculture_08 (Pig Farm)",
            "Agriculture_03 (Hop Farm)",
            "Agriculture_01 (Grain Farm)"
        };

        public static TheoryData<string> ArtisansPublicBuildingsTestdata => new()
        {
            "Institution_03 (Hospital)",
            "Service_05 (Cabaret)",
            "Service_07 (University)"
        };

        public static TheoryData<string> ArtisansProductionBuildingsTestdata => new()
        {
            "Food_03 (Goulash Factory)",
            "Food_05 (Canned Food Factory)",
            "Processing_06 (Glass Processing)",
            "Factory_07 (Window Factory)",
            "Agriculture_09 (Hunter's Cabin)",
            "Factory_05 (Fur Coat Workshop)",
            "Workshop_03 (Sewing Machines Factory)"
        };

        public static TheoryData<string> ArtisansFarmBuildingsTestdata => new()
        {
            "Agriculture_11 (Bell Pepper Farm)",
            "Agriculture_02 (Cattle Farm)"
        };

        public static TheoryData<string> EngineersPublicBuildingsTestdata => new()
        {
            "Service_03 (Bank)",
            "Electricity_02 (Oil Power Plant)"
        };

        public static TheoryData<string> EngineersProductionBuildingsTestdata => new()
        {
            "Factory_06 (Light Bulb Factory)",
            "Processing_08 (Carbon Filament Processing)",
            "Workshop_02 (Pocket Watch Workshop)",
            "Workshop_05 (Gold Workshop)",
            "Heavy_07 (Steam Motors Heavy Industry)",
            "Workshop_01 (High-Wheeler Workshop)",
            "Heavy_06 (Advanced Weapons Heavy Industry)",
            "Processing_05 (Dynamite Processing)",
            "Coastal_02 (Niter Coast Building)",
            "Workshop_07 (Glasses Workshop)",
            "Heavy_09 (Brass Heavy Industry)",
            "Heavy_10 (Oil Heavy Industry)",
            "Factory_01 (Concrete Factory)",
            "Heavy_10_field (Oil Pump)"
        };

        public static TheoryData<string> InvestorsPublicBuildingsTestdata => new()
        {
            "Service_09 (Club House)"
        };

        public static TheoryData<string> InvestorsProductionBuildingsTestdata => new()
        {
            "Heavy_08 (Steam Carriages Heavy Industry)",
            "Factory_10 (Chassis Factory)",
            "Workshop_04 (Phonographs Workshop)",
            "Processing_07 (Inlay Processing)",
            "Workshop_06 (Jewelry Workshop)",
            "Food_08 (Champagne Maker)"
        };

        public static TheoryData<string> InvestorsFarmBuildingsTestdata => new()
        {
            "Agriculture_10 (Vineyard)"
        };

        public static TheoryData<string> JornalerosPublicBuildingsTestdata => new()
        {
            "Institution_colony01_02 (Fire Department)",
            "Institution_colony01_01 (Police)",
            "Service_colony01_01 (Marketplace)",
            "Service_colony01_02 (Chapel)"
        };

        public static TheoryData<string> JornalerosProductionBuildingsTestdata => new()
        {
            "Processing_colony01_02 (Poncho Maker)",
            "Coastal_colony01_01 (Pearls Coast Building)",
            "Food_colony01_04 (Fried Banana Maker)",
            "Coastal_colony01_02 (Fish Coast Building)",
            "Factory_colony01_02 (Sailcloth Factory)",
            "Factory_colony01_01 (Timber Factory)",
            "Agriculture_colony01_06 (Timber Yard)",
            "Factory_colony01_03 (Cotton Cloth Processing)",
            "Food_colony01_01 (Rum Maker)"
        };

        public static TheoryData<string> JornalerosFarmBuildingsTestdata => new()
        {
            "Agriculture_colony01_11 (Alpaca Farm)",
            "Agriculture_colony01_08 (Banana Farm)",
            "Agriculture_colony01_03 (Cotton Farm)",
            "Agriculture_colony01_01 (Sugar Cane Farm)",
            "Agriculture_colony01_05 (Caoutchouc Farm)"
        };

        public static TheoryData<string> ObrerosPublicBuildingsTestdata => new()
        {
            "Institution_colony01_03 (Hospital)",
            "Service_colony01_03 (Boxing Arena)"
        };

        public static TheoryData<string> ObrerosProductionBuildingsTestdata => new()
        {
            "Food_colony01_02 (Chocolate Maker)",
            "Workshop_colony01_01 (Cigars Workshop)",
            "Factory_colony01_07 (Bombin Maker)",
            "Factory_colony01_06 (Felt Maker)",
            "Food_colony01_03 (Coffee Maker)",
            "Food_colony01_05 (Burrito Maker)",
            "Processing_colony01_01 (Sugar Processing)",
            "Processing_colony01_03 (Inlay Processing)",
            "Heavy_colony01_01 (Oil Heavy Industry)",
            "Heavy_colony01_01_field (Oil Pump)"
        };

        public static TheoryData<string> ObrerosFarmBuildingsTestdata => new()
        {
            "Agriculture_colony01_09 (Cattle Farm)",
            "Agriculture_colony01_04 (Cocoa Farm)",
            "Agriculture_colony01_02 (Tobacco Farm)",
            "Agriculture_colony01_07 (Coffee Beans Farm)",
            "Agriculture_colony01_10 (Corn Farm)"
        };

        public static TheoryData<string> SpecialBuildingsTestdata => new()
        {
            "Guild_house",
            "Town hall"
        };

        public static TheoryData<string> OrnamentalsTestdata => new()
        {
            "Culture_preorder_statue",
            "Uplay_ornament_2x1_lion_statue",
            "Uplay_ornament_2x2_pillar_chess_park",
            "Uplay_ornament_3x2_large_fountain"
        };

        public static TheoryData<string> ExplorersPublicBuildingsTestdata => new()
        {
            "Service_arctic_01 (Canteen)",
            "Institution_arctic_01 (Ranger Station)"
        };

        public static TheoryData<string> ExplorersProductionBuildingsTestdata => new()
        {
            "Agriculture_arctic_01 (Timber Yard)",
            "Factory_arctic_01 (Timber Factory)",
            "Agriculture_arctic_02 (Caribou Hunter)",
            "Factory_arctic_02 (Sleeping Bags Factory)",
            "Heavy_arctic_01 (Coal Heavy Industry)",
            "Coastal_arctic_01 (Whale Coast Building)",
            "Coastal_arctic_02 (Seal Hunter)",
            "Factory_arctic_03 (Oil Lamp Factory)",
            "Food_arctic_01 (Pemmican)"
        };

        public static TheoryData<string> ExplorersFarmBuildingsTestdata => new()
        {
            "Agriculture_arctic_03 (Goose Farm)"
        };

        public static TheoryData<string> TechniciansPublicBuildingsTestdata => new()
        {
            "Service_arctic_02 (Post Office)"
        };

        public static TheoryData<string> TechniciansProductionBuildingsTestdata => new()
        {
            "Agriculture_arctic_04 (Bear Hunter)",
            "Factory_arctic_04 (Parka Factory)",
            "Agriculture_arctic_06 (Normal Fur Hunter)",
            "Factory_arctic_05 (Sled Frame Factory)",
            "Factory_arctic_06 (Husky Sled Factory)",
            "Mining_arctic_01 (Gas Mine)",
            "Mining_arctic_02 (Gold Mine)",
            "Mining_arctic_01_pump (Gas Pump)",
            "Monument_arctic_01_00"
        };

        public static TheoryData<string> TechniciansFarmBuildingsTestdata => new()
        {
            "Agriculture_arctic_05 (Husky Farm)"
        };

        #endregion

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void GetNewFactionAndGroup1800IdentifierIsNullOrWhiteSpaceShouldThrow(string identifier)
        {
            // Arrange/Act
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null));

            // Assert
            Assert.NotNull(ex);
            Assert.Contains("No identifier was given.", ex.Message);
        }

        #region Farmers tests

        [Theory]
        [MemberData(nameof(FarmersPublicBuildingsTestdata))]
        [MemberData(nameof(FarmersProductionBuildingsTestdata))]
        [MemberData(nameof(FarmersFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToFarmersShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(01) Farmers", Faction);
        }

        [Theory]
        [MemberData(nameof(FarmersPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToFarmersPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(01) Farmers", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(FarmersProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToFarmersProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(01) Farmers", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(FarmersFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToFarmersFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(01) Farmers", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Workers tests

        [Theory]
        [MemberData(nameof(WorkersPublicBuildingsTestdata))]
        [MemberData(nameof(WorkersProductionBuildingsTestdata))]
        [MemberData(nameof(WorkersFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToWorkersShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(02) Workers", Faction);
        }

        [Theory]
        [MemberData(nameof(WorkersPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToWorkersPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(02) Workers", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(WorkersProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToWorkersProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(02) Workers", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(WorkersFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToWorkersFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(02) Workers", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Artisans tests

        [Theory]
        [MemberData(nameof(ArtisansPublicBuildingsTestdata))]
        [MemberData(nameof(ArtisansProductionBuildingsTestdata))]
        [MemberData(nameof(ArtisansFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToArtisansShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(03) Artisans", Faction);
        }

        [Theory]
        [MemberData(nameof(ArtisansPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToArtisansPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(03) Artisans", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(ArtisansProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToArtisansProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(03) Artisans", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(ArtisansFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToArtisansFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(03) Artisans", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Engineers tests

        [Theory]
        [MemberData(nameof(EngineersPublicBuildingsTestdata))]
        [MemberData(nameof(EngineersProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToEngineersShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(04) Engineers", Faction);
        }

        [Theory]
        [MemberData(nameof(EngineersPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToEngineersPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(04) Engineers", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(EngineersProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToEngineersProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(04) Engineers", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Investors tests

        [Theory]
        [MemberData(nameof(InvestorsPublicBuildingsTestdata))]
        [MemberData(nameof(InvestorsProductionBuildingsTestdata))]
        [MemberData(nameof(InvestorsFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToInvestorsShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(05) Investors", Faction);
        }

        [Theory]
        [MemberData(nameof(InvestorsPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToInvestorsPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(05) Investors", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(InvestorsProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToInvestorsProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(05) Investors", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(InvestorsFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToInvestorsFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(05) Investors", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Jornaleros tests

        [Theory]
        [MemberData(nameof(JornalerosPublicBuildingsTestdata))]
        [MemberData(nameof(JornalerosProductionBuildingsTestdata))]
        [MemberData(nameof(JornalerosFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToJornalerosShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(07) Jornaleros", Faction);
        }

        [Theory]
        [MemberData(nameof(JornalerosPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToJornalerosPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(07) Jornaleros", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(JornalerosProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToJornalerosProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(07) Jornaleros", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(JornalerosFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToJornalerosFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(07) Jornaleros", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Obreros tests

        [Theory]
        [MemberData(nameof(ObrerosPublicBuildingsTestdata))]
        [MemberData(nameof(ObrerosProductionBuildingsTestdata))]
        [MemberData(nameof(ObrerosFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToObrerosShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(08) Obreros", Faction);
        }

        [Theory]
        [MemberData(nameof(ObrerosPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToObrerosPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(08) Obreros", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(ObrerosProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToObrerosProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(08) Obreros", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(ObrerosFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToObrerosFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(08) Obreros", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Special buildings tests

        [Theory]
        [MemberData(nameof(SpecialBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToSpecialBuildingsShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("All Worlds", Faction);
        }

        [Theory]
        [MemberData(nameof(SpecialBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToSpecialBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Special Buildings", Group);
            Assert.Equal("All Worlds", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Ornamentals tests

        [Theory]
        [MemberData(nameof(OrnamentalsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToOrnamentalsShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Null(Faction);
        }

        [Theory]
        [MemberData(nameof(OrnamentalsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToOrnamentalsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Null(Group);
            Assert.Null(Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Explorers tests

        [Theory]
        [MemberData(nameof(ExplorersPublicBuildingsTestdata))]
        [MemberData(nameof(ExplorersProductionBuildingsTestdata))]
        [MemberData(nameof(ExplorersFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToExplorersShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(10) Explorers", Faction);
        }

        [Theory]
        [MemberData(nameof(ExplorersPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToExplorersPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(10) Explorers", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(ExplorersProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToExplorersProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(10) Explorers", Faction);
            Assert.Equal("FactoryBuilding7", Template);
        }

        [Theory]
        [MemberData(nameof(ExplorersFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToExplorersFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(10) Explorers", Faction);
            Assert.Empty(Template);
        }

        #endregion

        #region Technicians tests

        [Theory]
        [MemberData(nameof(TechniciansPublicBuildingsTestdata))]
        [MemberData(nameof(TechniciansProductionBuildingsTestdata))]
        [MemberData(nameof(TechniciansFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToTechniciansShouldReturnCorrectFaction(string identifier)
        {
            // Arrange/Act
            (string Faction, _, _) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("(11) Technicians", Faction);
        }

        [Theory]
        [MemberData(nameof(TechniciansPublicBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToTechniciansPublicBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Public Buildings", Group);
            Assert.Equal("(11) Technicians", Faction);
            Assert.Empty(Template);
        }

        [Theory]
        [MemberData(nameof(TechniciansProductionBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToTechniciansProductionBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Production Buildings", Group);
            Assert.Equal("(11) Technicians", Faction);
            Assert.Equal("FactoryBuilding7", Template);
        }

        [Theory]
        [MemberData(nameof(TechniciansFarmBuildingsTestdata))]
        public void GetNewFactionAndGroup1800IdentifierBelongsToTechniciansFarmBuildingsShouldReturnCorrectGroup(string identifier)
        {
            // Arrange/Act
            (string Faction, string Group, string Template) = NewFactionAndGroup1800.GetNewFactionAndGroup1800(identifier, null, null);

            // Assert
            Assert.Equal("Farm Buildings", Group);
            Assert.Equal("(11) Technicians", Faction);
            Assert.Empty(Template);
        }

        #endregion
    }
}
