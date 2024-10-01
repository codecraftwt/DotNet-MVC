namespace EmployeeManagement.Models
{
    public class Bank : UserActivity
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public int AccountNo { get; set; }
    }
}
