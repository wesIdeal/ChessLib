using System.ComponentModel;

namespace ChessLib.Data.MoveRepresentation.NAG
{
    public enum PositionalNAG
    {
        [Symbol("=")] [Description("drawish position or even")]
        Drawish = 10,

        [Symbol("")] [Description("equal chances, quiet position")]
        EqualQuietPosition = 11,

        [Symbol("")] [Description("equal chances, active position")]
        EqualActivePosition = 12,

        [Symbol("∞")] [Description("unclear position")]
        Unclear = 13,

        [Symbol("⩲")] [Description("White has a slight advantage")]
        WhiteSlightAdvantage = 14,

        [Symbol("⩱")] [Description("Black has a slight advantage")]
        BlacKingSidelightAdvantage = 15,

        [Symbol("±")] [Description("White has a moderate advantage")]
        WhiteModerateAdvantage = 16,

        [Symbol("∓")] [Description("Black has a moderate advantage")]
        BlackModerateAdvantage = 17,

        [Symbol("+ −")] [Description("White has a decisive advantage")]
        WhiteDecisiveAdvantage = 18,

        [Symbol("− +")] [Description("Black has a decisive advantage")]
        BlackDecisiveAdvantage = 19,

        [Symbol("")] [Description("White has a crushing advantage (Black should resign)")]
        WhiteWinning = 20,

        [Symbol("")] [Description("Black has a crushing advantage (White should resign)")]
        BlackWinning = 21,

        [Symbol("⨀")] [Description("White is in zugzwang")]
        WhiteZugZwang = 22,

        [Symbol("⨀")] [Description("Black is in zugzwang")]
        BlackZugZwang = 23,

        [Symbol("")] [Description("White has a slight space advantage")]
        WhiteSlightSpaceAdvantage = 24,

        [Symbol("")] [Description("Black has a slight space advantage")]
        BlacKingSidelightSpaceAdvantage = 25,

        [Symbol("")] [Description("White has a moderate space advantage")]
        WhiteModerateSpaceAdvantage = 26,

        [Symbol("")] [Description("Black has a moderate space advantage")]
        BlackModerateSpaceAdvantage = 27,

        [Symbol("")] [Description("White has a decisive space advantage")]
        WhiteDecisiveSpaceAdvantage = 28,

        [Symbol("")] [Description("Black has a decisive space advantage")]
        BlackDecisiveSpaceAdvantage = 29,

        [Symbol("")] [Description("White has a slight time (development) advantage")]
        WhiteSlightTempoAdvantage = 30,

        [Symbol("")] [Description("Black has a slight time (development) advantage")]
        BlacKingSidelightTempoAdvantage = 31,

        [Symbol("⟳")] [Description("White has a moderate time (development) advantage")]
        WhiteModerateTempoAdvantage = 32,

        [Symbol("⟳")] [Description("Black has a moderate time (development) advantage")]
        BlackModerateTempoAdvantage = 33,

        [Symbol("")] [Description("White has a decisive time (development) advantage")]
        WhiteDecisiveTempoAdvantage = 34,

        [Symbol("")] [Description("Black has a decisive time (development) advantage")]
        BlackDecisiveTempoAdvantage = 35,

        [Symbol("→")] [Description("White has the initiative")]
        WhiteHasInitiative = 36,

        [Symbol("→")] [Description("Black has the initiative")]
        BlackInitiative = 37,

        [Symbol("")] [Description("White has a lasting initiative")]
        WhiteLastingInitiative = 38,

        [Symbol("")] [Description("Black has a lasting initiative")]
        BlackLastingInitiative = 39,

        [Symbol("↑")] [Description("White has the attack")]
        WhiteHasTheAttack = 40,

        [Symbol("↑")] [Description("Black has the attack")]
        BlackHasTheAttack = 41,

        [Symbol("")] [Description("White has insufficient compensation for material deficit")]
        WhiteHasInsufficientCompensationForMaterialDefecit = 42,

