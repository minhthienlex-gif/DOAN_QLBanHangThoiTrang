using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOAN_QLBanHangThoiTrang
{
    internal class DataContext : DbContext
    {
        public DataContext() : base("name=MyConnectionString")
        {
        }

        public DbSet<tblCustomers> Customers { get; set; }

        public DbSet<tblEmployees> Employees { get; set; }

        public DbSet<tblAccounts> Accounts { get; set; }

        public DbSet<tblCategories> Categories { get; set; }

        public DbSet<tblProducts> products { get; set; }

        public DbSet<tblBills> Bills { get; set; }

        public DbSet<tblBillDetails> BillDetails { get; set; }

        public DbSet<tblImportDetails> ImportDetails { get; set; }

        public DbSet<tblImports> Imports { get; set; }

    }
}
