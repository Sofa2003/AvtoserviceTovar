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
using AvtoserviceTovar.Models;
using static AvtoserviceTovar.MainWindow;

namespace AvtoserviceTovar.Pages
{
    /// <summary>
    /// Логика взаимодействия для SpisokTovar.xaml
    /// </summary>
    public partial class SpisokTovar : Page
    {
        private List<Product> listspisokproduct;
        private string searchQuery = "";
        public SpisokTovar()
        {

            InitializeComponent();
            listspisokproduct = helper.GetContext().Product.ToList();
            SortBox.Items.Add("Все");
            SortBox.Items.Add("0 - 9.99%");
            SortBox.Items.Add("10 - 14.99%");
            SortBox.Items.Add("15 и более");
            SortBox.SelectedIndex = 0;
            DiscountBox.Items.Add("Без сортировки");
            DiscountBox.Items.Add("По возрастанию");
            DiscountBox.Items.Add("По убыванию");
            DiscountBox.SelectedIndex = 0;

        }
        public void Load()
        {
            List<Product> services = new List<Product>();
            var serviceQuery = listspisokproduct.AsQueryable();
            serviceGrid.ItemsSource = serviceQuery.ToList();
            int totalCount = listspisokproduct.Count();

            foreach (var service in serviceQuery)
            {
                if (string.IsNullOrEmpty(service.ProductPhoto))
                {
                    service.ProductPhoto = "TovarImage/picture.png"; // Путь к изображению по умолчанию
                }
                serviceGrid.ItemsSource = serviceQuery.ToList();
            }
            // Фильтрация по скидке
            switch (SortBox.SelectedItem.ToString())
            {
                case "0 - 9.99%":
                    serviceQuery = serviceQuery.Where(s => s.ProductDiscountAmount == null || (s.ProductDiscountAmount >= 0 && s.ProductDiscountAmount < 9.99));
                    break;
                case "10 - 14.99%":
                    serviceQuery = serviceQuery.Where(s => s.ProductDiscountAmount >= 10 && s.ProductDiscountAmount < 14.99);
                    break;
                case "15 и более":
                    serviceQuery = serviceQuery.Where(s => s.ProductDiscountAmount >= 15 && s.ProductDiscountAmount < 100);
                    break;
                case "Все":
                default:
                    break;
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                serviceQuery = serviceQuery.Where(Service =>
                Service.ProductName.Contains(searchQuery));
            }
            string selectedDiscount = DiscountBox.SelectedItem?.ToString(); // Получаем выбранный элемент и проверяем на null

            switch (selectedDiscount)
            {
                case "По возрастанию":
                    serviceQuery = serviceQuery.OrderBy(s => s.ProductCost); // Сортировка по возрастанию
                    break;
                case "По убыванию":
                    serviceQuery = serviceQuery.OrderByDescending(s => s.ProductCost); // Сортировка по убыванию
                    break;
                case "Без сортировки":
                default:
                    break;
            }

            var filteredServices = serviceQuery.ToList();
            serviceGrid.ItemsSource = filteredServices;

            // Обновляем текст с количеством элементов
            RecordCountLabel.Content = $"{filteredServices.Count} из {totalCount}";

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchQuery = SearchBox.Text;
            Load();
        }

        private void DiscountBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load();
        }

        private void btncreate_Click(object sender, RoutedEventArgs e)
        {
                EditTovar create = new EditTovar(null);
                NavigationService.Navigate(create);
        }

        private void serviceGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditTovar pageEdit = new EditTovar((Product)serviceGrid.SelectedItem);
            NavigationService.Navigate(pageEdit);
        }
    }
}
