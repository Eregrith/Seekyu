using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Logging
{
    public class EncryptedStringPropertyResolver : DefaultContractResolver
    {
        private byte[] encryptionKeyBytes;

        public EncryptedStringPropertyResolver(string encryptionKey)
        {
            if (encryptionKey == null)
                throw new ArgumentNullException("encryptionKey");

            HashKeyToMakeSureItIs256BitsLong(encryptionKey);
        }

        private void HashKeyToMakeSureItIs256BitsLong(string encryptionKey)
        {
            using (SHA256Managed sha = new SHA256Managed())
            {
                this.encryptionKeyBytes =
                    sha.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            }
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);

            AttachEncryptedStringValueProviderToPropertiesWithJsonEncryptAttribute(type, props);
            AttachLogIgnoredStringValueProviderToPropertiesWithJsonLogIgnoreAttribute(type, props);

            return props;
        }

        private void AttachEncryptedStringValueProviderToPropertiesWithJsonEncryptAttribute(Type type, IList<JsonProperty> props)
        {
            foreach (JsonProperty prop in props.Where(p => p.PropertyType == typeof(string)))
            {
                PropertyInfo pi = type.GetProperty(prop.UnderlyingName);
                if (pi != null && pi.GetCustomAttribute(typeof(LogEncryptedAttribute), true) != null)
                {
                    prop.ValueProvider =
                        new EncryptedStringValueProvider(pi, encryptionKeyBytes);
                }
            }
        }

        private void AttachLogIgnoredStringValueProviderToPropertiesWithJsonLogIgnoreAttribute(Type type, IList<JsonProperty> props)
        {
            foreach (JsonProperty prop in props.Where(p => p.PropertyType == typeof(string)))
            {
                PropertyInfo pi = type.GetProperty(prop.UnderlyingName);
                if (pi != null && pi.GetCustomAttribute(typeof(LogIgnoredAttribute), true) != null)
                {
                    prop.ValueProvider = new LogIgnoredStringValueProvider();
                }
            }
        }

        class EncryptedStringValueProvider : IValueProvider
        {
            private readonly PropertyInfo targetProperty;
            private readonly byte[] encryptionKey;

            private static byte[] _iv;

            public EncryptedStringValueProvider(PropertyInfo targetProperty, byte[] encryptionKey)
            {
                this.targetProperty = targetProperty;
                this.encryptionKey = encryptionKey;
            }

            public object GetValue(object target)
            {
                string value = (string)targetProperty.GetValue(target);
                if (value == null) return null;
                byte[] buffer = Encoding.UTF8.GetBytes(value);

                using (MemoryStream inputStream = new MemoryStream(buffer, false))
                using (MemoryStream outputStream = new MemoryStream())
                using (AesManaged aes = new AesManaged { Key = encryptionKey })
                {
                    _iv = _iv ?? aes.IV;
                    outputStream.Write(_iv, 0, _iv.Length);
                    outputStream.Flush();

                    ICryptoTransform encryptor = aes.CreateEncryptor(encryptionKey, _iv);
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                    {
                        inputStream.CopyTo(cryptoStream);
                    }

                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }

            public void SetValue(object target, object value)
            {
                byte[] buffer = Convert.FromBase64String((string)value);

                using (MemoryStream inputStream = new MemoryStream(buffer, false))
                using (MemoryStream outputStream = new MemoryStream())
                using (AesManaged aes = new AesManaged { Key = encryptionKey })
                {
                    byte[] iv = new byte[16];
                    int bytesRead = inputStream.Read(iv, 0, 16);
                    if (bytesRead < 16)
                    {
                        throw new CryptographicException("IV is missing or invalid.");
                    }

                    ICryptoTransform decryptor = aes.CreateDecryptor(encryptionKey, iv);
                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(outputStream);
                    }

                    string decryptedValue = Encoding.UTF8.GetString(outputStream.ToArray());
                    targetProperty.SetValue(target, decryptedValue);
                }
            }
        }

        class LogIgnoredStringValueProvider : IValueProvider
        {
            public object GetValue(object target) => "Log content ignored";

            public void SetValue(object target, object value) { /* Useless because it is ignored */ }
        }
    }
}
