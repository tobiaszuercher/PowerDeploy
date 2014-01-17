﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PowerDeploy.Core.Template
{
    /// <summary>
    /// Used for a usage of a variable. The followings notation is supported:
    /// 
    /// ${Var}              Simple usage of a variable with the name Var.
    /// ${Var=Tobi}         If Var is not set, then use Tobi as default value
    /// ${Var=Tobi_$[env]}  Default values may contain other variables. Use [ and ] instead of { }
    /// </summary>
    public class VariableUsage
    {
        private readonly Regex DefaultValueVariableRegex = new Regex(@"\$\[(?<Name>[^\]]+)\]", RegexOptions.Compiled);

        public Variable Variable { get; set; }

        public string DefaultValue { get; set; }

        public bool IsMissingValue
        {
            get { return (Variable.Value == null && DefaultValue == null); }
        }

        public VariableUsage(string usage, IEnumerable<Variable> variables)
        {
            ParseUsage(usage, variables);
        }

        public string GetValueOrDefault(IEnumerable<Variable> variables)
        {
            if (IsMissingValue) return "!!Missing variable for " + Variable.Name + "!!";

            if (Variable.Value == null)
            {
                var parsedDefaultValue = DefaultValue;

                while (DefaultValueVariableRegex.IsMatch(parsedDefaultValue))
                {
                    parsedDefaultValue = DefaultValueVariableRegex.Replace(
                        parsedDefaultValue,
                        m =>
                            {
                                var foundVariable = variables.FirstOrDefault(v => v.Name == m.Groups["Name"].Value);
                                if (foundVariable != null)
                                {
                                    return foundVariable.Value;
                                }

                                // TODO: parse this on variable usage creation to have IsMissing feature
                                return "<<Missing variable in default value " + m.Groups["Name"].Value + ">>";
                            });
                }

                return parsedDefaultValue;
            }

            return Variable.Value;
        }

        private void ParseUsage(string usage, IEnumerable<Variable> variables)
        {
            // no default value -> just map the name
            if (!usage.Contains("="))
            {
                Variable = variables.FirstOrDefault(v => v.Name == usage);

                // if variable not found create one with the name so we can report later which one was missing
                if (Variable == null)
                    Variable =  new Variable() { Name = usage };
                
                return;
            }

            var splitted = usage.Split('=');

            if (splitted.Count() != 2)
                throw new InvalidOperationException("Wrong format of variable usage. Use ${var} or ${var=defaultvalue}.");

            var foundVariable = variables.FirstOrDefault(v => v.Name == splitted[0]);

            Variable = foundVariable ?? new Variable() { Name = splitted[0] };
            DefaultValue = splitted[1];
        }
    }
}