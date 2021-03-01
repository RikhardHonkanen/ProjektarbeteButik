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
using ProjektarbeteButik;

namespace AdminVersion
{
    public partial class MainWindow : Window
    {
        public Thickness spacing = new Thickness(5);
        public const string productFilePath = @"C:\Windows\Temp\ProjektarbeteButik\ShopInventory.csv";
        public const string imageFilePath = @"C:\Windows\Temp\ProjektarbeteButik\ProjektarbeteButikImages\";
        public const string discountFilePath = @"C:\Windows\Temp\ProjektarbeteButik\DiscountCodes.csv";
        public TextBox nameBox;
        public TextBox descriptionBox;
        public TextBox priceBox;
        public TextBox imageFilePathBox;
        public string imageFileName;
        public TextBox discountCodeName;
        public TextBox discountPercentage;
        public StackPanel shopInventoryPanel;
        public ListBox discountCodes;
        public Button saveChangesButton;
        public List<string> discountsList = new List<string>();
        public List<Product> productsList = new List<Product>();        

        public MainWindow()
        {
            //For push
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
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
            grid.Margin = spacing;
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

            discountCodes = new ListBox { Margin = spacing };
            grid.Children.Add(discountCodes);
            Grid.SetColumn(discountCodes, 1);
            Grid.SetRow(discountCodes, 2);
            discountCodes.SelectionChanged += DiscountCodes_SelectionChanged;

            LoadProducts();
            LoadDiscounts();
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

            nameBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(nameBox);

            Label descriptionLabel = new Label
            {
                Content = "Product Description",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(descriptionLabel);

            descriptionBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(descriptionBox);

            Label priceLabel = new Label
            {
                Content = "Product Price",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(priceLabel);

            priceBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(priceBox);

            Button uploadImage = new Button
            {
                Content = "Upload Image",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(uploadImage);
            uploadImage.Click += UploadImage_Click;

            imageFilePathBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(imageFilePathBox);

            Button addProductButton = new Button
            {
                Content = "Add Product To File",
                Margin = spacing,
            };
            shopInventoryPanel.Children.Add(addProductButton);
            addProductButton.Click += AddProductButton_Click;

            saveChangesButton = new Button
            {
                Content = "Save Changes to Product",
                Margin = spacing,
                IsEnabled = false,
                Tag = null,
            };
            shopInventoryPanel.Children.Add(saveChangesButton);
            saveChangesButton.Click += SaveChangesButton_Click;

            return shopInventoryPanel;
        }
        public void LoadProducts()
        {
            shopInventoryPanel.Children.Clear();
            productsList.Clear();
            saveChangesButton.IsEnabled = false;
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
                        Price = decimal.Parse(productProperties[2]),
                        PicturePath = productProperties[3]
                    };
                    productsList.Add(p);

                    try
                    {
                        Image productImage = CreateImage(p.PicturePath);
                        productImage.Stretch = Stretch.Fill;
                        productGrid.Children.Add(productImage);
                        Grid.SetRow(productImage, 0);
                        Grid.SetRowSpan(productImage, 2);
                        Grid.SetColumn(productImage, 0);
                    }
                    catch
                    {
                        Image productImage = CreateImage(@"C:\Windows\Temp\ProjektarbeteButik\ProjektarbeteButikImages\no-photo.jpg");
                        productImage.Stretch = Stretch.Fill;
                        productGrid.Children.Add(productImage);
                        Grid.SetRow(productImage, 0);
                        Grid.SetRowSpan(productImage, 2);
                        Grid.SetColumn(productImage, 0);
                    }

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

                    TextBlock productDescription = new TextBlock
                    {
                        Text = p.Description,
                        FontSize = 12,
                        FontWeight = FontWeights.Normal,
                        FontStyle = FontStyles.Italic,
                        TextWrapping = TextWrapping.Wrap
                    };
                    productGrid.Children.Add(productDescription);
                    Grid.SetRow(productDescription, 1);
                    Grid.SetRowSpan(productDescription, 2);
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
            //string[] pathParts = imageFileName.Split(@"\");
            //string imageToDirectory = @"C:\Windows\Temp\ProjektarbeteButik\ProjektarbeteButikImages\" + pathParts.Last();
            //File.Copy(imageFileName, imageToDirectory);
        }
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Error handling to make sure the user inputs the price as a decimal
            bool errorHandling = decimal.TryParse(priceBox.Text, out decimal result);
            if (errorHandling)
            {
                if (nameBox.Text != "" && descriptionBox.Text != "")
                {
                    Product newProduct = new Product
                    {
                        Name = nameBox.Text,
                        Description = descriptionBox.Text,
                        Price = decimal.Parse(priceBox.Text),
                        PicturePath = imageFilePathBox.Text,
                    };
                    productsList.Add(newProduct);
                    SaveProductsToFile();
                    MessageBox.Show("Product Added To Inventory");
                }
                else
                {
                    MessageBox.Show("Please enter Name and Description");
                }
            }
            else
            {
                MessageBox.Show("Price Is Not In Correct Format");
            }
        }
        public void SaveProductsToFile()
        {
            List<string> productsToFileList = new List<string>();
            foreach (var i in productsList)
            {
                productsToFileList.Add(i.Name + "," + i.Description + "," + i.Price + "," + i.PicturePath);
            }
            File.WriteAllLines(productFilePath, productsToFileList);
            nameBox.Clear();
            descriptionBox.Clear();
            priceBox.Clear();
            imageFilePathBox.Text = "";
            LoadProducts();
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            //Completely removes a product from the inventory
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            productsList.Remove(product);
            SaveProductsToFile();
        }
        private void ChangeContentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            saveChangesButton.IsEnabled = true;
            saveChangesButton.Tag = product;

            nameBox.Text = product.Name;
            descriptionBox.Text = product.Description;
            priceBox.Text = product.Price.ToString();
            imageFilePathBox.Text = product.PicturePath;
        }
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            bool errorHandling = decimal.TryParse(priceBox.Text, out decimal result);
            if (errorHandling)
            {
                if (nameBox.Text != "" && descriptionBox.Text != "")
                {
                    product.Name = nameBox.Text;
                    product.Description = descriptionBox.Text;
                    product.Price = decimal.Parse(priceBox.Text);
                    product.PicturePath = imageFilePathBox.Text;
                    MessageBox.Show("Changes Saved");
                    SaveProductsToFile();
                }
                else
                {
                    MessageBox.Show("Please enter Name and Description");
                }
            }
            else
            {
                MessageBox.Show("Price Is Not In Correct Format");
            }
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

