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

namespace ProjektarbeteButik
{
    public class Product
    {
        public string Name;
        public string Description;
        public decimal Price;
        //Picture here, or path to picture??
    }
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);
        public StackPanel shopInventoryPanel;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "GUI App";
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
            //read from CSV
            //For each item: 
            //Create horizontal stackpanel with Picture, name, descrip, prize & "Add to cart button"
            Grid productGrid = new Grid();
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            shopInventoryPanel.Children.Add(productGrid);
            Grid.SetColumn(productGrid, 0);
            Grid.SetRow(productGrid, 1);

            Button addToCart = new Button
            {
                Content = "Add To Cart",
                Margin = spacing,
                Padding = spacing
            };
            productGrid.Children.Add(addToCart);
            Grid.SetRow(addToCart, 0);
            Grid.SetColumn(addToCart, 2);

            Label productLabel = new Label
            {
                Content = "Dodo-Bird",
                FontSize = 12,
            };
            productGrid.Children.Add(productLabel);
            Grid.SetRow(productLabel, 0);
            Grid.SetColumn(productLabel, 0);

            TextBox priceBox = new TextBox
            {
                Text = "Price",
                FontSize = 12,
                IsReadOnly = true,
            };
            productGrid.Children.Add(priceBox);
            Grid.SetRow(priceBox, 1);
            Grid.SetColumn(priceBox, 2);

            Image productImage = CreateImage(@"Images\exampleimage2.webp");
            var productImageResized = resizeImage(productImage, new Size(50, 50));
            productImage.Stretch = Stretch.Fill;
            productGrid.Children.Add(productImage);
            Grid.SetRow(productImage, 1);
            Grid.SetColumn(productImage, 0);
        }        

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
            Image image = new Image
            {
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
            };
            // A small rendering tweak to ensure maximum visual appeal.
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }
    }

    internal class Bitmap : Image
    {
        private Image imgToResize;
        private Size size;

        public Bitmap(Image imgToResize, Size size)
        {
            this.imgToResize = imgToResize;
            this.size = size;
        }
    }
}
