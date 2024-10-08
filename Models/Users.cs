using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Models
{
    public class Users : ApplicationUser
    {
        public string FullName { get; set; }
    }
}
