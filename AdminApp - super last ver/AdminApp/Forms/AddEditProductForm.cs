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
        // Вместо текстового поля для категории используем два ComboBox:
        private ComboBox cmbMainCategory;
        private ComboBox cmbSubCategory;
        private Button btnSave;
        private readonly ProductService _productService;
        private readonly Product? _product;

        // Словарь с основными категориями и списками подкатегорий
        private readonly Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>
        {
            {
                "Питание и напитки", new List<string>
                {
                    "Протеиновые батончики",
                    "Протеиновый порошок (сывороточный, казеиновый, растительный)",
                    "Гейнеры",
                    "BCAA (аминокислоты)",
                    "Креатин",
                    "Предтренировочные комплексы",
                    "Изотонические напитки",
                    "Витамины и минералы",
                    "Омега-3, рыбий жир",
                    "Энергетические напитки",
                    "Глютамин",
                    "Коллаген",
                    "Батончики без сахара",
                    "Натуральные соки и коктейли",
                    "Электролиты"
                }
            },
            {
                "Экипировка и одежда", new List<string>
                {
                    "Футболки, майки",
                    "Лосины, леггинсы",
                    "Шорты",
                    "Спортивные брюки",
                    "Компрессионная одежда",
                    "Спортивные куртки",
                    "Перчатки для зала",
                    "Лямки и ремни для тяги",
                    "Пояса для пауэрлифтинга",
                    "Бинты для коленей, локтей",
                    "Носки для тренировок",
                    "Спортивные сумки",
                    "Бутылки и шейкеры",
                    "Повязки на голову и запястья"
                }
            },
            {
                "Аксессуары и инвентарь", new List<string>
                {
                    "Фитнес-резинки",
                    "Скакалки",
                    "Лямки для подтягиваний",
                    "Утяжелители для ног и рук",
                    "Эспандеры",
                    "Ролики для пресса",
                    "Гимнастические кольца",
                    "Турники для дверного проема",
                    "Гири, гантели",
                    "Йога-коврики",
                    "Балансировочные платформы",
                    "Медицинские мячи",
                    "Тренировочные петли (TRX)",
                    "Пена-роллы (Foam Roller)",
                    "Блоки для йоги"
                }
            },
            {
                "Гигиена и уход за телом", new List<string>
                {
                    "Полотенца",
                    "Дезодоранты",
                    "Гели для душа",
                    "Шампуни",
                    "Кремы для восстановления мышц",
                    "Бальзамы для суставов",
                    "Кремы от мозолей",
                    "Пластырь спортивный",
                    "Магнезия (жидкая и порошковая)",
                    "Антисептики",
                    "Бальзамы для губ"
                }
            },
            {
                "Фармакология", new List<string>
                {
                    "Тренбалон"
                    // Можно добавить и другие подкатегории фармакологии по необходимости
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

            // Поле "Name"
            Label lblName = new Label() { Text = "Name:", Left = 20, Top = 20 };
            txtName = new TextBox() { Left = 120, Top = 20, Width = 200 };

            // Поле "Description"
            Label lblDescription = new Label() { Text = "Description:", Left = 20, Top = 60 };
            txtDescription = new TextBox() { Left = 120, Top = 60, Width = 200 };

            // Поле "Price"
            Label lblPrice = new Label() { Text = "Price:", Left = 20, Top = 100 };
            txtPrice = new TextBox() { Left = 120, Top = 100, Width = 200 };

            // Поле "Quantity"
            Label lblQuantity = new Label() { Text = "Quantity:", Left = 20, Top = 140 };
            txtQuantity = new TextBox() { Left = 120, Top = 140, Width = 200 };

            // Создаём ComboBox для подкатегории до назначения обработчика основного ComboBox
            Label lblSubCategory = new Label() { Text = "Subcategory:", Left = 20, Top = 220 };
            cmbSubCategory = new ComboBox()
            {
                Left = 120,
                Top = 220,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Создаём ComboBox для основной категории
            Label lblMainCategory = new Label() { Text = "Main Category:", Left = 20, Top = 180 };
            cmbMainCategory = new ComboBox()
            {
                Left = 120,
                Top = 180,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Заполняем список основных категорий и назначаем обработчик события
            cmbMainCategory.Items.AddRange(categories.Keys.ToArray());
            cmbMainCategory.SelectedIndexChanged += CmbMainCategory_SelectedIndexChanged;

            // Устанавливаем SelectedIndex после того, как оба ComboBox созданы
            if (cmbMainCategory.Items.Count > 0)
            {
                cmbMainCategory.SelectedIndex = 0;
            }

            // Кнопка "Save"
            btnSave = new Button() { Text = "Save", Left = 120, Top = 270, Width = 100 };
            btnSave.Click += BtnSave_Click;

            // Добавляем элементы управления на форму
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

        // Обновляем список подкатегорий при изменении основной категории
        private void CmbMainCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMainCategory.SelectedItem == null)
                return;

            string selectedMainCategory = cmbMainCategory.SelectedItem.ToString();
            cmbSubCategory.Items.Clear();
            if (categories.ContainsKey(selectedMainCategory))
            {
                cmbSubCategory.Items.AddRange(categories[selectedMainCategory].ToArray());
                if (cmbSubCategory.Items.Count > 0)
                {
                    cmbSubCategory.SelectedIndex = 0;
                }
            }
        }

        private void LoadProductData()
        {
            if (_product != null)
            {
                txtName.Text = _product.Name;
                txtDescription.Text = _product.Description;
                txtPrice.Text = _product.Price.ToString();
                txtQuantity.Text = _product.Quantity.ToString();

                // Если в базе сохранено значение подкатегории, определяем основную категорию
                if (!string.IsNullOrWhiteSpace(_product.Category))
                {
                    string savedSubCategory = _product.Category;
                    // Ищем основную категорию, в которой содержится эта подкатегория
                    string foundMainCategory = categories
                        .Where(kvp => kvp.Value.Contains(savedSubCategory))
                        .Select(kvp => kvp.Key)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(foundMainCategory))
                    {
                        cmbMainCategory.SelectedItem = foundMainCategory;
                        // После выбора основной категории список подкатегорий обновится,
                        // поэтому выбираем нужную подкатегорию
                        if (cmbSubCategory.Items.Contains(savedSubCategory))
                        {
                            cmbSubCategory.SelectedItem = savedSubCategory;
                        }
                    }
                }
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // Проверяем заполненность необходимых полей
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                !double.TryParse(txtPrice.Text, out double price) ||
                !int.TryParse(txtQuantity.Text, out int quantity) ||
                cmbSubCategory.SelectedItem == null)
            {
                MessageBox.Show("Please fill in valid product details.");
                return;
            }

            // Сохраняем только выбранную подкатегорию
            string category = cmbSubCategory.SelectedItem.ToString();

            if (_product == null)
            {
                Product newProduct = new Product
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Price = price,
                    Quantity = quantity,
                    Category = category,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                bool success = await _productService.InsertProductAsync(newProduct);
                if (success)
                {
                    MessageBox.Show("Product added successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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

                bool success = await _productService.UpdateProductAsync(_product);
                if (success)
                {
                    MessageBox.Show("Product updated successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error updating product.");
                }
            }
        }
    }
}
