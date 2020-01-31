using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Query
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

