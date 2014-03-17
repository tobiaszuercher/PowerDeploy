using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Route("/environments")]
    public class QueryEnvironment : IReturn<Environment>
    {
        [ApiMember(Name = "Id", ParameterType = "query", DataType = "integer")]
        public int Id { get; set; }
    }
}
