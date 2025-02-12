using AdminApp.Models;
using AdminApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminApp.Forms
{
    public class PurchaseHistoryForm : Form
    {
        private DataGridView dgvPurchases;
        private readonly PurchaseService _purchaseService;

        public PurchaseHistoryForm(PurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
            InitializeComponents();
            LoadPurchasesAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "Purchase History";
            this.Size = new System.Drawing.Size(800, 600);

            dgvPurchases = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(dgvPurchases);
        }

        private async void LoadPurchasesAsync()
        {
            List<Purchase> purchases = await _purchaseService.GetAllPurchasesAsync();
            dgvPurchases.DataSource = purchases;
        }
    }
}