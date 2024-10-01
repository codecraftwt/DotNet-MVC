using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
