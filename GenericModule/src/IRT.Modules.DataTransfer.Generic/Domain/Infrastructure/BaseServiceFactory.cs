using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommonServiceLocator;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.Models;
using Kernel.Plugins;
using AttributeNotFoundException = IRT.Modules.DataTransfer.Generic.Domain.Exceptions.AttributeNotFoundException;

namespace IRT.Modules.DataTransfer.Generic.Domain.Infrastructure
{
    public abstract class BaseServiceFactory<T> : IServiceFactory
    {
        protected readonly IPluginsResolver PluginsResolver;
        protected readonly Lazy<Dictionary<string, ServiceMetadata>> services;
        protected readonly IServiceProvider serviceProvider;

        protected BaseServiceFactory(IPluginsResolver pluginsResolver, IServiceProvider serviceProvider)
        {
            PluginsResolver = pluginsResolver;
            services = new Lazy<Dictionary<string, ServiceMetadata>>(Initialize, true);
            this.serviceProvider = serviceProvider;
        }

        private Dictionary<string, ServiceMetadata> Initialize()
        {
            try
            {
                var concreteImpl = PluginsResolver.GetRegistrationsFor<T>()
                    .Where(x => !x.Value.IsAbstract)
                    .ToDictionary(
                        x => x.Key,
                        x =>
                        {
                            var serviceId = x.Key;
                            var displayName = GetDisplayName(serviceId, x.Value);

                            return new ServiceMetadata(serviceId, displayName);
                        });

                return concreteImpl;
            }
            catch (KeyNotFoundException)
            {
                // KeyNotFoundException is thrown by IPluginsResolver.GetRegistrationsFor when
                // there are no registrations for a provided plugin type.
                return new Dictionary<string, ServiceMetadata>();
            }
        }

        protected virtual string GetDisplayName(string serviceId, Type type)
        {
            var descriptionAttribute = type.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute is null ?
                throw new AttributeNotFoundException(typeof(DescriptionAttribute), type)
                : descriptionAttribute.Description;
        }

        public virtual IEnumerable<ServiceMetadata> GetServices() => services.Value.Values;

        public virtual bool TryGetServiceInstance(string typeName, out T instance)
        {
            instance = default;

            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            if (services == null || services.Value == null)
            {
                return false;
            }

            // Check if the provider exists in the dictionary
            if (!services.Value.ContainsKey(typeName))
            {
                return false;
            }

            // Get the plugin type using the providerTypeName
            var pluginType = PluginsResolver.GetPluginType(typeName);
            if (pluginType == null)
            {
                return false;
            }

            // Attempt to get the instance of the provider using ServiceLocator
            instance = (T)ServiceLocator.Current.GetInstance(pluginType);

            return instance != null;
        }
    }
}
