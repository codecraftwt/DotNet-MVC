using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class UserActivity
    {
        public string CreatedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedOn { get; set; }
        public string ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ModifiedOn { get; set; }
    }

    public class ApprovalActivity : UserActivity
    {
        public string ApprovedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ApprovedOn { get; set; }
    }
}
