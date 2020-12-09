using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Uveta.Extensions.Jobs.Endpoints.Extensions;
using Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc
{
    public class JobControllerModelConvention : IControllerModelConvention
    {
        private readonly IDictionary<Type, EndpointConfiguration> _configurations =
            new Dictionary<Type, EndpointConfiguration>();

        public JobControllerModelConvention(IEnumerable<ControllerEndpoint> controllers)
        {
            foreach (var controller in controllers)
            {
                _configurations[controller.Endpoint] = controller.Configuration;
            }
        }

        public void Apply(ControllerModel controller)
        {
            var type = controller.ControllerType;
            if (!type.IsSubclassOfGeneric(typeof(JobsController<,,>))) return;
            Type[] genericArguments = type.GetBaseGenericArguments(typeof(JobsController<,,>));
            var endpointType = genericArguments[0];
            var configuration = FindByGenericTypes(endpointType);
            if (configuration is null) return;
            controller.ControllerName = configuration.Name;
        }

        private EndpointConfiguration? FindByGenericTypes(Type endpointType)
        {
            if (!_configurations.ContainsKey(endpointType)) return null;
            return _configurations[endpointType];
        }
    }
}
