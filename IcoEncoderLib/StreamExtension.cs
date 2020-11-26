using System;
using System.IO;

namespace IcoEncoderLib
{
    public static class StreamExtension
    {

        public static void WriteUInt16LE(this Stream stream, UInt16 value)
        {
            stream.WriteByte((byte)(value & 0xFF));
            stream.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void WriteUInt32LE(this Stream stream, UInt32 value)
        {
            stream.WriteByte((byte)(value & 0xFF));
            stream.WriteByte((byte)((value >> 8) & 0xFF));
            stream.WriteByte((byte)((value >> 16) & 0xFF));
            stream.WriteByte((byte)((value >> 24) & 0xFF));
        }
    }
}
