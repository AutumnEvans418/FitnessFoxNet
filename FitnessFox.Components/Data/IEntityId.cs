namespace FitnessFox.Data
{
    public interface IEntityId<T>
    {
        public T Id { get; set; }
    }
}
