﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Helpers
{
    /// <summary>
    ///     Helper methods to hash board states.
    /// </summary>
    /// <remarks>
    ///     This should be compliant with the polyglot standards, found
    ///     <see href="http://hardy.uhasselt.be/Toga/book_format.html">documentation here</see> for more details.
    /// </remarks>
    public static class PolyglotHelpers
    {
        private static readonly ulong[] Piece;
        private static readonly ulong[] Castle;
        private static readonly ulong[] EnPassant;
        private static readonly ulong[] Turn;

        #region array

        private static readonly ulong[] Random64 =
        {
            0x9D39247E33776D41ul, 0x2AF7398005AAA5C7ul, 0x44DB015024623547ul, 0x9C15F73E62A76AE2ul,
            0x75834465489C0C89ul, 0x3290AC3A203001BFul, 0x0FBBAD1F61042279ul, 0xE83A908FF2FB60CAul,
            0x0D7E765D58755C10ul, 0x1A083822CEAFE02Dul, 0x9605D5F0E25EC3B0ul, 0xD021FF5CD13A2ED5ul,
            0x40BDF15D4A672E32ul, 0x011355146FD56395ul, 0x5DB4832046F3D9E5ul, 0x239F8B2D7FF719CCul,
            0x05D1A1AE85B49AA1ul, 0x679F848F6E8FC971ul, 0x7449BBFF801FED0Bul, 0x7D11CDB1C3B7ADF0ul,
            0x82C7709E781EB7CCul, 0xF3218F1C9510786Cul, 0x331478F3AF51BBE6ul, 0x4BB38DE5E7219443ul,
            0xAA649C6EBCFD50FCul, 0x8DBD98A352AFD40Bul, 0x87D2074B81D79217ul, 0x19F3C751D3E92AE1ul,
            0xB4AB30F062B19ABFul, 0x7B0500AC42047AC4ul, 0xC9452CA81A09D85Dul, 0x24AA6C514DA27500ul,
            0x4C9F34427501B447ul, 0x14A68FD73C910841ul, 0xA71B9B83461CBD93ul, 0x03488B95B0F1850Ful,
            0x637B2B34FF93C040ul, 0x09D1BC9A3DD90A94ul, 0x3575668334A1DD3Bul, 0x735E2B97A4C45A23ul,
            0x18727070F1BD400Bul, 0x1FCBACD259BF02E7ul, 0xD310A7C2CE9B6555ul, 0xBF983FE0FE5D8244ul,
            0x9F74D14F7454A824ul, 0x51EBDC4AB9BA3035ul, 0x5C82C505DB9AB0FAul, 0xFCF7FE8A3430B241ul,
            0x3253A729B9BA3DDEul, 0x8C74C368081B3075ul, 0xB9BC6C87167C33E7ul, 0x7EF48F2B83024E20ul,
            0x11D505D4C351BD7Ful, 0x6568FCA92C76A243ul, 0x4DE0B0F40F32A7B8ul, 0x96D693460CC37E5Dul,
            0x42E240CB63689F2Ful, 0x6D2BDCDAE2919661ul, 0x42880B0236E4D951ul, 0x5F0F4A5898171BB6ul,
            0x39F890F579F92F88ul, 0x93C5B5F47356388Bul, 0x63DC359D8D231B78ul, 0xEC16CA8AEA98AD76ul,
            0x5355F900C2A82DC7ul, 0x07FB9F855A997142ul, 0x5093417AA8A7ED5Eul, 0x7BCBC38DA25A7F3Cul,
            0x19FC8A768CF4B6D4ul, 0x637A7780DECFC0D9ul, 0x8249A47AEE0E41F7ul, 0x79AD695501E7D1E8ul,
            0x14ACBAF4777D5776ul, 0xF145B6BECCDEA195ul, 0xDABF2AC8201752FCul, 0x24C3C94DF9C8D3F6ul,
            0xBB6E2924F03912EAul, 0x0CE26C0B95C980D9ul, 0xA49CD132BFBF7CC4ul, 0xE99D662AF4243939ul,
            0x27E6AD7891165C3Ful, 0x8535F040B9744FF1ul, 0x54B3F4FA5F40D873ul, 0x72B12C32127FED2Bul,
            0xEE954D3C7B411F47ul, 0x9A85AC909A24EAA1ul, 0x70AC4CD9F04F21F5ul, 0xF9B89D3E99A075C2ul,
            0x87B3E2B2B5C907B1ul, 0xA366E5B8C54F48B8ul, 0xAE4A9346CC3F7CF2ul, 0x1920C04D47267BBDul,
            0x87BF02C6B49E2AE9ul, 0x092237AC237F3859ul, 0xFF07F64EF8ED14D0ul, 0x8DE8DCA9F03CC54Eul,
            0x9C1633264DB49C89ul, 0xB3F22C3D0B0B38EDul, 0x390E5FB44D01144Bul, 0x5BFEA5B4712768E9ul,
            0x1E1032911FA78984ul, 0x9A74ACB964E78CB3ul, 0x4F80F7A035DAFB04ul, 0x6304D09A0B3738C4ul,
            0x2171E64683023A08ul, 0x5B9B63EB9CEFF80Cul, 0x506AACF489889342ul, 0x1881AFC9A3A701D6ul,
            0x6503080440750644ul, 0xDFD395339CDBF4A7ul, 0xEF927DBCF00C20F2ul, 0x7B32F7D1E03680ECul,
            0xB9FD7620E7316243ul, 0x05A7E8A57DB91B77ul, 0xB5889C6E15630A75ul, 0x4A750A09CE9573F7ul,
            0xCF464CEC899A2F8Aul, 0xF538639CE705B824ul, 0x3C79A0FF5580EF7Ful, 0xEDE6C87F8477609Dul,
            0x799E81F05BC93F31ul, 0x86536B8CF3428A8Cul, 0x97D7374C60087B73ul, 0xA246637CFF328532ul,
            0x043FCAE60CC0EBA0ul, 0x920E449535DD359Eul, 0x70EB093B15B290CCul, 0x73A1921916591CBDul,
            0x56436C9FE1A1AA8Dul, 0xEFAC4B70633B8F81ul, 0xBB215798D45DF7AFul, 0x45F20042F24F1768ul,
            0x930F80F4E8EB7462ul, 0xFF6712FFCFD75EA1ul, 0xAE623FD67468AA70ul, 0xDD2C5BC84BC8D8FCul,
            0x7EED120D54CF2DD9ul, 0x22FE545401165F1Cul, 0xC91800E98FB99929ul, 0x808BD68E6AC10365ul,
            0xDEC468145B7605F6ul, 0x1BEDE3A3AEF53302ul, 0x43539603D6C55602ul, 0xAA969B5C691CCB7Aul,
            0xA87832D392EFEE56ul, 0x65942C7B3C7E11AEul, 0xDED2D633CAD004F6ul, 0x21F08570F420E565ul,
            0xB415938D7DA94E3Cul, 0x91B859E59ECB6350ul, 0x10CFF333E0ED804Aul, 0x28AED140BE0BB7DDul,
            0xC5CC1D89724FA456ul, 0x5648F680F11A2741ul, 0x2D255069F0B7DAB3ul, 0x9BC5A38EF729ABD4ul,
            0xEF2F054308F6A2BCul, 0xAF2042F5CC5C2858ul, 0x480412BAB7F5BE2Aul, 0xAEF3AF4A563DFE43ul,
            0x19AFE59AE451497Ful, 0x52593803DFF1E840ul, 0xF4F076E65F2CE6F0ul, 0x11379625747D5AF3ul,
            0xBCE5D2248682C115ul, 0x9DA4243DE836994Ful, 0x066F70B33FE09017ul, 0x4DC4DE189B671A1Cul,
            0x51039AB7712457C3ul, 0xC07A3F80C31FB4B4ul, 0xB46EE9C5E64A6E7Cul, 0xB3819A42ABE61C87ul,
            0x21A007933A522A20ul, 0x2DF16F761598AA4Ful, 0x763C4A1371B368FDul, 0xF793C46702E086A0ul,
            0xD7288E012AEB8D31ul, 0xDE336A2A4BC1C44Bul, 0x0BF692B38D079F23ul, 0x2C604A7A177326B3ul,
            0x4850E73E03EB6064ul, 0xCFC447F1E53C8E1Bul, 0xB05CA3F564268D99ul, 0x9AE182C8BC9474E8ul,
            0xA4FC4BD4FC5558CAul, 0xE755178D58FC4E76ul, 0x69B97DB1A4C03DFEul, 0xF9B5B7C4ACC67C96ul,
            0xFC6A82D64B8655FBul, 0x9C684CB6C4D24417ul, 0x8EC97D2917456ED0ul, 0x6703DF9D2924E97Eul,
            0xC547F57E42A7444Eul, 0x78E37644E7CAD29Eul, 0xFE9A44E9362F05FAul, 0x08BD35CC38336615ul,
            0x9315E5EB3A129ACEul, 0x94061B871E04DF75ul, 0xDF1D9F9D784BA010ul, 0x3BBA57B68871B59Dul,
            0xD2B7ADEEDED1F73Ful, 0xF7A255D83BC373F8ul, 0xD7F4F2448C0CEB81ul, 0xD95BE88CD210FFA7ul,
            0x336F52F8FF4728E7ul, 0xA74049DAC312AC71ul, 0xA2F61BB6E437FDB5ul, 0x4F2A5CB07F6A35B3ul,
            0x87D380BDA5BF7859ul, 0x16B9F7E06C453A21ul, 0x7BA2484C8A0FD54Eul, 0xF3A678CAD9A2E38Cul,
            0x39B0BF7DDE437BA2ul, 0xFCAF55C1BF8A4424ul, 0x18FCF680573FA594ul, 0x4C0563B89F495AC3ul,
            0x40E087931A00930Dul, 0x8CFFA9412EB642C1ul, 0x68CA39053261169Ful, 0x7A1EE967D27579E2ul,
            0x9D1D60E5076F5B6Ful, 0x3810E399B6F65BA2ul, 0x32095B6D4AB5F9B1ul, 0x35CAB62109DD038Aul,
            0xA90B24499FCFAFB1ul, 0x77A225A07CC2C6BDul, 0x513E5E634C70E331ul, 0x4361C0CA3F692F12ul,
            0xD941ACA44B20A45Bul, 0x528F7C8602C5807Bul, 0x52AB92BEB9613989ul, 0x9D1DFA2EFC557F73ul,
            0x722FF175F572C348ul, 0x1D1260A51107FE97ul, 0x7A249A57EC0C9BA2ul, 0x04208FE9E8F7F2D6ul,
            0x5A110C6058B920A0ul, 0x0CD9A497658A5698ul, 0x56FD23C8F9715A4Cul, 0x284C847B9D887AAEul,
            0x04FEABFBBDB619CBul, 0x742E1E651C60BA83ul, 0x9A9632E65904AD3Cul, 0x881B82A13B51B9E2ul,
            0x506E6744CD974924ul, 0xB0183DB56FFC6A79ul, 0x0ED9B915C66ED37Eul, 0x5E11E86D5873D484ul,
            0xF678647E3519AC6Eul, 0x1B85D488D0F20CC5ul, 0xDAB9FE6525D89021ul, 0x0D151D86ADB73615ul,
            0xA865A54EDCC0F019ul, 0x93C42566AEF98FFBul, 0x99E7AFEABE000731ul, 0x48CBFF086DDF285Aul,
            0x7F9B6AF1EBF78BAFul, 0x58627E1A149BBA21ul, 0x2CD16E2ABD791E33ul, 0xD363EFF5F0977996ul,
            0x0CE2A38C344A6EEDul, 0x1A804AADB9CFA741ul, 0x907F30421D78C5DEul, 0x501F65EDB3034D07ul,
            0x37624AE5A48FA6E9ul, 0x957BAF61700CFF4Eul, 0x3A6C27934E31188Aul, 0xD49503536ABCA345ul,
            0x088E049589C432E0ul, 0xF943AEE7FEBF21B8ul, 0x6C3B8E3E336139D3ul, 0x364F6FFA464EE52Eul,
            0xD60F6DCEDC314222ul, 0x56963B0DCA418FC0ul, 0x16F50EDF91E513AFul, 0xEF1955914B609F93ul,
            0x565601C0364E3228ul, 0xECB53939887E8175ul, 0xBAC7A9A18531294Bul, 0xB344C470397BBA52ul,
            0x65D34954DAF3CEBDul, 0xB4B81B3FA97511E2ul, 0xB422061193D6F6A7ul, 0x071582401C38434Dul,
            0x7A13F18BBEDC4FF5ul, 0xBC4097B116C524D2ul, 0x59B97885E2F2EA28ul, 0x99170A5DC3115544ul,
            0x6F423357E7C6A9F9ul, 0x325928EE6E6F8794ul, 0xD0E4366228B03343ul, 0x565C31F7DE89EA27ul,
            0x30F5611484119414ul, 0xD873DB391292ED4Ful, 0x7BD94E1D8E17DEBCul, 0xC7D9F16864A76E94ul,
            0x947AE053EE56E63Cul, 0xC8C93882F9475F5Ful, 0x3A9BF55BA91F81CAul, 0xD9A11FBB3D9808E4ul,
            0x0FD22063EDC29FCAul, 0xB3F256D8ACA0B0B9ul, 0xB03031A8B4516E84ul, 0x35DD37D5871448AFul,
            0xE9F6082B05542E4Eul, 0xEBFAFA33D7254B59ul, 0x9255ABB50D532280ul, 0xB9AB4CE57F2D34F3ul,
            0x693501D628297551ul, 0xC62C58F97DD949BFul, 0xCD454F8F19C5126Aul, 0xBBE83F4ECC2BDECBul,
            0xDC842B7E2819E230ul, 0xBA89142E007503B8ul, 0xA3BC941D0A5061CBul, 0xE9F6760E32CD8021ul,
            0x09C7E552BC76492Ful, 0x852F54934DA55CC9ul, 0x8107FCCF064FCF56ul, 0x098954D51FFF6580ul,
            0x23B70EDB1955C4BFul, 0xC330DE426430F69Dul, 0x4715ED43E8A45C0Aul, 0xA8D7E4DAB780A08Dul,
            0x0572B974F03CE0BBul, 0xB57D2E985E1419C7ul, 0xE8D9ECBE2CF3D73Ful, 0x2FE4B17170E59750ul,
            0x11317BA87905E790ul, 0x7FBF21EC8A1F45ECul, 0x1725CABFCB045B00ul, 0x964E915CD5E2B207ul,
            0x3E2B8BCBF016D66Dul, 0xBE7444E39328A0ACul, 0xF85B2B4FBCDE44B7ul, 0x49353FEA39BA63B1ul,
            0x1DD01AAFCD53486Aul, 0x1FCA8A92FD719F85ul, 0xFC7C95D827357AFAul, 0x18A6A990C8B35EBDul,
            0xCCCB7005C6B9C28Dul, 0x3BDBB92C43B17F26ul, 0xAA70B5B4F89695A2ul, 0xE94C39A54A98307Ful,
            0xB7A0B174CFF6F36Eul, 0xD4DBA84729AF48ADul, 0x2E18BC1AD9704A68ul, 0x2DE0966DAF2F8B1Cul,
            0xB9C11D5B1E43A07Eul, 0x64972D68DEE33360ul, 0x94628D38D0C20584ul, 0xDBC0D2B6AB90A559ul,
            0xD2733C4335C6A72Ful, 0x7E75D99D94A70F4Dul, 0x6CED1983376FA72Bul, 0x97FCAACBF030BC24ul,
            0x7B77497B32503B12ul, 0x8547EDDFB81CCB94ul, 0x79999CDFF70902CBul, 0xCFFE1939438E9B24ul,
            0x829626E3892D95D7ul, 0x92FAE24291F2B3F1ul, 0x63E22C147B9C3403ul, 0xC678B6D860284A1Cul,
            0x5873888850659AE7ul, 0x0981DCD296A8736Dul, 0x9F65789A6509A440ul, 0x9FF38FED72E9052Ful,
            0xE479EE5B9930578Cul, 0xE7F28ECD2D49EECDul, 0x56C074A581EA17FEul, 0x5544F7D774B14AEFul,
            0x7B3F0195FC6F290Ful, 0x12153635B2C0CF57ul, 0x7F5126DBBA5E0CA7ul, 0x7A76956C3EAFB413ul,
            0x3D5774A11D31AB39ul, 0x8A1B083821F40CB4ul, 0x7B4A38E32537DF62ul, 0x950113646D1D6E03ul,
            0x4DA8979A0041E8A9ul, 0x3BC36E078F7515D7ul, 0x5D0A12F27AD310D1ul, 0x7F9D1A2E1EBE1327ul,
            0xDA3A361B1C5157B1ul, 0xDCDD7D20903D0C25ul, 0x36833336D068F707ul, 0xCE68341F79893389ul,
            0xAB9090168DD05F34ul, 0x43954B3252DC25E5ul, 0xB438C2B67F98E5E9ul, 0x10DCD78E3851A492ul,
            0xDBC27AB5447822BFul, 0x9B3CDB65F82CA382ul, 0xB67B7896167B4C84ul, 0xBFCED1B0048EAC50ul,
            0xA9119B60369FFEBDul, 0x1FFF7AC80904BF45ul, 0xAC12FB171817EEE7ul, 0xAF08DA9177DDA93Dul,
            0x1B0CAB936E65C744ul, 0xB559EB1D04E5E932ul, 0xC37B45B3F8D6F2BAul, 0xC3A9DC228CAAC9E9ul,
            0xF3B8B6675A6507FFul, 0x9FC477DE4ED681DAul, 0x67378D8ECCEF96CBul, 0x6DD856D94D259236ul,
            0xA319CE15B0B4DB31ul, 0x073973751F12DD5Eul, 0x8A8E849EB32781A5ul, 0xE1925C71285279F5ul,
            0x74C04BF1790C0EFEul, 0x4DDA48153C94938Aul, 0x9D266D6A1CC0542Cul, 0x7440FB816508C4FEul,
            0x13328503DF48229Ful, 0xD6BF7BAEE43CAC40ul, 0x4838D65F6EF6748Ful, 0x1E152328F3318DEAul,
            0x8F8419A348F296BFul, 0x72C8834A5957B511ul, 0xD7A023A73260B45Cul, 0x94EBC8ABCFB56DAEul,
            0x9FC10D0F989993E0ul, 0xDE68A2355B93CAE6ul, 0xA44CFE79AE538BBEul, 0x9D1D84FCCE371425ul,
            0x51D2B1AB2DDFB636ul, 0x2FD7E4B9E72CD38Cul, 0x65CA5B96B7552210ul, 0xDD69A0D8AB3B546Dul,
            0x604D51B25FBF70E2ul, 0x73AA8A564FB7AC9Eul, 0x1A8C1E992B941148ul, 0xAAC40A2703D9BEA0ul,
            0x764DBEAE7FA4F3A6ul, 0x1E99B96E70A9BE8Bul, 0x2C5E9DEB57EF4743ul, 0x3A938FEE32D29981ul,
            0x26E6DB8FFDF5ADFEul, 0x469356C504EC9F9Dul, 0xC8763C5B08D1908Cul, 0x3F6C6AF859D80055ul,
            0x7F7CC39420A3A545ul, 0x9BFB227EBDF4C5CEul, 0x89039D79D6FC5C5Cul, 0x8FE88B57305E2AB6ul,
            0xA09E8C8C35AB96DEul, 0xFA7E393983325753ul, 0xD6B6D0ECC617C699ul, 0xDFEA21EA9E7557E3ul,
            0xB67C1FA481680AF8ul, 0xCA1E3785A9E724E5ul, 0x1CFC8BED0D681639ul, 0xD18D8549D140CAEAul,
            0x4ED0FE7E9DC91335ul, 0xE4DBF0634473F5D2ul, 0x1761F93A44D5AEFEul, 0x53898E4C3910DA55ul,
            0x734DE8181F6EC39Aul, 0x2680B122BAA28D97ul, 0x298AF231C85BAFABul, 0x7983EED3740847D5ul,
            0x66C1A2A1A60CD889ul, 0x9E17E49642A3E4C1ul, 0xEDB454E7BADC0805ul, 0x50B704CAB602C329ul,
            0x4CC317FB9CDDD023ul, 0x66B4835D9EAFEA22ul, 0x219B97E26FFC81BDul, 0x261E4E4C0A333A9Dul,
            0x1FE2CCA76517DB90ul, 0xD7504DFA8816EDBBul, 0xB9571FA04DC089C8ul, 0x1DDC0325259B27DEul,
            0xCF3F4688801EB9AAul, 0xF4F5D05C10CAB243ul, 0x38B6525C21A42B0Eul, 0x36F60E2BA4FA6800ul,
            0xEB3593803173E0CEul, 0x9C4CD6257C5A3603ul, 0xAF0C317D32ADAA8Aul, 0x258E5A80C7204C4Bul,
            0x8B889D624D44885Dul, 0xF4D14597E660F855ul, 0xD4347F66EC8941C3ul, 0xE699ED85B0DFB40Dul,
            0x2472F6207C2D0484ul, 0xC2A1E7B5B459AEB5ul, 0xAB4F6451CC1D45ECul, 0x63767572AE3D6174ul,
            0xA59E0BD101731A28ul, 0x116D0016CB948F09ul, 0x2CF9C8CA052F6E9Ful, 0x0B090A7560A968E3ul,
            0xABEEDDB2DDE06FF1ul, 0x58EFC10B06A2068Dul, 0xC6E57A78FBD986E0ul, 0x2EAB8CA63CE802D7ul,
            0x14A195640116F336ul, 0x7C0828DD624EC390ul, 0xD74BBE77E6116AC7ul, 0x804456AF10F5FB53ul,
            0xEBE9EA2ADF4321C7ul, 0x03219A39EE587A30ul, 0x49787FEF17AF9924ul, 0xA1E9300CD8520548ul,
            0x5B45E522E4B1B4EFul, 0xB49C3B3995091A36ul, 0xD4490AD526F14431ul, 0x12A8F216AF9418C2ul,
            0x001F837CC7350524ul, 0x1877B51E57A764D5ul, 0xA2853B80F17F58EEul, 0x993E1DE72D36D310ul,
            0xB3598080CE64A656ul, 0x252F59CF0D9F04BBul, 0xD23C8E176D113600ul, 0x1BDA0492E7E4586Eul,
            0x21E0BD5026C619BFul, 0x3B097ADAF088F94Eul, 0x8D14DEDB30BE846Eul, 0xF95CFFA23AF5F6F4ul,
            0x3871700761B3F743ul, 0xCA672B91E9E4FA16ul, 0x64C8E531BFF53B55ul, 0x241260ED4AD1E87Dul,
            0x106C09B972D2E822ul, 0x7FBA195410E5CA30ul, 0x7884D9BC6CB569D8ul, 0x0647DFEDCD894A29ul,
            0x63573FF03E224774ul, 0x4FC8E9560F91B123ul, 0x1DB956E450275779ul, 0xB8D91274B9E9D4FBul,
            0xA2EBEE47E2FBFCE1ul, 0xD9F1F30CCD97FB09ul, 0xEFED53D75FD64E6Bul, 0x2E6D02C36017F67Ful,
            0xA9AA4D20DB084E9Bul, 0xB64BE8D8B25396C1ul, 0x70CB6AF7C2D5BCF0ul, 0x98F076A4F7A2322Eul,
            0xBF84470805E69B5Ful, 0x94C3251F06F90CF3ul, 0x3E003E616A6591E9ul, 0xB925A6CD0421AFF3ul,
            0x61BDD1307C66E300ul, 0xBF8D5108E27E0D48ul, 0x240AB57A8B888B20ul, 0xFC87614BAF287E07ul,
            0xEF02CDD06FFDB432ul, 0xA1082C0466DF6C0Aul, 0x8215E577001332C8ul, 0xD39BB9C3A48DB6CFul,
            0x2738259634305C14ul, 0x61CF4F94C97DF93Dul, 0x1B6BACA2AE4E125Bul, 0x758F450C88572E0Bul,
            0x959F587D507A8359ul, 0xB063E962E045F54Dul, 0x60E8ED72C0DFF5D1ul, 0x7B64978555326F9Ful,
            0xFD080D236DA814BAul, 0x8C90FD9B083F4558ul, 0x106F72FE81E2C590ul, 0x7976033A39F7D952ul,
            0xA4EC0132764CA04Bul, 0x733EA705FAE4FA77ul, 0xB4D8F77BC3E56167ul, 0x9E21F4F903B33FD9ul,
            0x9D765E419FB69F6Dul, 0xD30C088BA61EA5EFul, 0x5D94337FBFAF7F5Bul, 0x1A4E4822EB4D7A59ul,
            0x6FFE73E81B637FB3ul, 0xDDF957BC36D8B9CAul, 0x64D0E29EEA8838B3ul, 0x08DD9BDFD96B9F63ul,
            0x087E79E5A57D1D13ul, 0xE328E230E3E2B3FBul, 0x1C2559E30F0946BEul, 0x720BF5F26F4D2EAAul,
            0xB0774D261CC609DBul, 0x443F64EC5A371195ul, 0x4112CF68649A260Eul, 0xD813F2FAB7F5C5CAul,
            0x660D3257380841EEul, 0x59AC2C7873F910A3ul, 0xE846963877671A17ul, 0x93B633ABFA3469F8ul,
            0xC0C0F5A60EF4CDCFul, 0xCAF21ECD4377B28Cul, 0x57277707199B8175ul, 0x506C11B9D90E8B1Dul,
            0xD83CC2687A19255Ful, 0x4A29C6465A314CD1ul, 0xED2DF21216235097ul, 0xB5635C95FF7296E2ul,
            0x22AF003AB672E811ul, 0x52E762596BF68235ul, 0x9AEBA33AC6ECC6B0ul, 0x944F6DE09134DFB6ul,
            0x6C47BEC883A7DE39ul, 0x6AD047C430A12104ul, 0xA5B1CFDBA0AB4067ul, 0x7C45D833AFF07862ul,
            0x5092EF950A16DA0Bul, 0x9338E69C052B8E7Bul, 0x455A4B4CFE30E3F5ul, 0x6B02E63195AD0CF8ul,
            0x6B17B224BAD6BF27ul, 0xD1E0CCD25BB9C169ul, 0xDE0C89A556B9AE70ul, 0x50065E535A213CF6ul,
            0x9C1169FA2777B874ul, 0x78EDEFD694AF1EEDul, 0x6DC93D9526A50E68ul, 0xEE97F453F06791EDul,
            0x32AB0EDB696703D3ul, 0x3A6853C7E70757A7ul, 0x31865CED6120F37Dul, 0x67FEF95D92607890ul,
            0x1F2B1D1F15F6DC9Cul, 0xB69E38A8965C6B65ul, 0xAA9119FF184CCCF4ul, 0xF43C732873F24C13ul,
            0xFB4A3D794A9A80D2ul, 0x3550C2321FD6109Cul, 0x371F77E76BB8417Eul, 0x6BFA9AAE5EC05779ul,
            0xCD04F3FF001A4778ul, 0xE3273522064480CAul, 0x9F91508BFFCFC14Aul, 0x049A7F41061A9E60ul,
            0xFCB6BE43A9F2FE9Bul, 0x08DE8A1C7797DA9Bul, 0x8F9887E6078735A1ul, 0xB5B4071DBFC73A66ul,
            0x230E343DFBA08D33ul, 0x43ED7F5A0FAE657Dul, 0x3A88A0FBBCB05C63ul, 0x21874B8B4D2DBC4Ful,
            0x1BDEA12E35F6A8C9ul, 0x53C065C6C8E63528ul, 0xE34A1D250E7A8D6Bul, 0xD6B04D3B7651DD7Eul,
            0x5E90277E7CB39E2Dul, 0x2C046F22062DC67Dul, 0xB10BB459132D0A26ul, 0x3FA9DDFB67E2F199ul,
            0x0E09B88E1914F7AFul, 0x10E8B35AF3EEAB37ul, 0x9EEDECA8E272B933ul, 0xD4C718BC4AE8AE5Ful,
            0x81536D601170FC20ul, 0x91B534F885818A06ul, 0xEC8177F83F900978ul, 0x190E714FADA5156Eul,
            0xB592BF39B0364963ul, 0x89C350C893AE7DC1ul, 0xAC042E70F8B383F2ul, 0xB49B52E587A1EE60ul,
            0xFB152FE3FF26DA89ul, 0x3E666E6F69AE2C15ul, 0x3B544EBE544C19F9ul, 0xE805A1E290CF2456ul,
            0x24B33C9D7ED25117ul, 0xE74733427B72F0C1ul, 0x0A804D18B7097475ul, 0x57E3306D881EDB4Ful,
            0x4AE7D6A36EB5DBCBul, 0x2D8D5432157064C8ul, 0xD1E649DE1E7F268Bul, 0x8A328A1CEDFE552Cul,
            0x07A3AEC79624C7DAul, 0x84547DDC3E203C94ul, 0x990A98FD5071D263ul, 0x1A4FF12616EEFC89ul,
            0xF6F7FD1431714200ul, 0x30C05B1BA332F41Cul, 0x8D2636B81555A786ul, 0x46C9FEB55D120902ul,
            0xCCEC0A73B49C9921ul, 0x4E9D2827355FC492ul, 0x19EBB029435DCB0Ful, 0x4659D2B743848A2Cul,
            0x963EF2C96B33BE31ul, 0x74F85198B05A2E7Dul, 0x5A0F544DD2B1FB18ul, 0x03727073C2E134B1ul,
            0xC7F6AA2DE59AEA61ul, 0x352787BAA0D7C22Ful, 0x9853EAB63B5E0B35ul, 0xABBDCDD7ED5C0860ul,
            0xCF05DAF5AC8D77B0ul, 0x49CAD48CEBF4A71Eul, 0x7A4C10EC2158C4A6ul, 0xD9E92AA246BF719Eul,
            0x13AE978D09FE5557ul, 0x730499AF921549FFul, 0x4E4B705B92903BA4ul, 0xFF577222C14F0A3Aul,
            0x55B6344CF97AAFAEul, 0xB862225B055B6960ul, 0xCAC09AFBDDD2CDB4ul, 0xDAF8E9829FE96B5Ful,
            0xB5FDFC5D3132C498ul, 0x310CB380DB6F7503ul, 0xE87FBB46217A360Eul, 0x2102AE466EBB1148ul,
            0xF8549E1A3AA5E00Dul, 0x07A69AFDCC42261Aul, 0xC4C118BFE78FEAAEul, 0xF9F4892ED96BD438ul,
            0x1AF3DBE25D8F45DAul, 0xF5B4B0B0D2DEEEB4ul, 0x962ACEEFA82E1C84ul, 0x046E3ECAAF453CE9ul,
            0xF05D129681949A4Cul, 0x964781CE734B3C84ul, 0x9C2ED44081CE5FBDul, 0x522E23F3925E319Eul,
            0x177E00F9FC32F791ul, 0x2BC60A63A6F3B3F2ul, 0x222BBFAE61725606ul, 0x486289DDCC3D6780ul,
            0x7DC7785B8EFDFC80ul, 0x8AF38731C02BA980ul, 0x1FAB64EA29A2DDF7ul, 0xE4D9429322CD065Aul,
            0x9DA058C67844F20Cul, 0x24C0E332B70019B0ul, 0x233003B5A6CFE6ADul, 0xD586BD01C5C217F6ul,
            0x5E5637885F29BC2Bul, 0x7EBA726D8C94094Bul, 0x0A56A5F0BFE39272ul, 0xD79476A84EE20D06ul,
            0x9E4C1269BAA4BF37ul, 0x17EFEE45B0DEE640ul, 0x1D95B0A5FCF90BC6ul, 0x93CBE0B699C2585Dul,
            0x65FA4F227A2B6D79ul, 0xD5F9E858292504D5ul, 0xC2B5A03F71471A6Ful, 0x59300222B4561E00ul,
            0xCE2F8642CA0712DCul, 0x7CA9723FBB2E8988ul, 0x2785338347F2BA08ul, 0xC61BB3A141E50E8Cul,
            0x150F361DAB9DEC26ul, 0x9F6A419D382595F4ul, 0x64A53DC924FE7AC9ul, 0x142DE49FFF7A7C3Dul,
            0x0C335248857FA9E7ul, 0x0A9C32D5EAE45305ul, 0xE6C42178C4BBB92Eul, 0x71F1CE2490D20B07ul,
            0xF1BCC3D275AFE51Aul, 0xE728E8C83C334074ul, 0x96FBF83A12884624ul, 0x81A1549FD6573DA5ul,
            0x5FA7867CAF35E149ul, 0x56986E2EF3ED091Bul, 0x917F1DD5F8886C61ul, 0xD20D8C88C8FFE65Ful,
            0x31D71DCE64B2C310ul, 0xF165B587DF898190ul, 0xA57E6339DD2CF3A0ul, 0x1EF6E6DBB1961EC9ul,
            0x70CC73D90BC26E24ul, 0xE21A6B35DF0C3AD7ul, 0x003A93D8B2806962ul, 0x1C99DED33CB890A1ul,
            0xCF3145DE0ADD4289ul, 0xD0E4427A5514FB72ul, 0x77C621CC9FB3A483ul, 0x67A34DAC4356550Bul,
            0xF8D626AAAF278509ul
        };

        #endregion array

        static PolyglotHelpers()
        {
            Piece = new ulong[768];
            Castle = new ulong[4];
            EnPassant = new ulong[8];
            Turn = new ulong[1];
            Piece = Random64.Skip(0).Take(768).ToArray();
            Castle = Random64.Skip(768).Take(4).ToArray();
            EnPassant = Random64.Skip(772).Take(8).ToArray();
            Turn = Random64.Skip(780).Take(1).ToArray();
        }

        // <summary>Gets the hash of a board state, based on polyglot specs </summary>
        /// <param name="fen">FEN of the board state to hash</param>
        /// <returns>ulong representing the hash of the board</returns>
        public static ulong GetBoardStateHash(string fen)
        {
            return GetBoardStateHash(new FenTextToBoard().Translate(fen));
        }

        /// <summary>
        ///     Gets the hash of a board state, based on polyglot specs
        /// </summary>
        /// <param name="board">The ChessLib board to encode</param>
        /// <returns>ulong representing the hash of the board</returns>
        public static ulong GetBoardStateHash(Board board)
        {
            var pieceValue = GetPieceValues(board);
            var castleValue = GetCastleValue(board);
            var epValue = GetEnPassantValue(board);
            var turnValue = GetTurnValue(board);
            return pieceValue ^ castleValue ^ epValue ^ turnValue;
        }

        private static ulong GetTurnValue(Board board)
        {
            return board.ActivePlayer == Color.White ? Turn[0] : 0;
        }

        private static ulong GetEnPassantValue(Board board)
        {
            if (!board.EnPassantIndex.HasValue || !board.IsEnPassantCaptureAvailable())
            {
                return 0;
            }

            Debug.Assert(board.EnPassantIndex != null, "EnPassant square cannot be null after reaching this point");
            var file = board.EnPassantIndex.Value.GetFile();
            return EnPassant[file];
        }

        private static ulong GetCastleValue(Board board)
        {
            if (board.CastlingAvailability == CastlingAvailability.NoCastlingAvailable)
            {
                return 0;
            }

            var offsets = GetCastleKeyValues(board);
            var rv = offsets.Aggregate((acc, val) => acc ^ val);
            return rv;
        }

        private static ulong[] GetCastleKeyValues(Board board)
        {
            var lst = new List<ulong>();
            var ca = board.CastlingAvailability;
            if (ca.HasFlag(CastlingAvailability.WhiteKingside))
            {
                lst.Add(Castle[0]);
            }

            if (ca.HasFlag(CastlingAvailability.WhiteQueenside))
            {
                lst.Add(Castle[1]);
            }

            if (ca.HasFlag(CastlingAvailability.BlackKingside))
            {
                lst.Add(Castle[2]);
            }

            if (ca.HasFlag(CastlingAvailability.BlackQueenside))
            {
                lst.Add(Castle[3]);
            }

            return lst.ToArray();
        }

        private static ulong GetPieceValues(Board board)
        {
            ulong acc = 0;
            for (var color = 0; color < 2; color++)
            {
                for (var piece = 0; piece < 6; piece++)
                {
                    var piecePlacement = board.Occupancy[color][piece];
                    if (piecePlacement != 0)
                    {
                        foreach (var index in piecePlacement.GetSetBits())
                        {
                            var poc = new PieceOfColor {Color = (Color) color, Piece = (Piece) piece};
                            acc ^= GetPieceValue(poc, index);
                        }
                    }
                }
            }

            return acc;
        }


        private static ulong GetPieceValue(PieceOfColor poc, ushort boardIndexOfPiece)
        {
            Debug.Assert(boardIndexOfPiece < 64);
            var kindOfPiece = poc.GetPieceHashValue();
            var pieceOffset = 64 * kindOfPiece + boardIndexOfPiece;
            return Piece[pieceOffset];
        }
    }

    public class PolyglotMove
    {
        public ushort Move { get; }

        public ushort SourceIndex => (ushort) (((Move >> 9) & 255) * 8 + ((Move >> 6) & 255));
        public ushort DestIndex => (ushort) (((Move >> 6) & 255) * 8 + (Move & 255));

        public PolyglotMove(ushort polyglotMove)
        {
            Move = polyglotMove;
        }

        public PolyglotMove(IMove move)
        {
            Move = GetEncodedMove(move);
        }


        /// <summary>
        ///     Gets an encoded move that is standard to polyglot specifications
        /// </summary>
        /// <param name="move">The ushort-based move.</param>
        /// <returns>A polyglot encoded move</returns>
        public static ushort GetEncodedMove(IMove move)
        {
            ushort toFile;
            ushort toRank;
            ushort fromFile;
            ushort fromRank;
            if (move.MoveType != MoveType.Castle)
            {
                toFile = move.DestinationIndex.GetFile();
                toRank = move.DestinationIndex.GetRank();
                fromFile = move.SourceIndex.GetFile();
                fromRank = move.SourceIndex.GetRank();
            }
            else
            {
                GetCastlingMoveSrcAndDestValues(move, out toFile, out toRank, out fromRank, out fromFile);
            }

            var promotionPiece = GetEncodedPromotionPiece(move);
            var rv = (ushort) 0;
            rv |= (ushort) (promotionPiece << 12);
            rv |= (ushort) (fromRank << 9);
            rv |= (ushort) (fromFile << 6);
            rv |= (ushort) (toRank << 3);
            rv |= toFile;
            return rv;
        }

        private static void GetCastlingMoveSrcAndDestValues(IMove move, out ushort toFile, out ushort toRank,
            out ushort fromRank, out ushort fromFile)
        {
            fromFile = 4;
            if (move.DestinationIndex.GetRank() == 7)
            {
                toRank = fromRank = 7;
            }
            else
            {
                toRank = fromRank = 0;
            }

            if (move.MoveValue == MoveHelpers.WhiteCastleKingSide.MoveValue ||
                move.MoveValue == MoveHelpers.BlackCastleKingSide.MoveValue)
            {
                toFile = 7;
            }
            else
            {
                toFile = 0;
            }
        }

        private static ushort GetEncodedPromotionPiece(IMove move)
        {
            if (move.MoveType != MoveType.Promotion)
            {
                return 0;
            }

            var promotionPiece = (int) move.PromotionPiece;
            return (ushort) (promotionPiece + 1);
        }

        public override string ToString()
        {
            return $"{SourceIndex.IndexToSquareDisplay()}->{DestIndex.IndexToSquareDisplay()}";
        }
    }
}