using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MrCMS.Website.Optimization
{
    public class WhitespaceFilter : Stream
    {
        public WhitespaceFilter(Stream sink)
        {
            _sink = sink;
        }

        private readonly Stream _sink;
        private static readonly Regex reg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");

        #region Properites

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            _sink.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        private long _position;
        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        #region Methods

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _sink.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _sink.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _sink.SetLength(value);
        }

        public override void Close()
        {
            _sink.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            string html = System.Text.Encoding.Default.GetString(buffer);

            html = reg.Replace(html, string.Empty);

            byte[] outdata = System.Text.Encoding.Default.GetBytes(html);
            _sink.Write(outdata, 0, outdata.GetLength(0));
        }

        #endregion

    }
}