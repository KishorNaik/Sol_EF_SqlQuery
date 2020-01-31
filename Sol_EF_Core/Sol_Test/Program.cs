using Sol_Test.Repository;
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
                    UserRepository userRepository = new UserRepository(new DbModels.DbContexts.EFCoreContext());

                    var joinData =
                     await
                     userRepository
                         .GetUserJoinDataAsync();

                    var multipleData =
                        await
                           userRepository
                           .GetUserMultipleDataAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            
            }).Wait();

        }
    }
}
