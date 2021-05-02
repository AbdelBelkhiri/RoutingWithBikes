using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace HostingWPS
{
    class Program
    {
        // Host the service within this EXE console application.  
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type.  
            using (ServiceHost serviceHost =
                   new ServiceHost(typeof(WebProxyService.WebProxyService)))
            {
                // Open the ServiceHost to create listeners
                // and start listening for messages.  
                serviceHost.Open();

                // The service can now be accessed.  
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
