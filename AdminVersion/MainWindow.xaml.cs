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
    public class Product
    {
        public string Name;
        public string Description;
        public string Price;
        public string PicturePath;
    }
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);

        public const string productFilePath = @"C:\Windows\Temp\ShopInventory.csv";
        public const string imageFilePath = @"C:\Users\Dan Strandberg\Dropbox\Kod\repos\Teknikhögskolan\Projektarbete\AdminVersion\Images\";
        public const string discountFilePath = @"C:\Users\Dan Strandberg\Dropbox\Kod\repos\Teknikhögskolan\Projektarbete\ProjektarbeteButik\DiscountCodes.csv";
        public TextBox nameBox;
        public TextBox descriptionBox;
        public TextBox priceBox;
        public List<string> productsList = new List<string>();
        public string imageFileName;
        public TextBox discountCodeName;
        public TextBox discountPercentage;
        public TextBox imageFilePathBox;
        public StackPanel shopInventoryPanel;
        public List<Product> produktLista = new List<Product>();

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "GUI App";
            Height = 900;
            Width = 900;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
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

            StackPanel shopInventory = CreateShopInventoryPanel();
            grid.Children.Add(shopInventory);
            Grid.SetColumn(shopInventory, 0);
            Grid.SetRow(shopInventory, 2);
            Grid.SetRowSpan(shopInventory, 2);

            LoadProducts();
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

            // Error handling to make sure the user inputs the discount percentage as an int
            bool errorHandling = int.TryParse(discountPercentage.Text, out int result);

            if (errorHandling == true)
            {
                discountList.Add(discountCodeName.Text + "," + discountPercentage.Text);
                string discountCode = "";
                foreach (string i in discountList)
                {
                    discountCode = i;
                }
                File.AppendAllText(discountFilePath, Environment.NewLine + discountCode);
                discountCodeName.Clear();
                discountPercentage.Clear();
                MessageBox.Show("Discount Code Added To Inventory");
            }
            else
            {
                MessageBox.Show("Discount Percentage Not In Correct Format");
            }
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

            descriptionBox = new TextBox
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

            imageFilePathBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(imageFilePathBox);
            Grid.SetColumn(imageFilePathBox, 1);
            Grid.SetRow(imageFilePathBox, 5);

            Button addProductButton = new Button
            {
                Content = "Add Product To File",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(addProductButton);
            Grid.SetColumn(addProductButton, 0);
            Grid.SetRow(addProductButton, 6);
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
                imageFilePathBox.Text = imageFileName;
            }
        }
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Error handling to make sure the user inputs the price as a decimal
            bool errorHandling = decimal.TryParse(priceBox.Text, out decimal result);

            if (errorHandling == true)
            {
                Product newProduct = new Product
                {
                    Name = nameBox.Text,
                    Description = descriptionBox.Text,
                    Price = priceBox.Text,
                    PicturePath = imageFilePathBox.Text,
                };
                produktLista.Add(newProduct);
                foreach (var i in produktLista)
                {
                    productsList.Add(i.Name + "," + i.Description + "," + i.Price + "," + i.PicturePath + Environment.NewLine);
                }
                string product = "";
                foreach (string i in productsList)
                {
                    product = i;
                }
                File.AppendAllText(productFilePath, product);
                nameBox.Clear();
                descriptionBox.Clear();
                priceBox.Clear();
                imageFilePathBox.Text = " ";

                MessageBox.Show("Product Added To Inventory");
            }
            else
            {
                MessageBox.Show("Price Is Not In Correct Format");
            }
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

            return shopInventoryPanel;
        }
        public void LoadProducts()
        {
            if (!File.Exists(productFilePath))
            {
                File.Create(productFilePath);
            }
            else
            {
                //Reads inventory from .csv-file and creates a grid for each item. Grid is added to shopInventoryPanel.
                string[] products = File.ReadAllLines(productFilePath);

                foreach (string s in products)
                {
                    Grid productGrid = new Grid();
                    productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    productGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    shopInventoryPanel.Children.Add(productGrid);

                    var productProperties = s.Split(',');
                    Product p = new Product
                    {
                        Name = productProperties[0],
                        Description = productProperties[1],
                        Price = productProperties[2],
                        PicturePath = productProperties[3]
                    };
                    produktLista.Add(p);

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
                    Grid.SetColumnSpan(productLabel, 2);

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
                    Grid.SetColumn(priceLabel, 4);

                    Button deleteProduct = new Button
                    {
                        Content = "Delete",
                        Margin = spacing,
                        Tag = p,
                    };
                    productGrid.Children.Add(deleteProduct);
                    Grid.SetRow(deleteProduct, 1);
                    Grid.SetColumn(deleteProduct, 3);
                    deleteProduct.Click += DeleteProduct_Click;

                    Button changeContentButton = new Button
                    {
                        Content = "Change Content",
                        Margin = spacing,
                        Padding = spacing,
                        Tag = p,
                    };
                    productGrid.Children.Add(changeContentButton);
                    Grid.SetRow(changeContentButton, 1);
                    Grid.SetColumn(changeContentButton, 4);
                    changeContentButton.Click += ChangeContentButton_Click;
                }
            }
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            //Completely removes a product from the inventory
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            produktLista.Remove(product);
            productsList.Clear();

            foreach (var i in produktLista)
            {
                productsList.Add(i.Name + "," + i.Description + "," + i.Price + "," + i.PicturePath + Environment.NewLine);
            }
            string produkt = "";
            foreach (string s in productsList)
            {
                produkt += s;
            }
            File.WriteAllText(productFilePath, produkt);
        }
        private void ChangeContentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var product = (Product)button.Tag;

            nameBox.Text = product.Name;
            descriptionBox.Text = product.Description;
            priceBox.Text = product.Price;
            imageFilePathBox.Text = product.PicturePath;
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
                //Size of images limited so shop inventory looks uniform
                MaxHeight = 50,
                MaxWidth = 50
            };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }
    }
}

