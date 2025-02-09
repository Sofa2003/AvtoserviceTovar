using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
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
    /// Логика взаимодействия для Avtorise.xaml
    /// </summary>
    public partial class Avtorise : Page
    {
        public Avtorise()
        {
            InitializeComponent();
            txtboxCaptcha.Visibility = Visibility.Hidden;
            txtBlockCaptcha.Visibility = Visibility.Hidden;
        }
        private int countUnsuccessfull = 0;


        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            User users = new User();
            string login = txtbLogin.Text.Trim();
            string password = pswbPassword.Password.Trim();

            if (login.Length > 0 && password.Length > 0)
            {
                if (countUnsuccessfull < 1)
                {
                  
                    var id = helper.GetContext().User.Where(u => u.UserLogin == login && u.UserPassword == password).Select(i => i.UserID).FirstOrDefault();
                    var useri = helper.GetContext().User.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
                    //Проверка пользователя базы данных по логину и паролю
                    if (useri != null)
                    {

                        //Проверка на корректность введеных данных и переход на двухфакторную аутификацию

                        switch (useri.UserRole)
                        {
                            case 1:
                                NavigationService.Navigate(new AvtoserviceTovar.Pages.SpisokTovar());
                                break;

                            case 2:
                                NavigationService.Navigate(new AvtoserviceTovar.Pages.SpisokTovar());
                                break;
                            case 3:
                                NavigationService.Navigate(new AvtoserviceTovar.Pages.SpisokTovar());
                                break;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким логином или паролем не существует", "Ошибка", MessageBoxButton.OK);
                        countUnsuccessfull++;
                        GenerateCaptcha();
                    }



                }
                //Генерация каптчи
                else
                {
                    var useri = helper.GetContext().User.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
                    string capctha = txtBlockCaptcha.Text.Trim();
                    if (useri != null && capctha == txtboxCaptcha.Text)
                    {
                        txtBlockCaptcha.Visibility = Visibility.Hidden;
                        txtboxCaptcha.Visibility = Visibility.Hidden;
                        txtboxCaptcha.Text = "";
                        countUnsuccessfull = 0;
                        var id = helper.GetContext().User.Where(u => u.UserLogin == login && u.UserPassword == password).Select(i => i.UserID).FirstOrDefault();
                        switch (useri.UserRole)
                        {
                            case 1:
                                NavigationService.Navigate(new AvtoserviceTovar.Pages.SpisokTovar());
                                break;

                            case 2:
                                NavigationService.Navigate(new AvtoserviceTovar.Pages.SpisokTovar());
                                break;
                            case 3:
                                NavigationService.Navigate(new AvtoserviceTovar.Pages.SpisokTovar());
                                break;
                        }
                    } //Проверка на количество ошибок вввода
                    else
                    {
                        if (countUnsuccessfull <= 2)
                        {
                            MessageBox.Show("Капча введена неверно", "Ошибка", MessageBoxButton.OK);
                            countUnsuccessfull++;
                            GenerateCaptcha();

                        }
                        //Блокировка ввода если каптча была 2 раза неправильной
                        else
                        {
                            MessageBox.Show("Капча введена неверно более двух раз", "Ошибка", MessageBoxButton.OK);
                            countUnsuccessfull++;
                            GenerateCaptcha();
                            wait();
                        }


                    }





                }
            }
            else
            {
                MessageBox.Show("Не все обязательные поля были заполнены! Проверьте заполненность и повторите попытку!", "Внимание!", MessageBoxButton.OK);
            }

        }
        public void GenerateCaptcha()
        {
            txtboxCaptcha.Visibility = Visibility.Visible;
            txtBlockCaptcha.Visibility = Visibility.Visible;
            const string chars = "ABCDEFGHIJKLMNOPQRS0123456789";
            Random random = new Random();
            string captcha = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            txtBlockCaptcha.Text = captcha;
            txtBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }
        private async void wait()
        {
            lblTime.Visibility = Visibility.Visible;
            int remainingTime = 10;
            btnEnterGuests.IsEnabled = false;
            btnEnter.IsEnabled = false;
            txtbLogin.IsEnabled = false;
            pswbPassword.IsEnabled = false;
            txtboxCaptcha.IsEnabled = false;

            while (remainingTime > 0)
            {
                lblTime.Content = $"До конца блокировки {remainingTime} сек";
                await Task.Delay(1000);
                remainingTime--;
            }
            lblTime.Content = "Блокировка завершена!";
            btnEnterGuests.IsEnabled = true;
            btnEnter.IsEnabled = true;
            txtbLogin.IsEnabled = true;
            pswbPassword.IsEnabled = true;
            txtboxCaptcha.IsEnabled = true;
            await Task.Delay(2000);
            lblTime.Content = " ";

        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SpisokTovar());
        }
    }
}
