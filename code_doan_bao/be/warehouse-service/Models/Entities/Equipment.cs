using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace warehouse_service.Models.Entities
{
    public class Equipment
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = null!;


        [MaxLength(100)]
        [Column("manufacturer", TypeName = "varchar(100)")]
        public string? Manufacturer { get; set; }

        // corresponds to database column [code_manufacturer] NVARCHAR(50)
        [MaxLength(50)]
        [Column("code_manufacturer", TypeName = "nvarchar(50)")]
        public string? CodeManufacturer { get; set; }

        [Required]
        [MaxLength(50)]
    [Column("status", TypeName = "nvarchar(50)")]
    public string Status { get; set; } = null!;

        [Column("last_maintenance_date", TypeName = "date")]
        public DateTime? LastMaintenanceDate { get; set; }

        [Required]
        [MaxLength(100)]
    [Column("equipment_name", TypeName = "nvarchar(100)")]
    public string EquipmentName { get; set; } = null!;

        [MaxLength(255)]
        [Column("description", TypeName = "nvarchar(255)")]
        public string? Description { get; set; }
    }
}