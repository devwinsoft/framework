using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Devarc
{
    public static class Cryptor
    {
        static string tdes_IV = "awwxewwo";
        static string tdes_Key = "xiinwwwWwW@aw()r@@@rrrss";

        public static string Encrypt(string input)
        {
            try
            {
                TripleDES tripleDes = TripleDES.Create();
                tripleDes.IV = Encoding.ASCII.GetBytes(tdes_IV);
                tripleDes.Key = Encoding.ASCII.GetBytes(tdes_Key);
                tripleDes.Mode = CipherMode.CBC;
                //tripleDes.Padding = PaddingMode.Zeros;

                ICryptoTransform crypto = tripleDes.CreateEncryptor();
                byte[] decodedInput = Encoding.UTF8.GetBytes(input);
                byte[] decryptedBytes = crypto.TransformFinalBlock(decodedInput, 0, decodedInput.Length);
                return System.Convert.ToBase64String(decryptedBytes, 0, decryptedBytes.Length);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return string.Empty;
            }
        }

        public static string Decrypt(string input)
        {
            try
            {
                TripleDES tripleDes = TripleDES.Create();
                tripleDes.IV = Encoding.ASCII.GetBytes(tdes_IV);
                tripleDes.Key = Encoding.ASCII.GetBytes(tdes_Key);
                tripleDes.Mode = CipherMode.CBC;
                //tripleDes.Padding = PaddingMode.Zeros;

                ICryptoTransform crypto = tripleDes.CreateDecryptor();
                byte[] decodedInput = System.Convert.FromBase64String(input);
                //Debug.LogWarning("---- " + input);
                //Debug.LogWarning("---- " + decodedInput);
                byte[] decryptedBytes = crypto.TransformFinalBlock(decodedInput, 0, decodedInput.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return "";
            }
        }
    }
}
