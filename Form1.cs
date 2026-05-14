using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOAN_QLBanHangThoiTrang
{
    public partial class frmMain : Form
    {
        string Username, Fullname, Role;
        private void OpenForm(Form frm)
        {
            frm.MdiParent = this;
            frm.Show();
        }
        private void mn2Account_Click(object sender, EventArgs e)
        {
            OpenForm(new frmAccounts());
        }

        private void mn2Employee_Click(object sender, EventArgs e)
        {
            OpenForm(new frmEmployees());
        }

        private void mn2Product_Click(object sender, EventArgs e)
        {
            OpenForm(new frmProducts());
        }

        private void mn2Categorie_Click(object sender, EventArgs e)
        {
            OpenForm(new frmCategories());
        }

        private void mn2Customer_Click(object sender, EventArgs e)
        {
            OpenForm(new frmCustomers());
        }

        private void mn2Bill_Click(object sender, EventArgs e)
        {
            OpenForm(new frmBills());
        }

        private void mn2BillDetail_Click(object sender, EventArgs e)
        {
            OpenForm(new frmBillDetails());
        }

        private void mn2Import_Click(object sender, EventArgs e)
        {
            OpenForm(new frmImports());
        }

        private void mn2ImportDetail_Click(object sender, EventArgs e)
        {
            OpenForm(new frmImportDetails());
        }

        private void mn2Exit_Click(object sender, EventArgs e)
        {
            this.Close();
            frmLogin loginForm = new frmLogin();
            loginForm.Show();
        }

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain( string un,string fn, string ro)
        {
            InitializeComponent();
            Username = un;
            Fullname = fn;
            Role = ro;
            this.Text = "Phần mềm quản lý bán hàng thời trang [" + Fullname + "]";


        }
    }
}
