using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rox
{
    public class ServicesConfigureContext
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public ServicesConfigureContext(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }
    }
}
