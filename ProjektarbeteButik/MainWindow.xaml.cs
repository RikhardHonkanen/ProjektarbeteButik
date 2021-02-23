using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace ProjektarbeteButik
{
    public class Product
    {
        public string Name;
        public string Description;
        public decimal Price;
        public string PicturePath;
    }
    public class Receipt
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Amount { get; set; }
    }
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);
        public StackPanel shopInventoryPanel;
        public StackPanel cartInventoryPanel;
        public decimal subTotal;
        public decimal totalCost;
        public Label cartSubTotalLabel;
        public Label totalCostLabel;
        public List<Product> productsList = new List<Product>();
        public static Dictionary<Product, int> shoppingCart = new Dictionary<Product, int>();
        public TextBox couponTextBox;
        public const string CartFilePath = @"C:\Windows\Temp\TheExcellentCart.csv";
        public Grid checkOutGrid;
        public DataGrid receiptGrid;

        public MainWindow()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "The Wonderful Items Shoppe";
            Width = 900;
            Height = 800;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid mainGrid = new Grid();
            root.Content = mainGrid;
            mainGrid.Margin = spacing;
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            //Shop Inventory (Left side)
            StackPanel shopInventory = CreateShopInventoryPanel();
            mainGrid.Children.Add(shopInventory);
            Grid.SetColumn(shopInventory, 0);
            Grid.SetRow(shopInventory, 0);
            Grid.SetRowSpan(shopInventory, 2);

            //Shopping Cart (Top Right side)
            Grid shoppingCartGrid = CreateCartGrid();
            mainGrid.Children.Add(shoppingCartGrid);
            Grid.SetColumn(shoppingCartGrid, 1);
            Grid.SetRow(shoppingCartGrid, 0);

            //Check Out (Bottom Right side)
            Grid checkOutGrid = CreateCheckOutGrid();
            mainGrid.Children.Add(checkOutGrid);
            Grid.SetColumn(checkOutGrid, 1);
            Grid.SetRow(checkOutGrid, 1);

            LoadCart();
            UpdateCart();
        }

        public StackPanel CreateShopInventoryPanel()
        {
            shopInventoryPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
                Background = Brushes.OldLace
            };

            Label shopInventoryLabel = new Label
            {
                Content = "Shopping Inventory",
                Margin = spacing,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            shopInventoryPanel.Children.Add(shopInventoryLabel);

            AddProducts();

            return shopInventoryPanel;
        }
        public Grid CreateCartGrid()
        {
            Grid shoppingCartGrid = new Grid();
            shoppingCartGrid.Margin = spacing;
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition());
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Label cartInventoryLabel = new Label
            {
                Margin = spacing,
                Content = "Cart Inventory",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18
            };
            shoppingCartGrid.Children.Add(cartInventoryLabel);

            cartInventoryPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
            };
            shoppingCartGrid.Children.Add(cartInventoryPanel);
            Grid.SetColumn(cartInventoryPanel, 0);
            Grid.SetRow(cartInventoryPanel, 1);

            Button clearCartButton = new Button
            {
                Margin = spacing,
                Content = "Clear Cart"
            };
            shoppingCartGrid.Children.Add(clearCartButton);
            Grid.SetColumn(clearCartButton, 0);
            Grid.SetRow(clearCartButton, 2);
            clearCartButton.Click += ClearCart;

            cartSubTotalLabel = new Label
            {
                Margin = spacing,
                Content = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 12
            };
            shoppingCartGrid.Children.Add(cartSubTotalLabel);
            Grid.SetColumn(cartSubTotalLabel, 0);
            Grid.SetRow(cartSubTotalLabel, 3);

            totalCostLabel = new Label
            {
                Margin = spacing,
                Content = "",
                HorizontalAlignment = HorizontalAlignment.Right,
                FontSize = 12
            };
            shoppingCartGrid.Children.Add(totalCostLabel);
            Grid.SetColumn(totalCostLabel, 0);
            Grid.SetRow(totalCostLabel, 3);

            return shoppingCartGrid;
        }

        public Grid CreateCheckOutGrid()
        {
            checkOutGrid = new Grid();
            checkOutGrid.Margin = spacing;
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Label couponLabel = new Label
            {
                Margin = spacing,
                Content = "(Optional) Discount Coupon"
            };
            checkOutGrid.Children.Add(couponLabel);
            Grid.SetColumn(couponLabel, 0);
            Grid.SetRow(couponLabel, 0);
            Grid.SetColumnSpan(couponLabel, 2);

            couponTextBox = new TextBox
            {
                Margin = spacing,
                TextAlignment = TextAlignment.Center
            };
            checkOutGrid.Children.Add(couponTextBox);
            Grid.SetColumn(couponTextBox, 2);
            Grid.SetRow(couponTextBox, 0);

            Button applyDiscountCode = new Button
            {
                Margin = spacing,
                Content = "Apply Discount Code",
                FontSize = 14,
            };
            checkOutGrid.Children.Add(applyDiscountCode);
            Grid.SetColumn(applyDiscountCode, 0);
            Grid.SetRow(applyDiscountCode, 1);
            Grid.SetColumnSpan(applyDiscountCode, 2);
            applyDiscountCode.Click += ApplyDiscountCode; 

            Button checkOutButton = new Button
            {
                Margin = spacing,
                Content = "BUY",
                FontSize = 14
            };
            checkOutGrid.Children.Add(checkOutButton);
            Grid.SetColumn(checkOutButton, 2);
            Grid.SetRow(checkOutButton, 1);
            checkOutButton.Click += CheckOut;

            return checkOutGrid;
        }
        public void AddProducts()
        //We still need to display item description somewhere (included in .csv-file)
        {
            string[] products = File.ReadAllLines("ShopInventory.csv");
            foreach (string s in products)
            {
                Grid productGrid = new Grid();
                productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                shopInventoryPanel.Children.Add(productGrid);                

                var productProperties = s.Split(',');
                Product p = new Product
                {
                    Name = productProperties[0],
                    Description = productProperties[1],
                    Price = decimal.Parse(productProperties[2]),
                    PicturePath = productProperties[3]
                };
                productsList.Add(p);
                Image productImage = CreateImage(p.PicturePath);
                productImage.Stretch = Stretch.Fill;
                productGrid.Children.Add(productImage);
                Grid.SetRow(productImage, 0);
                Grid.SetRowSpan(productImage, 2);
                Grid.SetColumn(productImage, 0);

                Label productLabel = new Label
                {
                    Content = p.Name,
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                };
                productGrid.Children.Add(productLabel);
                Grid.SetRow(productLabel, 0);
                Grid.SetColumn(productLabel, 1);

                Label productDescription = new Label
                {
                    Content = p.Description,
                    FontSize = 12,
                    FontWeight = FontWeights.Regular,
                };
                productGrid.Children.Add(productDescription);
                Grid.SetRow(productDescription, 1);
                Grid.SetColumn(productDescription, 1);
                Grid.SetColumnSpan(productDescription, 2);

                Label priceLabel = new Label
                {
                    Margin = spacing,
                    Content = "$" + p.Price,
                    FontSize = 12
                };
                productGrid.Children.Add(priceLabel);
                Grid.SetRow(priceLabel, 0);
                Grid.SetColumn(priceLabel, 3);

                Button addToCart = new Button
                {
                    Content = "Add To Cart",
                    Margin = spacing,
                    Padding = spacing,
                    Tag = p
                };
                productGrid.Children.Add(addToCart);
                Grid.SetRow(addToCart, 1);
                Grid.SetColumn(addToCart, 3);
                addToCart.Click += AddToCart;
            }
        }
        private void AddToCart(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var product = (Product)button.Tag;

            if (shoppingCart.ContainsKey(product))
            {
                shoppingCart[product] += 1;
            }
            else
            {
                shoppingCart[product] = 1;
            }
            UpdateCart();
            SaveCart();
        }
        public void UpdateCart()
        {
            subTotal = 0;
            totalCost = 0;
            cartSubTotalLabel.Content = "";
            totalCostLabel.Content = "";
            cartInventoryPanel.Children.Clear();
            foreach (var item in shoppingCart)
            {
                subTotal += item.Key.Price * item.Value;
                cartSubTotalLabel.Content = "Subtotal: $" + subTotal;
                Grid itemGrid = new Grid();
                itemGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                cartInventoryPanel.Children.Add(itemGrid);

                Label itemName = new Label
                {
                    Margin = spacing,
                    Content = item.Key.Name,
                    FontSize = 12
                };
                itemGrid.Children.Add(itemName);
                Grid.SetColumn(itemName, 0);

                Label itemPrice = new Label
                {
                    Margin = spacing,
                    Content = "$" + item.Key.Price * item.Value,
                    FontSize = 12
                };
                itemGrid.Children.Add(itemPrice);
                Grid.SetColumn(itemPrice, 1);

                Label itemAmount = new Label
                {
                    Margin = spacing,
                    Content = item.Value,
                    FontSize = 12
                };
                itemGrid.Children.Add(itemAmount);
                Grid.SetColumn(itemAmount, 2);

                Button deleteFromCart = new Button
                {
                    Content = "Delete",
                    Margin = spacing,
                    Padding = spacing,
                    Tag = item.Key
                };
                itemGrid.Children.Add(deleteFromCart);
                Grid.SetColumn(deleteFromCart, 3);
                deleteFromCart.Click += DeleteFromCart;
            }
        }
        private void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            shoppingCart.Remove(product);
            UpdateCart();
            SaveCart();
        }
        private void ClearCart(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Clear Shopping Cart, Are You Sure?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                shoppingCart.Clear();
                File.Delete(CartFilePath);
                UpdateCart();
                SaveCart();
            }
        }        
        public Dictionary<Product, int> LoadCart()
        {
            if (!File.Exists(CartFilePath))
            {
            }
            else
            {
                string[] lines = File.ReadAllLines(CartFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    string name = parts[0];
                    int amount = int.Parse(parts[1]);

                    Product current = null;
                    foreach (Product p in productsList)
                    {
                        if (p.Name == name)
                        {
                            current = p;
                        }
                    }
                    shoppingCart[current] = amount;
                }
            }
            return shoppingCart;
        }
        private void SaveCart()
        {
            List<string> linesList = new List<string>();
            foreach (KeyValuePair<Product, int> pair in shoppingCart)
            {
                Product p = pair.Key;
                int amount = pair.Value;
                linesList.Add(p.Name + "," + amount);
            }
            File.WriteAllLines(CartFilePath, linesList);
        }
        private void ApplyDiscountCode(object sender, RoutedEventArgs e)
        {
            string couponInput = couponTextBox.Text.ToLower();
            string[] lines = File.ReadAllLines("DiscountCodes.csv");
            var discountCodes = new Dictionary<string, decimal>();
            totalCost = subTotal;
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string discountCode = parts[0];
                decimal discountAmount = decimal.Parse(parts[1]);
                discountCodes[discountCode] = discountAmount;
            }
            if (discountCodes.ContainsKey(couponInput))
            {
                totalCost = Math.Round(subTotal * discountCodes[couponInput], 2);
                MessageBox.Show("Discount " + (int)((1 - discountCodes[couponInput]) * 100) + "%. Total for this order: $" + totalCost);
                totalCostLabel.Content = "Total (with discount): $" + totalCost;
            }
            else
            {
                MessageBox.Show("Code does not exist.");
            }            
        }
        private void CheckOut(object sender, RoutedEventArgs e)
        {
            receiptGrid = new DataGrid
            {
                AutoGenerateColumns = true,
            };
            checkOutGrid.Children.Add(receiptGrid);
            Grid.SetRow(receiptGrid, 2);
            Grid.SetColumn(receiptGrid, 0);
            Grid.SetColumnSpan(receiptGrid, 3);
            receiptGrid.RowBackground = Brushes.OldLace;
            receiptGrid.Foreground = Brushes.Black;
            receiptGrid.AlternatingRowBackground = Brushes.Gray;

            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Name";
            c1.Binding = new Binding("Name");
            c1.Width = 110;
            receiptGrid.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Unit Price";
            c2.Width = 110;
            c2.Binding = new Binding("Price");
            receiptGrid.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Total Price";
            c3.Width = 110;
            c3.Binding = new Binding("TotalPrice");
            receiptGrid.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Amount";
            c4.Width = 120;
            c4.Binding = new Binding("Amount");
            receiptGrid.Columns.Add(c4);

            foreach (KeyValuePair<Product, int> pair in shoppingCart)
            {
                Receipt kvitto = new Receipt
                {
                    Name = pair.Key.Name,
                    Amount = pair.Value,
                    Price = pair.Key.Price,
                    TotalPrice = pair.Value * pair.Key.Price,
                };
                receiptGrid.Items.Add(kvitto);
            }
        }
        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
            Image image = new Image
            {
                Margin = spacing,
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                MaxHeight = 50,
                MaxWidth = 50
            };
            // A small rendering tweak to ensure maximum visual appeal.
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }
        private void DummyMethod()
        {
            //Delete this method, only for push
            MessageBox.Show("Hello");
        }
    }
}