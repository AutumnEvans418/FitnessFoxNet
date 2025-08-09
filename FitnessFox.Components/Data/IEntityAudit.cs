namespace FitnessFox.Data
{
    public interface IEntityAudit
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
