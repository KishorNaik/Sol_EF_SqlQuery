using Sol_Test.DbModels.DbContexts;
using Sol_Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EntityFrameworkCore.Query;
using Sol_Test.ResultSets;

namespace Sol_Test.Repository
{
    public class UserRepository
    {
        private readonly EFCoreContext efCoreContext = null;

        public UserRepository(EFCoreContext eFCoreContext)
        {
            this.efCoreContext = eFCoreContext;
        }

        public async Task<IEnumerable<UserModel>> GetUserJoinDataAsync()
        {
            try
            {
                String sqlCommand = "EXEC uspGetUsersJoins";

                return
                    (await
                    efCoreContext
                    .SqlQueryAsync<UsersJoinResultSetModel>(sqlCommand)
                    )
                    ?.Select((userJoinResultSetModelObj) => new UserModel()
                    {
                        FirstName = userJoinResultSetModelObj?.FirstName,
                        LastName = userJoinResultSetModelObj.LastName,
                        UserLogin = new UserLoginModel()
                        {
                            UserName = userJoinResultSetModelObj?.UserName,
                            Password = userJoinResultSetModelObj?.Password
                        }
                    })
                    ?.ToList();

            }
            catch
            {
                throw;
            }
        }

        public async Task<UsersMultipleResultSetModel> GetUserMultipleDataAsync()
        {
            try
            {
                String sqlCommand = "uspGetUsersMultiResultSet";

                List<UserModel> listUserModel = new List<UserModel>();
                List<UserLoginModel> listUserLoginModel = new List<UserLoginModel>();

                return
                    (await
                    efCoreContext
                    .SqlQueryMultipleAsync<UsersMultipleResultSetModel>(
                        sqlCommand,
                        System.Data.CommandType.StoredProcedure,
                        async (dbReaderObj) =>
                        {
                            while (await dbReaderObj.ReadAsync())
                            {
                                listUserModel
                                    .Add(new UserModel()
                                    {
                                        FirstName = Convert.ToString(dbReaderObj["FirstName"]),
                                        LastName = Convert.ToString(dbReaderObj["LastName"])
                                    });
                            }

                            await dbReaderObj.NextResultAsync();

                            while (await dbReaderObj.ReadAsync())
                            {
                                listUserLoginModel.Add(new UserLoginModel()
                                {
                                    UserName = Convert.ToString(dbReaderObj["UserName"]),
                                    Password = Convert.ToString(dbReaderObj["Password"])
                                });
                            }

                            return new UsersMultipleResultSetModel()
                            {
                                ListUsers = listUserModel,
                                ListUserLogin = listUserLoginModel
                            };
                        }
                        )
                    );
                    
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
