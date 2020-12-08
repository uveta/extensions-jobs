using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Uveta.Extensions.Jobs.Endpoints.Extensions;
using Uveta.Extensions.Jobs.Endpoints;
using Uveta.Extensions.Jobs.Endpoints.Mvc.Services;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc
{
    public class JobsApplicationPart : ApplicationPart, IApplicationPartTypeProvider
    {
        private readonly List<TypeInfo> _types = new List<TypeInfo>();

        public JobsApplicationPart(IEnumerable<EndpointConfiguration> configurations)
        {
            ControllerGenerator controllerGenerator = new ControllerGenerator();
            foreach (var configuration in configurations)
            {
                var type = configuration.GetType();
                if (!type.IsSubclassOfGeneric(typeof(EndpointConfiguration<>))) return;
                var configurationGenericArguments = type.GetBaseGenericArguments(typeof(EndpointConfiguration<>));
                var endpointType = configurationGenericArguments[0];
                if (!endpointType.IsSubclassOfGeneric(typeof(Endpoint<,,>)))
                    throw new InvalidOperationException($"{endpointType.FullName} does not inherit from {typeof(Endpoint<,,>).FullName}");
                var controllerGenericArguments = endpointType.GetBaseGenericArguments(typeof(Endpoint<,,>));
                var controllerType = typeof(JobsController<,,>).MakeGenericType(controllerGenericArguments);
                controllerGenerator.GenerateControllerType(controllerType, endpointType);
            }
            _types.AddRange(controllerGenerator.ControllerTypes.Select(x => x.GetTypeInfo()));
        }

        public override string Name => "JobsController";

        public IEnumerable<TypeInfo> Types => _types;
    }
}
