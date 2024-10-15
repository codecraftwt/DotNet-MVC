using System.ComponentModel;

namespace EmployeeManagement.Models
{
    public class Employee : UserActivity
    {
        public int Id { get; set; }
        public String EmpNo { get; set; }

        [DisplayName("First Name")]
        public String FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        [DisplayName("Phone Number")]
        public int PhoneNumber { get; set; }

        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }

        [DisplayName("Country")]
        public int? CountryId { get; set; }
        public Country Country { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }
        public Designation Designation { get; set; }

        [DisplayName("Gender")]
        public int? GenderId { get; set; }
        public SystemCodeDetail Gender { get; set; }
    }
}
