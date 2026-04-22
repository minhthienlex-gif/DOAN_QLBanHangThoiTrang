using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOAN_QLBanHangThoiTrang
{
    public partial class frmAccounts : Form
    {
        Datacontext db = new Datacontext();
        bool Addnew = false;
        public frmAccounts()
        {
            InitializeComponent();
        }
        private void SetContol(bool check)
        {
            txtAccountID.Enabled = false;
            txtUsername.Enabled = check;
            txtPassword.Enabled = check;
            cbbRole.Enabled = check;
            ckbStatus.Enabled = check;
            dtpCreatedDate.Enabled = check;
            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvAccounts.Enabled = !check;
        }
        private void LoadGridData()
        {
            var data = from a in db.Accounts
                       select a;

            dgvAccounts.DataSource = data.ToList();
            SetContol(false);
        }
        private void frmAccounts_Load(object sender, EventArgs e)
        {
            dgvAccounts.AutoGenerateColumns = false;
            dgvAccounts.AllowUserToAddRows = false;

            LoadGridData();
        }
        public string GetMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Addnew = true;
            SetContol(true);
            txtAccountID.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cbbRole.Text = "Staff";
            dtpCreatedDate.Value = DateTime.Today;
            ckbStatus.Checked = true;
            txtUsername.Focus();
        }

        private void dgvAccounts_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;
            txtAccountID.Text = dgvAccounts.Rows[i].Cells["AccountID"].Value.ToString();
            txtUsername.Text = dgvAccounts.Rows[i].Cells["Username"].Value.ToString();
            txtPassword.Text = dgvAccounts.Rows[i].Cells["Password"].Value.ToString();
            cbbRole.Text = dgvAccounts.Rows[i].Cells["Role"].Value.ToString();
            dtpCreatedDate.Value = Convert.ToDateTime(dgvAccounts.Rows[i].Cells["CreatedDate"].Value);
            if ((bool)dgvAccounts.Rows[i].Cells["Status"].Value == true)
            {
                ckbStatus.Checked = true;
                ckbStatus.Text = "Tài khoản đã mở khóa";
            }
            else
            {
                ckbStatus.Checked = false;
                ckbStatus.Text = "Tài khoản đã đóng";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dgvAccounts.CurrentRow == null) return;
            if(MessageBox.Show("Bạn muốn xóa bản ghi này không", "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvAccounts.CurrentRow.Cells["AccountID"].Value;

                var AccountDelete = db.Accounts.SingleOrDefault(u => u.AccountID == id);

                if (AccountDelete != null)
                {
                    db.Accounts.Remove(AccountDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Addnew = false;
            SetContol(true);
            txtUsername.Enabled = true;
            txtUsername.Focus();
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            SetContol(false);
            if (dgvAccounts.CurrentRow != null)
            {
                int rowIndex = dgvAccounts.CurrentRow.Index;
                int colIndex = dgvAccounts.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvAccounts_CellEnter(dgvAccounts, args);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "")
            {
                MessageBox.Show("Tên khách hàng không được để trống!", "Thông báo");
                txtUsername.Focus();
                return;
            }

            if (Addnew)
            {
                string inputUsername = txtUsername.Text.Trim();
                bool isExisted = db.Accounts.Any(u => u.Username == inputUsername);
                if (isExisted)
                {
                    MessageBox.Show("Tên đăng nhập này đã tồn tại! Vui lòng chọn tên khác.",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }
                tblAccounts newAccounts = new tblAccounts
                {
                    Username = txtUsername.Text.Trim(),
                    Password = GetMD5(txtPassword.Text.Trim()),
                    Role = cbbRole.Text.Trim(),
                    Status = ckbStatus.Checked,
                    CreatedDate = dtpCreatedDate.Value
                };
                db.Accounts.Add(newAccounts);
                db.SaveChanges();
                LoadGridData();
            }
            else
            {
                if (dgvAccounts.CurrentRow == null) return;
                int id = int.Parse(txtAccountID.Text);
                tblAccounts accountsUpdate = db.Accounts.SingleOrDefault(a => a.AccountID == id);
                if (accountsUpdate != null) 
                { 
                    accountsUpdate.Username = txtUsername.Text.Trim();
                    accountsUpdate.Role = cbbRole.Text.Trim();
                    accountsUpdate.Status = ckbStatus.Checked;
                    accountsUpdate.CreatedDate = dtpCreatedDate.Value;

                    db.SaveChanges();
                    LoadGridData();
                }

            }
        }
    }
}
