using NUnit.Framework;

namespace ChessLib.Data.Tests.Magic
{
    [TestFixture]
    public class PieceAttackPatterns
    {
        #region MoveBoard TestCases
        /// <summary>
        /// Test Move Boards
        /// </summary>
        /// <param name="expected">ulong array in this order [King, Queen, Rook, Bishop, Knight, White Pawn, Black Pawn]</param>

        [TestCase(0, new ulong[] { 770ul, 0ul, 282578800148862ul, 9241421688590303744ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on a1")]
        [TestCase(1, new ulong[] { 1797ul, 0ul, 565157600297596ul, 36099303471056128ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on b1")]
        [TestCase(2, new ulong[] { 3594ul, 0ul, 1130315200595066ul, 141012904249856ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on c1")]
        [TestCase(3, new ulong[] { 7188ul, 0ul, 2260630401190006ul, 550848566272ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on d1")]
        [TestCase(4, new ulong[] { 14376ul, 0ul, 4521260802379886ul, 6480472064ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on e1")]
        [TestCase(5, new ulong[] { 28752ul, 0ul, 9042521604759646ul, 1108177604608ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on f1")]
        [TestCase(6, new ulong[] { 57504ul, 0ul, 18085043209519166ul, 283691315142656ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on g1")]
        [TestCase(7, new ulong[] { 49216ul, 0ul, 36170086419038334ul, 72624976668147712ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on h1")]
        [TestCase(8, new ulong[] { 197123ul, 0ul, 282578800180736ul, 4620710844295151618ul, 0ul/*KnightMoveMask is same as attack mask*/, 16842752ul, 1ul }, "Piece on a2")]
        [TestCase(9, new ulong[] { 460039ul, 0ul, 565157600328704ul, 9241421688590368773ul, 0ul/*KnightMoveMask is same as attack mask*/, 33685504ul, 2ul }, "Piece on b2")]
        [TestCase(10, new ulong[] { 920078ul, 0ul, 1130315200625152ul, 36099303487963146ul, 0ul/*KnightMoveMask is same as attack mask*/, 67371008ul, 4ul }, "Piece on c2")]
        [TestCase(11, new ulong[] { 1840156ul, 0ul, 2260630401218048ul, 141017232965652ul, 0ul/*KnightMoveMask is same as attack mask*/, 134742016ul, 8ul }, "Piece on d2")]
        [TestCase(12, new ulong[] { 3680312ul, 0ul, 4521260802403840ul, 1659000848424ul, 0ul/*KnightMoveMask is same as attack mask*/, 269484032ul, 16ul }, "Piece on e2")]
        [TestCase(13, new ulong[] { 7360624ul, 0ul, 9042521604775424ul, 283693466779728ul, 0ul/*KnightMoveMask is same as attack mask*/, 538968064ul, 32ul }, "Piece on f2")]
        [TestCase(14, new ulong[] { 14721248ul, 0ul, 18085043209518592ul, 72624976676520096ul, 0ul/*KnightMoveMask is same as attack mask*/, 1077936128ul, 64ul }, "Piece on g2")]
        [TestCase(15, new ulong[] { 12599488ul, 0ul, 36170086419037696ul, 145249953336262720ul, 0ul/*KnightMoveMask is same as attack mask*/, 2155872256ul, 128ul }, "Piece on h2")]
        [TestCase(16, new ulong[] { 50463488ul, 0ul, 282578808340736ul, 2310355422147510788ul, 0ul/*KnightMoveMask is same as attack mask*/, 16777216ul, 256ul }, "Piece on a3")]
        [TestCase(17, new ulong[] { 117769984ul, 0ul, 565157608292864ul, 4620710844311799048ul, 0ul/*KnightMoveMask is same as attack mask*/, 33554432ul, 512ul }, "Piece on b3")]
        [TestCase(18, new ulong[] { 235539968ul, 0ul, 1130315208328192ul, 9241421692918565393ul, 0ul/*KnightMoveMask is same as attack mask*/, 67108864ul, 1024ul }, "Piece on c3")]
        [TestCase(19, new ulong[] { 471079936ul, 0ul, 2260630408398848ul, 36100411639206946ul, 0ul/*KnightMoveMask is same as attack mask*/, 134217728ul, 2048ul }, "Piece on d3")]
        [TestCase(20, new ulong[] { 942159872ul, 0ul, 4521260808540160ul, 424704217196612ul, 0ul/*KnightMoveMask is same as attack mask*/, 268435456ul, 4096ul }, "Piece on e3")]
        [TestCase(21, new ulong[] { 1884319744ul, 0ul, 9042521608822784ul, 72625527495610504ul, 0ul/*KnightMoveMask is same as attack mask*/, 536870912ul, 8192ul }, "Piece on f3")]
        [TestCase(22, new ulong[] { 3768639488ul, 0ul, 18085043209388032ul, 145249955479592976ul, 0ul/*KnightMoveMask is same as attack mask*/, 1073741824ul, 16384ul }, "Piece on g3")]
        [TestCase(23, new ulong[] { 3225468928ul, 0ul, 36170086418907136ul, 290499906664153120ul, 0ul/*KnightMoveMask is same as attack mask*/, 2147483648ul, 32768ul }, "Piece on h3")]
        [TestCase(24, new ulong[] { 12918652928ul, 0ul, 282580897300736ul, 1155177711057110024ul, 0ul/*KnightMoveMask is same as attack mask*/, 4294967296ul, 65536ul }, "Piece on a4")]
        [TestCase(25, new ulong[] { 30149115904ul, 0ul, 565159647117824ul, 2310355426409252880ul, 0ul/*KnightMoveMask is same as attack mask*/, 8589934592ul, 131072ul }, "Piece on b4")]
        [TestCase(26, new ulong[] { 60298231808ul, 0ul, 1130317180306432ul, 4620711952330133792ul, 0ul/*KnightMoveMask is same as attack mask*/, 17179869184ul, 262144ul }, "Piece on c4")]
        [TestCase(27, new ulong[] { 120596463616ul, 0ul, 2260632246683648ul, 9241705379636978241ul, 0ul/*KnightMoveMask is same as attack mask*/, 34359738368ul, 524288ul }, "Piece on d4")]
        [TestCase(28, new ulong[] { 241192927232ul, 0ul, 4521262379438080ul, 108724279602332802ul, 0ul/*KnightMoveMask is same as attack mask*/, 68719476736ul, 1048576ul }, "Piece on e4")]
        [TestCase(29, new ulong[] { 482385854464ul, 0ul, 9042522644946944ul, 145390965166737412ul, 0ul/*KnightMoveMask is same as attack mask*/, 137438953472ul, 2097152ul }, "Piece on f4")]
        [TestCase(30, new ulong[] { 964771708928ul, 0ul, 18085043175964672ul, 290500455356698632ul, 0ul/*KnightMoveMask is same as attack mask*/, 274877906944ul, 4194304ul }, "Piece on g4")]
        [TestCase(31, new ulong[] { 825720045568ul, 0ul, 36170086385483776ul, 580999811184992272ul, 0ul/*KnightMoveMask is same as attack mask*/, 549755813888ul, 8388608ul }, "Piece on h4")]
        [TestCase(32, new ulong[] { 3307175149568ul, 0ul, 283115671060736ul, 577588851267340304ul, 0ul/*KnightMoveMask is same as attack mask*/, 1099511627776ul, 16777216ul }, "Piece on a5")]
        [TestCase(33, new ulong[] { 7718173671424ul, 0ul, 565681586307584ul, 1155178802063085600ul, 0ul/*KnightMoveMask is same as attack mask*/, 2199023255552ul, 33554432ul }, "Piece on b5")]
        [TestCase(34, new ulong[] { 15436347342848ul, 0ul, 1130822006735872ul, 2310639079102947392ul, 0ul/*KnightMoveMask is same as attack mask*/, 4398046511104ul, 67108864ul }, "Piece on c5")]
        [TestCase(35, new ulong[] { 30872694685696ul, 0ul, 2261102847592448ul, 4693335752243822976ul, 0ul/*KnightMoveMask is same as attack mask*/, 8796093022208ul, 134217728ul }, "Piece on d5")]
        [TestCase(36, new ulong[] { 61745389371392ul, 0ul, 4521664529305600ul, 9386671504487645697ul, 0ul/*KnightMoveMask is same as attack mask*/, 17592186044416ul, 268435456ul }, "Piece on e5")]
        [TestCase(37, new ulong[] { 123490778742784ul, 0ul, 9042787892731904ul, 326598935265674242ul, 0ul/*KnightMoveMask is same as attack mask*/, 35184372088832ul, 536870912ul }, "Piece on f5")]
        [TestCase(38, new ulong[] { 246981557485568ul, 0ul, 18085034619584512ul, 581140276476643332ul, 0ul/*KnightMoveMask is same as attack mask*/, 70368744177664ul, 1073741824ul }, "Piece on g5")]
        [TestCase(39, new ulong[] { 211384331665408ul, 0ul, 36170077829103616ul, 1161999073681608712ul, 0ul/*KnightMoveMask is same as attack mask*/, 140737488355328ul, 2147483648ul }, "Piece on h5")]
        [TestCase(40, new ulong[] { 846636838289408ul, 0ul, 420017753620736ul, 288793334762704928ul, 0ul/*KnightMoveMask is same as attack mask*/, 281474976710656ul, 4294967296ul }, "Piece on a6")]
        [TestCase(41, new ulong[] { 1975852459884544ul, 0ul, 699298018886144ul, 577868148797087808ul, 0ul/*KnightMoveMask is same as attack mask*/, 562949953421312ul, 8589934592ul }, "Piece on b6")]
        [TestCase(42, new ulong[] { 3951704919769088ul, 0ul, 1260057572672512ul, 1227793891648880768ul, 0ul/*KnightMoveMask is same as attack mask*/, 1125899906842624ul, 17179869184ul }, "Piece on c6")]
        [TestCase(43, new ulong[] { 7903409839538176ul, 0ul, 2381576680245248ul, 2455587783297826816ul, 0ul/*KnightMoveMask is same as attack mask*/, 2251799813685248ul, 34359738368ul }, "Piece on d6")]
        [TestCase(44, new ulong[] { 15806819679076352ul, 0ul, 4624614895390720ul, 4911175566595588352ul, 0ul/*KnightMoveMask is same as attack mask*/, 4503599627370496ul, 68719476736ul }, "Piece on e6")]
        [TestCase(45, new ulong[] { 31613639358152704ul, 0ul, 9110691325681664ul, 9822351133174399489ul, 0ul/*KnightMoveMask is same as attack mask*/, 9007199254740992ul, 137438953472ul }, "Piece on f6")]
        [TestCase(46, new ulong[] { 63227278716305408ul, 0ul, 18082844186263552ul, 1197958188344280066ul, 0ul/*KnightMoveMask is same as attack mask*/, 18014398509481984ul, 274877906944ul }, "Piece on g6")]
        [TestCase(47, new ulong[] { 54114388906344448ul, 0ul, 36167887395782656ul, 2323857683139004420ul, 0ul/*KnightMoveMask is same as attack mask*/, 36028797018963968ul, 549755813888ul }, "Piece on h6")]
        [TestCase(48, new ulong[] { 216739030602088448ul, 0ul, 35466950888980736ul, 144117404414255168ul, 0ul/*KnightMoveMask is same as attack mask*/, 72057594037927936ul, 1103806595072ul }, "Piece on a7")]
        [TestCase(49, new ulong[] { 505818229730443264ul, 0ul, 34905104758997504ul, 360293502378066048ul, 0ul/*KnightMoveMask is same as attack mask*/, 144115188075855872ul, 2207613190144ul }, "Piece on b7")]
        [TestCase(50, new ulong[] { 1011636459460886528ul, 0ul, 34344362452452352ul, 720587009051099136ul, 0ul/*KnightMoveMask is same as attack mask*/, 288230376151711744ul, 4415226380288ul }, "Piece on c7")]
        [TestCase(51, new ulong[] { 2023272918921773056ul, 0ul, 33222877839362048ul, 1441174018118909952ul, 0ul/*KnightMoveMask is same as attack mask*/, 576460752303423488ul, 8830452760576ul }, "Piece on d7")]
        [TestCase(52, new ulong[] { 4046545837843546112ul, 0ul, 30979908613181440ul, 2882348036221108224ul, 0ul/*KnightMoveMask is same as attack mask*/, 1152921504606846976ul, 17660905521152ul }, "Piece on e7")]
        [TestCase(53, new ulong[] { 8093091675687092224ul, 0ul, 26493970160820224ul, 5764696068147249408ul, 0ul/*KnightMoveMask is same as attack mask*/, 2305843009213693952ul, 35321811042304ul }, "Piece on f7")]
        [TestCase(54, new ulong[] { 16186183351374184448ul, 0ul, 17522093256097792ul, 11529391036782871041ul, 0ul/*KnightMoveMask is same as attack mask*/, 4611686018427387904ul, 70643622084608ul }, "Piece on g7")]
        [TestCase(55, new ulong[] { 13853283560024178688ul, 0ul, 35607136465616896ul, 4611756524879479810ul, 0ul/*KnightMoveMask is same as attack mask*/, 9223372036854775808ul, 141287244169216ul }, "Piece on h7")]
        [TestCase(56, new ulong[] { 144959613005987840ul, 0ul, 9079539427579068672ul, 567382630219904ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on a8")]
        [TestCase(57, new ulong[] { 362258295026614272ul, 0ul, 8935706818303361536ul, 1416240237150208ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on b8")]
        [TestCase(58, new ulong[] { 724516590053228544ul, 0ul, 8792156787827803136ul, 2833579985862656ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on c8")]
        [TestCase(59, new ulong[] { 1449033180106457088ul, 0ul, 8505056726876686336ul, 5667164249915392ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on d8")]
        [TestCase(60, new ulong[] { 2898066360212914176ul, 0ul, 7930856604974452736ul, 11334324221640704ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on e8")]
        [TestCase(61, new ulong[] { 5796132720425828352ul, 0ul, 6782456361169985536ul, 22667548931719168ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on f8")]
        [TestCase(62, new ulong[] { 11592265440851656704ul, 0ul, 4485655873561051136ul, 45053622886727936ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on g8")]
        [TestCase(63, new ulong[] { 4665729213955833856ul, 0ul, 9115426935197958144ul, 18049651735527937ul, 0ul/*KnightMoveMask is same as attack mask*/, 0ul, 0ul }, "Piece on h8")]
        #endregion
        public void TestMoveMasks(int index, ulong[] expected, string error)
        {
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.KingMoveMask[index], Is.EqualTo(expected[0]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.QueenMoveMask[index], Is.EqualTo(expected[1]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.RookMoveMask[index], Is.EqualTo(expected[2]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.BishopMoveMask[index], Is.EqualTo(expected[3]), error);
            //Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.KnightAttackMask[index], Is.EqualTo(expected[4]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.PawnMoveMask[0][index], Is.EqualTo(expected[5]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.PawnMoveMask[1][index], Is.EqualTo(expected[6]), error);

        }
        #region AttackBoard TestCases 
        /// <summary>
        /// Test Attack Boards
        /// </summary>
        /// <param name="expected">ulong array in this order [King, Queen, Rook, Bishop, Knight, White Pawn, Black Pawn]</param>

        [TestCase(0, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 282578800148862ul, 18049651735527936ul, 132096ul, 512ul, 0ul }, "Piece on a1")]
        [TestCase(1, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 565157600297596ul, 70506452091904ul, 329728ul, 1280ul, 0ul }, "Piece on b1")]
        [TestCase(2, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 1130315200595066ul, 275415828992ul, 659712ul, 2560ul, 0ul }, "Piece on c1")]
        [TestCase(3, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 2260630401190006ul, 1075975168ul, 1319424ul, 5120ul, 0ul }, "Piece on d1")]
        [TestCase(4, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4521260802379886ul, 38021120ul, 2638848ul, 10240ul, 0ul }, "Piece on e1")]
        [TestCase(5, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9042521604759646ul, 8657588224ul, 5277696ul, 20480ul, 0ul }, "Piece on f1")]
        [TestCase(6, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 18085043209519166ul, 2216338399232ul, 10489856ul, 40960ul, 0ul }, "Piece on g1")]
        [TestCase(7, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 36170086419038334ul, 567382630219776ul, 4202496ul, 16384ul, 0ul }, "Piece on h1")]
        [TestCase(8, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 282578800180736ul, 9024825867763712ul, 33816580ul, 131072ul, 2ul }, "Piece on a2")]
        [TestCase(9, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 565157600328704ul, 18049651735527424ul, 84410376ul, 327680ul, 5ul }, "Piece on b2")]
        [TestCase(10, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 1130315200625152ul, 70506452221952ul, 168886289ul, 655360ul, 10ul }, "Piece on c2")]
        [TestCase(11, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 2260630401218048ul, 275449643008ul, 337772578ul, 1310720ul, 20ul }, "Piece on d2")]
        [TestCase(12, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4521260802403840ul, 9733406720ul, 675545156ul, 2621440ul, 40ul }, "Piece on e2")]
        [TestCase(13, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9042521604775424ul, 2216342585344ul, 1351090312ul, 5242880ul, 80ul }, "Piece on f2")]
        [TestCase(14, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 18085043209518592ul, 567382630203392ul, 2685403152ul, 10485760ul, 160ul }, "Piece on g2")]
        [TestCase(15, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 36170086419037696ul, 1134765260406784ul, 1075839008ul, 4194304ul, 64ul }, "Piece on h2")]
        [TestCase(16, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 282578808340736ul, 4512412933816832ul, 8657044482ul, 33554432ul, 512ul }, "Piece on a3")]
        [TestCase(17, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 565157608292864ul, 9024825867633664ul, 21609056261ul, 83886080ul, 1280ul }, "Piece on b3")]
        [TestCase(18, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 1130315208328192ul, 18049651768822272ul, 43234889994ul, 167772160ul, 2560ul }, "Piece on c3")]
        [TestCase(19, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 2260630408398848ul, 70515108615168ul, 86469779988ul, 335544320ul, 5120ul }, "Piece on d3")]
        [TestCase(20, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4521260808540160ul, 2491752130560ul, 172939559976ul, 671088640ul, 10240ul }, "Piece on e3")]
        [TestCase(21, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9042521608822784ul, 567383701868544ul, 345879119952ul, 1342177280ul, 20480ul }, "Piece on f3")]
        [TestCase(22, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 18085043209388032ul, 1134765256220672ul, 687463207072ul, 2684354560ul, 40960ul }, "Piece on g3")]
        [TestCase(23, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 36170086418907136ul, 2269530512441344ul, 275414786112ul, 1073741824ul, 16384ul }, "Piece on h3")]
        [TestCase(24, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 282580897300736ul, 2256206450263040ul, 2216203387392ul, 8589934592ul, 131072ul }, "Piece on a4")]
        [TestCase(25, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 565159647117824ul, 4512412900526080ul, 5531918402816ul, 21474836480ul, 327680ul }, "Piece on b4")]
        [TestCase(26, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 1130317180306432ul, 9024834391117824ul, 11068131838464ul, 42949672960ul, 655360ul }, "Piece on c4")]
        [TestCase(27, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 2260632246683648ul, 18051867805491712ul, 22136263676928ul, 85899345920ul, 1310720ul }, "Piece on d4")]
        [TestCase(28, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4521262379438080ul, 637888545440768ul, 44272527353856ul, 171798691840ul, 2621440ul }, "Piece on e4")]
        [TestCase(29, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9042522644946944ul, 1135039602493440ul, 88545054707712ul, 343597383680ul, 5242880ul }, "Piece on f4")]
        [TestCase(30, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 18085043175964672ul, 2269529440784384ul, 175990581010432ul, 687194767360ul, 10485760ul }, "Piece on g4")]
        [TestCase(31, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 36170086385483776ul, 4539058881568768ul, 70506185244672ul, 274877906944ul, 4194304ul }, "Piece on h4")]
        [TestCase(32, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 283115671060736ul, 1128098963916800ul, 567348067172352ul, 2199023255552ul, 33554432ul }, "Piece on a5")]
        [TestCase(33, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 565681586307584ul, 2256197927833600ul, 1416171111120896ul, 5497558138880ul, 83886080ul }, "Piece on b5")]
        [TestCase(34, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 1130822006735872ul, 4514594912477184ul, 2833441750646784ul, 10995116277760ul, 167772160ul }, "Piece on c5")]
        [TestCase(35, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 2261102847592448ul, 9592139778506752ul, 5666883501293568ul, 21990232555520ul, 335544320ul }, "Piece on d5")]
        [TestCase(36, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4521664529305600ul, 19184279556981248ul, 11333767002587136ul, 43980465111040ul, 671088640ul }, "Piece on e5")]
        [TestCase(37, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9042787892731904ul, 2339762086609920ul, 22667534005174272ul, 87960930222080ul, 1342177280ul }, "Piece on f5")]
        [TestCase(38, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 18085034619584512ul, 4538784537380864ul, 45053588738670592ul, 175921860444160ul, 2684354560ul }, "Piece on g5")]
        [TestCase(39, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 36170077829103616ul, 9077569074761728ul, 18049583422636032ul, 70368744177664ul, 1073741824ul }, "Piece on h5")]
        [TestCase(40, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 420017753620736ul, 562958610993152ul, 145241105196122112ul, 562949953421312ul, 8589934592ul }, "Piece on a6")]
        [TestCase(41, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 699298018886144ul, 1125917221986304ul, 362539804446949376ul, 1407374883553280ul, 21474836480ul }, "Piece on b6")]
        [TestCase(42, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 1260057572672512ul, 2814792987328512ul, 725361088165576704ul, 2814749767106560ul, 42949672960ul }, "Piece on c6")]
        [TestCase(43, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 2381576680245248ul, 5629586008178688ul, 1450722176331153408ul, 5629499534213120ul, 85899345920ul }, "Piece on d6")]
        [TestCase(44, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4624614895390720ul, 11259172008099840ul, 2901444352662306816ul, 11258999068426240ul, 171798691840ul }, "Piece on e6")]
        [TestCase(45, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9110691325681664ul, 22518341868716544ul, 5802888705324613632ul, 22517998136852480ul, 343597383680ul }, "Piece on f6")]
        [TestCase(46, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 18082844186263552ul, 9007336962655232ul, 11533718717099671552ul, 45035996273704960ul, 687194767360ul }, "Piece on g6")]
        [TestCase(47, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 36167887395782656ul, 18014673925310464ul, 4620693356194824192ul, 18014398509481984ul, 274877906944ul }, "Piece on h6")]
        [TestCase(48, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 35466950888980736ul, 2216338399232ul, 288234782788157440ul, 144115188075855872ul, 2199023255552ul }, "Piece on a7")]
        [TestCase(49, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 34905104758997504ul, 4432676798464ul, 576469569871282176ul, 360287970189639680ul, 5497558138880ul }, "Piece on b7")]
        [TestCase(50, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 34344362452452352ul, 11064376819712ul, 1224997833292120064ul, 720575940379279360ul, 10995116277760ul }, "Piece on c7")]
        [TestCase(51, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 33222877839362048ul, 22137335185408ul, 2449995666584240128ul, 1441151880758558720ul, 21990232555520ul }, "Piece on d7")]
        [TestCase(52, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 30979908613181440ul, 44272556441600ul, 4899991333168480256ul, 2882303761517117440ul, 43980465111040ul }, "Piece on e7")]
        [TestCase(53, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 26493970160820224ul, 87995357200384ul, 9799982666336960512ul, 5764607523034234880ul, 87960930222080ul }, "Piece on f7")]
        [TestCase(54, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 17522093256097792ul, 35253226045952ul, 1152939783987658752ul, 11529215046068469760ul, 175921860444160ul }, "Piece on g7")]
        [TestCase(55, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 35607136465616896ul, 70506452091904ul, 2305878468463689728ul, 4611686018427387904ul, 70368744177664ul }, "Piece on h7")]
        [TestCase(56, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9079539427579068672ul, 567382630219776ul, 1128098930098176ul, 0ul, 0x2000000000000ul }, "Piece on a8")]
        [TestCase(57, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 8935706818303361536ul, 1134765260406784ul, 2257297371824128ul, 0ul, 0x5000000000000ul }, "Piece on b8")]
        [TestCase(58, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 8792156787827803136ul, 2832480465846272ul, 4796069720358912ul, 0ul, 0xa000000000000ul }, "Piece on c8")]
        [TestCase(59, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 8505056726876686336ul, 5667157807464448ul, 9592139440717824ul, 0ul, 0x14000000000000ul }, "Piece on d8")]
        [TestCase(60, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 7930856604974452736ul, 11333774449049600ul, 19184278881435648ul, 0ul, 0x28000000000000ul }, "Piece on e8")]
        [TestCase(61, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 6782456361169985536ul, 22526811443298304ul, 38368557762871296ul, 0ul, 0x50000000000000ul }, "Piece on f8")]
        [TestCase(62, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 4485655873561051136ul, 9024825867763712ul, 4679521487814656ul, 0ul, 0xa0000000000000ul }, "Piece on g8")]
        [TestCase(63, new ulong[] { 0ul/*King moves are same as attacks*/, 0ul, 9115426935197958144ul, 18049651735527936ul, 9077567998918656ul, 0ul, 0x40000000000000ul }, "Piece on h8")]
        #endregion
        public void TestAttackMasks(int index, ulong[] expected, string error)
        {
            //Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.KingAttackMask[index], Is.EqualTo(expected[0]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.QueenAttackMask[index], Is.EqualTo(expected[1]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.RookAttackMask[index], Is.EqualTo(expected[2]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.BishopAttackMask[index], Is.EqualTo(expected[3]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.KnightAttackMask[index], Is.EqualTo(expected[4]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.PawnAttackMask[0][index], Is.EqualTo(expected[5]), error);
            Assert.That(Data.Magic.Init.PieceAttackPatterns.Instance.PawnAttackMask[1][index], Is.EqualTo(expected[6]), error);

        }
    }
}
