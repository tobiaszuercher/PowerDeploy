namespace PowerDeploy.Core
{
    public interface IEnviornmentProvider
    {
        Environment GetEnvironment(string environmentName);

        Environment GetEnvironmentFromFile(string file);
    }
}