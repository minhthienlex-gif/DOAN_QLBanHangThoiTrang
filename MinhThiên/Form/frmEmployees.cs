using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOAN_QLBanHangThoiTrang
{
    public partial class frmEmployees : Form
    {
        string selectedImagePath = "";
        DataContext db = new DataContext();
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
            picImage.Enabled = check;
            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnAddImage.Enabled = check;
            dgvEmployees.Enabled = !check;
        }
        private void LoadAccount()
        {
            var data = from a in db.Accounts
                       select a;

            cbbAccountID.DataSource = data.ToList();
            cbbAccountID.DisplayMember = "FullName"; 
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
            cbbAccountID.SelectedValue = dgvEmployees.Rows[i].Cells["AccountID"].Value;
            txtPhone.Text = dgvEmployees.Rows[i].Cells["Phone"].Value.ToString();
            txtBirthDate.Text = dgvEmployees.Rows[i].Cells["BirthDate"].Value.ToString();
            txtAddress.Text = dgvEmployees.Rows[i].Cells["Address"].Value.ToString();
            txtGender.Text = dgvEmployees.Rows[i].Cells["Gender"].Value.ToString();
            dtpCreatedDate.Value = Convert.ToDateTime(dgvEmployees.Rows[i].Cells["CreatedDate"].Value);

            string imgPath = dgvEmployees.Rows[i].Cells["ImagePath"].Value?.ToString();
            selectedImagePath = imgPath ?? "";
            if(!string.IsNullOrEmpty(imgPath))
            {
                try
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                    string fullPath = Path.Combine(projectPath, imgPath);
                    if (File.Exists(fullPath))
                    {
                        using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            if (picImage.Image != null) picImage.Image.Dispose();
                            picImage.Image = Image.FromStream(stream);
                            picImage.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    else picImage.Image = null;
                }
                catch { picImage.Image = null; }
            }
            else picImage.Image = null;
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
            picImage.Image = null;
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
                    CreatedDate = dtpCreatedDate.Value,
                    EmployeeImage = selectedImagePath
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
                    if (!string.IsNullOrEmpty(selectedImagePath))
                    {
                        employeesUpdate.EmployeeImage = selectedImagePath;
                    }
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
                ofd.Title = "Chọn ảnh cho Nhân viên";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            if (picImage.Image != null) picImage.Image.Dispose();
                            picImage.Image = Image.FromStream(stream);
                        }
                        picImage.SizeMode = PictureBoxSizeMode.Zoom;

                        string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                        string folderPath = Path.Combine(projectPath, "images");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string extension = Path.GetExtension(ofd.FileName);
                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string uniqueFileName = timestamp + extension;
                        string destPath = Path.Combine(folderPath, uniqueFileName);
                        File.Copy(ofd.FileName, destPath, true);
                        selectedImagePath = "images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xử lý ảnh: " + ex.Message);
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var data = db.Employees.Where(ea =>
                            ea.FullName.ToLower().Contains(keyword) ||
                            ea.Phone.ToLower().Contains(keyword) ||
                            ea.Address.ToLower().Contains(keyword) ||
                            ea.Gender.ToLower().Contains(keyword) ||
                            ea.AccountID.ToString().Contains(keyword))
                            .ToList();

            dgvEmployees.DataSource = data;
        }
    }
}
