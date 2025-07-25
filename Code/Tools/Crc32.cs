using System;
using System.Security.Cryptography;


namespace QR.Code.Tools
{
    internal class Crc32 : HashAlgorithm
    {
        private const uint Polynom = 0xEDB88320U;

        private readonly uint[] _rawData = new uint[256];
        private uint _value;

        public Crc32()
        {
            HashSizeValue = 32;
            Initialize();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (int i = ibStart; i < ibStart + cbSize; i++)
            {
                byte ti = (byte)(_value & 0xFFU ^ array[i]);
                _value = (_value >> 8) ^ _rawData[ti];
            }
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(~_value);
        }

        public override byte[] Hash
        {
            get => HashFinal();
        }

        public override void Initialize()
        {
            for (uint i = 0; i < 256; i++)
            {
                uint v = i;
                for (int b = 0; b < 8; b++)
                {
                    if ((v & 1) == 1)
                    {
                        v = (v >> 1) ^ Polynom;
                    }
                    else
                    {
                        v >>= 1;
                    }
                }
                _rawData[i] = v;
            }
            _value = uint.MaxValue;
        }
    }
}