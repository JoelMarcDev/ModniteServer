using System;
using System.Net;
using System.Text;

public static class TypeExtensions
{
    public static string ToDateTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public static void Write(this HttpListenerResponse response, string value)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(value);
        response.OutputStream.Write(buffer, 0, buffer.Length);
    }

    public static void Write(this HttpListenerResponse response, byte[] values)
    {
        response.OutputStream.Write(values, 0, values.Length);
    }
}