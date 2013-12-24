using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PowerDeploy.Core.Template
{
    public class VariableResolver
    {
        private IList<Variable> Variables { get; set; }

        public IList<VariableUsage> VariableUsageList { get; private set; }

        private readonly Regex VariableRegex = new Regex(@"\$\{(?<Name>[^\}]+)\}", RegexOptions.Compiled);

        public VariableResolver(IList<Variable> variables)
        {
            Variables = variables;
            VariableUsageList = new List<VariableUsage>();
        }

        public string TransformVariables(string content)
        {
            return VariableRegex.Replace(content, ReplaceVariables);
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
    }
}