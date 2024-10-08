namespace EmployeeManagement.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public String UserId { get; set; }
        public string AuditType { get; set; }
        public string Tablename { get; set; }
        public DateTime DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
    }

    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3,
        Login = 4,
    }
}