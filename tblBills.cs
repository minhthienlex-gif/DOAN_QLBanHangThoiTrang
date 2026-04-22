using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOAN_QLBanHangThoiTrang
{
    [Table("Bills")]
    internal class tblBills
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public DateTime BillDate { get; set; }

        public int DiscountPercent { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
