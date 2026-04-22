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
    public partial class frmEmployees : Form
    {
        Datacontext db = new Datacontext();
        bool Addnew = false;
        public frmEmployees()
        {
            InitializeComponent();
        }
        private void SetContol(bool check)
        {
            txtEmployeeID.Enabled = false;
            txtFullName.Enabled = check;
            cbbAccountID.Enabled = check;
            txtPhone.Enabled = check;
            txtBirthDate.Enabled = check;
            txtAddress.Enabled = check;
            txtGender.Enabled = check;
            dtpCreatedDate.Enabled = check;
            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvEmployees.Enabled = !check;
        }
        private void LoadAccount()
        {
            var data = from a in db.Accounts
                       select a;

            cbbAccountID.DataSource = data.ToList();
            cbbAccountID.DisplayMember = "Username"; 
            cbbAccountID.ValueMember = "AccountID";   
        }
        private void LoadGridData()
        {
            var data = from e in db.Employees
                       select e;

            dgvEmployees.DataSource = data.ToList();
            SetContol(false);
        }
        private void frmEmployees_Load(object sender, EventArgs e)
        {
            dgvEmployees.AutoGenerateColumns = false;
            dgvEmployees.AllowUserToAddRows = false;

            LoadGridData();
            LoadAccount();
        }
        private void dgvEmployees_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;
            txtEmployeeID.Text = dgvEmployees.Rows[i].Cells["EmployeeID"].Value.ToString();
            txtFullName.Text = dgvEmployees.Rows[i].Cells["Fullname"].Value.ToString();
            cbbAccountID.Text = dgvEmployees.Rows[i].Cells["AccountID"].Value.ToString();
            txtPhone.Text = dgvEmployees.Rows[i].Cells["Phone"].Value.ToString();
            txtBirthDate.Text = dgvEmployees.Rows[i].Cells["BirthDate"].Value.ToString();
            txtAddress.Text = dgvEmployees.Rows[i].Cells["Address"].Value.ToString();
            txtGender.Text = dgvEmployees.Rows[i].Cells["Gender"].Value.ToString();
            dtpCreatedDate.Value = Convert.ToDateTime(dgvEmployees.Rows[i].Cells["CreatedDate"].Value);
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            SetContol(false);
            if (dgvEmployees.CurrentRow != null)
            {
                int rowIndex = dgvEmployees.CurrentRow.Index;
                int colIndex = dgvEmployees.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvEmployees_CellEnter(dgvEmployees, args);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) return;
            if (MessageBox.Show("Bạn muốn xóa bản ghi này không", "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvEmployees.CurrentRow.Cells["EmployeeID"].Value;

                var EmployeeDelete = db.Employees.SingleOrDefault(u => u.EmployeeID == id);

                if (EmployeeDelete != null)
                {
                    db.Employees.Remove(EmployeeDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Addnew = false;
            SetContol(true);
            txtFullName.Enabled = true;
            txtFullName.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Addnew = true;
            SetContol(true);
            txtEmployeeID.Clear();
            txtFullName.Clear();
            cbbAccountID.SelectedIndex = -1;
            txtPhone.Clear();
            txtBirthDate.Clear();
            txtAddress.Clear();
            txtGender.Clear();
            dtpCreatedDate.Value = DateTime.Today;
            txtFullName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtFullName.Text.Trim() == "")
            {
                MessageBox.Show("Tên khách hàng không được để trống!", "Thông báo");
                txtFullName.Focus();
                return;
            }
            if (Addnew)
            {
                
                tblEmployees newEmployees = new tblEmployees
                {
                    FullName = txtFullName.Text.Trim(),
                    AccountID = (int)cbbAccountID.SelectedValue,
                    Phone = txtPhone.Text.Trim(),
                    BirthDate =  DateTime.Parse(txtBirthDate.Text.Trim()),
                    Address = txtAddress.Text.Trim(),
                    Gender = txtGender.Text.Trim(),
                    CreatedDate = dtpCreatedDate.Value
                };
                db.Employees.Add(newEmployees);
                db.SaveChanges();
                LoadGridData();
            }
            else
            {
                if (dgvEmployees.CurrentRow == null) return;
                int id = int.Parse(txtEmployeeID.Text);

                tblEmployees employeesUpdate = db.Employees.SingleOrDefault(u => u.EmployeeID == id);
                if (employeesUpdate != null) 
                {
                    employeesUpdate.FullName = txtFullName.Text.Trim();
                    employeesUpdate.AccountID = (int)cbbAccountID.SelectedValue;
                    employeesUpdate.Phone = txtPhone.Text.Trim();
                    employeesUpdate.BirthDate = DateTime.Parse(txtBirthDate.Text.Trim());
                    employeesUpdate.Address = txtAddress.Text.Trim();
                    employeesUpdate.Gender = txtGender.Text.Trim();
                    employeesUpdate.CreatedDate = dtpCreatedDate.Value;

                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }
    }
}
