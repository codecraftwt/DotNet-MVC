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

        [DisplayName("Full Name")]
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

        [DisplayName("Employee Photo")]
        public string? Photo { get; set; }

        [DisplayName("Employment Date")]
        public DateTime? EmploymentDate { get; set; }

        public int? StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }

        public DateTime? InactiveDate { get; set; }

        public int? CauseofInactivityId { get; set; }
        public SystemCodeDetail CauseofInactivity { get; set; }

        public DateTime? TerminationDate { get; set; }

        public int? ReasonforterminationId { get; set; }
        public SystemCodeDetail Reasonfortermination { get; set; }

        [DisplayName("Bank Name")]
        public int? BankId { get; set; }
        public Bank Bank { get; set; }

        [DisplayName("Bank Account Number")]
        public string? BankAccountNo { get; set; }

        [DisplayName("International Bank Account Number")]
        public string? IBAN { get; set; }

        [DisplayName("SWIFT Code")]
        public string? SWIFTCode { get; set; }

        [DisplayName("N.S.S.F Number")]
        public string? NSSFNO { get; set; }

        [DisplayName("NHIF Number")]
        public string? NHIF { get; set; }

        [DisplayName("Company Email")]
        public string? CompanyEmail { get; set; }

        [DisplayName("KRA Pin")]
        public string? KRAPIN { get; set; }

        [DisplayName("Passport Number")]
        public string? PassportNo { get; set; }

        [DisplayName("Employment Term")]
        public int? EmploymentTermsId { get; set; }
        public SystemCodeDetail EmploymentTerms { get; set; }

        [DisplayName("Allocated Leave Days")]
        public Decimal? AllocatedLeaveDays { get; set; }
        [DisplayName("Leave Balance")]
        public Decimal? LeaveOutstandingBalance { get; set; }

    }
}
