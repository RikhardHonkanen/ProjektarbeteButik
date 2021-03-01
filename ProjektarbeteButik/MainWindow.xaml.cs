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
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PicturePath { get; set; }
    }
    //Class for Receipt objects, used for data grid in CheckOut
    public class Receipt
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Amount { get; set; }
    }
    public partial class MainWindow : Window
    {
        //Instance variables further explained later (where necessary)
        public Thickness spacing = new Thickness(5);
        public const string cartFilePath = @"C:\Windows\Temp\ProjektarbeteButik\TheExcellentCart.csv";
        public StackPanel shopInventoryPanel;
        public StackPanel cartInventoryPanel;
        public StackPanel receiptPanel;
        public decimal subTotal;
        public decimal totalCost;
        public Label subTotalLabel;
        public Label totalCostLabel;
        public TextBox couponTextBox;
        public Button checkOutButton;
        public Grid checkOutGrid;
        public DataGrid receiptGrid;
        public List<Product> productsList = new List<Product>();
        public static Dictionary<Product, int> shoppingCart = new Dictionary<Product, int>();
        public Label receiptLabel;
        public bool acceptedDiscountCode;
        public bool generatedReceipt;
        public decimal discountAmount;
        public Dictionary<string, decimal> discountCodes = new Dictionary<string, decimal>();

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

            //Cart is loaded from .csv-file.
            shoppingCart = LoadCart(productsList, cartFilePath);

            //Valid discount codes read from .csv-file and stored in dictionary for later use.
            discountCodes = LoadDiscountCodes();

            //This method is used in several places to refresh the GUI.
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

            //Items will be added to this panel in the UpdateCart method
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

            //Instance variable refreshed when cart is updated
            subTotalLabel = new Label
            {
                Margin = spacing,
                Content = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 12
            };
            shoppingCartGrid.Children.Add(subTotalLabel);
            Grid.SetColumn(subTotalLabel, 0);
            Grid.SetRow(subTotalLabel, 3);

            //Instance variable refreshed when discount code is applied
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
            //Instance variable, because Grid is used in CheckOut method
            checkOutGrid = new Grid();
            checkOutGrid.Margin = spacing;
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Label couponLabel = new Label
            {
                Margin = spacing,
                Content = "(Optional) Discount Coupon",
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            checkOutGrid.Children.Add(couponLabel);
            Grid.SetColumn(couponLabel, 0);
            Grid.SetRow(couponLabel, 0);
            Grid.SetColumnSpan(couponLabel, 2);

            //Text needs to be parsed in method ApplyDiscountCode
            couponTextBox = new TextBox
            {
                Margin = spacing,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            checkOutGrid.Children.Add(couponTextBox);
            Grid.SetColumn(couponTextBox, 2);
            Grid.SetRow(couponTextBox, 0);

            Button applyDiscountCode = new Button
            {
                Margin = spacing,
                Content = "Apply Discount Coupon",
                FontSize = 14
            };
            checkOutGrid.Children.Add(applyDiscountCode);
            Grid.SetColumn(applyDiscountCode, 0);
            Grid.SetRow(applyDiscountCode, 1);
            Grid.SetColumnSpan(applyDiscountCode, 2);
            applyDiscountCode.Click += ApplyDiscountCode;

            //Needs to be accessed in UpdateCart, to toggle Enabled/Disabled
            checkOutButton = new Button
            {
                Margin = spacing,
                Content = "BUY",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                IsDefault = true,
                IsEnabled = false                
            };
            checkOutGrid.Children.Add(checkOutButton);
            Grid.SetColumn(checkOutButton, 2);
            Grid.SetRow(checkOutButton, 1);
            checkOutButton.Click += CheckOut;

            //Panel is populated in CheckOut
            receiptPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
            };
            checkOutGrid.Children.Add(receiptPanel);
            Grid.SetColumn(receiptPanel, 0);
            Grid.SetRow(receiptPanel, 5);
            Grid.SetColumnSpan(receiptPanel, 3);

            return checkOutGrid;
        }
        public void AddProducts()
        {
            //Reads inventory from .csv-file and creates a grid for each item. Grid is added to shopInventoryPanel.
            string[] products = File.ReadAllLines(@"C:\Windows\Temp\ProjektarbeteButik\ShopInventory.csv");
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
        }
        public void UpdateCart()
        {
            //Each time cart is changed, the GUI is cleared and re-populated 
            subTotal = 0;
            totalCost = 0;            
            if (generatedReceipt == true)
            {
                MessageBoxResult result = MessageBox.Show("Receipt will be cleared, start shopping again?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    couponTextBox.Text = "";
                    receiptPanel.Children.Clear();
                    checkOutGrid.Children.RemoveAt(6);
                    checkOutGrid.Children.RemoveAt(5);
                }
                else
                {
                    shoppingCart.Clear();
                    return;
                }
            }
            generatedReceipt = false;
            acceptedDiscountCode = false;
            subTotalLabel.Content = "";
            totalCostLabel.Content = "";
            cartInventoryPanel.Children.Clear();
            int counter = 0;
            foreach (var item in shoppingCart)
            {
                subTotal += item.Key.Price * item.Value;
                subTotalLabel.Content = "Subtotal: $" + subTotal;

                Grid itemGrid = new Grid();
                itemGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition());
                if (counter % 2 == 0)
                {
                    itemGrid.Background = Brushes.AntiqueWhite;
                }
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

                Button decreaseAmount = new Button
                {
                    Height = 16,
                    Width = 16,
                    Content = "-",
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = spacing,
                    //For some reason negative padding (sort of) centers the "-" symbol in the button
                    Padding = new Thickness(-1),
                    Tag = item.Key
                };
                itemGrid.Children.Add(decreaseAmount);
                Grid.SetColumn(decreaseAmount, 3);
                decreaseAmount.Click += DecreaseItemAmount;

                Button increaseAmount = new Button
                {
                    Height = 16,
                    Width = 16,
                    Content = "+",
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = spacing,
                    Padding = new Thickness(-1),
                    Tag = item.Key
                };
                itemGrid.Children.Add(increaseAmount);
                Grid.SetColumn(increaseAmount, 4);
                increaseAmount.Click += IncreaseItemAmount; ;

                Button deleteFromCart = new Button
                {
                    Content = "Delete",
                    Margin = spacing,
                    Padding = spacing,
                    Tag = item.Key
                };
                itemGrid.Children.Add(deleteFromCart);
                Grid.SetColumn(deleteFromCart, 5);
                deleteFromCart.Click += DeleteFromCart;

                counter++;
            }
            if (shoppingCart.Count > 0)
            {
                checkOutButton.IsEnabled = true;
            }
            else
            {
                checkOutButton.IsEnabled = false;
            }
            
            SaveCart();
        }
        private void DecreaseItemAmount(object sender, RoutedEventArgs e)
        {
            //Decrease amount of a product by 1, or remove from cart if none left
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            if (shoppingCart[product] > 1)
            {
                shoppingCart[product] -= 1;
                UpdateCart();
            }
            else
            {
                DeleteFromCart(sender, e);
            }
        }
        private void IncreaseItemAmount(object sender, RoutedEventArgs e)
        {
            //Increase amount of a product in cart by 1
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            shoppingCart[product] += 1;
            UpdateCart();
        }
        private void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            //Completely removes a product from the cart
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            shoppingCart.Remove(product);
            UpdateCart();
        }
        private void ClearCart(object sender, RoutedEventArgs e)
        {
            //Removes all products from cart
            if (shoppingCart.Count == 0)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Clear Shopping Cart, Are You Sure?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                shoppingCart.Clear();
                File.Delete(cartFilePath);
                UpdateCart();
            }
        }
        public static Dictionary<Product, int> LoadCart(List<Product> productsList, string cartFilePath)
        {
            //Loads saved cart from .csv-file. If-statement to avoid program crash on missing file
            if (!File.Exists(cartFilePath))
            {

            }
            else
            {
                string[] lines = File.ReadAllLines(cartFilePath);
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
            //Cart is automatically saved to .csv-file whenever cart is updated
            List<string> linesList = new List<string>();
            foreach (KeyValuePair<Product, int> pair in shoppingCart)
            {
                Product p = pair.Key;
                int amount = pair.Value;
                linesList.Add(p.Name + "," + amount);
            }
            File.WriteAllLines(cartFilePath, linesList);
        }
        private static Dictionary<string, decimal> LoadDiscountCodes()
        {
            string[] lines = File.ReadAllLines(@"C:\Windows\Temp\ProjektarbeteButik\DiscountCodes.csv");
            var discountCodes = new Dictionary<string, decimal>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string discountCode = parts[0];
                decimal discountAmount = decimal.Parse(parts[1]);
                discountCodes[discountCode] = discountAmount;
            }
            return discountCodes;
        }
        private void ApplyDiscountCode(object sender, RoutedEventArgs e)
        {
            //Reads user input from couponTextBox, and applies appropriate discount if the code is valid.            
            //To prevent discount being applied several times variable totalCost is set equal to subTotal.
            totalCost = subTotal;            
            string input = couponTextBox.Text.ToLower();
            acceptedDiscountCode = CheckIfCodeIsValid(discountCodes, input);
            if (acceptedDiscountCode)
            {
                discountAmount = discountCodes[input];
                totalCost = CalculateTotalCost(discountAmount, subTotal);
                MessageBox.Show("Discount " + (int)((1 - discountAmount) * 100) + "%. Total for this order: $" + totalCost);
                totalCostLabel.Content = "Total (with discount): $" + totalCost;
            }
            else
            {
                MessageBox.Show("Coupon does not exist.");
            }
        }
        public static decimal CalculateTotalCost(decimal discountAmount, decimal subTotal)
        {            
            decimal totalCost = Math.Round(subTotal * discountAmount, 2);
            return totalCost;
        }
        public static bool CheckIfCodeIsValid(Dictionary<string, decimal> discountCodes, string input)
        {
            if (discountCodes.ContainsKey(input))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void CheckOut(object sender, RoutedEventArgs e)
        {
            //Displays a receipt to the user. Also checks whether a discount is applied to format receipt correctly.
            MessageBoxResult result;
            if (acceptedDiscountCode == false && couponTextBox.Text != "")
            {
                result = MessageBox.Show("Discount coupon not applied, proceed to checkout anyway?", "", MessageBoxButton.YesNo);
            }
            else
            {
                result = MessageBox.Show("             Proceed to checkout?", "", MessageBoxButton.YesNo);
            }
            if (result == MessageBoxResult.Yes)
            {
                receiptPanel.Children.Clear();
                receiptGrid = new DataGrid
                {
                    AutoGenerateColumns = true,
                };
                checkOutGrid.Children.Add(receiptGrid);
                Grid.SetRow(receiptGrid, 3);
                Grid.SetColumn(receiptGrid, 0);
                Grid.SetColumnSpan(receiptGrid, 3);
                receiptGrid.RowBackground = Brushes.White;
                receiptGrid.Foreground = Brushes.Black;
                receiptGrid.AlternatingRowBackground = Brushes.Gray;

                Label textlabel = new Label
                {
                    Content = "Receipt",
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 20,
                };
                checkOutGrid.Children.Add(textlabel);
                Grid.SetRow(textlabel, 2);
                Grid.SetColumn(textlabel, 1);

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

                if (acceptedDiscountCode == true)
                {                    
                    receiptLabel = new Label
                    {
                        Content = "Discount Code: " + couponTextBox.Text
                    };
                    receiptPanel.Children.Add(receiptLabel);
                    Grid.SetRow(receiptLabel, 5);
                    Grid.SetColumn(receiptLabel, 0);
                    Grid.SetColumnSpan(receiptLabel, 2);

                    receiptLabel = new Label
                    {
                        Content = "Sum Before Discount: $" + subTotal
                    };
                    receiptPanel.Children.Add(receiptLabel);
                    Grid.SetRow(receiptLabel, 6);
                    Grid.SetColumn(receiptLabel, 0);
                    Grid.SetColumnSpan(receiptLabel, 2);

                    receiptLabel = new Label
                    {
                        Content = "Discount: $" + (subTotal - totalCost) + "(" + (int)((1 - discountCodes[couponTextBox.Text.ToLower()]) * 100) +" %)"
                    };
                    receiptPanel.Children.Add(receiptLabel);
                    Grid.SetRow(receiptLabel, 7);
                    Grid.SetColumn(receiptLabel, 0);
                    Grid.SetColumnSpan(receiptLabel, 2);
                }
                if (totalCost == 0)
                {
                    totalCost = subTotal;
                }
                receiptLabel = new Label
                {
                    Content = "Total for this purchase: $" + totalCost,
                    FontWeight = FontWeights.Bold
                };
                receiptPanel.Children.Add(receiptLabel);
                Grid.SetRow(receiptLabel, 8);
                Grid.SetColumn(receiptLabel, 0);
                Grid.SetColumnSpan(receiptLabel, 2);

                shoppingCart.Clear();
                File.Delete(cartFilePath);
                UpdateCart();
                generatedReceipt = true;
            }
        }
        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            Image image = new Image
            {
                Margin = spacing,
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                //Size of images limited so shop inventory looks uniform
                MaxHeight = 50,
                MaxWidth = 50
            };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }
    }
}