            discountCodeName = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(discountCodeName);

            Label discountPercent = new Label
            {
                Content = "Discount Percentage",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(discountPercent);

            discountPercentage = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(discountPercentage);

            Button saveDiscountCode = new Button
            {
                Content = "Add Discount Code",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(saveDiscountCode);
            saveDiscountCode.Click += SaveDiscountCode_Click;

            Button deleteDiscountCode = new Button
            {
                Content = "Delete Discount Code",
                Margin = spacing,
            };
            discountInventoryPanel.Children.Add(deleteDiscountCode);
            deleteDiscountCode.Click += DeleteDiscountCode;

            return discountInventoryPanel;
        }
        public void LoadDiscounts()
        {
            if (!File.Exists(discountFilePath))
            {
                File.Create(discountFilePath);
            }
            else
            {
                string[] discounts = File.ReadAllLines(discountFilePath);
                foreach (string s in discounts)
                {
                    discountsList.Add(s);
                }
            }
            RefreshDiscounts();
        }
        private void DeleteDiscountCode(object sender, RoutedEventArgs e)
        {
            if (discountCodes.SelectedIndex == -1)
            {

            }
            int index = discountCodes.SelectedIndex;
            try
            {
                discountsList.RemoveAt(index);
            }
            catch
            {
                MessageBox.Show("No discount code selected");
            }
            File.WriteAllLines(discountFilePath, discountsList);
            RefreshDiscounts();
        }
        public void RefreshDiscounts()
        {
            discountCodes.Items.Clear();
            discountCodeName.Clear();
            discountPercentage.Clear();
            foreach (string s in discountsList)
            {
                string[] parts = s.Split(",");
                double display = Math.Round((1 - double.Parse(parts[1])) * 100);
                string displayDiscount = "Code: " + parts[0] + " Discount: " + display + "%";
                discountCodes.Items.Add(displayDiscount);
            }
        }
        private void DiscountCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Added try-catch, program crashed on ListBox being updated otherwise
            try
            {
                int index = discountCodes.SelectedIndex;
                string[] discount = discountsList[index].Split(",");
                double display = Math.Round((1 - double.Parse(discount[1])) * 100);
                discountCodeName.Text = discount[0];
                discountPercentage.Text = display.ToString();
            }
            catch
            {

            }
        }
        private void SaveDiscountCode_Click(object sender, RoutedEventArgs e)
        {
            // Error handling to make sure the user inputs the discount percentage as an int
            bool errorHandling = int.TryParse(discountPercentage.Text, out int result);

            if (errorHandling == true)
            {
                double discount = 1 - int.Parse(discountPercentage.Text) * 0.01;
                discountsList.Add(discountCodeName.Text.ToLower() + "," + discount);
                File.WriteAllLines(discountFilePath, discountsList);                
                MessageBox.Show("Discount Code Added To Inventory");
                RefreshDiscounts();
            }
            else
            {
                MessageBox.Show("Discount Percentage Not In Correct Format");
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