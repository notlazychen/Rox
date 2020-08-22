using System;
using System.Collections.Generic;
using System.Text;

namespace Rox
{

    public class ApplicationInitializationContext
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public ApplicationInitializationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }

    public class ApplicationShutdownContext
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public ApplicationShutdownContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
