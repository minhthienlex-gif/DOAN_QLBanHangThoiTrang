using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOAN_QLBanHangThoiTrang
{
    public partial class frmBillDetails : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;
        public frmBillDetails()
        {
            InitializeComponent();
        }

        private void setContol(bool check)
        {
            txtBillDetailID.Enabled = false;
            cbbBillID.Enabled = check;
            cbbProductID.Enabled = check;
            nudQuantity.Enabled = check;
            txtPrice.Enabled = check;
            txtTotal.Enabled = false;

            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
         
    
            dgvBillDetail.Enabled = !check;
        }
        private void LoadProductID()
        {
            var data = from p in db.products
                       select p;

            cbbProductID.DataSource = data.ToList();
            cbbProductID.DisplayMember = "ProductTitle";
            cbbProductID.ValueMember = "ProductID";
        }
        private void LoadBillID()
        {
            var data = from b in db.Bills
                       select b;

            cbbBillID.DataSource = data.ToList();
            cbbBillID.DisplayMember = "BillID";
            cbbBillID.ValueMember = "BillID";
        }
        private void LoadGridData()
        {
            var data = from i in db.BillDetails
                       select i;
            dgvBillDetail.DataSource = data.ToList();
            setContol(false);
        }
        private void frmBillDetails_Load(object sender, EventArgs e)
        {
            dgvBillDetail.AutoGenerateColumns = false;
            dgvBillDetail.AllowUserToAddRows = false;
            LoadGridData();
            LoadBillID();
            LoadProductID();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddNew = false;
            setContol(true);
            cbbBillID.Enabled = false;
            cbbBillID.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBillDetail.CurrentRow == null) return;
            if (MessageBox.Show("Bạn muốn xóa bản ghi này không?", "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvBillDetail.CurrentRow.Cells["BillDetailID"].Value;
                var userDelete = db.BillDetails.SingleOrDefault(u => u.BillDetailID == id);

                if (userDelete != null)
                {
                    int billID = userDelete.BillID;
                    db.BillDetails.Remove(userDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            setContol(false);
            if (dgvBillDetail.CurrentRow != null)
            {
                int rowIndex = dgvBillDetail.CurrentRow.Index;
                int colIndex = dgvBillDetail.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvBillDetail_CellEnter(dgvBillDetail, args);
            }
        }
        private void TinhThanhTien()
        {
            int quantity = (int)nudQuantity.Value;

            decimal price = 0;

            if (txtPrice.Text.Trim() != "")
                price = decimal.Parse(txtPrice.Text);

            txtTotal.Text = (quantity * price).ToString();
        }
        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }
        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            TinhThanhTien();
        }
        private void CapNhatTongHoaDon(int billID)
        {
            decimal tong = db.BillDetails.Where(x => x.BillID == billID).Sum(x => (decimal?)x.Total) ?? 0;

            var bill = db.Bills.SingleOrDefault(x => x.BillID == billID);

            if (bill != null)
            {
                bill.TotalAmount = tong;
                db.SaveChanges();
            }
        }
        private void dgvBillDetail_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;

            txtBillDetailID.Text = dgvBillDetail.Rows[i].Cells["BillDetailID"].Value.ToString();
            cbbBillID.SelectedValue = dgvBillDetail.Rows[i].Cells["BillID"].Value;
            cbbProductID.SelectedValue = dgvBillDetail.Rows[i].Cells["ProductID"].Value;
            nudQuantity.Value = Convert.ToDecimal(dgvBillDetail.Rows[i].Cells["Quantity"].Value);
            txtPrice.Text = dgvBillDetail.Rows[i].Cells["Price"].Value.ToString();
            txtTotal.Text = dgvBillDetail.Rows[i].Cells["Total"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setContol(true);
            txtBillDetailID.Clear();
            cbbBillID.SelectedIndex = -1;
            cbbProductID.SelectedIndex = -1;
            nudQuantity.Value = 0;
            txtPrice.Clear();
            txtTotal.Clear();

            cbbBillID.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbbBillID.Text.Trim() == "")
            {
                MessageBox.Show("Thông tin mã phiếu xuất không được để trống!", "Thông báo");
                cbbBillID.Focus();
                return;
            }
            if (cbbBillID.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn!");
                return;
            }

            if (cbbProductID.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!");
                return;
            }
            decimal price;
            if (!decimal.TryParse(txtPrice.Text.Trim(), out price))
            {
                MessageBox.Show("Giá không hợp lệ!");
                txtPrice.Focus();
                return;
            }
            if (AddNew)
            {
                tblBillDetails newBillDetail = new tblBillDetails
                {
                    BillID = (int)cbbBillID.SelectedValue,
                    ProductID = (int)cbbProductID.SelectedValue,
                    Quantity = (int)nudQuantity.Value,
                    Price = price,
                    Total = (int)nudQuantity.Value * price
                };

                db.BillDetails.Add(newBillDetail);
                db.SaveChanges();
                CapNhatTongHoaDon(newBillDetail.BillID);
                
                LoadGridData();
            }
            else 
            {
                if (dgvBillDetail.CurrentRow == null) return;

                int id = int.Parse(txtBillDetailID.Text);
                
                tblBillDetails importUpdate = db.BillDetails.SingleOrDefault(i => i.BillDetailID == id);

                if (importUpdate != null)
                {
                    importUpdate.BillID = (int)cbbBillID.SelectedValue;
                    importUpdate.ProductID = (int)cbbProductID.SelectedValue;
                    importUpdate.Quantity = (int)nudQuantity.Value;
                    importUpdate.Price = price;
                    importUpdate.Total = (int)nudQuantity.Value * price;

                    db.SaveChanges();
                    CapNhatTongHoaDon(importUpdate.BillID);
                    LoadGridData();
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var data = db.BillDetails
                        .ToList()
                        .Where(b =>
                            b.BillDetailID.ToString().Contains(keyword) ||
                            b.BillID.ToString().Contains(keyword) ||
                            b.ProductID.ToString().Contains(keyword) ||
                            b.Quantity.ToString().Contains(keyword) ||
                            b.Price.ToString().Contains(keyword) ||
                            b.Total.ToString().Contains(keyword))
                        .ToList();

            dgvBillDetail.DataSource = data;
        }

    }
}
