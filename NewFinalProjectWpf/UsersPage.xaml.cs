using APIService;
using Model;
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

namespace NewFinalProjectWpf
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public event EventHandler ClosePage;
        Button BackB;
        ApiService apiService;
        UserList userlist;
        public UsersPage()
        {
            InitializeComponent();
            start();
        }
        //Start
        public async void start()
        {
            //Design
            BackB = new Button() { Height = 50, Width = 50, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Top };
            ImageBrush imageBrush11 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\BackAr.png", UriKind.Relative))
            };
            BackB.Background = imageBrush11;
            BackB.Click += ButtonClickBack;
            MainGrid.Children.Add(BackB);
            //
            //Update Stats
            apiService = new ApiService();
            userlist = await apiService.GetUser();
            MainListView.ItemsSource = userlist;

            //
        }
        //
        //Click on back Arrow Button
        public void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            BackB.Visibility = Visibility.Hidden;
            ClosePage?.Invoke(this, EventArgs.Empty);
        }
        //
        //Refreash Users
        public async Task RefreashUsers()
        {
            userlist = await apiService.GetUser();
            MainListView.ItemsSource = userlist;
        }
        //
        //Insert Update Delete List
        public async void DoneUpdate(object sender, RoutedEventArgs e)
        {
            userlist = await apiService.GetUser();
            foreach (User item in MainListView.Items)
            {
                User u = userlist.Find(x => x.Id == item.Id);
                if (u != null)
                {
                    u.UserName = item.UserName.Trim();
                    u.Pass = item.Pass.Trim();
                    u.Permission = item.Permission;
                    await apiService.UpdateUser(u);
                }
                
            }

        }
        private async void ButtonClickList(object sender, RoutedEventArgs e)
        {
            string head = (sender as MenuItem).Header.ToString();
            if (head == "Add")
            {
                DoneUpdateB.Visibility = Visibility.Collapsed;
                DelUserStack.Visibility = Visibility.Collapsed;
                AddUserStack.Visibility = Visibility.Visible;

                DoneAdd.Click += async delegate
                {
                    await RefreashUsers();
                    string Username = UserNameBoxAdd.Text;
                    string pass = PassBoxAdd.Text;
                    bool per;
                    try
                    {
                         per = bool.Parse(PermissionBoxAdd.Text);
                    }
                    catch { MessageBox.Show("Invalid"); return; }
                    if (Username == "" || Username == "Enter The UserName And Delete This Content"  )
                    {
                        MessageBox.Show("Invalid");
                    }
                    else
                    {
                        AddUserStack.Visibility = Visibility.Collapsed;
                        DoneUpdateB.Visibility = Visibility.Visible;
                        User u = new User() {UserName = Username, Pass = pass, Permission = per};
                        await apiService.InsertUser(u);
                        MainListView.ItemsSource = await apiService.GetUser();
                    }
                    await RefreashUsers();
                };
            }
            else if (head == "Delete") 
            {
                DoneUpdateB.Visibility = Visibility.Collapsed;
                AddUserStack.Visibility = Visibility.Collapsed;
                DelUserStack.Visibility = Visibility.Visible;
                DoneDel.Click += async delegate
                {
                    await RefreashUsers();
                    string username = UserNameBoxDel.Text;
                    if (userlist.Find(x => x.UserName == username) == null)
                    {
                        MessageBox.Show("Invalid");
                    }
                    else
                    {
                        DoneUpdateB.Visibility = Visibility.Visible;
                        DelUserStack.Visibility = Visibility.Collapsed;
                        User u = userlist.Find(x => x.UserName == username);
                        await apiService.DeleteUser(u);
                        MainListView.ItemsSource = await apiService.GetUser();
                        await RefreashUsers();
                    }
                };
                await RefreashUsers();
            }
        }
        //
    }
}
