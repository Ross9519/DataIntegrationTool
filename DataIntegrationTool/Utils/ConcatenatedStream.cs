namespace DataIntegrationTool.Utils
{
    public class ConcatenatedStream(Stream firstStream, Stream secondStream, bool leaveOpen = false) : Stream
    {
        private Stream? _firstStream = firstStream ?? throw new ArgumentNullException(nameof(firstStream));
        private readonly Stream _secondStream = secondStream ?? throw new ArgumentNullException(nameof(secondStream));
        private bool _readingFirst = true;

        public override bool CanRead => (_firstStream?.CanRead ?? false) || _secondStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_readingFirst && _firstStream != null)
            {
                int bytesRead = _firstStream.Read(buffer, offset, count);
                if (bytesRead == 0)
                {
                    _readingFirst = false;
                    _firstStream.Dispose();
                    _firstStream = null;
                    return _secondStream.Read(buffer, offset, count);
                }
                return bytesRead;
            }
            else
            {
                return _secondStream.Read(buffer, offset, count);
            }
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_readingFirst && _firstStream != null)
            {
                int bytesRead = await _firstStream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0)
                {
                    _readingFirst = false;
                    _firstStream.Dispose();
                    _firstStream = null;
                    return await _secondStream.ReadAsync(buffer, cancellationToken);
                }
                return bytesRead;
            }
            else
            {
                return await _secondStream.ReadAsync(buffer, cancellationToken);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing && !leaveOpen)
            {
                _firstStream?.Dispose();
                _secondStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
