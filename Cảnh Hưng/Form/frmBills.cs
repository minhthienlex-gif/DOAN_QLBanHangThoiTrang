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
        bool Addnew = false;
        public frmBills()
        {
            InitializeComponent();
        }
        private void setControl(bool check)
        {
            txtBillID.Enabled = false;
            cbbCustomerID.Enabled = check;
            cbbEmployeeID.Enabled = check;
            dtpBillDate.Enabled = check;
            cbbDiscountPercent.Enabled = check;
            txtTotalAmount.Enabled = check;

            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvBills.Enabled = !check;
        }
        private void LoadDiscountPercent()
        {
            for (int i = 5; i <= 50; i += 5)
            {
                cbbDiscountPercent.Items.Add(i + "%");
            }
        }
        private void LoadComboBox()
        {
            cbbEmployeeID.DataSource = db.Employees.ToList();
            cbbEmployeeID.DisplayMember = "FullName";
            cbbEmployeeID.ValueMember = "EmployeeID";
            cbbCustomerID.DataSource = db.Customers.ToList();
            cbbCustomerID.DisplayMember = "FullName";
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
            LoadDiscountPercent();
            setControl(false);
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Addnew = true;
            setControl(true);
            cbbCustomerID.SelectedIndex = -1;
            cbbEmployeeID.SelectedIndex = -1;
            txtBillID.Clear();
            dtpBillDate.Value = DateTime.Now;
            cbbDiscountPercent.SelectedIndex = -1;
            txtTotalAmount.Text = "0";
            cbbCustomerID.Focus();

        }

        private void dgvBills_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;

            txtBillID.Text = dgvBills.Rows[i].Cells["BillID"].Value.ToString();
            cbbCustomerID.SelectedValue = dgvBills.Rows[i].Cells["CustomerID"].Value;
            cbbEmployeeID.SelectedValue = dgvBills.Rows[i].Cells["EmployeeID"].Value;
            dtpBillDate.Value = Convert.ToDateTime(dgvBills.Rows[i].Cells["BillDate"].Value);
            cbbDiscountPercent.SelectedValue = dgvBills.Rows[i].Cells["DiscountPercent"].Value;
            txtTotalAmount.Text = dgvBills.Rows[i].Cells["TotalAmount"].Value.ToString();

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Addnew = false;
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
            if (Addnew)
            {
                if (cbbCustomerID.SelectedValue == null || cbbEmployeeID.SelectedValue == null || string.IsNullOrEmpty(cbbDiscountPercent.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                tblBills newBills = new tblBills
                {
                    CustomerID = (int)cbbCustomerID.SelectedValue,
                    EmployeeID = (int)cbbEmployeeID.SelectedValue,
                    BillDate = dtpBillDate.Value,
                    DiscountPercent = int.Parse(cbbDiscountPercent.Text.Replace("%", "")),
                    TotalAmount = decimal.Parse(txtTotalAmount.Text.Trim())
                };
                db.Bills.Add(newBills);
                db.SaveChanges();
                LoadGridData();
            }
            else
            {
                if (dgvBills.CurrentRow == null) return;
                int id = int.Parse(txtBillID.Text);
                tblBills billUpdate = db.Bills.SingleOrDefault(b => b.BillID == id);
                if (billUpdate != null)
                {
                    billUpdate.CustomerID = (int)cbbCustomerID.SelectedValue;
                    billUpdate.EmployeeID = (int)cbbEmployeeID.SelectedValue;
                    billUpdate.BillDate = dtpBillDate.Value;
                    billUpdate.DiscountPercent = int.TryParse(cbbDiscountPercent.Text.Replace("%", ""), out int d) ? d : 0;
                    billUpdate.TotalAmount = decimal.Parse(txtTotalAmount.Text.Trim());

                    db.SaveChanges();
                    LoadGridData();

                }
                setControl(false);
            }

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var data = db.Bills.ToList().Where(b =>
                    b.BillID.ToString().Contains(keyword) ||
                    b.CustomerID.ToString().Contains(keyword) ||
                    b.EmployeeID.ToString().Contains(keyword) ||
                    b.TotalAmount.ToString().Contains(keyword))
                    .Select(b => new
                    {
                        b.BillID,
                        b.CustomerID,
                        b.EmployeeID,
                        b.BillDate,
                        b.DiscountPercent,
                        b.TotalAmount
                    })
                    .ToList();

            dgvBills.DataSource = data;
        }
    }
}
