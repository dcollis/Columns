using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Columns
{
    public class Encryption
    {
        public static string Sky = "lkirwf8heeyy66#cs!9hjvz5qq=498j5";
        public static string Siv = "741952h97+22#bbtrm8814887mxx7@8y";

        public static string DecryptRj256(string prmKey, string prmIv, string prmTextToDecrypt)
        {
            var myRijndael = new RijndaelManaged
            {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256
            };
            byte[] keyb = Encoding.ASCII.GetBytes(prmKey);
            byte[] ivb = Encoding.ASCII.GetBytes(prmIv);
            ICryptoTransform decryptor = myRijndael.CreateDecryptor(keyb, ivb);
            byte[] sEncrypted = Convert.FromBase64String(prmTextToDecrypt);
            var fromEncrypt = new byte[sEncrypted.Length];
            var msDecrypt = new MemoryStream(sEncrypted);
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
            return (Encoding.ASCII.GetString(fromEncrypt));
        }

        public static string EncryptRj256(string prmKey, string prmIv, string prmTextToEncrypt)
        {
            var myRijndael = new RijndaelManaged
                                 {
                                     Padding = PaddingMode.Zeros,
                                     Mode = CipherMode.CBC,
                                     KeySize = 256,
                                     BlockSize = 256
                                 };
            byte[] keyb = Encoding.ASCII.GetBytes(prmKey);
            byte[] ivb = Encoding.ASCII.GetBytes(prmIv);
            ICryptoTransform encryptor = myRijndael.CreateEncryptor(keyb, ivb);
            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            byte[] toEncrypt = Encoding.ASCII.GetBytes(prmTextToEncrypt);
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();
            byte[] encrypted = msEncrypt.ToArray();
            return (Convert.ToBase64String(encrypted));
        }
    }
}