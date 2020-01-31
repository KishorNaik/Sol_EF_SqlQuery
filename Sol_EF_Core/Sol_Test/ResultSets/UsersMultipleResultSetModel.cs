using Sol_Test.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Test.ResultSets
{
    public class UsersMultipleResultSetModel
    {
        public IEnumerable<UserModel> ListUsers { get; set; }

        public IEnumerable<UserLoginModel> ListUserLogin { get; set; }
    }
}
