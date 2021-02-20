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
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);
        public Label TextLabel;

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

            StackPanel shopInventory = CreateLeftPanel();
            mainGrid.Children.Add(shopInventory);
            Grid.SetColumn(shopInventory, 0);
            Grid.SetRow(shopInventory, 0);
            Grid.SetRowSpan(shopInventory, 2);
        }

        public StackPanel CreateLeftPanel()
        {
            StackPanel shopInventory = new StackPanel 
            { 
                //Orientation = Orientation.Vertical,
                Background = Brushes.OldLace,
            };

            TextLabel = new Label
            {
                Content = "Shopping Inventory",
                Margin = spacing,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            shopInventory.Children.Add(TextLabel);

            Grid productGrid = new Grid();
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            //productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            shopInventory.Children.Add(productGrid);
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

            TextLabel = new Label
            {
                Content = "Dodo-Bird",
                FontSize = 13,
            };
            productGrid.Children.Add(TextLabel);
            Grid.SetRow(TextLabel, 0);
            Grid.SetColumn(TextLabel, 0);

            TextBox priceBox = new TextBox
            {
                Text = "Price",
                FontSize = 13,
                IsReadOnly = true,
            };
            productGrid.Children.Add(priceBox);
            Grid.SetRow(priceBox, 1);
            Grid.SetColumn(priceBox, 2);

            Image productImage = CreateImage("nedladdning.jfif");
            var productImageResized = resizeImage(productImage, new Size(50, 50));
            productImage.Stretch = Stretch.Fill;
            productGrid.Children.Add(productImage);
            Grid.SetRow(productImage, 1);
            Grid.SetColumn(productImage, 0);

            

            return shopInventory;
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
