namespace DataCockpit
{
    public abstract class ModelBase<TKey>
    {
        public virtual required TKey Id { get; set; }
    }
}