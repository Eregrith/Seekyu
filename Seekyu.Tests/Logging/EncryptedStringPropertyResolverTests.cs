using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Seekyu.Logging;
using System;
using System.Security.Cryptography;

namespace Seekyu.Tests.Logging
{
    [TestFixture]
    class EncryptedStringPropertyResolverTests
    {
        [Test]
        public void EncryptedStringPropertyResolver_Should_BeDerivedFrom_DefaultContractResolver()
        {
            typeof(EncryptedStringPropertyResolver).Should().BeDerivedFrom<DefaultContractResolver>();
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_When_EncryptionKey_Is_Null()
        {
            Action action = () => new EncryptedStringPropertyResolver(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void JsonSerialization_With_EncryptedStringPropertyResolver_As_ContractResolver_Should_Encrypt_Strings_With_JsonEncrypt_Attribute()
        {
            TestQuery query = new TestQuery { Password = "toto", Name = "tata" };
            string json = JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
            });

            json.Should().Contain($"\"{nameof(TestQuery.Password)}\":");
            json.Should().NotContain(query.Password);

            json.Should().Contain($"\"{nameof(TestQuery.Name)}\":");
            json.Should().Contain(query.Name);
        }

        [Test]
        public void JsonSerialization_With_EncryptedStringPropertyResolver_As_ContractResolver_Should_Ignore_LogContent_With_JsonLogIgnore_Attribute()
        {
            TestQuery query = new TestQuery { LogContent = "Log containing serialized log content" };
            string json = JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
            });

            json.Should().Contain($"\"{nameof(TestQuery.LogContent)}\":");
            json.Should().NotContain(query.LogContent);
        }

        [Test]
        public void JsonDeSerialization_With_EncryptedStringPropertyResolver_As_ContractResolver_Should_Allow_Decryption_Of_Strings_Crypted_With_Same_Key()
        {
            TestQuery query = new TestQuery { Password = "toto" };
            var settings = new JsonSerializerSettings { ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y") };
            string json = JsonConvert.SerializeObject(query, Formatting.Indented, settings);

            TestQuery result = JsonConvert.DeserializeObject<TestQuery>(json, settings);

            result.Password.Should().Be(query.Password);
        }

        [Test]
        public void JsonDeSerialization_With_EncryptedStringPropertyResolver_As_ContractResolver_Should_Throw_CryptographicException_When_Value_Is_Incorrect()
        {
            TestQuery query = new TestQuery { Password = "toto" };
            var settings = new JsonSerializerSettings { ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y") };
            string json = "{ Password: 'nope' }";

            Action action = () => JsonConvert.DeserializeObject<TestQuery>(json, settings);

            action.Should().Throw<CryptographicException>();
        }
    }
}
