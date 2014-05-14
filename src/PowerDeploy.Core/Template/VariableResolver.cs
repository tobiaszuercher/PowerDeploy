using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PowerDeploy.Core.Logging;

namespace PowerDeploy.Core.Template
{
    /// <summary>
    /// Variable Syntax:
    /// 
    /// ${host}             without default value
    /// ${host=localhost}   with default value localhost
    /// 
    /// Conditionals:
    /// <!--[if ${condition}]-->
    /// gugus
    /// <!--[endif]-->
    /// 
    /// if condition variable is true, on, enabled or 1 -> gugus will be used, otherwise not.
    /// </summary>
    public class VariableResolver
    {
        private IList<Variable> Variables { get; set; }
        private static readonly string[] TrueStrings = { "TRUE", "ON", "1", "ENABLED" };
        private static readonly ILog Log = LogManager.GetLogger(typeof(VariableResolver));
        public IList<VariableUsage> VariableUsageList { get; private set; }

        private readonly Regex VariableRegex = new Regex(@"\$\{(?<Name>[^\}]+)\}", RegexOptions.Compiled);
        private readonly Regex ConditionalRegex = new Regex(@"<!--\s*\[if\s*(?<expression>[^\]]*)]\s*-->\s*(?<content>.*)\s*<!--\s*\[endif]\s*-->", RegexOptions.Compiled);

        public VariableResolver(IList<Variable> variables)
        {
            Variables = variables;
            VariableUsageList = new List<VariableUsage>();
        }

        public string TransformVariables(string content)
        {
            var transformedVariables = VariableRegex.Replace(content, ReplaceVariables);
            
            return ConditionalRegex.Replace(transformedVariables, ReplaceConditionals);
        }

        private string ReplaceVariables(Match match)
        {
            var variableUsage = new VariableUsage(match.Groups["Name"].Value, Variables);
            VariableUsageList.Add(variableUsage);

            var parsed = variableUsage.GetValueOrDefault(Variables);

            while (VariableRegex.IsMatch(parsed))
            {
                parsed = TransformVariables(parsed);
            }

            return parsed;
        }

        private string ReplaceConditionals(Match match)
        {
            if (TrueStrings.Contains(match.Groups["expression"].Value.ToUpperInvariant()))
            {
                return match.Groups["content"].Value;
            }

            return string.Empty;
        }
    }
}