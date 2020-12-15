using System;
using System.Threading.Tasks;

namespace SampleApp
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            await Startup.RunAsync(args);
        }
    }
}
