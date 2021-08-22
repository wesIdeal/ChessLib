namespace ChessLib.Core.Types.PgnExport
{
    internal class GameSerializer : PgnSerializer
    {
        public GameSerializer(PGNFormatterOptions options) : base(options)
        {
            tagSerializer = new TagSerializer(Options);
            _moveSectionSerializer = new MoveSectionSerializer(Options);
        }

        private readonly MoveSectionSerializer _moveSectionSerializer;

        private readonly TagSerializer tagSerializer;

        public string SerializeToString(Game game)
        {
            using var writer = new PgnWriter(Options);
            tagSerializer.Serialize(writer, game.Tags);
            _moveSectionSerializer.Serialize(game.InitialNode.Node, writer);
            writer.WriteResult(game.Result);

            return writer.ToString();
        }
    }
}