using System.Text;

namespace Notusu.NBS;
public static class StreamReader
{
    public static long ReadInt(this Stream s, int size)
    {
        if (size < 0 || size > sizeof(long))
        {
            throw new ArgumentException("Not valid size", nameof(size));
        }
        if (!s.CanRead)
        {
            throw new ArgumentException("Stream is unreadable", nameof(s));
        }
        long result = 0;
        for (int i = 0; i < size; i++)
        {
            int read = s.ReadByte();
            if (read < 0)
            {
                throw new Exception("Stream ended unexpectedly");
            }
            byte b = (byte)read;
            result += b << (i * 8);
        }
        return result;
    }

    public static string ReadString(this Stream s)
    {
        if (!s.CanRead)
        {
            throw new ArgumentException("Stream is unreadable", nameof(s));
        }

        int length = (int)s.ReadInt(sizeof(int));
        byte[] buffer = new byte[length];
        for (int i = 0; i < length; i++)
        {
            int read = s.ReadByte();
            if (read < 0)
            {
                throw new Exception("Stream ended unexpectedly");
            }
            byte b = (byte)read;
            buffer[i] = b;
        }
        return Encoding.UTF8.GetString(buffer);
    }
}