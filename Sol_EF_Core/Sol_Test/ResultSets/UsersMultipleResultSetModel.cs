using Sol_Test.DbModels.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Test.ResultSets
{
    public class UsersMultipleResultSetModel
    {
        public IEnumerable<TblUsers> ListUsers { get; set; }

        public IEnumerable<TblUserLogin> ListUserLogin { get; set; }
    }
}
