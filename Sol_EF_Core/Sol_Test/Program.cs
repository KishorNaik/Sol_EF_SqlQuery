using System;
using System.Threading.Tasks;

namespace Sol_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task.Run(async () => {

                try
                {
                   
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            
            }).Wait();

        }
    }
}
