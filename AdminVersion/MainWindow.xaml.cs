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
        public TextBox discountCodeName;
        public TextBox discountPercentage;
        public StackPanel shopInventoryPanel;
        public ListBox discountCodes;
        public Button saveChangesButton;
        public Button changeDiscountCode;
        public List<string> discountsList = new List<string>();
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
            Title = "The Wonderful Items Shoppe - Admin";
            Height = 900;
            Width = 900;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Main grid
            Grid grid = new Grid();
            grid.Margin = spacing;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Content = grid;

            //Add Products (Top Left)
            StackPanel productAddPanel = AddProductPanel();
            grid.Children.Add(productAddPanel);
            Grid.SetColumn(productAddPanel, 0);
            Grid.SetRow(productAddPanel, 0);
            Grid.SetRowSpan(productAddPanel, 2);

            //Add Discounts (Top Right)
            StackPanel discountCodePanel = AddDiscountCodePanel();
            grid.Children.Add(discountCodePanel);
            Grid.SetColumn(discountCodePanel, 1);
            Grid.SetRow(discountCodePanel, 0);
            Grid.SetRowSpan(discountCodePanel, 2);

            //Scrolling for shop products list (Bottom Left)
            ScrollViewer inventoryScroller = new ScrollViewer();
            grid.Children.Add(inventoryScroller);
            Grid.SetColumn(inventoryScroller, 0);
            Grid.SetRow(inventoryScroller, 2);
            inventoryScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            inventoryScroller.CanContentScroll = true;

            //Panel connected to ScrollViewer
            shopInventoryPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
                Background = Brushes.OldLace
            };
            inventoryScroller.Content = shopInventoryPanel;

            //ListBox with existing discounts (Bottom Right)
            discountCodes = new ListBox { Margin = spacing };
            grid.Children.Add(discountCodes);
            Grid.SetColumn(discountCodes, 1);
            Grid.SetRow(discountCodes, 2);
            discountCodes.SelectionChanged += DiscountCodes_SelectionChanged;

            //Products loaded from .csv-file
            LoadProducts();
            //Discounts loaded from .csv-file
            LoadDiscounts();
        }
        public StackPanel AddProductPanel()
        {            
            StackPanel addProductPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
                Background = Brushes.OldLace
            };

            Label addProductLabel = new Label
            {
                Content = "Add Product",
                Margin = spacing,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            addProductPanel.Children.Add(addProductLabel);

            Label nameLabel = new Label
            {
                Content = "Product Name",
                Margin = spacing,
            };
            addProductPanel.Children.Add(nameLabel);

            nameBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            addProductPanel.Children.Add(nameBox);

            Label descriptionLabel = new Label
            {
                Content = "Product Description",
                Margin = spacing,
            };
            addProductPanel.Children.Add(descriptionLabel);

            descriptionBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            addProductPanel.Children.Add(descriptionBox);

            Label priceLabel = new Label
            {
                Content = "Product Price",
                Margin = spacing,
            };
            addProductPanel.Children.Add(priceLabel);

            priceBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            addProductPanel.Children.Add(priceBox);

            Button uploadImage = new Button
            {
                Content = "Upload Image",
                Margin = spacing,
            };
            addProductPanel.Children.Add(uploadImage);
            uploadImage.Click += UploadImage;

            imageFilePathBox = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            addProductPanel.Children.Add(imageFilePathBox);

            Button addProductButton = new Button
            {
                Content = "Add Product To File",
                Margin = spacing,
            };
            addProductPanel.Children.Add(addProductButton);
            addProductButton.Click += AddNewProduct;

            saveChangesButton = new Button
            {
                Content = "Save Changes to Product",
                Margin = spacing,
                IsEnabled = false,
                Tag = null,
            };
            addProductPanel.Children.Add(saveChangesButton);
            saveChangesButton.Click += SaveProductChanges;

            return addProductPanel;
        }
        public void LoadProducts()
        {
            //Method used to refresh the GUI whenever changes are saved, and on start-up
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
                        //Default image used if image path is invalid/missing
                        Image productImage = CreateImage(imageFilePath + "no-photo.jpg");
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
                    deleteProduct.Click += DeleteProduct;

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
                    changeContentButton.Click += ChangeProductContent;
                }
            }
        }
        public void UploadImage(object sender, RoutedEventArgs e)
        {
            //Allows user to select an image with File Explorer
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string imageFileName = dlg.FileName;
                imageFilePathBox.Text = imageFileName;
            }
        }
        private void AddNewProduct(object sender, RoutedEventArgs e)
        {
            // Error handling to make sure the user inputs the price as a decimal
            bool errorHandling = decimal.TryParse(priceBox.Text, out decimal result);
            if (errorHandling)
            {
                if (nameBox.Text != "" && descriptionBox.Text != "" && !nameBox.Text.Contains(",") && !descriptionBox.Text.Contains(","))
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
                else if (nameBox.Text == "" || descriptionBox.Text == "")
                {
                    MessageBox.Show("Please enter Name and Description");
                }
                else
                {
                    MessageBox.Show("Name or product description can not contain a ','");
                }
            }
            else
            {
                MessageBox.Show("Price Is Not In Correct Format");
            }
        }
        public void SaveProductsToFile()
        {
            //Changes saved to .csv-file and GUI is cleared
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
        private void ChangeProductContent(object sender, RoutedEventArgs e)
        {
            //Moves information of selected product to corresponding GUI elements
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            //Also enables the button to save changes and gives it the selected product as "Tag"
            saveChangesButton.IsEnabled = true;
            saveChangesButton.Tag = product;

            nameBox.Text = product.Name;
            descriptionBox.Text = product.Description;
            priceBox.Text = product.Price.ToString();
            imageFilePathBox.Text = product.PicturePath;
        }
        private void DeleteProduct(object sender, RoutedEventArgs e)
        {
            //Completely removes a product from the inventory
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            productsList.Remove(product);
            SaveProductsToFile();
        }
        private void SaveProductChanges(object sender, RoutedEventArgs e)
        {
            //Edits an existing product in productsList and saves changes to .csv-file
            Button button = (Button)sender;
            var product = (Product)button.Tag;
            bool errorHandling = decimal.TryParse(priceBox.Text, out decimal result);
            if (errorHandling)
            {
                if (nameBox.Text != "" && descriptionBox.Text != "" && !nameBox.Text.Contains(",") && !descriptionBox.Text.Contains(","))
                {
                    product.Name = nameBox.Text;
                    product.Description = descriptionBox.Text;
                    product.Price = decimal.Parse(priceBox.Text);
                    product.PicturePath = imageFilePathBox.Text;
                    SaveProductsToFile();
                    MessageBox.Show("Changes Saved");
                }
                else if (nameBox.Text == "" || descriptionBox.Text == "")
                {
                    MessageBox.Show("Please enter Name and Description");
                }
                else
                {
                    MessageBox.Show("Name or product description can not contain a ','");
                }
            }
            else
            {
                MessageBox.Show("Price Is Not In Correct Format");
            }
        }
        public StackPanel AddDiscountCodePanel()
        {
            StackPanel addDiscountPanel = new StackPanel
            {
                Margin = spacing,
                Orientation = Orientation.Vertical,
                Background = Brushes.OldLace
            };

            Label addDiscountLabel = new Label
            {
                Content = "Add Discount Code",
                Margin = spacing,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            addDiscountPanel.Children.Add(addDiscountLabel);

            Label nameLabel = new Label
            {
                Content = "Discount Code Name",
                Margin = spacing,
            };
            addDiscountPanel.Children.Add(nameLabel);

            discountCodeName = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            addDiscountPanel.Children.Add(discountCodeName);

            Label discountPercent = new Label
            {
                Content = "Discount Percentage",
                Margin = spacing,
            };
            addDiscountPanel.Children.Add(discountPercent);

            discountPercentage = new TextBox
            {
                Text = "",
                Margin = spacing,
            };
            addDiscountPanel.Children.Add(discountPercentage);

            Button saveDiscountCode = new Button
            {
                Content = "Add Discount Code",
                Margin = spacing,
            };
            addDiscountPanel.Children.Add(saveDiscountCode);
            saveDiscountCode.Click += SaveDiscountCode;

            Button deleteDiscountCode = new Button
            {
                Content = "Delete Discount Code",
                Margin = spacing,
            };
            addDiscountPanel.Children.Add(deleteDiscountCode);
            deleteDiscountCode.Click += DeleteDiscountCode;

            changeDiscountCode = new Button
            {
                Content = "Save Changes To Discount Code",
                Margin = spacing,
                IsEnabled = false,
            };
            addDiscountPanel.Children.Add(changeDiscountCode);
            changeDiscountCode.Click += ChangeDiscountCode;

            return addDiscountPanel;
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
        private void SaveDiscountCode(object sender, RoutedEventArgs e)
        {
            // Error handling to make sure the user inputs the discount percentage as an int
            bool errorHandling = int.TryParse(discountPercentage.Text, out int result);

            if (errorHandling)
            {
                //Discount converted from a percentage to a multiplier between 0 and 1
                double discount = 1 - int.Parse(discountPercentage.Text) * 0.01;
                if (!discountCodeName.Text.Contains(",") && discountCodeName.Text != "" && discount > 0.01 && 1 >= discount)
                {
                    discountsList.Add(discountCodeName.Text.ToLower() + "," + discount);
                    File.WriteAllLines(discountFilePath, discountsList);
                    RefreshDiscounts();
                    MessageBox.Show("Discount Code Added To Inventory");                    
                }
                else if (discountCodeName.Text.Contains(","))
                {
                    MessageBox.Show("Discount Code Name Can Not Contain ','");
                }
                else if (discountCodeName.Text == "")
                {
                    MessageBox.Show("Discount Name Can Not Be Empty");
                }
                else
                {
                    MessageBox.Show("Discount must be between 1 and 100%");
                }
            }
            else
            {
                MessageBox.Show("Discount Percentage must be an integer between 1 and 100");
            }
        }
        private void DeleteDiscountCode(object sender, RoutedEventArgs e)
        {
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
            //Refreshes the GUI whenever changes are made to Discount Codes
            discountCodes.Items.Clear();
            discountCodeName.Clear();
            discountPercentage.Clear();
            changeDiscountCode.IsEnabled = false;
            foreach (string s in discountsList)
            {
                string[] parts = s.Split(",");
                double display = Math.Round((1 - double.Parse(parts[1])) * 100);
                string displayDiscount = "Code: " + parts[0] + " - " + " Discount: " + display + "%";
                discountCodes.Items.Add(displayDiscount);
            }
        }
        private void DiscountCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeDiscountCode.IsEnabled = true;
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
        private void ChangeDiscountCode(object sender, RoutedEventArgs e)
        {
            // Error handling to make sure the user inputs the discount percentage as an int
            bool errorHandling = int.TryParse(discountPercentage.Text, out int result);
            
            if (errorHandling)
            {
                int index = discountCodes.SelectedIndex;
                //Discount converted from a percentage to a multiplier between 0 and 1
                double discount = 1 - int.Parse(discountPercentage.Text) * 0.01;
                if (!discountCodeName.Text.Contains(",") && discountCodeName.Text != "" && discount > 0.01 && 1 >= discount)
                {
                    discountsList[index] = discountCodeName.Text.ToLower() + "," + discount;
                    File.WriteAllLines(discountFilePath, discountsList);
                    RefreshDiscounts();
                    MessageBox.Show("Saved changes to Discount Code");
                }
                else if (discountCodeName.Text.Contains(","))
                {
                    MessageBox.Show("Discount Code Name Can Not Contain ','");
                }
                else if (discountCodeName.Text == "")
                {
                    MessageBox.Show("Discount Name Can Not Be Empty");
                }
                else
                {
                    MessageBox.Show("Discount must be between 1 and 100%");
                }
            }
            else
            {
                MessageBox.Show("Discount Percentage must be an integer between 1 and 100");
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