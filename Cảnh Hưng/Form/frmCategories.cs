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
    public partial class frmCategories : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;

        public frmCategories()
        {
            InitializeComponent();
        }
        private void setControl(bool check)
        {
            txtCategoryID.Enabled = false;
            txtCategoryName.Enabled = check;
            txtDescription.Enabled = check;

            btnSave.Enabled = check;
            btnNotsaved.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;

            dgvCategories.Enabled = !check;
        }
        private void LoadGridData()
        {
            var data = from c in db.Categories
                       select new
                       {
                           c.CategoryID,
                           c.CategoryName,
                           c.Description
                       };
            dgvCategories.DataSource = data.ToList();
            setControl(false); 
        }

        private void frmCategories_Load(object sender, EventArgs e)
        {
            dgvCategories.AutoGenerateColumns = false;
            dgvCategories.AllowUserToAddRows = false;
            LoadGridData();

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            txtCategoryID.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();
            txtCategoryName.Focus();
        }

        private void btnNotsaved_Click(object sender, EventArgs e)
        {
            setControl(false);
            if (dgvCategories.CurrentRow != null)
            {
                int rowIndex = dgvCategories.CurrentRow.Index;
                int colIndex = dgvCategories.CurrentCell.ColumnIndex;
                DataGridViewCellEventArgs args = new DataGridViewCellEventArgs(colIndex, rowIndex);
                dgvCategories_CellEnter(dgvCategories, args);
            }
        }

        private void dgvCategories_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i < 0) return;

            txtCategoryID.Text = dgvCategories.Rows[i].Cells["CategoryID"].Value.ToString();
            txtCategoryName.Text = dgvCategories.Rows[i].Cells["CategoryName"].Value.ToString();

            if (dgvCategories.Rows[i].Cells["Description"].Value != null)
            {
                txtDescription.Text = dgvCategories.Rows[i].Cells["Description"].Value.ToString();
            }
            else
            {
                txtDescription.Text = "";
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null) return;

            if (MessageBox.Show("Bạn muốn xóa bản ghi này không?", "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                int id = (int)dgvCategories.CurrentRow.Cells["CategoryID"].Value;
                var itemDelete = db.Categories.SingleOrDefault(c => c.CategoryID == id);

                if (itemDelete != null)
                {
                    db.Categories.Remove(itemDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            AddNew = false;
            setControl(true);
            txtCategoryName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại sản phẩm!", "Thông báo");
                txtCategoryName.Focus();
                return;
            }

            if (AddNew)
            {
                tblCategories cat = new tblCategories();
                cat.CategoryName = txtCategoryName.Text;
                cat.Description = txtDescription.Text;

                db.Categories.Add(cat);
            }
            else
            {
                int id = int.Parse(txtCategoryID.Text);
                var cat = db.Categories.SingleOrDefault(c => c.CategoryID == id);
                if (cat != null)
                {
                    cat.CategoryName = txtCategoryName.Text;
                    cat.Description = txtDescription.Text;
                }
            }

            db.SaveChanges();
            LoadGridData();
            setControl(false);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var data = db.Categories
                        .ToList()
                        .Where(c =>
                            c.CategoryID.ToString().Contains(keyword) ||
                            c.CategoryName.ToLower().Contains(keyword) ||
                            (c.Description != null && c.Description.ToLower().Contains(keyword)))
                        .Select(c => new
                        {
                            c.CategoryID,
                            c.CategoryName,
                            c.Description
                        })
                        .ToList();

            dgvCategories.DataSource = data;
        }
    }
}

