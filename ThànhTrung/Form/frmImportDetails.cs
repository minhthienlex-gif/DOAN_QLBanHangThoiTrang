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
    public partial class frmImportDetails : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;
        public frmImportDetails()
        {
            InitializeComponent();
        }

        private void setContol(bool check)
        {
            txtImportDetailID.Enabled = false;
            cbbImportID.Enabled = check;
            cbbProductID.Enabled = check;
            nudQuantity.Enabled = check;
            txtPrice.Enabled = check;
            txtTotal.Enabled = check;

            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvImportDetail.Enabled = !check;
        }
        private void LoadGridData()
        {
            var data = from i in db.ImportDetails
                       select i;
            dgvImportDetail.DataSource = data.ToList();
            setContol(false);
        }
        private void frmImportDetails_Load(object sender, EventArgs e)
        {
            dgvImportDetail.AutoGenerateColumns = false;
            dgvImportDetail.AllowUserToAddRows = false;
            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvImportDetail.CurrentRow == null) return;
            if (MessageBox.Show("Bạn muốn xóa bản ghi này không?", "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvImportDetail.CurrentRow.Cells["ImportDetailID"].Value;
                var userDelete = db.ImportDetails.SingleOrDefault(u => u.ImportDetailID == id);

                if (userDelete != null)
                {
                    db.ImportDetails.Remove(userDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            setContol(false);
            if (dgvImportDetail.CurrentRow != null)
            {
                int rowIndex = dgvImportDetail.CurrentRow.Index;
                int colIndex = dgvImportDetail.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvImportDetail_CellEnter(dgvImportDetail, args);
            }
        }

        private void dgvImportDetail_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;

            txtImportDetailID.Text = dgvImportDetail.Rows[i].Cells["ImportDetailID"].Value.ToString();
            cbbImportID.Text = dgvImportDetail.Rows[i].Cells["ImportID"].Value.ToString();
            cbbProductID.Text = dgvImportDetail.Rows[i].Cells["ProductID"].Value.ToString();
            nudQuantity.Text = dgvImportDetail.Rows[i].Cells["Quantity"].Value.ToString();
            txtPrice.Text = dgvImportDetail.Rows[i].Cells["Price"].Value.ToString();
            txtTotal.Text = dgvImportDetail.Rows[i].Cells["Total"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setContol(true);
            txtImportDetailID.Clear();
            cbbImportID.Text= "";
            cbbProductID.Text = "";
            nudQuantity.Value = 0;
            txtPrice.Clear();
            txtTotal.Clear();

            cbbImportID.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddNew = false;
            setContol(true);
            cbbImportID.Enabled = false;
            cbbImportID.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbbImportID.Text.Trim() == "")
            {
                MessageBox.Show("Thông tin mã phiếu xuất không được để trống!", "Thông báo");
                cbbImportID.Focus();
                return;
            }


            if (AddNew)
            {
                //Kiểm tra trùng EmployeeID
                //int inputEmployeeID = int.Parse(txtEmployeeID.Text.Trim());
                // bool isExisted = db.Imports.Any(i => i.ImportID == inputEmployeeID);
                // if (isExisted)
                //  {
                // MessageBox.Show("Mã nhân viên này đã tồn tại! Vui lòng chọn mã khác.",
                //   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //   txtImportID.Focus();
                //   return;
                //   }

                //Nếu không trùng thì tiến hành thêm mới
                tblImportDetails newImportDetail = new tblImportDetails
                {

                    ImportID = int.Parse(cbbImportID.Text.Trim()),
                    ProductID = int.Parse(cbbProductID.Text.Trim()),
                    Quantity = int.Parse(nudQuantity.Text.Trim()),
                    Price = decimal.Parse(txtPrice.Text.Trim()),
                    Total = decimal.Parse(txtTotal.Text.Trim())
                };

                db.ImportDetails.Add(newImportDetail);
                db.SaveChanges();
                LoadGridData();
            }
            else //Nếu trước đó ấn vào nút sửa thì đoạn này sẽ thực hiện
            {
                if (dgvImportDetail.CurrentRow == null) return;

                int id = int.Parse(txtImportDetailID.Text);
                // Tìm đối tượng cần sửa bằng LINQ
                tblImportDetails importUpdate = db.ImportDetails.SingleOrDefault(i => i.ImportDetailID == id);

                if (importUpdate != null)
                {
                    importUpdate.ImportID = int.Parse(cbbImportID.Text.Trim());
                    importUpdate.ProductID = int.Parse(cbbProductID.Text.Trim());
                    importUpdate.Quantity = int.Parse(nudQuantity.Text.Trim());
                    importUpdate.Price = decimal.Parse(txtPrice.Text.Trim());
                    importUpdate.Total = decimal.Parse(txtTotal.Text.Trim());

                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }
    }
}
