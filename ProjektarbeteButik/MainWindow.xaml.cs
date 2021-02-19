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
        public List<Product> productsList = new List<Product>();

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
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition());
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel cartInventoryPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
            };
            shoppingCartGrid.Children.Add(cartInventoryPanel);
            Grid.SetColumn(cartInventoryPanel, 0);
            Grid.SetRow(cartInventoryPanel, 0);

            Label cartInventoryLabel = new Label
            {
                Margin = spacing,
                Content = "Cart Inventory",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18
            };
            cartInventoryPanel.Children.Add(cartInventoryLabel);

            Button clearCartButton = new Button
            {
                Margin = spacing,
                Content = "Clear Cart"
            };
            shoppingCartGrid.Children.Add(clearCartButton);
            Grid.SetColumn(clearCartButton, 0);
            Grid.SetRow(clearCartButton, 1);

            Label cartTotalLabel = new Label
            {
                Margin = spacing,
                Content = "Total: 1000 spänn",
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 12
            };
            shoppingCartGrid.Children.Add(cartTotalLabel);
            Grid.SetColumn(cartTotalLabel, 0);
            Grid.SetRow(cartTotalLabel, 2);

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

            TextBox couponTextBox = new TextBox
            {
                Margin = spacing,
                Text = "(Optional: Enter Coupon Code here)"
            };
            checkOutGrid.Children.Add(couponTextBox);
            Grid.SetColumn(couponTextBox, 0);
            Grid.SetRow(couponTextBox, 0);
            Grid.SetColumnSpan(couponTextBox, 2);

            Button checkOutButton = new Button
            {
                Margin = spacing,
                Content = "BUY",
                FontSize = 14
            };
            checkOutGrid.Children.Add(checkOutButton);
            Grid.SetColumn(checkOutButton, 1);
            Grid.SetRow(checkOutButton, 1);

            return checkOutGrid;
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
                Grid.SetColumn(productGrid, 0);
                Grid.SetRow(productGrid, 1);

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
            throw new NotImplementedException();
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
    }
}
