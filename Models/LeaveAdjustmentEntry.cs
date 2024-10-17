namespace EmployeeManagement.Models
{
    public class LeaveAdjustmentEntry
    {
        public int Id { get; set; }
        public string LeavePeriod { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public Decimal NoOfDays { get; set; }
        public DateTime LeaveAdjustmentDays { get; set; }
        public DateTime? LeaveStartDate { get; set; }
        public DateTime? LeaveEndDate { get; set; }
        public string AdjustmentDescription { get; set; }
        public int AdjustmentTypeId { get; set; }
        public SystemCodeDetail AdjustmentType { get; set; }
    }
}
