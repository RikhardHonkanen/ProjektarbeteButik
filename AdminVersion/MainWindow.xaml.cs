using System;
using System.Collections.Generic;
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
using ProjektarbeteButik;

namespace AdminVersion
{
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);
        
        public const string productFilePath = @"C:\Windows\Temp\Products.csv";
        public const string imageFilePath = @"C:\Users\Dan Strandberg\Dropbox\Kod\repos\Teknikhögskolan\Projektarbete\ProjektarbeteButik\Images\";
        public const string discountFilePath = @"C:\Users\Dan Strandberg\Dropbox\Kod\repos\Teknikhögskolan\Projektarbete\ProjektarbeteButik\DiscountCodes.csv";
        public TextBox nameBox;
        public TextBox descriptionBox;
        public TextBox priceBox;
        public List<string> productsList = new List<string>();
        public string imageFileName;
        public TextBox discountCodeName;
        public TextBox discountPercentage;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "GUI App";
            Height = 600;
            Width = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel productAddPanel = AddProductPanel();
            grid.Children.Add(productAddPanel);
            Grid.SetColumn(productAddPanel, 0);
            Grid.SetRow(productAddPanel, 0);
            Grid.SetRowSpan(productAddPanel, 2);

            StackPanel discountCodePanel = AddDiscountCodePanel();
            grid.Children.Add(discountCodePanel);
            Grid.SetColumn(discountCodePanel, 1);
            Grid.SetRow(discountCodePanel, 0);
            Grid.SetRowSpan(discountCodePanel, 2);
        }
        public StackPanel AddDiscountCodePanel()
        {
            StackPanel discountInventoryPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
                Background = Brushes.OldLace
            };

            Label discountyPanel = new Label
            {
                Content = "Add Discount Code",
                Margin = spacing,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            discountInventoryPanel.Children.Add(discountyPanel);

            Label nameLabel = new Label
            {
                Content = "Discount Code Name",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(nameLabel);
            Grid.SetColumn(nameLabel, 0);
            Grid.SetRow(nameLabel, 1);

            discountCodeName = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(discountCodeName);
            Grid.SetColumn(discountCodeName, 1);
            Grid.SetRow(discountCodeName, 1);

            Label discountPercent = new Label
            {
                Content = "Discount Percentage",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(discountPercent);
            Grid.SetColumn(discountPercent, 0);
            Grid.SetRow(discountPercent, 2);

            discountPercentage = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(discountPercentage);
            Grid.SetColumn(discountPercentage, 1);
            Grid.SetRow(discountPercentage, 2);

            Button saveDiscountCode = new Button
            {
                Content = "Add Discount Code",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(saveDiscountCode);
            Grid.SetColumn(saveDiscountCode, 1);
            Grid.SetRow(saveDiscountCode, 3);
            saveDiscountCode.Click += SaveDiscountCode_Click;

            return discountInventoryPanel;
        }

        private void SaveDiscountCode_Click(object sender, RoutedEventArgs e)
        {
            List<string> discountList = new List<string>();
            discountList.Add(discountCodeName.Text + "," + discountPercentage.Text);
            string discountCode = "";
            foreach (string i in discountList)
            {
                discountCode = i;
            }
            File.AppendAllText(discountFilePath, Environment.NewLine + discountCode);
            discountCodeName.Clear();
            discountPercentage.Clear();
        }

        public StackPanel AddProductPanel()
        {
            StackPanel shopInventoryPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
                Background = Brushes.OldLace
            };

            Label shopInventoryLabel = new Label
            {
                Content = "Add Product",
                Margin = spacing,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            shopInventoryPanel.Children.Add(shopInventoryLabel);

            Label nameLabel = new Label
            {
                Content = "Product Name",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(nameLabel);
            Grid.SetColumn(nameLabel, 0);
            Grid.SetRow(nameLabel, 1);

            nameBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(nameBox);
            Grid.SetColumn(nameBox, 1);
            Grid.SetRow(nameBox, 1);
            
            Label descriptionLabel = new Label
            {
                Content = "Product Description",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(descriptionLabel);
            Grid.SetColumn(descriptionLabel, 0);
            Grid.SetRow(descriptionLabel, 2);
            
            descriptionBox  = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(descriptionBox);
            Grid.SetColumn(descriptionBox, 1);
            Grid.SetRow(descriptionBox, 2);
            
            Label priceLabel = new Label
            {
                Content = "Product Price",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(priceLabel);
            Grid.SetColumn(priceLabel, 0);
            Grid.SetRow(priceLabel, 3);

            priceBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(priceBox);
            Grid.SetColumn(priceBox, 1);
            Grid.SetRow(priceBox, 3);

            Button uploadImage = new Button
            {
                Content = "Upload Image",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(uploadImage);
            Grid.SetColumn(uploadImage, 1);
            Grid.SetRow(uploadImage, 4);
            uploadImage.Click += UploadImage_Click;

            Button addProductButton = new Button
            {
                Content = "Add Product To File",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(addProductButton);
            Grid.SetColumn(addProductButton, 0);
            Grid.SetRow(addProductButton, 5);
            Grid.SetColumnSpan(addProductButton, 2);
            addProductButton.Click += AddProductButton_Click;

            return shopInventoryPanel;
        }

        public void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                imageFileName = dlg.FileName;
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            productsList.Add(nameBox.Text + "," + descriptionBox.Text + "," + priceBox.Text + "," + imageFileName);
            var hej = productsList.ToArray();
            string product = "";
            foreach (string i in productsList)
            {
                product = i;
            }
            File.AppendAllText(productFilePath, Environment.NewLine + product);
            nameBox.Clear();
            descriptionBox.Clear();
            priceBox.Clear();
            imageFileName = "";
        }
    }
}
