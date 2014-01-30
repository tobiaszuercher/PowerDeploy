namespace PowerDeploy.Core
{
    public interface IEnvironmentSerializer
    {
        Environment Deserialize(string file);
    }
}