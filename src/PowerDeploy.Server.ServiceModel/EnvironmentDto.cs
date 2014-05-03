using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Route("/environment", Verbs = "POST,PUT")]
    [Route("/environment/{Id}", Verbs = "POST,PUT")]
    public class EnvironmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }

    [Route("/environment", Verbs = "GET")]
    [Route("/environment/{Id}", Verbs = "GET")]
    public class QueryEnvironment : IReturn<EnvironmentDto>
    {
        [ApiMember(Name = "Id", ParameterType = "query", DataType = "integer")]
        public int Id { get; set; }
    }

    [Route("/environment", Verbs = "DELETE")]
    [Route("/environment/{Id}", Verbs = "DELETE")]
    public class DeleteEnvironment : IReturnVoid
    {
        [ApiMember(Name = "Id", ParameterType = "query", DataType = "integer")]
        public int Id { get; set; }
    }
}
