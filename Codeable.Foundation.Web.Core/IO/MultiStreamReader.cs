using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Codeable.Foundation.UI.Web.Core.IO
{
    public class MultiStreamReader : Stream
    {
        public MultiStreamReader()
        {
        }
        public MultiStreamReader(IEnumerable<Stream> streams)
        {
            _streams.AddRange(streams);
        }

        private List<Stream> _streams = new List<Stream>();
        private long _position = 0;

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
            get { return false; }
        }

        public override long Length
        {
            get
            {
                long result = 0;
                foreach (Stream stream in _streams)
                {
                    result += stream.Length;
                }
                return result;
            }
        }
        public override long Position
        {
            get { return _position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin)
        {
            long length = this.Length;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;
                case SeekOrigin.Current:
                    _position += offset;
                    break;
                case SeekOrigin.End:
                    _position = length - offset;
                    break;
            }
            if (_position > length)
            {
                _position = length;
            }
            else if (_position < 0)
            {
                _position = 0;
            }
            return _position;
        }

        public override void SetLength(long value) { }
        public void AddStream(Stream stream)
        {
            _streams.Add(stream);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long len = 0;
            int result = 0;
            int buf_pos = offset;
            int bytesRead;
            foreach (Stream stream in _streams)
            {
                if (_position < (len + stream.Length))
                {
                    stream.Position = _position - len;
                    //TODO: Manual Strip: UTF-8 Encoding strip
                    if (stream.Position == 0)// test for the UTF-8 Encoding
                    {
                        byte[] first3 = new byte[3];
                        stream.Read(first3, 0, 3);
                        if ((first3[0] == 239) && (first3[1] == 187) && (first3[2] == 191))
                        {
                            _position += 3;
                            // its utf encoding tag, ignore it
                            // leave stream at current position, move on.
                        }
                        else
                        {
                            stream.Position = 0;
                        }
                    }
                    bytesRead = stream.Read(buffer, buf_pos, count);
                    result += bytesRead;
                    buf_pos += bytesRead;
                    _position += bytesRead;
                    if (bytesRead < count)
                    {
                        count -= bytesRead;
                    }
                    else
                    {
                        break;
                    }
                }
                len += stream.Length;
            }
            return result;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    foreach (Stream item in _streams)
                    {
                        item.Dispose();
                    }
                }
                catch { }
            }
            base.Dispose(disposing);
        }
    }
}
