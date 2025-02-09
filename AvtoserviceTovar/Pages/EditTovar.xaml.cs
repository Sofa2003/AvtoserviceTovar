using AvtoserviceTovar.Models;
using Microsoft.Win32;
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
using static AvtoserviceTovar.MainWindow;

namespace AvtoserviceTovar.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditTovar.xaml
    /// </summary>
    public partial class EditTovar : Page
    {
        Product sr = new Product();
        private Product currentService;
        private string selectedImagePath;
        public string seldnull = "";
        public EditTovar(Product service)
        {
            InitializeComponent();
            currentService = service;
            if (service != null)
            {
                tbID.Visibility = Visibility.Visible;
                tbID.Text = service.ProductArticleNumber.ToString();
                // Заполнение полей данными услуги
                tbName.Text = service.ProductName;
                tbQualet.Text = service.ProductQuantityInStock.ToString();
                tbManufactur.Text = service.ProductManufacturer;
                tbCost.Text = service.ProductCost.ToString();
                tbdescription.Text = service.ProductDescription;
                tbStatus.Text = service.ProductStatus;
                tbCategori.Text = service.ProductCategory; 
                tbDiscount.Text = service.ProductDiscountAmount.ToString();

                // Загрузка изображения
                var imagePath = System.IO.Path.Combine("TovarImage", service.ProductPhoto);
                ImageService.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                selectedImagePath = service.ProductPhoto; // Сохраняем путь изображения
            }
            else
            {
                lblID.Visibility = Visibility.Hidden;
                // Если это новая услуга, скрываем поле ID
                tbID.Visibility = Visibility.Collapsed;
            }

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new List<string>();

            //if (string.IsNullOrWhiteSpace(tbName.Text))
            //    errors.Add("Услуга не может быть пустой");

            if (!int.TryParse(tbCost.Text, out int cost) /*|| cost <= 0*/)
                errors.Add("Цена должна быть положительным числом");

            ////if (!int.TryParse(tbStatus.Text, out int duration) || duration <= 0)
            ////    errors.Add("Длительность должна быть положительным числом");

            if (int.TryParse(tbQualet.Text, out int category) /*|| category <= 0*/)
            {
                errors.Add("Длительность должна быть положительным числом");
            }
            //if (errors.Count > 0)
            //{
            //    MessageBox.Show(string.Join("\n", errors), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return; // Прерываем выполнение, если есть ошибки
            //}
            var existingService = helper.GetContext().Product
            .FirstOrDefault(s => s.ProductName.Equals(tbName.Text, StringComparison.OrdinalIgnoreCase));

            if (existingService != null && currentService == null)
            {
                MessageBox.Show("Услуга с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Прерываем выполнение, если услуга уже существует
            }

            // Если текущая услуга существует, обновляем ее, иначе создаем новую
            if (currentService != null)
            {   currentService.ProductArticleNumber = tbID.Text;
                currentService.ProductManufacturer = tbManufactur.Text;
                currentService.ProductCategory = tbCategori.Text;
                currentService.ProductQuantityInStock = category;
                currentService.ProductName = tbName.Text;
                currentService.ProductCost = cost;
                currentService.ProductStatus = tbStatus.Text;
                currentService.ProductDescription = tbdescription.Text;
                currentService.ProductDiscountAmount = byte.TryParse(tbDiscount.Text, out byte newDiscount) ? newDiscount : (byte?)null;


                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    if (!currentService.ProductPhoto.StartsWith("/TovarImage/"))
                    {
                        // Если не начинается, добавляем префикс
                        currentService.ProductPhoto = "/TovarImage/" + selectedImagePath;
                        MessageBox.Show("Типо нет: " + currentService.ProductPhoto);
                    }
                    else
                    {
                        // Если уже начинается с нужной строки, проверяем, не дублируем ли мы путь
                        string imageFileName = System.IO.Path.GetFileName(selectedImagePath);
                        string existingFileName = System.IO.Path.GetFileName(currentService.ProductPhoto);

                        if (existingFileName != imageFileName)
                        {
                            // Добавляем только если имена файлов различаются
                            currentService.ProductPhoto += selectedImagePath; // Или другой способ обработки
                        }
                        else
                        {
                            //MessageBox.Show("Изображение уже добавлено: " + currentService.MainImagePath);
                        }
                    }
                }
                // Обновляем существующую запись
                helper.GetContext().SaveChanges();
                MessageBox.Show("Услуга успешно отредактирована");

            }
            else
            {
                // Создаем новую услугу
                Product newService = new Product
                {   
                    ProductName = tbName.Text,
                    ProductCost = cost,
                    ProductDescription = tbdescription.Text,
                    ProductDiscountAmount = byte.TryParse(tbDiscount.Text, out byte newDiscount) ? newDiscount : (byte?)null,
                    ProductPhoto = !string.IsNullOrEmpty(selectedImagePath) ? "/Услуги салона красоты/" + selectedImagePath : null,
                    ProductCategory  = tbCategori.Text,
                    ProductManufacturer  = tbManufactur.Text,
                    ProductStatus = tbStatus.Text

                };

                helper.GetContext().Product.Add(newService);
                helper.GetContext().SaveChanges();
                MessageBox.Show("Услуга успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new SpisokTovar());
            }

        }

        private void btnDownloadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; // Фильтр по типам изображений

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    // Проверяем размер файла (не более 2 МБ)
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > 2 * 1024 * 1024) // 2 МБ
                    {
                        MessageBox.Show("Размер изображения не должен превышать 2 МБ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Загружаем изображение в ImageControl
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filePath);
                    bitmap.EndInit();
                    bitmap.Freeze(); // Замораживаем изображение для многопоточности
                    this.ImageService.Source = bitmap;

                    // Сохраняем имя файла для дальнейшего использования
                    selectedImagePath = System.IO.Path.GetFileName(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        
    }
}
