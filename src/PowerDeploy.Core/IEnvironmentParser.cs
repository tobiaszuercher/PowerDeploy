namespace PowerDeploy.Core
{
    public interface IEnvironmentParser
    {
        Environment GetEnvironment(string environmentName);

        Environment GetEnvironmentFromFile(string file);
    }
}