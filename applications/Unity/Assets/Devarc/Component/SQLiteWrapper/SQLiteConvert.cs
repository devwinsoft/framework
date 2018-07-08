using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SQLiteWrapper
{
    public static class SQLiteConvert
    {
        #region Private Static Fields

        private static Encoding utf8 = new UTF8Encoding();
        private static Encoding utf16 = new UnicodeEncoding();

        #endregion

        #region Public Static Methods

        public static byte[] ToUTF8(string sourceText)
        {
            Byte[] byteArray;
            int nlen = utf8.GetByteCount(sourceText) + 1;

            byteArray = new byte[nlen];
            nlen = utf8.GetBytes(sourceText, 0, sourceText.Length, byteArray, 0);
            byteArray[nlen] = 0;

            return byteArray;
        }

        public static byte[] ToUTF16(string sourceText)
        {
            Byte[] byteArray;
            int nlen = utf16.GetByteCount(sourceText) + 1;

            byteArray = new byte[nlen];
            nlen = utf16.GetBytes(sourceText, 0, sourceText.Length, byteArray, 0);
            byteArray[nlen] = 0;

            return byteArray;
        }

        public static string UTF8ToString(IntPtr nativestring, int nativestringlen)
        {
            if (nativestringlen == 0 || nativestring == IntPtr.Zero) return "";
            if (nativestringlen == -1)
            {
                do
                {
                    nativestringlen++;
                } while (Marshal.ReadByte(nativestring, nativestringlen) != 0);
            }

            byte[] byteArray = new byte[nativestringlen];

            Marshal.Copy(nativestring, byteArray, 0, nativestringlen);

            return utf8.GetString(byteArray, 0, nativestringlen);
        }

        #endregion
    }
}
