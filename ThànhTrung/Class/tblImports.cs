using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOAN_QLBanHangThoiTrang
{
    [Table("Imports")]
    internal class tblImports
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImportID { get; set; }

        public int? EmployeeID { get; set; }

        public DateTime? ImportDate { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? TotalAmount { get; set; }
    }
}
