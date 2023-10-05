using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataCockpit.EfCore
{
    public class EntityBase<TKey>
    {
        [Key]
        public required TKey Id { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class EntityBase<TKey1, TKey2>
    {
        [Key, Column(Order = 0)]
        public required TKey1 Key1 { get; set; }

        [Key, Column(Order = 1)]
        public required TKey2 Key2 { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}