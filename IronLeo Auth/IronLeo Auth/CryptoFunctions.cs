using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Web;
using System.IO;

namespace IronLeo_Auth
{
    public class CryptoFunctions
    {
        public string cryptoIV;
        public string clientKey;
        public string serverKey;

        public void retrieveKeys()
        {
            if (Application.Current.Properties.ContainsKey("cryptoIV"))
            {
                cryptoIV = Application.Current.Properties["cryptoIV"] as string;
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert("Please specify \"Crypto IV\"");
            }
            if (Application.Current.Properties.ContainsKey("clientKey"))
            {
                clientKey = Application.Current.Properties["clientKey"] as string;
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert("Please specify \"Client Key\"");
            }
            if (Application.Current.Properties.ContainsKey("serverKey"))
            {
                serverKey = Application.Current.Properties["serverKey"] as string;
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert("Please specify \"Server Key\"");
            }
        }

        public string encryptToken(string auth)
        {
            try
            {
                Aes encryptor = Aes.Create();
                encryptor.Mode = CipherMode.CBC;
                encryptor.Key = Encoding.ASCII.GetBytes(clientKey);
                encryptor.IV = Encoding.ASCII.GetBytes(cryptoIV);
                MemoryStream memoryStream = new MemoryStream();
                ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
                byte[] authBytes = Encoding.ASCII.GetBytes(auth);
                cryptoStream.Write(authBytes, 0, authBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string token = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
                return HttpUtility.UrlEncode(token);
            }
            catch
            {
                DependencyService.Get<IMessage>().ShortAlert("Invalid encryption keys!");
                return "";
            }
        }

        public string decryptToken(string auth)
        {
            try
            {
                Aes decryptor = Aes.Create();
                decryptor.Mode = CipherMode.CBC;
                decryptor.Key = Encoding.ASCII.GetBytes(serverKey);
                decryptor.IV = Encoding.ASCII.GetBytes(cryptoIV);
                MemoryStream memoryStream = new MemoryStream();
                ICryptoTransform aesDecryptor = decryptor.CreateDecryptor();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

                byte[] cipherBytes = Convert.FromBase64String(auth);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                string plainToken = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return plainToken;
            }
            catch
            {
                DependencyService.Get<IMessage>().ShortAlert("Invalid encryption keys!");
                return "";
            }
        }
    }
}
