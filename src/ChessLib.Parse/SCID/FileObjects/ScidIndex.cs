using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChessLib.Parse.SCID
{
    using GameNumberT = UInt32;
    class ScidIndex
    {
        public ScidIndex()
        {
            FileHeader = new Header();
        }
        public Header FileHeader { get; set; }
        public class Header
        {
            const uint DescriptionLength = 107;
            const uint CustomFlagMaxCount = 6;
            const uint CustomFlagLength = 8;
            public Header()
            {
                Magic = "Scid.si";
                CustomFlagDescriptions = new string[CustomFlagMaxCount];
            }

            public string Magic { get; set; } //8 bytes with terminating '00'
            public ushort Version { get; set; }
            public uint BaseType { get; set; }
            public GameNumberT NumberOfGames { get; set; }
            public GameNumberT AutoLoadGameNumber { get; set; }
            public string Description { get; set; }
            public string[] CustomFlagDescriptions { get; set; }

            public void ReadHeader(FileStream fStream)
            {
                fStream.Position = 0;
                using (var br = new BinaryReader(fStream))
                {
                    Magic = new string(br.ReadChars(8));
                    Version = br.ReadUInt16();
                    BaseType = br.ReadUInt32();
                    NumberOfGames = Convert.ToUInt32(br.ReadBytes(3));
                    AutoLoadGameNumber = Convert.ToUInt32(br.ReadBytes(3));
                    Description = new string(br.ReadChars((int)DescriptionLength + 1));
                    for (int i = 0; i < CustomFlagMaxCount; i++)
                    {
                        CustomFlagDescriptions[i] = new string(br.ReadChars((int)CustomFlagLength));
                    }
                    fStream.Position = 182;
                }
            }
        }

        public void ReadIndexFromFile(FileStream fStream)
        {
            FileHeader.ReadHeader(fStream);
        }
    }
}
