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
            StackPanel shopInventory = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = spacing                
            };
            mainGrid.Children.Add(shopInventory);
            Grid.SetColumn(shopInventory, 0);
            Grid.SetRow(shopInventory, 0);
            Grid.SetRowSpan(shopInventory, 2);

            Label inventoryLabel = new Label
            {
                Margin = spacing,
                Content = "Shop Inventory",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18
            };
            shopInventory.Children.Add(inventoryLabel);

            //Shopping Cart (Top Right side)
            Grid shoppingCartGrid = new Grid();
            shoppingCartGrid.Margin = spacing;
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition());
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            shoppingCartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Children.Add(shoppingCartGrid);
            Grid.SetColumn(shoppingCartGrid, 1);
            Grid.SetRow(shoppingCartGrid, 0);

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

            //Check Out (Bottom Right side)
            Grid checkOutGrid = new Grid();
            checkOutGrid.Margin = spacing;
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            checkOutGrid.RowDefinitions.Add(new RowDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Children.Add(checkOutGrid);
            Grid.SetColumn(checkOutGrid, 1);
            Grid.SetRow(checkOutGrid, 1);

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
        }
        public void AddProducts()
        {
            //read from CSV
            //For each item: 
            //Create horizontal stackpanel with Picture, name, descrip, prize & "Add to cart button"
        }
    }
}
