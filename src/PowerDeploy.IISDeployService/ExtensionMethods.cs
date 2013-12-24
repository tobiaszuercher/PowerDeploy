using System;

using PowerDeploy.IISDeployService.Contract;

namespace PowerDeploy.IISDeployService
{
    public static class ExtensionMethods
    {
        public static string ToVersionNumber(this RuntimeVersion target)
        {
            switch (target)
            {
                case RuntimeVersion.Version11:
                    return "1.1";
                case RuntimeVersion.Version20:
                    return "2.0";
                case RuntimeVersion.Version40:
                    return "4.0";
                case RuntimeVersion.Version45:
                    return "4.5";

                default:
                    throw new ArgumentException("Given RuntimeVersion is not supported.");
            }
        }
    }
}