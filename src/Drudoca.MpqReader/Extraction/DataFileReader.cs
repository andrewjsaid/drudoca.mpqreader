using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader.Extraction
{
    internal class DataFileReader
    {
        private bool HasFlag(BlockFileFlags value, BlockFileFlags flag) => (value & flag) == flag;

        public async Task<byte[]> ReadAsync(MpqArchive archive, IFileTableEntry file)
        {
            archive.Seek(file.FileOffset);

            var sectorTableSize = CalculateSectorTableSize(archive.BlockSize, file);
            var compressedFileSize = (int)file.CompressedFileSize;
            var totalCompressedFileSize = sectorTableSize + compressedFileSize;

            using (var memory = MemoryPool<byte>.Shared.Rent(totalCompressedFileSize))
            {
                var source = memory.Memory.Slice(0, totalCompressedFileSize);

                await archive.ReadAsync(source);

                var sectors = ReadSectorsTable(archive.BlockSize, file, source);

                var fileData = new byte[file.FileSize];
                var target = new Memory<byte>(fileData);
                var targetIndex = 0;

                foreach (var (offset, compressedSize, fileSize) in sectors)
                {
                    var fileSource = source.Slice(offset, compressedSize);
                    var fileTarget = target.Slice(0, fileSize);

                    ReadSector(fileSource, fileTarget, file.Flags);

                    targetIndex += fileSize;
                }

                if (targetIndex != fileData.Length)
                {
                    throw new InvalidDataException("Unable to read archive file");
                }

                return fileData;

            }
        }

        private int CalculateSectorTableSize(int blockSize, IFileTableEntry file)
        {
            if (HasFlag(file.Flags, BlockFileFlags.SingleUnit))
                return 0;

            var numSectors = (file.CompressedFileSize + blockSize - 1) / blockSize;
            var numEntries = 1 + numSectors;

            return (int)(numEntries * 4); // 4 bytes each
        }

        private (int offset, int compressedSize, int fileSize)[] ReadSectorsTable(int blockSize, IFileTableEntry file, Memory<byte> source)
        {
            if (HasFlag(file.Flags, BlockFileFlags.SingleUnit))
            {
                return new[]
                {
                    (0, (int)file.CompressedFileSize, (int)file.FileSize)
                };
            }
            else
            {
                throw new NotImplementedException("TODO AJS");
            }
        }

        private void ReadSector(Memory<byte> source, Memory<byte> target, BlockFileFlags flags)
        {
            if (HasFlag(flags, BlockFileFlags.Encrpyted))
            {
                // TODO AJS: Decrypt
                throw new NotImplementedException("Decryption not yet supported");
            }

            if(source.Length == target.Length)
            {
                source.CopyTo(target);
            }
            else if (HasFlag(flags, BlockFileFlags.Compressed))
            {
                var compressionType = (MpqCompressionType)source.Span[0];
                source = source.Slice(1);

                var compressionConductor = new CompressionConductor();
                var size = compressionConductor.Decompress(
                    source, target, compressionType);

                if (size != target.Length)
                {
                    throw new InvalidDataException($"Decompression size was {size}. Expected {target.Length}.");
                }

            }
            else if (HasFlag(flags, BlockFileFlags.Imploded))
            {
                throw new NotSupportedException("Imploded files are not supported");
            }
            else
            {
                throw new InvalidDataException("Invalid flags or sector size.");
            }
        }
    }
}
