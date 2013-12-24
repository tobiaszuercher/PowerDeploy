using System;

namespace PowerDeploy.Core.Logging
{
    /// <summary>
    /// Factory to create ILog instances
    /// </summary>
    /// /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public interface ILogFactory
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILog GetLogger(Type type);

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILog GetLogger(string typeName);
    }
}