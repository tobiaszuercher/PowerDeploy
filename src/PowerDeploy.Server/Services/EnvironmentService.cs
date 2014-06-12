using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerDeploy.Core;
using PowerDeploy.Core.Template;
using PowerDeploy.Server.Mapping;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Environment;
using Raven.Client;

using ServiceStack;
using Environment = PowerDeploy.Server.Model.Environment;

namespace PowerDeploy.Server.Services
{
    public class EnvironmentService : Service
    {
        public IDocumentStore DocumentStore { get; set; }
        public IFileSystem FileSystem { get; set; }
        public ServerSettings ServerSettings { get; set; }

        public List<EnvironmentDto> Get(GetAllEnvironmentsRequest request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                return session.Query<Environment>().OrderBy(e => e.Order).ToList().Select(e => e.ToDto()).ToList();
            }
        }

        public EnvironmentDto Get(GetEnvironmentRequest request)
        {
            return GetEnvironment(request.Name, false).Environment;
        }

        public EnvironmentVariablesDto Get(GetEnvironmentWithVariablesRequest request)
        {
            return GetEnvironment(request.Name, true);
        }

        private EnvironmentVariablesDto GetEnvironment(string name, bool fetchVariables)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var environment = session.Query<Environment>().FirstOrDefault(e => e.Name == name);

                if (environment == null)
                {
                    throw HttpError.NotFound("Environment {0} not found.".Fmt(name));
                }

                using (var workspace = new Workspace(FileSystem, ServerSettings))
                {
                    var result = new EnvironmentVariablesDto()
                    {
                        Environment = environment.ToDto()
                    };

                    if (fetchVariables)
                    {
                        workspace.UpdateSources();

                        var provider = new EnvironmentProvider();

                        try
                        {
                            var serializedEnvironment = provider.GetEnvironmentFromFile(Path.Combine(workspace.EnviornmentPath, name + ".xml"));


                            var resolver = new VariableResolver(serializedEnvironment.Variables);

                            result.Variables = new List<VariableDto>();
                            result.Variables.AddRange(serializedEnvironment.Variables.Select(v => new VariableDto()
                            {
                                Name = v.Name,
                                Value = v.Value,
                                Resolved = resolver.TransformVariables(v.Value)
                            }));
                        }
                        catch (FileNotFoundException e)
                        {
                            result.Variables = new List<VariableDto>();
                            // todo: think about what to send back if there was no xml file for it.
                        }
                        
                    }

                    return result;
                }
            }
        }

        public EnvironmentDto Put(EnvironmentDto request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var environment = session.Load<Environment>("Environments/" + request.Id);
                environment.PopulateWith(request);

                session.SaveChanges();

                return environment.ToDto();
            }
        }

        public EnvironmentDto Post(EnvironmentDto request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(request);
                session.SaveChanges();

                return request;
            }
        }

        ////public void Delete(DeleteEnvironment request)
        ////{
        ////    using (var session = DocumentStore.OpenSession())
        ////    {
        ////        session.Advanced.DocumentStore.DatabaseCommands.Delete("Environments/" + request.Id, null);
        ////        session.SaveChanges();
        ////    }
        ////}

    }
}