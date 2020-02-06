using System;
using System.Collections.Generic;

namespace Sol_Test.DbModels.DbContexts
{
    public partial class TblUserLogin
    {
        public decimal UserLoginId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public decimal? UserId { get; set; }
    }
}
