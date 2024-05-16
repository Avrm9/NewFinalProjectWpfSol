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
using System.Data.SqlClient;
using System.Configuration;
using NewFinalProjectWpf;
using APIService;
using Model;

namespace StartPage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApiService apiService;
        UserList userList;
        public MainWindow()
        {
            InitializeComponent();
            apiService = new ApiService();
            start();
        }
        public async void start()
        {
            userList = await apiService.GetUser();
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\logo.png", UriKind.Relative))
            };
            MainImage.Background = imageBrush;
        }
        public async void RefreashUsers()
        {
            userList = await apiService.GetUser();

        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            RefreashUsers();

            string username = txtUsername.Text;
            string password = txtPassword.Text;
            if (userList.Find(x => x.UserName == username && x.Pass == password) == null)
            {
                MessageBox.Show("There is no account with this details");
            }
            else
            {
                User u = userList.Find(x => x.UserName == username && x.Pass == password);
                OpenApp(u);
            }


        }
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            RefreashUsers();
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            if (userList.Find(x => x.UserName == username) != null)
            {
                MessageBox.Show("User Is Used");
            }
            else if(username == "" || password == null){
                MessageBox.Show("Invalid");
            }
            else
            {
                User user = new User() { UserName = username, Pass = password, Permission = false };
                apiService.InsertUser(user);
                OpenApp(user);
            }

        }


        private void OpenApp(User user)
        {
            NewFinalProjectWpf.MainWindow main = new  NewFinalProjectWpf.MainWindow(user);
            main.Show();
            Close();

        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    
}

