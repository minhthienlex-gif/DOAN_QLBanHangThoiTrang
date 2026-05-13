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
    public partial class frmBills : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;
        public frmBills()
        {
            InitializeComponent();
        }
        private void setControl(bool check)
        {
            txtBillID.Enabled = false;
            cbbCustomerID.Enabled = check;
            txtEmployeeID.Enabled = check;
            dtpBillDate.Enabled = check;
            txtDiscountPercent.Enabled = check;
            txtTotalAmount.Enabled = check;

            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvBills.Enabled = !check;
        }
        private void LoadComboBox()
        {
            txtEmployeeID.DataSource = db.Employees.ToList();
            txtEmployeeID.DisplayMember = "Name";
            txtEmployeeID.ValueMember = "EmployeeID";
            cbbCustomerID.DataSource = db.Customers.ToList();
            cbbCustomerID.DisplayMember = "Name";
            cbbCustomerID.ValueMember = "CustomerID";
        }
        private void LoadGridData()
        {
            var data = from b in db.Bills
                       select new
                       {
                           b.BillID,
                           b.CustomerID,
                           b.EmployeeID,
                           b.BillDate,
                           b.DiscountPercent,
                           b.TotalAmount
                       };
            dgvBills.DataSource = data.ToList();
        }

        private void frmBills_Load(object sender, EventArgs e)
        {
            dgvBills.AutoGenerateColumns = false;
            dgvBills.AllowUserToAddRows = false;

            LoadComboBox();
            LoadGridData();
            setControl(false);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            cbbCustomerID.SelectedIndex = -1;
            txtEmployeeID.SelectedIndex = -1;
            txtBillID.Clear();
            dtpBillDate.Value = DateTime.Now;
            txtDiscountPercent.Text = "0";
            txtTotalAmount.Text = "0";
            cbbCustomerID.Focus();

        }

        private void dgvBills_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;

            txtBillID.Text = dgvBills.Rows[i].Cells["BillID"].Value.ToString();
            cbbCustomerID.SelectedValue = dgvBills.Rows[i].Cells["CustomerID"].Value;
            txtEmployeeID.SelectedValue = dgvBills.Rows[i].Cells["EmployeeID"].Value;
            dtpBillDate.Value = Convert.ToDateTime(dgvBills.Rows[i].Cells["BillDate"].Value);
            txtDiscountPercent.Text = dgvBills.Rows[i].Cells["DiscountPercent"].Value.ToString();
            txtTotalAmount.Text = dgvBills.Rows[i].Cells["TotalAmount"].Value.ToString();

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddNew = false;
            setControl(true);
            cbbCustomerID.Focus();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBills.CurrentRow == null) return;

            if (MessageBox.Show("Bạn muốn xóa hóa đơn này không?", "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvBills.CurrentRow.Cells["BillID"].Value;

                var details = db.BillDetails.Where(d => d.BillID == id);
                db.BillDetails.RemoveRange(details);

                var itemDelete = db.Bills.SingleOrDefault(b => b.BillID == id);
                if (itemDelete != null)
                {
                    db.Bills.Remove(itemDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }

        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            setControl(false);
            if (dgvBills.CurrentRow != null)
            {
                int rowIndex = dgvBills.CurrentRow.Index;
                int colIndex = dgvBills.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvBills_CellEnter(dgvBills, args);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvBills.CurrentRow == null) return;

            if (MessageBox.Show("Bạn muốn xóa hóa đơn này không?", "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvBills.CurrentRow.Cells["BillID"].Value;

                var details = db.BillDetails.Where(d => d.BillID == id);
                db.BillDetails.RemoveRange(details);

                var itemDelete = db.Bills.SingleOrDefault(b => b.BillID == id);
                if (itemDelete != null)
                {
                    db.Bills.Remove(itemDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }

        }
    }
}
