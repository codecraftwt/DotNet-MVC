using EmployeeManagement.Models;
using System.ComponentModel;

namespace EmployeeManagement.ViewModels
{
    public class ProfileViewModel
    {
        public ICollection<SystemProfile> Profiles { get; set; }

        [DisplayName("Role")]
        public string RoleId { get; set; }

        [DisplayName("System Task")]
        public int TaskId { get; set; }
    }
}
