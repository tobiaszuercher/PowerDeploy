using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PowerDeploy.Core;
using PowerDeploy.Core.Template;
using ServiceStack;

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
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("on")]
        [TestCase("1")]
        [TestCase("enabled")]
        public void Conditional_Is_Visible_With_True(string condition)
        {
            var target = new VariableResolver(new List<Variable>());
            var result = target.TransformVariables("Hello\n<!--[if " + condition + "]-->\ntobi!\n<!--[endif]-->");

            Assert.AreEqual("Hello\r\ntobi!", result);
        }

        [Test]
        [TestCase("false")]
        [TestCase("FALSE")]
        [TestCase("off")]
        [TestCase("0")]
        [TestCase("disabled")]
        public void Conditional_Is_Not_Visible_With_True(string condition)
        {
            var target = new VariableResolver(new List<Variable>());
            var result = target.TransformVariables("Hello\n<!--[if not " + condition + "]-->\ntobi!\n<!--[endif]-->");

            Assert.AreEqual("Hello\r\ntobi!", result);
        }

        [Test]
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("on")]
        [TestCase("1")]
        [TestCase("enabled")]
        public void Conditional_Is_Visible_With_True_Variable(string condition)
        {
            var variableList = new List<Variable>()
            {
                new Variable() {Name = "condition", Value = condition}
            };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello\n<!--[if ${condition}]-->\ntobi!\n<!--[endif]-->");

            Assert.AreEqual("Hello\r\ntobi!", result);
        }

        [Test]
        [TestCase("false")]
        [TestCase("False")]
        [TestCase("FALSE")]
        [TestCase("off")]
        [TestCase("0")]
        [TestCase("disable")]
        public void Conditional_Is_Not_Visible_With_True_Variable(string condition)
        {
            var variableList = new List<Variable>()
            {
                new Variable() {Name = "condition", Value = condition}
            };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello\n<!--[if not ${condition}]-->\ntobi!\n<!--[endif]-->");

            Assert.AreEqual("Hello\r\ntobi!", result);
        }

        [Test]
        [TestCase("false")]
        [TestCase("FALSE")]
        [TestCase("off")]
        [TestCase("0")]
        [TestCase("disabled")]
        public void Conditional_Not_Visible_With_False(string condition)
        {
            var target = new VariableResolver(new List<Variable>());
            var result = target.TransformVariables("Hello\n<!--[if " + condition + "]-->\ntobi!\n<!--[endif]-->");

            Assert.AreEqual("Hello", result);
        }

        [Test]
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("on")]
        [TestCase("1")]
        [TestCase("enabled")]
        public void Conditional_Not_Visible_With_False_Negated(string condition)
        {
            var target = new VariableResolver(new List<Variable>());
            var result = target.TransformVariables("Hello\n<!--[if not " + condition + "]-->\ntobi!\n<!--[endif]-->");

            Assert.AreEqual("Hello", result);
        }

        [Test]
        [TestCase("false")]
        [TestCase("FALSE")]
        [TestCase("off")]
        [TestCase("0")]
        [TestCase("disabled")]
        public void Conditional_Is_Visible_With_False_Variable(string condition)
        {
            var variableList = new List<Variable>()
            {
                new Variable() {Name = "condition", Value = condition}
            };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello\r\n<!--[if ${condition}]-->\r\ntobi!\r\n<!--[endif]-->");

            Assert.AreEqual("Hello", result);
        }

        [Test]
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("on")]
        [TestCase("1")]
        [TestCase("enabled")]
        public void Conditional_Is_Visible_With_False_Variable_Negated(string condition)
        {
            var variableList = new List<Variable>()
            {
                new Variable() {Name = "condition", Value = condition}
            };

            var target = new VariableResolver(variableList);
            var result = target.TransformVariables("Hello\r\n<!--[if not ${condition}]-->\r\ntobi!\r\n<!--[endif]-->");

            Assert.AreEqual("Hello", result);
        }

        [Test]
        [TestCase("true", "true", "content 1\r\ncontent 2")]
        [TestCase("true", "false", "content 1")]
        [TestCase("false", "false", "")]
        [TestCase("${true.condition}", "${true.condition}", "content 1\r\ncontent 2")]
        [TestCase("${false.condition}", "${false.condition}", "")]
        public void Multiple_Conditional_Transforming(string condition1, string condition2, string assert)
        {
            var variableList = new List<Variable>()
            {
                new Variable() { Name = "true.condition", Value = "true" },
                new Variable() { Name = "false.condition", Value = "false" },
            };

            var target = new VariableResolver(variableList);
            
            var result = target.TransformVariables("<!-- [if " + condition1 + "] -->" + System.Environment.NewLine +
                                                   "content 1" + System.Environment.NewLine +
                                                   "<!-- [endif] -->" + System.Environment.NewLine +
                                                   "<!-- [if " + condition2 + "] -->" + System.Environment.NewLine +
                                                   "content 2" + System.Environment.NewLine +
                                                   "<!-- [endif] -->");

            Assert.AreEqual(assert, result);
        }

        [Test]
        public void Dont_Resolve_Escaped_Variable()
        {
            var target = new VariableResolver(new List<Variable>());
            var result = target.TransformVariables("Hello _$_{Escaped}!");

            Assert.AreEqual("Hello ${Escaped}!", result);
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