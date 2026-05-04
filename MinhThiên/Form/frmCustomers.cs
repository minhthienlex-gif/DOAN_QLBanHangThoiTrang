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
    public partial class frmCustomers : Form
    {
        DataContext db = new DataContext();
        bool Addnew = false;
        public frmCustomers()
        {
            InitializeComponent();
        }
        private void SetContol(bool check)
        {
            txtCustomerID.Enabled = false;
            txtFullName.Enabled = check;         
            txtPhone.Enabled = check;
            txtAddress.Enabled = check;
            txtEmail.Enabled = check;
            dtpCreatedDate.Enabled = check;
            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvCustomers.Enabled = !check;
        }
        private void LoadGridData()
        {
            var data = from c in db.Customers
                       select c;

            dgvCustomers.DataSource = data.ToList();
            SetContol(false);
        }
        private void frmCustomers_Load(object sender, EventArgs e)
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.AllowUserToAddRows = false;

            LoadGridData();
        }
        private void dgvCustomers_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;
            txtCustomerID.Text = dgvCustomers.Rows[i].Cells["CustomerID"].Value.ToString();
            txtFullName.Text = dgvCustomers.Rows[i].Cells["Fullname"].Value.ToString();
            txtPhone.Text = dgvCustomers.Rows[i].Cells["Phone"].Value.ToString();
            txtAddress.Text = dgvCustomers.Rows[i].Cells["Address"].Value.ToString();
            txtEmail.Text = dgvCustomers.Rows[i].Cells["Email"].Value.ToString();
            dtpCreatedDate.Value = Convert.ToDateTime(dgvCustomers.Rows[i].Cells["CreatedDate"].Value);
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            SetContol(false);
            if (dgvCustomers.CurrentRow != null)
            {
                int rowIndex = dgvCustomers.CurrentRow.Index;
                int colIndex = dgvCustomers.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvCustomers_CellEnter(dgvCustomers, args);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null) return;
            if (MessageBox.Show("Bạn muốn xóa bản ghi này không", "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvCustomers.CurrentRow.Cells["CustomerID"].Value;

                var CustomerDelete = db.Customers.SingleOrDefault(u => u.CustomerID == id);

                if (CustomerDelete != null)
                {
                    db.Customers.Remove(CustomerDelete);
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
            txtCustomerID.Clear();
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
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
                tblCustomers newCustomers = new tblCustomers
                {
                    FullName = txtFullName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    CreatedDate = dtpCreatedDate.Value
                };
                db.Customers.Add(newCustomers);
                db.SaveChanges();
                LoadGridData();
            }
            else
            {
                if (dgvCustomers.CurrentRow == null) return;
                int id = int.Parse(txtCustomerID.Text);

                tblCustomers customersUpdate = db.Customers.SingleOrDefault(u => u.CustomerID == id);
                if (customersUpdate != null) { 
                    customersUpdate.FullName = txtFullName.Text.Trim();
                    customersUpdate.Phone = txtPhone.Text.Trim();
                    customersUpdate.Address = txtAddress.Text.Trim();
                    customersUpdate.Email = txtEmail.Text.Trim();
                    customersUpdate.CreatedDate = dtpCreatedDate.Value;

                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }
    }
}
