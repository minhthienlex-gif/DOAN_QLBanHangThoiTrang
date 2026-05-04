using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOAN_QLBanHangThoiTrang
{
    [Table("BillDetails")]
    internal class tblBillDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillDetailID { get; set; }

        [Required]
        public int BillID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Total { get; set; }
    }
}