        [Symbol("")] [Description("Black has insufficient compensation for material deficit")]
        BlackHasInsufficientCompensationForMaterialDefecit = 43,

        [Symbol("")] [Description("White has sufficient compensation for material deficit")]
        WhiteHasCompensationForMaterialDefecit = 44,

        [Symbol("")] [Description("Black has sufficient compensation for material deficit")]
        BlackHasCompensationForMaterialDefecit = 45,

        [Symbol("")] [Description("White has more than adequate compensation for material deficit")]
        WhiteHasMoreThanAdequateCompensationForMaterialDefecit = 46,

        [Symbol("")] [Description("Black has more than adequate compensation for material deficit")]
        BlackHasMoreThanAdequateCompensationForMaterialDefecit = 47,

        [Symbol("")] [Description("White has a slight center control advantage")]
        WhiteHasASlightCentralControlAdvantage = 48,

        [Symbol("")] [Description("Black has a slight center control advantage")]
        BlackHasASlightCentralControlAdvantage = 49,

        [Symbol("")] [Description("White has a moderate center control advantage")]
        WhiteHasAModerateCentralControlAdvantage = 50,

        [Symbol("")] [Description("Black has a moderate center control advantage")]
        BlackHasAModerateCentralControlAdvantage = 51,

        [Symbol("")] [Description("White has a decisive center control advantage")]
        WhiteHasADecisiveCentralControlAdvantage = 52,

        [Symbol("")] [Description("Black has a decisive center control advantage")]
        BlackHasADecisiveCentralControlAdvantage = 53,

        [Symbol("")] [Description("White has a slight kingside control advantage")]
        WhiteHasSlightKingSideControlAdvantage = 54,

        [Symbol("")] [Description("Black has a slight kingside control advantage")]
        BlackHasSlightKingSideControlAdvantage = 55,

        [Symbol("")] [Description("White has a moderate kingside control advantage")]
        WhiteHasModerateKingSideControlAdvantage = 56,

        [Symbol("")] [Description("Black has a moderate kingside control advantage")]
        BlackHasModerateKingSideControlAdvantage = 57,

        [Symbol("")] [Description("White has a decisive kingside control advantage")]
        WhiteHasDecisiveKingSideControlAdvantage = 58,

        [Symbol("")] [Description("Black has a decisive kingside control advantage")]
        BlackHasDecisiveKingSideControlAdvantage = 59,

        [Symbol("")] [Description("White has a slight queenside control advantage")]
        WhiteHasSlightQueenSideControlAdvantage = 60,

        [Symbol("")] [Description("Black has a slight queenside control advantage")]
        BlackHasSlightQueenSideControlAdvantage = 61,

        [Symbol("")] [Description("White has a moderate queenside control advantage")]
        WhiteHasModerateQueenSideControlAdvantage = 62,

        [Symbol("")] [Description("Black has a moderate queenside control advantage")]
        BlackHasModerateQueenSideControlAdvantage = 63,

        [Symbol("")] [Description("White has a decisive queenside control advantage")]
        WhiteHasDecisiveQueenSideControlAdvantage = 64,

        [Symbol("")] [Description("Black has a decisive queenside control advantage")]
        BlackHasDecisiveQueenSideControlAdvantage = 65,

        [Symbol("")] [Description("White has a vulnerable first rank")]
        WhiteVulnerableFirstRank = 66,

        [Symbol("")] [Description("Black has a vulnerable first rank")]
        BlackVulnerableFirstRank = 67,

        [Symbol("")] [Description("White has a well protected first rank")]
        WhiteWellProtectedFirstRank = 68,

        [Symbol("")] [Description("Black has a well protected first rank")]
        BlackWellProtectedFirstRank = 69,

        [Symbol("")] [Description("White has a poorly protected king")]
        WhitePoorlyProtectedKing = 70,

