﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using NUnit.Framework;

using PowerDeploy.Core;

using Environment = PowerDeploy.Core.Environment;

namespace PowerDeploy.Tests
{
    [TestFixture]
    public class EnvironmentSerializationTests
    {
        [Test]
        public void Serialize_Environment_Test()
        {
            var target = new Environment();
            target.Name = "Local";
            target.Description = "Used for unit tests, not a real environment";

            target.Variables = new List<Variable>();
            target.Variables.Add(new Variable() { Name = "Name", Value = "Tobi" });
            target.Variables.Add(new Variable() { Name = "Jack", Value = "Bauer" });

            var xml = new StringWriter();

            var serializer = new XmlSerializer(typeof(Environment));
            serializer.Serialize(xml, target);

            Console.WriteLine(xml);
        }

        [Test]
        public void Deserialize_Environment_Test()
        {
            var xml = @"<?xml version=""1.0""?>
                        <environment name=""local"" description=""Used for unit tests, not a real environment"">
                          <variable name=""Name"" value=""Tobi"" />
                          <variable name=""Jack"" value=""Bauer"" />
                        </environment>";

            var serializer = new XmlSerializer(typeof(Environment));
            var result = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as Environment;

            Assert.AreEqual("local", result.Name);

            Assert.AreEqual("Name", result.Variables[0].Name);
            Assert.AreEqual("Jack", result.Variables[1].Name);
            Assert.AreEqual("Tobi", result.Variables[0].Value);
            Assert.AreEqual("Bauer", result.Variables[1].Value);
        }
    }
}