using System.Data.Common;
using System.Text;

namespace DataIntegrationTool.Shared.Utils
{
    public class DbDataReaderCsvStream(DbDataReader reader, Encoding encoding, string delimiter = ",") : Stream
    {
        private readonly DbDataReader _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        private readonly Encoding _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        private byte[] _buffer = [];
        private int _bufferOffset = 0;
        private bool _headerWritten = false;
        private bool _endOfStream = false;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush() { /* niente da fare */ }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        // Produce dati CSV nel buffer se necessario, e copia nel buffer di output i dati richiesti
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_endOfStream && _bufferOffset >= _buffer.Length)
                return 0; // fine stream

            if (_bufferOffset >= _buffer.Length)
            {
                if (!ProduceNextCsvLine())
                    return 0; // fine stream
            }

            int bytesAvailable = _buffer.Length - _bufferOffset;
            int bytesToCopy = Math.Min(count, bytesAvailable);

            Array.Copy(_buffer, _bufferOffset, buffer, offset, bytesToCopy);
            _bufferOffset += bytesToCopy;

            return bytesToCopy;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_endOfStream && _bufferOffset >= _buffer.Length)
                return 0; // fine stream

            if (_bufferOffset >= _buffer.Length)
            {
                if (!await ProduceNextCsvLineAsync(cancellationToken).ConfigureAwait(false))
                    return 0; // fine stream
            }

            int bytesAvailable = _buffer.Length - _bufferOffset;
            int bytesToCopy = Math.Min(buffer.Length, bytesAvailable);

            _buffer.AsSpan(_bufferOffset, bytesToCopy).CopyTo(buffer.Span);

            _bufferOffset += bytesToCopy;

            return bytesToCopy;
        }

        private bool ProduceNextCsvLine()
        {
            if (!_headerWritten)
            {
                // Prima riga: header colonna CSV
                var headers = new string[_reader.FieldCount];
                for (int i = 0; i < _reader.FieldCount; i++)
                    headers[i] = EscapeCsvField(_reader.GetName(i));
                var headerLine = string.Join(delimiter, headers) + "\r\n";
                _buffer = _encoding.GetBytes(headerLine);
                _bufferOffset = 0;
                _headerWritten = true;
                return true;
            }

            if (_reader.Read())
            {
                var fields = new string[_reader.FieldCount];
                for (int i = 0; i < _reader.FieldCount; i++)
                {
                    var val = _reader.IsDBNull(i) ? "" : _reader.GetValue(i)?.ToString() ?? "";
                    fields[i] = EscapeCsvField(val);
                }
                var line = string.Join(delimiter, fields) + "\r\n";
                _buffer = _encoding.GetBytes(line);
                _bufferOffset = 0;
                return true;
            }
            else
            {
                _endOfStream = true;
                return false;
            }
        }

        private async Task<bool> ProduceNextCsvLineAsync(CancellationToken cancellationToken)
        {
            if (!_headerWritten)
            {
                var headers = new string[_reader.FieldCount];
                for (int i = 0; i < _reader.FieldCount; i++)
                    headers[i] = EscapeCsvField(_reader.GetName(i));
                var headerLine = string.Join(delimiter, headers) + "\r\n";
                _buffer = _encoding.GetBytes(headerLine);
                _bufferOffset = 0;
                _headerWritten = true;
                return true;
            }

            if (await _reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var fields = new string[_reader.FieldCount];
                for (int i = 0; i < _reader.FieldCount; i++)
                {
                    var val = await _reader.IsDBNullAsync(i, cancellationToken).ConfigureAwait(false) ? "" : _reader.GetValue(i)?.ToString() ?? "";
                    fields[i] = EscapeCsvField(val);
                }
                var line = string.Join(delimiter, fields) + "\r\n";
                _buffer = _encoding.GetBytes(line);
                _bufferOffset = 0;
                return true;
            }
            else
            {
                _endOfStream = true;
                return false;
            }
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(delimiter) || field.Contains("\"") || field.Contains("\r") || field.Contains("\n"))
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            return field;
        }
    }
}
