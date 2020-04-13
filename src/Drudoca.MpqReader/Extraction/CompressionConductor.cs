using System;
using System.Buffers;
using System.Collections.Generic;

namespace Drudoca.MpqReader.Extraction
{
    internal class CompressionConductor
    {

        public long Decompress(
            Memory<byte> source,
            Memory<byte> target,
            MpqCompressionType type)
        {
            var compressions = GetCompressions(type);

            if (compressions.Length == 1)
            {
                var length = compressions[0].Decompress(source, target);
                return length;
            }
            else
            {
                // We need a temporary buffer
                using (var buffer = MemoryPool<byte>.Shared.Rent(target.Length))
                {
                    var prevTarget = source;
                    int prevDataLength = source.Length;

                    // We swap between buffers and always end on "target"
                    var isBufferTarget = compressions.Length % 2 == 0;

                    Memory<byte> currentTarget;

                    foreach (var c in compressions)
                    {
                        currentTarget = isBufferTarget
                            ? buffer.Memory.Slice(0, prevDataLength)
                            : target.Slice(0, prevDataLength);

                        prevDataLength = c.Decompress(prevTarget, currentTarget);
                        prevTarget = currentTarget;
                    }

                    return prevDataLength;
                }
            }
        }

        private ICompression[] GetCompressions(MpqCompressionType type)
        {
            var result = new List<ICompression>();

            if ((type & MpqCompressionType.BZip2) != 0)
            {
                result.Add(new BZip2Compression());
                type &= ~MpqCompressionType.BZip2;
            }

            if (type != 0)
            {
                throw new NotSupportedException($"Compression type not supported: {type}.");
            }

            return result.ToArray();
        }

    }
}
