using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace warehouse_service.Models
{
    public class Reagent
    {
        [Key]
        [MaxLength(20)]
        [Column("id", TypeName = "nvarchar(20)")]
        public string Id { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("reagent_code", TypeName = "varchar(50)")]
        public string ReagentCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("reagent_name", TypeName = "nvarchar(100)")]
        public string ReagentName { get; set; } = null!;

        [MaxLength(100)]
        [Column("manufacturer", TypeName = "nvarchar(100)")]
        public string? Manufacturer { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("import_date", TypeName = "date")]
        public DateTime ImportDate { get; set; }

        [Column("expiry_date", TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(100)]
        [Column("status", TypeName = "nvarchar(50)")]
        public string? Status { get; set; }

        [MaxLength(255)]
        [Column("description", TypeName = "nvarchar(255)")]
        public string? Description { get; set; }

        [MaxLength(50)]
        [Column("import_lot", TypeName = "nvarchar(50)")]
        public string? ImportLot { get; set; }

        [MaxLength(20)]
        [Column("unit", TypeName = "nvarchar(20)")]
        public string? Unit { get; set; }
    }
}
