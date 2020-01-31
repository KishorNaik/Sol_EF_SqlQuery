using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Test.Models
{
    public class UserModel
    {
        public decimal UserId { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public UserLoginModel UserLogin { get; set; }
    }
}
