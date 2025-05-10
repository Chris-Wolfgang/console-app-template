using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTemplate.Command
{
    internal class Sub3Command
    {

        public async Task<int> OnExecuteAsync()
        {
            Console.WriteLine("SubCommand3 executed");
            return await Task.FromResult(0);
        }
    }
}
