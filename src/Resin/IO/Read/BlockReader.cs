﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Resin.IO.Write;

namespace Resin.IO.Read
{
    public class BlockReader<T> : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveOpen;
        private long _position;

        public BlockReader(Stream stream, bool leaveOpen = false)
        {
            _stream = stream;
            _leaveOpen = leaveOpen;
            _position = 0;
        }

        public IEnumerable<T> Get(IEnumerable<BlockInfo> blocks)
        {
            return blocks.Select(Get);
        }

        private T Get(BlockInfo info)
        {
            var distance = info.Position - _position;

            if (distance > 0)
            {
                _stream.Seek(distance, SeekOrigin.Current);
            }

            var buffer = new byte[info.Length];

            _stream.Read(buffer, 0, buffer.Length);

            _position = info.Position + info.Length;

            return Deserialize(buffer);
        }

        protected virtual T Deserialize(byte[] data)
        {
            return TrieSerializer.BytesToType<T>(data);
        }

        public void Dispose()
        {
            if(!_leaveOpen) _stream.Dispose();
        }
    }
}