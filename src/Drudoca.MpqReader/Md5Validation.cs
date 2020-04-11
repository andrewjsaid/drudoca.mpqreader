using System.Diagnostics;
using System.Security.Cryptography;

namespace Drudoca.MpqReader
{
    internal class Md5Validation : IMd5Validation
    {

        public bool Check(byte[] data, int offset, int length, byte[] hash)
        {
            const int Md5DigestSize = 16; // 16 byes
            Debug.Assert(hash.Length == Md5DigestSize);

            byte[] computed;

            // Todo verify md5 of md5MpqHeader
            using (var md5 = MD5.Create())
            {
                computed = md5.ComputeHash(data, offset, length);
            }

            Debug.Assert(computed.Length == Md5DigestSize);

            // Compare 2 longs rather than 16 bytes
            unsafe
            {
                fixed (byte* l = hash)
                fixed (byte* r = computed)
                {
                    var ll = (long*)l;
                    var rl = (long*)r;

                    if ((*ll++) != (*rl++))
                    {
                        return false;
                    }

                    if ((*ll++) != (*rl++))
                    {
                        return false;
                    }

                }
            }

            return true;
        }

    }
}
