using Sol_Test.DbModels.DbContexts;
using Sol_Test.ResultSets;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EntityFramework.Query;
using System.Linq;

namespace Sol_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Entity Framework!");

            Task.Run(async () => {

                try
                {

                    // Make instance of EfCoreContext
                    EFCoreContext efCoreContext = new EFCoreContext();

                    #region How to call and return join data from stored procedures in Entity Framework Core

                        try
                        {
                            decimal userId = 1; // Get Current User Data

                            // Make Sql Parameters
                            List<SqlParameter> sqlParameters = new List<SqlParameter>();
                            sqlParameters.Add(new SqlParameter("@UserId", userId));

                            // Set procedure name with parameters
                            String sqlCommand = "EXEC uspGetUsersJoins @UserId";

                            // get Join Data
                            var joinData =
                                (
                                    await
                                    efCoreContext
                                    ?.SqlQueryAsync<UsersJoinResultSetModel>(sqlCommand, sqlParameters)
                                )
                                ?.ToList();
                        }
                        catch
                        {
                            throw;
                        }

                    #endregion

                    #region Returning Multiple Result Sets from a Stored Procedure

                    try
                    {
                       

                        List<TblUsers> listUserModel = new List<TblUsers>();
                        List<TblUserLogin> listUserLoginModel = new List<TblUserLogin>();


                        decimal userId = 2; // Get Current User Data

                        // Make Sql Parameters
                        List<SqlParameter> sqlParameters1 = new List<SqlParameter>();
                        sqlParameters1.Add(new SqlParameter("@UserId", userId));

                        // Specify the procedure name with parameter
                        String sqlCommand = "uspGetUsersMultiResultSet";

                        // get Multiple Select query data
                        var getMultileSelectQueryData=
                            (
                                await
                                efCoreContext
                                .SqlQueryMultipleAsync<UsersMultipleResultSetModel>(
                                    sqlCommand,
                                    sqlParameters1,
                                    System.Data.CommandType.StoredProcedure,
                                    async (dbReaderObj) =>
                                    {
                                        // get First Result Set (First Select Query)
                                        while (await dbReaderObj.ReadAsync())
                                        {
                                            listUserModel
                                                .Add(new TblUsers()
                                                {
                                                    FirstName = Convert.ToString(dbReaderObj["FirstName"]),
                                                    LastName = Convert.ToString(dbReaderObj["LastName"])
                                                });
                                        }

                                        // get Next Result Set
                                        await dbReaderObj.NextResultAsync();

                                        // get Second Result Set (Second Select Query)
                                        while (await dbReaderObj.ReadAsync())
                                        {
                                            listUserLoginModel.Add(new TblUserLogin()
                                            {
                                                UserName = Convert.ToString(dbReaderObj["UserName"]),
                                                Password = Convert.ToString(dbReaderObj["Password"])
                                            });
                                        }

                                        // Map two lists Object into MultiResult Set Model
                                        return new UsersMultipleResultSetModel()
                                        {
                                            ListUsers = listUserModel,
                                            ListUserLogin = listUserLoginModel
                                        };
                                    }
                                )
                            );

                    }
                    catch
                    {
                        throw;
                    }

                    #endregion 

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            
            }).Wait();

        }
    }
}
