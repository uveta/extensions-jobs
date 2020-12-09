using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc.Services
{
    public class ControllerGenerator
    {
        private const string NAMESPACE = "Extensions.Jobs.Endpoints.Generated.Controllers";

        private readonly ModuleBuilder _moduleBuilder;
        private readonly HashSet<TypeInfo> _controllerTypes;

        private static ConcurrentDictionary<Type, Type> _alreadyGeneratedControllerTypes = new ConcurrentDictionary<Type, Type>();

        public ControllerGenerator()
            : this(Guid.NewGuid().ToString())
        {
        }

        public ControllerGenerator(string dynamicAssemblyName)
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(dynamicAssemblyName), AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule($"{assemblyBuilder.GetName().Name}.Module");
            _controllerTypes = new HashSet<TypeInfo>();
        }

        public IReadOnlyCollection<Type> ControllerTypes { get => _controllerTypes; }

        public void GenerateControllerType(Type controllerType, Type endpointType)
        {
            // Check, whether controller was already generated for given endPoint type
            if (_alreadyGeneratedControllerTypes.TryGetValue(endpointType, out Type? generatedControllerType))
            {
                _controllerTypes.Add(generatedControllerType.GetTypeInfo());
                return;
            }

            if (controllerType.IsGenericType)
            {
                foreach (Type genericParameterType in controllerType.GetGenericArguments())
                {
                    if (!genericParameterType.IsPublic)
                        throw new ArgumentException($"Type '{genericParameterType.FullName}' is not public");
                }
            }
            generatedControllerType = GenerateControllerType(NAMESPACE, GetGeneratedTypeName(controllerType), controllerType);
            if (generatedControllerType is null)
            { 
                throw new InvalidOperationException(
                    $"Unable to generate controller type {NAMESPACE}.{controllerType.Name}");
            }
            _alreadyGeneratedControllerTypes.TryAdd(endpointType, generatedControllerType);
        }

        private Type? GenerateControllerType(string nmspace, string name, Type baseControllerType)
        {
            var builder = CreateType(_moduleBuilder, baseControllerType, nmspace, name);
            CreateControllerConstructor(builder, baseControllerType);
            Type? controllerType = builder.CreateTypeInfo();
            if (controllerType is null) return null;
            _controllerTypes.Add(controllerType.GetTypeInfo());
            return controllerType;
        }

        private void CreateControllerConstructor(TypeBuilder typeBuilder, Type baseControllerType)
        {
            ConstructorInfo baseCtor = baseControllerType.GetConstructors().Single();
            Type[] parameterTypes = baseCtor.GetParameters().Select(x => x.ParameterType).ToArray();

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                parameterTypes);

            ConstructorInfo baseConstructor = baseControllerType.GetConstructor(parameterTypes);

            // Call base constructor
            ILGenerator il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            if (parameterTypes.Length >= 1)
                il.Emit(OpCodes.Ldarg_1);
            if (parameterTypes.Length >= 2)
                il.Emit(OpCodes.Ldarg_2);
            if (parameterTypes.Length >= 3)
                il.Emit(OpCodes.Ldarg_3);
            if (parameterTypes.Length > 3)
                throw new InvalidOperationException("Only 3 constructor parameters are suported now");

            il.Emit(OpCodes.Call, baseConstructor);
            il.Emit(OpCodes.Ret);
        }

        private TypeBuilder CreateType(ModuleBuilder modBuilder, Type baseType, string nmspace, string name)
        {
            return modBuilder.DefineType(
                $"{nmspace}.{name}",
                TypeAttributes.Public
                | TypeAttributes.Class
                | TypeAttributes.AutoClass
                | TypeAttributes.AnsiClass
                | TypeAttributes.BeforeFieldInit
                | TypeAttributes.AutoLayout,
                baseType,
                Type.EmptyTypes);
        }

        private static string GetGeneratedTypeName(Type type)
        {
            // Type.Name returns for generic types something like "MyType`3" for "MyType<int, string, char>"
            // For types having same name and same number of generic arguments, this returns the same name, which causes name conflicts during generating controller types
            // We need to make type name unique -> "MyType_[Guid]"
            string typeName = type.Name;
            int index = typeName.IndexOf('`');
            if (index >= 0)
                typeName = typeName.Substring(0, index); // "MyType`3" -> "MyType"

            string generatedTypeName = $"{typeName}_{Guid.NewGuid().ToString("N")}";
            return generatedTypeName;
        }
    }
}
