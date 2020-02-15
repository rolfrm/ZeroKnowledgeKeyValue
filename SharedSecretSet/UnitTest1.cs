using System;
using System.Threading;
using NUnit.Framework;
using SharedSecret;
using SharedSecret.Common;

namespace SharedSecredSet
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var trd = new Thread(() =>
            {
                var server = new Server();
                server.Start(Array.Empty<string>());
            });
            trd.Start();

            var scli = new SecretClientConnection();

            var msg = new Message()
            {
                Data = System.Text.Encoding.UTF8.GetBytes("Hello world"),
                File = "firstfile",
                Mode = MessageMode.Write
            };

            var result = scli.SendMessage(msg).Result;
            Assert.IsTrue(result.File == msg.File);

        }
    }
}