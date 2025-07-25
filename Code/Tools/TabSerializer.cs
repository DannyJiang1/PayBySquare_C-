using System;
using System.Text;


namespace QR.Code.Tools
{
    internal class TabSerializer
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public void Append(int value)
        {
            Append(value.ToString());
        }

        public void Append(DateTime? value)
        {
            if (value.HasValue)
            {
                Append(value.Value.ToString("yyyyMMdd"));
            }
            else
            {
                Append((string?)null);
            }
        }

        public void Append(string? value)
        {
            if (value != null)
            {
                _sb.Append(value.Replace("\t", " "));
            }
            _sb.Append('\t'); // Extra tabs will be trimmed
        }

        public override string ToString()
        {
            return _sb.ToString().TrimEnd('\t');
        }
    }
}