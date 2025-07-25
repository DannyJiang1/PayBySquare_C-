using ManagedLzma;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QR.Code.Tools
{
    public class StreamReaderAdapter : IStreamReader, IDisposable
    {
        private readonly Stream _stream;
        public StreamReaderAdapter(Stream stream) => _stream = stream;
        public Task<int> ReadAsync(byte[] buffer, int offset, int length, StreamMode mode)
        {
            // Synchronous read for simplicity; you can use _stream.ReadAsync if desired
            int read = _stream.Read(buffer, offset, length);
            return Task.FromResult(read);
        }
        public void Dispose() => _stream.Dispose();
    }

    public class StreamWriterAdapter : IStreamWriter, IDisposable
    {
        private readonly Stream _stream;
        public StreamWriterAdapter(Stream stream) => _stream = stream;
        public Task<int> WriteAsync(byte[] buffer, int offset, int length, StreamMode mode)
        {
            _stream.Write(buffer, offset, length);
            return Task.FromResult(length);
        }
        public Task CompleteAsync() => Task.CompletedTask;
        public void Dispose() => _stream.Dispose();
    }
} 