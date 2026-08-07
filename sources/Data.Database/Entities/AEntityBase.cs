using System.ComponentModel.DataAnnotations;

namespace Data.Database.Entities
{
    public class AEntityBase
    {
        [Key]
        public int Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
