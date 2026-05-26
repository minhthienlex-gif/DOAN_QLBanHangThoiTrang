using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOAN_QLBanHangThoiTrang
{
    public partial class frmProducts : Form
    {
        string selectedImagePath = "";
        DataContext db = new DataContext();
        bool AddNew = false;
        public frmProducts()
        {
            InitializeComponent();
        }
        private void setControl(bool check)
        {
            txtProductID.Enabled = false;
            cbbCategoryID.Enabled = check;
            txtProductTitle.Enabled = check;
            txtBrandName.Enabled = check;
            txtPrice.Enabled = check;
            nudQuantity.Enabled = check;
            txtDescription.Enabled = check;
            dtpCreatedDate.Enabled = true;
            picImage.Enabled = check;
            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;

            btnAddImage.Enabled = check;
            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            dgvProducts.Enabled = !check;
        }
        private void LoadCategoryComboBox()
        {
            cbbCategoryID.DataSource = db.Categories.ToList();
            cbbCategoryID.DisplayMember = "CategoryName";
            cbbCategoryID.ValueMember = "CategoryID";
        }
        private void LoadGridData()
        {
            var data = from p in db.products
                       select new
                       {
                           p.ProductID,
                           p.CategoryID,
                           p.ProductTitle,
                           p.BrandName,
                           p.Price,
                           p.Quantity,
                           p.Description,
                           p.CreatedDate,
                           p.ProductImage
                       };

            dgvProducts.DataSource = data.ToList();
            setControl(false);
        }

        private void frmProducts_Load(object sender, EventArgs e)
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.AllowUserToAddRows = false;
            LoadCategoryComboBox();
            LoadGridData();

        }
        private void dgvProducts_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;

            txtProductID.Text = dgvProducts.Rows[i].Cells["ProductID"].Value.ToString();
            txtProductTitle.Text = dgvProducts.Rows[i].Cells["ProductTitle"].Value.ToString();
            txtBrandName.Text = dgvProducts.Rows[i].Cells["BrandName"].Value?.ToString();
            txtPrice.Text = dgvProducts.Rows[i].Cells["Price"].Value.ToString();
            nudQuantity.Value = Convert.ToInt32(dgvProducts.Rows[i].Cells["Quantity"].Value);
            txtDescription.Text = dgvProducts.Rows[i].Cells["Description"].Value?.ToString();
            string imgPath = dgvProducts.Rows[i].Cells["ImagePath"].Value?.ToString();
            if (!string.IsNullOrEmpty(imgPath))
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            txtProductID.Clear();
            txtProductTitle.Clear();
            txtBrandName.Clear();
            txtPrice.Clear();
            nudQuantity.Value = 0;
            txtDescription.Clear();
            cbbCategoryID.SelectedIndex = -1;
            dtpCreatedDate.Value = DateTime.Now;
            picImage.Image = null;
            txtProductTitle.Focus();

        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            setControl(false);
            if (dgvProducts.CurrentRow != null)
            {
                int rowIndex = dgvProducts.CurrentRow.Index;
                int colIndex = dgvProducts.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvProducts_CellEnter(dgvProducts, args);
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddNew = false;
            setControl(true);
            txtProductTitle.Focus();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;

            if (MessageBox.Show("Xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = (int)dgvProducts.CurrentRow.Cells["ProductID"].Value;
                var p = db.products.SingleOrDefault(x => x.ProductID == id);
                if (p != null)
                {
                    db.products.Remove(p);
                    db.SaveChanges();
                    LoadGridData();
                }
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (AddNew)
            {
                tblProducts p = new tblProducts();
                p.ProductTitle = txtProductTitle.Text;
                p.BrandName = txtBrandName.Text;
                p.Price = decimal.Parse(txtPrice.Text);
                p.Quantity = (int)nudQuantity.Value;
                p.Description = txtDescription.Text;
                p.CategoryID = (int)cbbCategoryID.SelectedValue;
                p.CreatedDate = DateTime.Now;
                p.ProductImage = selectedImagePath;
                db.products.Add(p);
            }
            else
            {
                int id = int.Parse(txtProductID.Text);
                var p = db.products.SingleOrDefault(x => x.ProductID == id);
                if (p != null)
                {
                    p.ProductTitle = txtProductTitle.Text;
                    p.BrandName = txtBrandName.Text;
                    p.Price = decimal.Parse(txtPrice.Text);
                    p.Quantity = (int)nudQuantity.Value;
                    p.Description = txtDescription.Text;
                    p.CategoryID = (int)cbbCategoryID.SelectedValue;
                    if (!string.IsNullOrEmpty(selectedImagePath))
                    {
                        p.ProductImage = selectedImagePath;
                    }
                }
            }

            db.SaveChanges();
            LoadGridData();
            setControl(false);
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
                ofd.Title = "Chọn ảnh cho sách";

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

            var data = db.products
                        .ToList()
                        .Where(p =>
                            p.ProductID.ToString().Contains(keyword) ||
                            p.ProductTitle.ToLower().Contains(keyword) ||
                            (p.BrandName != null && p.BrandName.ToLower().Contains(keyword)) ||
                            p.Price.ToString().Contains(keyword) ||
                            p.Quantity.ToString().Contains(keyword) ||
                            (p.Description != null && p.Description.ToLower().Contains(keyword)))
                        .Select(p => new
                        {
                            p.ProductID,
                            p.CategoryID,
                            p.ProductTitle,
                            p.BrandName,
                            p.Price,
                            p.Quantity,
                            p.Description,
                            p.CreatedDate,
                            p.ProductImage
                        })
                        .ToList();

            dgvProducts.DataSource = data;
        }
    }
}
 
