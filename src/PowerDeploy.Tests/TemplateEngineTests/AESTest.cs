using System.IO;

using NUnit.Framework;

using PowerDeploy.Core.Cryptography;

using Assert = Xunit.Assert;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestFixture]
    public class AESTest
    {
        [Test]
        public void EncryptFromFilePasswordOrString()
        {
            const string password = "test";
            const string textToEncrypt = "blub";

            var tempFileName = Path.GetTempFileName();

            File.WriteAllText(tempFileName, password);

            var passwordFromFile = File.ReadAllText(tempFileName);

            var encrypted1 = AES.Encrypt(textToEncrypt, password);
            var encrypted2 = AES.Encrypt(textToEncrypt, passwordFromFile);

            var decrypted1 = AES.Decrypt(encrypted1, password);
            var decrypted2 = AES.Decrypt(encrypted2, passwordFromFile);
            var decrypted3 = AES.Decrypt(encrypted2, passwordFromFile);
            var decrypted4 = AES.Decrypt(encrypted2, password);

            Assert.Equal(textToEncrypt, decrypted1);
            Assert.Equal(textToEncrypt, decrypted2);
            Assert.Equal(textToEncrypt, decrypted3);
            Assert.Equal(textToEncrypt, decrypted4);
            
            Assert.Equal(password, passwordFromFile);
        }
    }
}