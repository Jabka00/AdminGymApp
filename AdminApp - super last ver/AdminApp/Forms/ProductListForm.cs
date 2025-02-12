using AdminApp.Models;
using AdminApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminApp.Forms
{
    public class ProductListForm : Form
    {
        private DataGridView dgvProducts;
        private Button btnAddProduct;
        private Button btnEditProduct;
        private Button btnDeleteProduct;
        private readonly ProductService _productService;

        public ProductListForm(ProductService productService)
        {
            _productService = productService;
            InitializeComponents();
            LoadProductsAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "Product Management";
            this.Size = new System.Drawing.Size(800, 600);

            dgvProducts = new DataGridView()
            {
                Dock = DockStyle.Top,
                Height = 400,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            btnAddProduct = new Button() { Text = "Add Product", Left = 50, Top = 420, Width = 150 };
            btnEditProduct = new Button() { Text = "Edit Product", Left = 220, Top = 420, Width = 150 };
            btnDeleteProduct = new Button() { Text = "Delete Product", Left = 390, Top = 420, Width = 150 };

            btnAddProduct.Click += BtnAddProduct_Click;
            btnEditProduct.Click += BtnEditProduct_Click;
            btnDeleteProduct.Click += BtnDeleteProduct_Click;

            this.Controls.Add(dgvProducts);
            this.Controls.Add(btnAddProduct);
            this.Controls.Add(btnEditProduct);
            this.Controls.Add(btnDeleteProduct);
        }

        private async void LoadProductsAsync()
        {
            List<Product> products = await _productService.GetAllProductsAsync();
            dgvProducts.DataSource = products;
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            var addEditForm = new AddEditProductForm(_productService);
            if (addEditForm.ShowDialog() == DialogResult.OK)
            {
                LoadProductsAsync();
            }
        }

        private void BtnEditProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var product = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                var addEditForm = new AddEditProductForm(_productService, product);
                if (addEditForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProductsAsync();
                }
            }
            else
            {
                MessageBox.Show("Please select a product to edit.");
            }
        }

        private async void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var product = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                var confirm = MessageBox.Show($"Are you sure you want to delete '{product.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    bool success = await _productService.DeleteProductAsync(product.Id);
                    if (success)
                    {
                        MessageBox.Show("Product deleted successfully.");
                        LoadProductsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Error deleting product.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.");
            }
        }
    }
}
