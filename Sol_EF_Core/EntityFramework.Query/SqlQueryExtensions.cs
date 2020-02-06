using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Query
{
    public static class SqlQueryExtensions
    {
        public static async Task<IList<TResultSet>> SqlQueryAsync<TResultSet>(this DbContext dbContext, string sql) where TResultSet : class
        {
            try
            {
                using (var dbContextForQueryTypeObj = new ContextForQueryType<TResultSet>(dbContext.Database.GetDbConnection()))
                {
                    return await dbContextForQueryTypeObj.Query<TResultSet>().FromSql(sql).ToListAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IList<TResultSet>> SqlQueryAsync<TResultSet>(this DbContext dbContext, string sql, params object[] parameters) where TResultSet : class
        {
            try
            {
                using (var dbContextForQueryTypeObj = new ContextForQueryType<TResultSet>(dbContext.Database.GetDbConnection()))
                {
                    return await dbContextForQueryTypeObj.Query<TResultSet>().FromSql(sql, parameters).ToListAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IList<TResultSet>> SqlQueryAsync<TResultSet>(this DbContext dbContext, string sql, IEnumerable<SqlParameter> sqlParameters) where TResultSet : class
        {
            try
            {
                using (var dbContextForQueryTypeObj = new ContextForQueryType<TResultSet>(dbContext.Database.GetDbConnection()))
                {
                    return await dbContextForQueryTypeObj.Query<TResultSet>().FromSql(sql, sqlParameters.Cast<Object>().ToArray()).ToListAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<TMultipleResultSet> SqlQueryMultipleAsync<TMultipleResultSet>(
           this DbContext db,
           string sql,
           CommandType commandType,
           Func<DbDataReader, Task<TMultipleResultSet>> funcReaders
           )
           where TMultipleResultSet : class
        {
            DbConnection dbConnection = null;
            try
            {
                dbConnection = db.Database.GetDbConnection();

                if (dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken)
                {
                    await dbConnection.OpenAsync();
                }

                var dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandType = commandType;
                dbCommand.CommandText = sql;
                dbCommand.Connection = dbConnection;

                var dbReader = await dbCommand.ExecuteReaderAsync();

                var data = await funcReaders(dbReader);

                return data;
            }
            catch
            {
                throw;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public static async Task<TMultipleResultSet> SqlQueryMultipleAsync<TMultipleResultSet>(
          this DbContext db,
          string sql,
          Object[] parameters,
          CommandType commandType,
          Func<DbDataReader, Task<TMultipleResultSet>> funcReaders
          )
          where TMultipleResultSet : class
        {
            DbConnection dbConnection = null;
            try
            {
                dbConnection = db.Database.GetDbConnection();

                if (dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken)
                {
                    await dbConnection.OpenAsync();
                }

                var dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandType = commandType;
                dbCommand.CommandText = sql;
                dbCommand.Connection = dbConnection;

                if (parameters != null)
                {
                    dbCommand.Parameters.AddRange(parameters);
                }

                var dbReader = await dbCommand.ExecuteReaderAsync();

                var data = await funcReaders(dbReader);

                return data;
            }
            catch
            {
                throw;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public static async Task<TMultipleResultSet> SqlQueryMultipleAsync<TMultipleResultSet>(
        this DbContext db,
        string sql,
        IEnumerable<SqlParameter> sqlParameters,
        CommandType commandType,
        Func<DbDataReader, Task<TMultipleResultSet>> funcReaders
        )
        where TMultipleResultSet : class
        {
            DbConnection dbConnection = null;
            try
            {
                dbConnection = db.Database.GetDbConnection();

                if (dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken)
                {
                    await dbConnection.OpenAsync();
                }

                var dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandType = commandType;
                dbCommand.CommandText = sql;
                dbCommand.Connection = dbConnection;

                if (sqlParameters != null)
                {
                    dbCommand.Parameters.AddRange(sqlParameters.Cast<Object>().ToArray());
                }

                var dbReader = await dbCommand.ExecuteReaderAsync();

                var data = await funcReaders(dbReader);

                return data;
            }
            catch
            {
                throw;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public async static Task<List<TResultSet>> SelectReadAsync<TResultSet>(this DbDataReader dataReader)
        {
            try
            {
                List<TResultSet> list = new List<TResultSet>();
                TResultSet obj = default(TResultSet);

                while (await dataReader.ReadAsync())
                {
                    obj = Activator.CreateInstance<TResultSet>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (await dataReader.HasColumnAsync(prop.Name))
                        {
                            if (!object.Equals(dataReader[prop.Name], DBNull.Value))
                            {
                                prop.SetValue(obj, dataReader[prop.Name], null);
                            }
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch
            {
                throw;
            }
        }

        private static async Task<bool> HasColumnAsync(this DbDataReader dbDataReader, string columnName)
        {
            try
            {
                return await Task.Run(() =>
                {
                    for (int counter = 0; counter < dbDataReader.FieldCount; counter++)
                    {
                        if (dbDataReader.GetName(counter).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                            return true;
                    }
                    return false;
                });
            }
            catch
            {
                throw;
            }
        }

        private class ContextForQueryType<T> : DbContext where T : class
        {
            private readonly DbConnection connection;

            public ContextForQueryType(DbConnection connection)
            {
                this.connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // switch on the connection type name to enable support multiple providers
                // var name = con.GetType().Name;
                optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure());

                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Query<T>();
                base.OnModelCreating(modelBuilder);
            }
        }
    }
}