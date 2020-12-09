using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Uveta.Extensions.Jobs.Endpoints.Mvc.Services;
using Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc
{
    public class JobsApplicationPart : ApplicationPart, IApplicationPartTypeProvider
    {
        private readonly List<TypeInfo> _types = new List<TypeInfo>();

        public JobsApplicationPart(IEnumerable<ControllerEndpoint> controllers)
        {
            ControllerGenerator controllerGenerator = new ControllerGenerator();
            foreach (var controller in controllers)
            {
                var controllerType = typeof(JobsController<,,>).MakeGenericType(
                    controller.Endpoint,
                    controller.Input,
                    controller.Output);
                controllerGenerator.GenerateControllerType(controllerType, controller.Endpoint);
            }
            _types.AddRange(controllerGenerator.ControllerTypes.Select(x => x.GetTypeInfo()));
        }

        public override string Name => "JobsController";

        public IEnumerable<TypeInfo> Types => _types;
    }
}