        [Symbol("")] [Description("Black has a poorly protected king")]
        BlackPoorlyProtectedKing = 71,

        [Symbol("")] [Description("White has a well protected king")]
        WhiteWellProtectedKing = 72,

        [Symbol("")] [Description("Black has a well protected king")]
        BlackWellProtectedKing = 73,

        [Symbol("")] [Description("White has a poorly placed king")]
        WhitePoorlyPlacedKing = 74,

        [Symbol("")] [Description("Black has a poorly placed king")]
        BlackPoorlyPlacedKing = 75,

        [Symbol("")] [Description("White has a well placed king")]
        WhiteWellPlacedKing = 76,

        [Symbol("")] [Description("Black has a well placed king")]
        BlackWellPlacedKing = 77,

        [Symbol("")] [Description("White has a very weak pawn structure")]
        WhiteHasAVeryWeakPawnStructure = 78,

        [Symbol("")] [Description("Black has a very weak pawn structure")]
        BlackHasAVeryWeakPawnStructure = 79,

        [Symbol("")] [Description("White has a moderately weak pawn structure")]
        WhiteHasAModeratelyWeakPawnStructure = 80,

        [Symbol("")] [Description("Black has a moderately weak pawn structure")]
        BlackHasAModeratelyWeakPawnStructure = 81,

        [Symbol("")] [Description("White has a moderately strong pawn structure")]
        WhiteHasAModeratelyStrongPawnStructure = 82,

        [Symbol("")] [Description("Black has a moderately strong pawn structure")]
        BlackHasAModeratelyStrongPawnStructure = 83,

        [Symbol("")] [Description("White has a very strong pawn structure")]
        WhiteHasAVeryStrongPawnStructure = 84,

        [Symbol("")] [Description("Black has a very strong pawn structure")]
        BlackHasAVeryStrongPawnStructure = 85,

        [Symbol("")] [Description("White has poor knight placement")]
        WhiteHasPoorKnightPlacement = 86,

        [Symbol("")] [Description("Black has poor knight placement")]
        BlackHasPoorKnightPlacement = 87,

        [Symbol("")] [Description("White has good knight placement")]
        WhiteHasGoodKnightPlacement = 88,

        [Symbol("")] [Description("Black has good knight placement")]
        BlackHasGoodKnightPlacement = 89,

        [Symbol("")] [Description("White has poor bishop placement")]
        WhiteHasPoorBishopPlacement = 90,

        [Symbol("")] [Description("Black has poor bishop placement")]
        BlackHasPoorBishopPlacement = 91,

        [Symbol("")] [Description("White has good bishop placement")]
        WhiteHasGoodBishopPlacement = 92,

        [Symbol("")] [Description("Black has good bishop placement")]
        BlackHasGoodBishopPlacement = 93,

        [Symbol("")] [Description("White has poor rook placement")]
        WhiteHasPoorRookPlacement = 94,

        [Symbol("")] [Description("Black has poor rook placement")]
        BlackHasPoorRookPlacement = 95,

        [Symbol("")] [Description("White has good rook placement")]
        WhiteHasGoodRookPlacement = 96,

        [Symbol("")] [Description("Black has good rook placement")]
        BlackHasGoodRookPlacement = 97,

        [Symbol("")] [Description("White has poor queen placement")]
        WhiteHasPoorQueenPlacement = 98,

        [Symbol("")] [Description("Black has poor queen placement")]
        BlackHasPoorQueenPlacement = 99,

        [Symbol("")] [Description("White has good queen placement")]
        WhiteHasGoodQueenPlacement = 100,

        [Symbol("")] [Description("Black has good queen placement")]
        BlackHasGoodQueenPlacement = 101,

        [Symbol("")] [Description("White has poor piece coordination")]
        WhiteHasPoorPieceCoordination = 102,

