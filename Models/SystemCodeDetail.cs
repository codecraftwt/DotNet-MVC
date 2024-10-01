using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class SystemCodeDetail : UserActivity
    {
        [Key]
        public int Id { get; set; }
        public int SystemCodeId { get; set; }
        public SystemCode SystemCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string OrderNo { get; set; }
    }
}
