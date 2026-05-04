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
    public partial class frmImports : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;
        public frmImports()
        {
            InitializeComponent();
        }
        private void setContol(bool check)
        {
            txtImportID.Enabled = false;
            cbbEmployeeID.Enabled = check;
            dtpImportDate.Enabled = false;
            txtTotalAmount.Enabled = check;

            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;

            dgvImport.Enabled = !check;
        }
        private void LoadGridData()
        {
            var data = from i in db.Imports
                       select i;
            dgvImport.DataSource = data.ToList();
            setContol(false);
        }
        private void frmImports_Load(object sender, EventArgs e)
        {
            dgvImport.AutoGenerateColumns = false;
            dgvImport.AllowUserToAddRows = false;
            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvImport.CurrentRow == null) return;
            if (MessageBox.Show("Bạn muốn xóa bản ghi này không?", "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvImport.CurrentRow.Cells["ImportID"].Value;
                var userDelete = db.Imports.SingleOrDefault(u => u.ImportID == id);

                if (userDelete != null)
                {
                    db.Imports.Remove(userDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            setContol(false);
            if (dgvImport.CurrentRow != null)
            {
                int rowIndex = dgvImport.CurrentRow.Index;
                int colIndex = dgvImport.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvImport_CellEnter(dgvImport, args);
            }
        }

        private void dgvImport_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;
            txtImportID.Text = dgvImport.Rows[i].Cells["ImportID"].Value.ToString();
            cbbEmployeeID.Text = dgvImport.Rows[i].Cells["EmployeeID"].Value.ToString();
            dtpImportDate.Text = Convert.ToDateTime(dgvImport.Rows[i].Cells["ImportDate"].Value).ToString("dd/MM/yyyy");
            txtTotalAmount.Text = dgvImport.Rows[i].Cells["TotalAmount"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setContol(true);
            txtImportID.Clear();
            txtTotalAmount.Clear();
            cbbEmployeeID.Text = "";
            dtpImportDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            cbbEmployeeID.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddNew = false;
            setContol(true);
            cbbEmployeeID.Enabled = false;
            cbbEmployeeID.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbbEmployeeID.Text.Trim() == "")
            {
                MessageBox.Show("Thông tin tên nhân viên không được để trống!", "Thông báo");
                cbbEmployeeID.Focus();
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
                tblImports newImport = new tblImports
                {

                    EmployeeID = int.Parse(cbbEmployeeID.Text.Trim()),
                    ImportDate = DateTime.ParseExact(dtpImportDate.Text, "dd/MM/yyyy", null),
                    TotalAmount = decimal.Parse(txtTotalAmount.Text.Trim())
                };

                db.Imports.Add(newImport);
                db.SaveChanges();
                LoadGridData();
            }
            else //Nếu trước đó ấn vào nút sửa thì đoạn này sẽ thực hiện
            {
                if (dgvImport.CurrentRow == null) return;

                int id = int.Parse(txtImportID.Text);
                // Tìm đối tượng cần sửa bằng LINQ
                tblImports importUpdate = db.Imports.SingleOrDefault(i => i.ImportID == id);

                if (importUpdate != null)
                {
                    importUpdate.EmployeeID = int.Parse(cbbEmployeeID.Text.Trim());
                    importUpdate.ImportDate = DateTime.ParseExact(dtpImportDate.Text, "dd/MM/yyyy", null);
                    importUpdate.TotalAmount = decimal.Parse(txtTotalAmount.Text.Trim());

                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }
    }
}
