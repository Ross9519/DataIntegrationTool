using System.Text;

namespace DataIntegrationTool.Shared.Utils
{
    public static class StreamUtils
    {
        public static (Stream, Encoding) PrepareStreamAndEncoding(Stream input, string fallbackEncodingName)
        {
            var fallbackEncoding = Encoding.GetEncoding(fallbackEncodingName);

            using var mem = new MemoryStream();
            var buffer = new byte[4]; // Max BOM size
            int read = input.Read(buffer, 0, buffer.Length);
            mem.Write(buffer, 0, read);

            Encoding? detected = null;
            if (read >= 2)
            {
                if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                    detected = Encoding.Unicode;
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                    detected = Encoding.BigEndianUnicode;
            }
            if (read >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                detected = Encoding.UTF8;

            // No BOM found? Use fallback
            var encoding = detected ?? fallbackEncoding;

            // Now rewind stream
            var combined = new ConcatenatedStream(new MemoryStream(mem.ToArray()), input, leaveOpen: true);
            return (combined, encoding);
        }
    }

}
