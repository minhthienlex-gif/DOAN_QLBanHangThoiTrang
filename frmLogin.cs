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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            frmAccounts f = new frmAccounts();
      
            string user = txtUsername.Text.Trim();
            string pass = f.GetMD5(txtPassword.Text.Trim());
            if(string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }
            using (var db = new DataContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.Username == user && a.Password == pass && a.Status == true);
         

                if (account != null)
                {
                    if(account.Status == false)
                    {
                        MessageBox.Show("Tài khoản của bạn đang bị khóa");
                        return;
                    }
                    frmMain fMain = new frmMain(account.Username,account.FullName, account.Role);
                    fMain.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu");
                }
            }
        }

     
    }
}
