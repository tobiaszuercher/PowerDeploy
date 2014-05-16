using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

        private static readonly Regex VariableRegex = new Regex(@"\$\{(?<Name>[^\}]+)\}", RegexOptions.Compiled);
        private static readonly Regex NormalizeRegex = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);
        private static readonly Regex ConditionalOpenRegex = new Regex(@"<!--\s*\[if\s*(?<expression>[^\]]*)]\s*-->", RegexOptions.Compiled);
        private static readonly Regex ConditionalCloseRegex = new Regex(@"<!--\s*\[endif]\s*-->", RegexOptions.Compiled);

        public VariableResolver(IList<Variable> variables)
        {
            Variables = variables;
            VariableUsageList = new List<VariableUsage>();
        }

        public string TransformVariables(string content)
        {
            var transformedVariables = VariableRegex.Replace(content, ReplaceVariables);
            
            return ParseConditional(transformedVariables);
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

        private string ParseConditional(string input)
        {
            // normalize line endings in order to be aple to split line by line
            var normalized = NormalizeRegex.Replace(input, e => "\r\n");

            var transformed = new List<string>();
            bool lastCondition = false;
            bool insideCondition = false;

            foreach (var line in normalized.Split(new [] { System.Environment.NewLine}, StringSplitOptions.None))
            {
                var match = ConditionalOpenRegex.Match(line);

                // we have a <!-- [if xxxx] -->
                if (match.Success)
                {
                    lastCondition = TrueStrings.Contains(match.Groups["expression"].Value.ToUpperInvariant());
                    insideCondition = true;
                }
                else if(ConditionalCloseRegex.IsMatch(line))
                {
                    insideCondition = false;
                }
                else if (lastCondition || !insideCondition)
                {
                    transformed.Add(line);
                }
            }

            return string.Join(System.Environment.NewLine, transformed);
        }
    }
}