namespace PowerDeploy.Core
{
    public interface IEnviornmentProvider
    {
        Environment GetVariables(string environmentName);
    }
}