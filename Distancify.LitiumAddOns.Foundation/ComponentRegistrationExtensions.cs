using Litium.Owin.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.Foundation
{
    public static class ComponentRegistrationExtensions
    {
        public static LifeStyleComponentRegistration ImplementedBy(this ComponentRegistration registration, Type type)
        {
            return registration.UsingFactoryMethod(container =>
            {
                var item = container.Resolve(type, false);
                if (item != null)
                    return item;

                var constructors = type.GetConstructors().OrderByDescending(r => r.GetParameters().Count());

                foreach (var c in constructors)
                {
                    var parameters = c.GetParameters().Select(p =>
                    {
                        if (container.HasComponent(p.ParameterType))
                        {
                            return container.Resolve(p.ParameterType, false);
                        }

                        var enumerable = p.ParameterType.GetInterfaces()
                            .FirstOrDefault(
                                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                        if (enumerable == null && p.ParameterType.IsInterface && p.ParameterType.IsGenericType &&
                            p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        {
                            enumerable = p.ParameterType;
                        }

                        if (enumerable != null)
                        {
                            return container.ResolveAll(enumerable.GenericTypeArguments[0]);
                        }
                        return null;
                    }).Where(r => r != null).ToArray();

                    if (parameters.Length != c.GetParameters().Length)
                    {
                        // Could not resolve all parameters. Skip constructor.
                        continue;
                    }

                    return c.Invoke(parameters);
                }

                throw new Exception("Could not find any constructor on type '" + type.Name + "' where all dependencies could be satisfied.");
            });
        }

        public static LifeStyleComponentRegistration ImplementedBy<T>(this ComponentRegistration registration)
        {
            return registration.ImplementedBy(typeof(T));
        }
    }
}
