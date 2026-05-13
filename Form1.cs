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
