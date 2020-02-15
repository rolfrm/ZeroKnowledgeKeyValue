using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SharedSecret.Common
{
    public class CryptoFile
    {
        byte[] key = null;
        byte[] iv = null;
        public (byte[], byte[]) GetCryptoKey()
        {
            return (key, iv);
        }

        public void Gen()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                key = aes.Key;
                iv = aes.IV;
            }
        }

        public void EncryptStreamToStream(Stream inStream, Stream outStream)
        {
            using (Aes aes = Aes.Create())
            {
                var (key, iv) = GetCryptoKey();

                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (var crypt = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write, true))
                {
                    inStream.CopyTo(crypt);
                }
            }
        }

        public void DecryptStreamToStream(Stream inStream, Stream outStream)
        {
            using (Aes aes = Aes.Create())
            {
                var (key, iv) = GetCryptoKey();

                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                using (var crypt = new CryptoStream(outStream, decryptor, CryptoStreamMode.Write, true))
                {
                    inStream.CopyTo(crypt);
                }
            }
        }   
    }

    public enum MessageMode
    {
        Read,
        Write
    }

    public class Message
    {
        public MessageMode Mode { get; set; }
        public string File { get; set; }
        public byte[] Data { get; set; }

        static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        public async Task WriteToStream(Stream str)
        {
            await JsonSerializer.SerializeAsync(str, this, Options);
        }

        public static async Task<Message> ReadFromStream(Stream str)
        {
            return await JsonSerializer.DeserializeAsync<Message>(str);
        }
    }

    public class SecretClientConnection
    {
        public string URL { get; set; } = "localhost:5000";

        public async Task<Message> SendMessage(Message msg)
        {
            using var cli = new HttpClient();
            var memstr = new MemoryStream();
            await msg.WriteToStream(memstr);
            memstr.Seek(0, SeekOrigin.Begin);
            var result = await cli.PostAsync(URL + "/files", new StreamContent(memstr));
            var str = await result.Content.ReadAsStreamAsync();
            return await Message.ReadFromStream(str);
        }
        
    }
}