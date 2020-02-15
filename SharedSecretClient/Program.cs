using System;
using System.IO;
using System.Net;
using SharedSecret.Common;

namespace SharedSecretClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            ArgumentCollection baseArgs = new ArgumentCollection();
            
            baseArgs.Add(new Argument("verbose", 'v'));
            
            ArgumentCollection syncArgs = new ArgumentCollection()
            {
                Name = "sync",
            };
            
            baseArgs.SubCollections.Add(syncArgs);

            var parser = new ArgumentsParser()
            {
                AllOptions = baseArgs
            };

            parser.Parse(args);
*/
            // sync [file] 

            var crypt = new CryptoFile();
            crypt.Gen();
            crypt.GetCryptoKey();
            
            using (var f = new MemoryStream(new byte[]{1,2,3,4,5,6,7,8,9,10}))
            {
                using (var memstr = new MemoryStream())
                {
                    crypt.EncryptStreamToStream(f, memstr);
                    memstr.Seek(0, SeekOrigin.Begin);
                    using (var memstr2 = new MemoryStream())
                    {
                        crypt.DecryptStreamToStream(memstr, memstr2);
                        var data = memstr2.ToArray();
                    }
                }
                
            }

        }
    }
}