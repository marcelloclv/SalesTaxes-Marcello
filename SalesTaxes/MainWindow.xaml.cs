/**
 * 
 * Marcello Cysneiros Landim Valença
 * Candidate for the Software Developer position in 7Pixel
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalesTaxes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Items = new List<Item>();
            ShoppingCart = new Dictionary<Item, int>();

            fillData();
        }
        
        #region Properties
        public List<Item> Items { get; set; }
        public Dictionary<Item, int> ShoppingCart { get; set; } 
        #endregion

        #region Methods
        private void fillData()
        {
            /** BEGIN OF HARD CODED DATA */
            createItem("book", 12.49m, true, false);
            createItem("music CD", 14.99m, false, false);
            createItem("chocolate bar", 0.85m, true, false);
            createItem("bottle of perfume", 18.99m, false, false);
            createItem("packet of headache pills", 9.75m, true, false);
            createItem("imported box of chocolates", 10.00m, true, true);
            createItem("imported bottle of perfume", 47.50m, false, true);
            createItem("imported bottle of perfume 2", 27.99m, false, true);
            createItem("box of imported chocolates", 11.25m, true, true);
            /** END OF HARD CODED DATA */
        }

        private void createItem(string name, decimal price, bool exempt, bool imported)
        {
            Item item = new Item(name, price, exempt, imported);
            Items.Add(item);
            cboItems.Items.Add(item.GetProductAndPriceString());
        }
        #endregion

        #region Events
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Item item = Items[cboItems.SelectedIndex];
            int amount = string.IsNullOrEmpty(txtCount.Text) ? 1 : Convert.ToInt32(txtCount.Text);

            lstItems.Items.Add(amount + " " + cboItems.SelectedItem);

            if (ShoppingCart.Keys.Contains(item))
                ShoppingCart[item] += amount;
            else
                ShoppingCart.Add(item, amount);
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {
            decimal salesTaxes = 0;
            decimal total = 0;
            Item item;
            int amount;

            lstReceipt.Items.Clear();

            foreach (KeyValuePair<Item, int> entry in ShoppingCart)
            {
                item = entry.Key;
                amount = entry.Value;
                lstReceipt.Items.Add(amount + " " + item.GetName() + ": " + item.GetTotalPrice() * amount);

                salesTaxes += item.GetTotalTaxes() * amount;
                total += item.GetTotalPrice() * amount;
            }

            lstReceipt.Items.Add("Sales Taxes: " + salesTaxes);
            lstReceipt.Items.Add("Total: " + total);
        }

        private void txtCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Only numbers in the count textbox
            string textCount = txtCount.Text;
            int count;
            if ((!string.IsNullOrEmpty(textCount) && !int.TryParse(textCount, out count)) || textCount == "0")
                txtCount.Text = textCount.Substring(0, textCount.Length - 1);
        } 
        
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lstItems.Items.Clear();
            lstReceipt.Items.Clear();
            ShoppingCart.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Auxiliar Classes
        public class Item
        {
            const decimal basicSalesTax = 0.10m; // Tax of 10%
            const decimal additionalSalesTax = 0.05m; // imported goods

            private string name;
            private decimal price;
            private bool exempt; // Books, food and medical products
            private bool imported;

            public Item(string name, decimal price, bool exempt, bool imported)
            {
                this.name = name;
                this.price = price;
                this.exempt = exempt;
                this.imported = imported;
            }

            public string GetName()
            {
                return name;
            }

            public decimal GetPrice()
            {
                return price;
            }

            public decimal GetTotalPrice()
            {
                decimal totalPrice = price;
                if (!exempt)
                    totalPrice += Math.Ceiling((price * basicSalesTax) / 0.05m) * 0.05m;
                if (imported)
                    totalPrice += Math.Ceiling((price * additionalSalesTax) / 0.05m) * 0.05m;
                return totalPrice;
            }

            public decimal GetTotalTaxes()
            {
                return GetTotalPrice() - GetPrice();
            }

            public string GetProductAndPriceString()
            {
                return name + " at " + price;
            }
        } 
        #endregion

    }
}
