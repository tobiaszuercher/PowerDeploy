using System.Collections.Generic;

using NUnit.Framework;

using PowerDeploy.Core;
using PowerDeploy.Core.Template;

using System.Linq;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestFixture]
    public class VariableResolverTests
    {
         [Test]
         public void Resolve_One_Variable_Test()
         {
             var variableList = new List<Variable>() { new Variable() { Name = "Var1", Value = "Jack Bauer" } };

             var target = new VariableResolver(variableList);
             var result = target.TransformVariables("Hello ${Var1}!");

             Assert.AreEqual("Hello Jack Bauer!", result);
         }

        [Test]
        public void Resolve_Multiple_Variables_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName} ${LastName}!");

            Assert.AreEqual("Hello Jack Bauer!", result);
        }

        [Test]
        public void Resolve_On_Multiple_Lines_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                    new Variable() { Name = "SecondLine", Value = "2" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables(@"Hello ${FirstName} ${LastName}!\r\nThis is another line:${SecondLine}");

            Assert.AreEqual(@"Hello Jack Bauer!\r\nThis is another line:2", result);
        }

        [Test]
        public void Resolve_With_Default_Value_Set_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName=Mila} ${LastName=Kunis}!");

            Assert.AreEqual("Hello Jack Bauer!", result);
        }

        [Test]
        public void Resolve_Using_Default_Value_Test()
        {
            var variableList = new List<Variable>();

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName=Mila} ${LastName=Kunis}!");

            Assert.AreEqual("Hello Mila Kunis!", result);
        }

        [Test]
        public void Resolve_Variable_In_Variable_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Mila" },
                    new Variable() { Name = "LastName", Value = "Kunis" },
                    new Variable() { Name = "Fullname", Value = "${FirstName} ${LastName}" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${Fullname}!");

            Assert.AreEqual("Hello Mila Kunis!", result);
        }

        [Test]
        public void Resolve_Variable_In_Variable_In_Variable_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "A", Value = "A + ${B}" },
                    new Variable() { Name = "B", Value = "B + ${C}" },
                    new Variable() { Name = "C", Value = "C" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("${A}");

            Assert.AreEqual("A + B + C", result);
        }

        [Test]
        public void List_Missing_Variables_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FirstName} ${LastName}!");

            Assert.AreEqual(1, target.VariableUsageList.Count(v => v.IsMissingValue));
            Assert.AreEqual("Hello Jack !!Missing variable for LastName!!!", result);
        }

        [Test]
        public void List_Multiple_Missing_Variables_Test()
        {
            var target = new VariableResolver(new List<Variable>());

            target.TransformVariables("${a} ${b} ${c}");

            Assert.AreEqual(3, target.VariableUsageList.Count(v => v.IsMissingValue));
        }

        [Test]
        public void Resolve_Default_Value_With_Variable_In_It_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" },
                    new Variable() { Name = "LastName", Value = "Bauer" },
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${Fullname=$[FirstName] $[LastName]}!");

            Assert.AreEqual("Hello Jack Bauer!", result);
        }

        [Test]
        [Ignore] // TODO: add warning also for default values
        public void Resolve_Default_Value_With_Missing_Variable_Test()
        {
            var variableList = new List<Variable>()
                {
                    new Variable() { Name = "FirstName", Value = "Jack" }
                };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello ${FullName=$[gugus]}");

            Assert.AreEqual(1, target.VariableUsageList.Count(vu => vu.IsMissingValue));
        }
    }
}