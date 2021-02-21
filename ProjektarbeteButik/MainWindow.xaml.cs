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
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);
        public StackPanel shopInventoryPanel;
        public StackPanel cartInventoryPanel;
        public decimal totalCost;
        Label cartSubTotalLabel;
        public List<Product> productsList = new List<Product>();
        public static Dictionary<Product, int> shoppingCart = new Dictionary<Product, int>();
        public Dictionary<string, string> discountCodes = new Dictionary<string, string>();
        public TextBox couponTextBox;
        public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";

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
            Width = 600;
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

            return shoppingCartGrid;
        }

        public Grid CreateCheckOutGrid()
        {
            Grid checkOutGrid = new Grid();
            checkOutGrid.Margin = spacing;
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());

            couponTextBox = new TextBox
            {
                Margin = spacing,
                Text = "Optional: Enter Coupon Code here..."
            };
            checkOutGrid.Children.Add(couponTextBox);
            Grid.SetColumn(couponTextBox, 0);
            Grid.SetRow(couponTextBox, 0);
            Grid.SetColumnSpan(couponTextBox, 3);

            Button applyDiscountCode = new Button
            {
                Margin = spacing,
                Content = "Apply Discount Code",
                FontSize = 10,
            };
            checkOutGrid.Children.Add(applyDiscountCode);
            Grid.SetColumn(applyDiscountCode, 0);
            Grid.SetRow(applyDiscountCode, 1);
            Grid.SetColumnSpan(applyDiscountCode, 3);
            applyDiscountCode.Click += ApplyDiscountCode_Click;

            Button saveCart = new Button
            {
                Margin = spacing,
                Content = "Save Cart",
            };
            checkOutGrid.Children.Add(saveCart);
            Grid.SetColumn(saveCart, 0);
            Grid.SetRow(saveCart, 1);
            saveCart.Click += SaveCart_Click;

            Button checkOutButton = new Button
            {
                Margin = spacing,
                Content = "BUY",
                FontSize = 14
            };
            checkOutGrid.Children.Add(checkOutButton);
            Grid.SetColumn(checkOutButton, 2);
            Grid.SetRow(checkOutButton, 1);

            return checkOutGrid;
        }

        private void ApplyDiscountCode_Click(object sender, RoutedEventArgs e)
        {
            
        }
        public void AddProducts()
        {
            string[] products = File.ReadAllLines("ShopInventory.csv");
            foreach (string s in products)
            {
                Grid productGrid = new Grid();
                productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
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

                Button addToCart = new Button
                {
                    Content = "Add To Cart",
                    Margin = spacing,
                    Padding = spacing,
                    Tag = p
                };
                productGrid.Children.Add(addToCart);
                Grid.SetRow(addToCart, 0);
                Grid.SetColumn(addToCart, 2);
                addToCart.Click += AddToCart_Click;

                Label productLabel = new Label
                {
                    Content = p.Name,
                    FontSize = 12,
                };
                productGrid.Children.Add(productLabel);
                Grid.SetRow(productLabel, 0);
                Grid.SetColumn(productLabel, 0);

                Label priceLabel = new Label
                {
                    Margin = spacing,
                    Content = "$" + p.Price,
                    FontSize = 12
                };
                productGrid.Children.Add(priceLabel);
                Grid.SetRow(priceLabel, 1);
                Grid.SetColumn(priceLabel, 2);

                Image productImage = CreateImage(p.PicturePath);
                productImage.Stretch = Stretch.Fill;
                productGrid.Children.Add(productImage);
                Grid.SetRow(productImage, 1);
                Grid.SetColumn(productImage, 0);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
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
        }

        public void UpdateCart()
        {
            totalCost = 0;
            cartSubTotalLabel.Content = "";
            cartInventoryPanel.Children.Clear();
            foreach (var item in shoppingCart)
            {
                totalCost += item.Key.Price * item.Value;
                cartSubTotalLabel.Content = "Total: $" + totalCost;
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
        }
        private void ClearCart(object sender, RoutedEventArgs e)
        {
            shoppingCart.Clear();
            File.Delete(CartFilePath);
            UpdateCart();
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
        public Dictionary<Product, int> LoadCart()
        {
            if (!File.Exists(CartFilePath))
            {
                MessageBox.Show("No cart to load");
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
        private void SaveCart_Click(object sender, RoutedEventArgs e)
        {
            List<string> linesList = new List<string>();
            foreach (KeyValuePair<Product, int> pair in shoppingCart)
            {
                Product p = pair.Key;
                int amount = pair.Value;
                linesList.Add(p.Name + "," + amount);
            }
            File.WriteAllLines(CartFilePath, linesList);
            MessageBox.Show("Your Cart Was Saved");
        }
    }
}
