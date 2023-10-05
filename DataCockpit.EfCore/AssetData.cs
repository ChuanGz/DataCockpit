using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataCockpit.EfCore
{
    [Table("Assets")]
    public class AssetData : EntityBase<string>
    {
        public string Name { get; set; }

        [Required]
        public int AssetType { get; set; }

    }
}