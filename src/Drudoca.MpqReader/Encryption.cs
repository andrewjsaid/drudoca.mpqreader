using System;

namespace Drudoca.MpqReader
{
    internal class Encryption : IEncryption
    {

        private readonly uint[] _cryptTable;

        public Encryption()
        {
            _cryptTable = CreateCryptTable();
        }

        private uint[] CreateCryptTable()
        {
            unchecked
            {
                var result = new uint[0x500];

                var seed = 0x00100001u;

                for (var index1 = 0ul; index1 < 0x100; index1++)
                {
                    var index2 = index1;
                    for (var i = 0; i < 5; i++)
                    {
                        uint temp1;
                        uint temp2;

                        seed = (seed * 125 + 3) % 0x2AAAAB;
                        temp1 = (seed & 0xFFFFu) << 0x10;

                        seed = (seed * 125 + 3) % 0x2AAAAB;
                        temp2 = (seed & 0xFFFFu);

                        result[index2] = (temp1 | temp2);
                        index2 += 0x100;
                    }
                }

                return result;
            }
        }

        public uint Hash(string text, int hashType)
        {
            unchecked
            {
                uint seed1 = 0x7FED7FED;
                uint seed2 = 0xEEEEEEEE;

                for (int i = 0; i < text.Length; i++)
                {
                    var c = char.ToUpper(text[i]);
                    if(c == '/')
                    {
                        c = '\\';
                    }
                    seed1 = _cryptTable[(hashType << 8) + c] ^ (seed1 + seed2);
                    seed2 = c + seed1 + seed2 + (seed2 << 5) + 3;
                }

                return seed1;
            }
        }

        public void DecryptInPlace(byte[] data, int offset, int length, uint key)
        {
            unchecked
            {
                uint seed = 0xEEEEEEEE;

                var end = offset + length;
                for (int i = offset; i < end; i += 4)
                {
                    seed += _cryptTable[0x400 + (key & 0xFF)];

                    var ch = BitConverter.ToUInt32(data, i) ^ (key + seed);

                    key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
                    seed = ch + seed + (seed << 5) + 3;

                    data[i + 0] = (byte)((ch >> 0) & 0xFF);
                    data[i + 1] = (byte)((ch >> 8) & 0xFF);
                    data[i + 2] = (byte)((ch >> 16) & 0xFF);
                    data[i + 3] = (byte)((ch >> 24) & 0xFF);
                }
            }
        }
    }
}
