using System;
using System.Text;

namespace FSR.DigitalTwin.Client.Common.Utils
{
    public static class Base64Converter
    {
        /// <summary>
        /// Converts a string to Base64 using UTF-8 encoding.
        /// </summary>
        public static string EncodeString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes a Base64 string to plain text using UTF-8 encoding.
        /// </summary>
        public static string DecodeString(string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text))
                return string.Empty;

            byte[] bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Encodes a byte array to Base64.
        /// </summary>
        public static string EncodeBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Decodes a Base64 string to a byte array.
        /// </summary>
        public static byte[] DecodeToBytes(string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text))
                return Array.Empty<byte>();

            return Convert.FromBase64String(base64Text);
        }
    }
}
