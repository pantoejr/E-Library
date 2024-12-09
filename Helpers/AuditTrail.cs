namespace E_Library.Helpers
{
    public class AuditTrail
    {
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
}
