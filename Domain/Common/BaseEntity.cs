namespace Domain.Common
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get;set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public long LastUpdatedBy
        {
            get; set;
        }
        protected BaseEntity(TId id) => Id = id;
    }
}
