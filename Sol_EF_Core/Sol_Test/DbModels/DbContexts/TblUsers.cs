using System;
using System.Collections.Generic;

namespace Sol_Test.DbModels.DbContexts
{
    public partial class TblUsers
    {
        public decimal UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
    }
}
