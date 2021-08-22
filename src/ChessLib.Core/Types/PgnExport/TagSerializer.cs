using System.Collections.Generic;
using System.Linq;

namespace ChessLib.Core.Types.PgnExport
{
    internal class TagSerializer
    {
        private const string TagFormatSpecifier = "[{0}{1}\"{2}\"]";

        public TagSerializer() : this(new PGNFormatterOptions())
        {
        }

        public TagSerializer(PGNFormatterOptions options)
        {
            _options = options;
        }

        private readonly PGNFormatterOptions _options;

        private string FormatTag(KeyValuePair<string, string> tag)
        {
            return string.Format(TagFormatSpecifier, tag.Key, _options.KeyValueWhitespaceSeparator, tag.Value);
        }

        /// <summary>
        /// Writes each tag and the tag termination character
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="tagsToSerialize"></param>
        public void Serialize(PgnWriter writer, Tags tagsToSerialize)
        {
            Serialize(writer, Convert(tagsToSerialize.RequiredTags));
            if (!_options.KeepOnlyRequiredTags)
            {
                Serialize(writer, Convert(tagsToSerialize.SupplementalTags));
            }
        }

        public IEnumerable<string> Convert(IEnumerable<KeyValuePair<string, string>> tags)
        {
            foreach (var tag in tags)
            {
                yield return FormatTag(tag);
            }
        }

        private void Serialize(PgnWriter writer, IEnumerable<string> tagPairs)
        {
            var arrTagPairs = tagPairs as string[] ?? tagPairs.ToArray();
            if (!arrTagPairs.Any())
            {
                return;

            }
            foreach (var tag in arrTagPairs)
            {
                writer.WriteTag(tag);
            }

            writer.WriteNewLine();
        }
    }
}