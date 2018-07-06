using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Seekyu.Logging;
using System;
using System.Collections.Generic;

namespace Seekyu.Tests.Logging
{
    public class TestQueryException
    {
        public string Name
        {
            get
            {
                throw new Exception(string.Empty);
            }
        }
    }
    public class TestQuery<T> : IQuery
    { }
    public class TestQuery<T1, T2> : IQuery
    { }

    [TestFixture]
    class JsonObjectSerializerTests
    {
        [Test]
        public void JsonObjectSerializer_Should_Implement_IQuerySerializer()
        {
            typeof(JsonObjectSerializer).Should().Implement<IObjectSerializer>();
        }

        [Test]
        public void Serialize_Should_Include_Query_Type_To_Result()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            string expectedJson = "TestQuery";
            TestQuery query = new TestQuery();

            string json = SerializerTested.Serialize(query);

            json.Should().StartWith(expectedJson);
        }

        [Test]
        public void Serialize_Should_Include_Query_TypeGenerics_To_Result()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            string expectedJson = "TestQuery<String,TestQuery<TestQuery<TestQueryException,Boolean>>>";
            TestQuery<string, TestQuery<TestQuery<TestQueryException, bool>>> query
                = new TestQuery<string, TestQuery<TestQuery<TestQueryException, bool>>>();

            string json = SerializerTested.Serialize(query);

            json.Should().StartWith(expectedJson);
        }

        [Test]
        public void Serialize_Should_Include_Query_Values_To_Result_As_Json()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            TestQuery query = new TestQuery { Birthday = DateTime.Now, Name = "Joseph", Sizes = new List<int> { 1, 2, 3, 4 } };
            string expectedJson = JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
            });

            string json = SerializerTested.Serialize(query);

            json.Should().Contain(expectedJson);
        }

        [Test]
        public void Serialize_Should_Serialize_Exception_And_Message_When_JsonConvert_SerializeObject_Raise_Exception()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            string message = $"Error getting value from '{nameof(TestQueryException.Name)}' on '{typeof(TestQueryException).FullName}'.";
            TestQueryException query = new TestQueryException();
            string expectedJson = $"{query.GetType().Name}{Environment.NewLine}Failed to SerializeObject{Environment.NewLine}{message}";

            string json = SerializerTested.Serialize(query);

            json.Should().Contain(expectedJson);
        }

        [Test]
        public void Serialize_Should_Not_Get_Stuck_In_A_Loop()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            TestQuery query = new TestQuery();
            TestQuery otherQuery = new TestQuery();
            query.NextQuery = otherQuery;
            otherQuery.NextQuery = otherQuery;
            string expectedJson = JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
            });

            string json = SerializerTested.Serialize(query);

            json.Should().Contain(expectedJson);
        }

        [Test]
        public void Serialize_Should_Hash_Properties_Decorated_With_JsonEncryptAttribute()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            TestQuery query = new TestQuery { Password = "toto" };
            string expectedJson = JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
            });

            string json = SerializerTested.Serialize(query);

            json.Should().Contain(expectedJson);
        }

        [Test]
        public void Serialize_Should_Ignore_Content_Of_Properties_Decorated_With_JsonLogIgnoreAttribute()
        {
            IObjectSerializer SerializerTested = new JsonObjectSerializer();
            TestQuery query = new TestQuery { LogContent = "Log content to be ignored" };
            string expectedJson = JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
            });

            string json = SerializerTested.Serialize(query);

            json.Should().Contain(expectedJson);
        }
    }
}