        [Symbol("")] [Description("Black has poor piece coordination")]
        BlackHasPoorPieceCoordination = 103,

        [Symbol("")] [Description("White has good piece coordination")]
        WhiteHasGoodPieceCoordination = 104,

        [Symbol("")] [Description("Black has good piece coordination")]
        BlackHasGoodPieceCoordination = 105,

        [Symbol("")] [Description("White has played the opening very poorly")]
        WhitePlayedOpeningVeryPoorly = 106,

        [Symbol("")] [Description("Black has played the opening very poorly")]
        BlackPlayedOpeningVeryPoorly = 107,

        [Symbol("")] [Description("White has played the opening poorly")]
        WhitePlayedOpeningPoorly = 108,

        [Symbol("")] [Description("Black has played the opening poorly")]
        BlackPlayedOpeningPoorly = 109,

        [Symbol("")] [Description("White has played the opening well")]
        WhitePlayedOpeningWell = 110,

        [Symbol("")] [Description("Black has played the opening well")]
        BlackPlayedOpeningWell = 111,

        [Symbol("")] [Description("White has played the opening very well")]
        WhitePlayedOpeningVeryWell = 112,

        [Symbol("")] [Description("Black has played the opening very well")]
        BlackPlayedOpeningVeryWell = 113,

        [Symbol("")] [Description("White has played the middlegame very poorly")]
        WhitePlayedMiddleGameVeryPoorly = 114,

        [Symbol("")] [Description("Black has played the middlegame very poorly")]
        BlackPlayedMiddleGameVeryPoorly = 115,

        [Symbol("")] [Description("White has played the middlegame poorly")]
        WhitePlayedMiddleGamePoorly = 116,

        [Symbol("")] [Description("Black has played the middlegame poorly")]
        BlackPlayedMiddleGamePoorly = 117,

        [Symbol("")] [Description("White has played the middlegame well")]
        WhitePlayedMiddleGameWell = 118,

        [Symbol("")] [Description("Black has played the middlegame well")]
        BlackPlayedMiddleGameWell = 119,

        [Symbol("")] [Description("White has played the middlegame very well")]
        WhitePlayedMiddleGameVeryWell = 120,

        [Symbol("")] [Description("Black has played the middlegame very well")]
        BlackPlayedMiddleGameVeryWell = 121,

        [Symbol("")] [Description("White has played the ending very poorly")]
        WhitePlayedEndingVeryPoorly = 122,

        [Symbol("")] [Description("Black has played the ending very poorly")]
        BlackPlayedEndingVeryPoorly = 123,

        [Symbol("")] [Description("White has played the ending poorly")]
        WhitePlayedEndingPoorly = 124,

        [Symbol("")] [Description("Black has played the ending poorly")]
        BlackPlayedEndingPoorly = 125,

        [Symbol("")] [Description("White has played the ending well")]
        WhitePlayedEndingWell = 126,

        [Symbol("")] [Description("Black has played the ending well")]
        BlackPlayedEndingWell = 127,

        [Symbol("")] [Description("White has played the ending very well")]
        WhitePlayedEndingVeryWell = 128,

        [Symbol("")] [Description("Black has played the ending very well")]
        BlackPlayedEndingVeryWell = 129,

        [Symbol("")] [Description("White has slight counterplay")]
        WhiteHasSlightCounterplay = 130,

        [Symbol("")] [Description("Black has slight counterplay")]
        BlackHasSlightCounterplay = 131,

        [Symbol("⇆")] [Description("White has moderate counterplay")]
        WhiteHasModerateCounterplay = 132,

        [Symbol("⇆")] [Description("Black has moderate counterplay")]
        BlackHasModerateCounterplay = 133,

        [Symbol("")] [Description("White has decisive counterplay")]
        WhiteHasDecisiveCounterplay = 134,

        [Symbol("")] [Description("Black has decisive counterplay")]
        BlackHasDecisiveCounterplay = 135
    }
}