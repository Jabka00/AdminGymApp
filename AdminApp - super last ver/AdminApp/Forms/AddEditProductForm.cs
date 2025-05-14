using AdminApp.Models;
using AdminApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AdminApp.Forms
{
    public class AddEditProductForm : Form
    {
        private TextBox txtName;
        private TextBox txtDescription;
        private TextBox txtPrice;
        private TextBox txtQuantity;
        private ComboBox cmbMainCategory;
        private ComboBox cmbSubCategory;
        private Button btnSave;
        private readonly ProductService _productService;
        private readonly Product? _product;

        private readonly Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>
        {
            {
                "Nutrition and Beverages", new List<string>
                {
                    "Protein Bars",
                    "Protein Powder (Whey, Casein, Plant-Based)",
                    "Gainers",
                    "BCAAs (Amino Acids)",
                    "Creatine",
                    "Pre-Workout Complexes",
                    "Isotonic Drinks",
                    "Vitamins and Minerals",
                    "Omega-3, Fish Oil",
                    "Energy Drinks",
                    "Glutamine",
                    "Collagen",
                    "Sugar-Free Bars",
                    "Natural Juices and Smoothies",
                    "Electrolytes"
                }
            },
            {
                "Equipment and Apparel", new List<string>
                {
                    "T-Shirts, Tank Tops",
                    "Leggings",
                    "Shorts",
                    "Sports Pants",
                    "Compression Wear",
                    "Sports Jackets",
                    "Gym Gloves",
                    "Lifting Straps and Belts",
                    "Powerlifting Belts",
                    "Knee and Elbow Wraps",
                    "Workout Socks",
                    "Gym Bags",
                    "Bottles and Shakers",
                    "Headbands and Wristbands"
                }
            },
            {
                "Accessories and Equipment", new List<string>
                {
                    "Resistance Bands",
                    "Skipping Ropes",
                    "Pull-Up Straps",
                    "Ankle and Wrist Weights",
                    "Hand Grippers",
                    "Ab Rollers",
                    "Gymnastic Rings",
                    "Doorway Pull-Up Bars",
                    "Kettlebells, Dumbbells",
                    "Yoga Mats",
                    "Balance Boards",
                    "Medicine Balls",
                    "TRX Suspension Trainers",
                    "Foam Rollers",
                    "Yoga Blocks"
                }
            },
            {
                "Hygiene and Body Care", new List<string>
                {
                    "Towels",
                    "Deodorants",
                    "Shower Gels",
                    "Shampoos",
                    "Muscle Recovery Creams",
                    "Joint Balms",
                    "Blister Creams",
                    "Sporting Plasters",
                    "Chalk (Liquid and Powder)",
                    "Antiseptics",
                    "Lip Balms"
                }
            },
            {
                "Pharmacology", new List<string>
                {
                    "Trenbolone"
                }
            }
        };

        public AddEditProductForm(ProductService productService, Product? product = null)
        {
            _productService = productService;
            _product = product;
            InitializeComponents();
            if (_product != null)
            {
                LoadProductData();
            }
        }

        private void InitializeComponents()
        {
            this.Text = _product == null ? "Add Product" : "Edit Product";
            this.Size = new System.Drawing.Size(400, 450);

            Label lblName = new Label() { Text = "Name:", Left = 20, Top = 20 };
            txtName = new TextBox() { Left = 120, Top = 20, Width = 200 };

            Label lblDescription = new Label() { Text = "Description:", Left = 20, Top = 60 };
            txtDescription = new TextBox() { Left = 120, Top = 60, Width = 200 };

            Label lblPrice = new Label() { Text = "Price:", Left = 20, Top = 100 };
            txtPrice = new TextBox() { Left = 120, Top = 100, Width = 200 };

            Label lblQuantity = new Label() { Text = "Quantity:", Left = 20, Top = 140 };
            txtQuantity = new TextBox() { Left = 120, Top = 140, Width = 200 };

            Label lblMainCategory = new Label() { Text = "Main Category:", Left = 20, Top = 180 };
            cmbMainCategory = new ComboBox()
            {
                Left = 120,
                Top = 180,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblSubCategory = new Label() { Text = "Subcategory:", Left = 20, Top = 220 };
            cmbSubCategory = new ComboBox()
            {
                Left = 120,
                Top = 220,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbMainCategory.Items.AddRange(categories.Keys.ToArray());
            cmbMainCategory.SelectedIndexChanged += CmbMainCategory_SelectedIndexChanged;
            if (cmbMainCategory.Items.Count > 0)
                cmbMainCategory.SelectedIndex = 0;

            btnSave = new Button() { Text = "Save", Left = 120, Top = 270, Width = 100 };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblPrice);
            this.Controls.Add(txtPrice);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(txtQuantity);
            this.Controls.Add(lblMainCategory);
            this.Controls.Add(cmbMainCategory);
            this.Controls.Add(lblSubCategory);
            this.Controls.Add(cmbSubCategory);
            this.Controls.Add(btnSave);
        }

        private void CmbMainCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMainCategory.SelectedItem == null)
                return;

            string selectedMainCategory = cmbMainCategory.SelectedItem.ToString()!;
            cmbSubCategory.Items.Clear();
            if (categories.TryGetValue(selectedMainCategory, out var subcats))
            {
                cmbSubCategory.Items.AddRange(subcats.ToArray());
                if (cmbSubCategory.Items.Count > 0)
                    cmbSubCategory.SelectedIndex = 0;
            }
        }

        private void LoadProductData()
        {
            if (_product == null) return;

            txtName.Text = _product.Name;
            txtDescription.Text = _product.Description;
            txtPrice.Text = _product.Price.ToString();
            txtQuantity.Text = _product.Quantity.ToString();

            if (!string.IsNullOrWhiteSpace(_product.Category))
            {
                var foundMain = categories
                    .FirstOrDefault(kvp => kvp.Value.Contains(_product.Category)).Key;
                if (foundMain != null)
                {
                    cmbMainCategory.SelectedItem = foundMain;
                    if (cmbSubCategory.Items.Contains(_product.Category))
                        cmbSubCategory.SelectedItem = _product.Category;
                }
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)
                || !double.TryParse(txtPrice.Text, out var price)
                || !int.TryParse(txtQuantity.Text, out var quantity)
                || cmbSubCategory.SelectedItem == null)
            {
                MessageBox.Show("Please fill in valid product details.");
                return;
            }

            string category = cmbSubCategory.SelectedItem.ToString()!;

            if (_product == null)
            {
                var newProduct = new Product
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Price = price,
                    Quantity = quantity,
                    Category = category,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                if (await _productService.InsertProductAsync(newProduct))
                {
                    MessageBox.Show("Product added successfully.");
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Error adding product.");
                }
            }
            else
            {
                _product.Name = txtName.Text;
                _product.Description = txtDescription.Text;
                _product.Price = price;
                _product.Quantity = quantity;
                _product.Category = category;
                _product.UpdatedAt = DateTime.Now;

                if (await _productService.UpdateProductAsync(_product))
                {
                    MessageBox.Show("Product updated successfully.");
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Error updating product.");
                }
            }
        }
    }
}