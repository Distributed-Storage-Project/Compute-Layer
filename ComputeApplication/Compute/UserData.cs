using BCrypt.Net;
using System.Collections.Generic;

namespace ComputeLayer.Controllers
{
    public static class UserData
    {
        public static List<User> Users = new List<User>
        {
            new User { Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("myPassword123"), Role = "SystemAdmin" },
            new User { Username = "user", Password = BCrypt.Net.BCrypt.HashPassword("myPassword123"), Role = "User" },
            new User { Username = "Jiayue", Password = BCrypt.Net.BCrypt.HashPassword("jiayuechi123"), Role = "SystemAdmin" },
            new User { Username = "Ryan", Password = BCrypt.Net.BCrypt.HashPassword("ryanspassword"), Role = "User" }
        };
    }
}
