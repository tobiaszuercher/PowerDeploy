using System.Collections.Generic;
using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Environment
{
    [Route("/environments", "GET")]
    public class GetAllEnvironmentsRequest : IReturn<EnvironmentVariablesDto>
    {
    }

    [Route("/environments/{Name}", "GET")]
    public class GetEnvironmentRequest : IReturn<EnvironmentDto>
    {
        [ApiMember(Name = "Name", ParameterType = "query", DataType = "string")]
        public string Name { get; set; }
    }

    [Route("/environments/{Name}/variables", "GET")]
    public class GetEnvironmentWithVariablesRequest : IReturn<EnvironmentVariablesDto>
    {
        [ApiMember(Name = "Name", ParameterType = "query", DataType = "string")]
        public string Name { get; set; }
    }

    public class EnvironmentVariablesDto
    {
        public EnvironmentDto Environment { get; set; }
        public List<VariableDto> Variables { get; set; }
        public string Warning { get; set; }
        public List<string> MissingVariables { get; set; }
    }

    public class VariableDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Resolved { get; set; }
    }

    ////[Route("/environment", Verbs = "POST,PUT")]
    ////[Route("/environment/{Id}", Verbs = "POST,PUT")]
    public class EnvironmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}
