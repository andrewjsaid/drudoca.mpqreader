﻿using System;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHetTableReader : IStructureReader<MpqHetTable?>
    {
        public int InitialSize => throw new NotImplementedException();

        public ValueTask<MpqHetTable?> ReadAsync(MpqStreamReaderContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
