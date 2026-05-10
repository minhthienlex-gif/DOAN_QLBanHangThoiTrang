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
        string Username, FullName, Role;
        public frmMain()
        {
            InitializeComponent();
        }
        public frmMain( string un, string ro)
        {
            InitializeComponent();
            Username = un;
            Role = ro;
            
        }
    }
}
