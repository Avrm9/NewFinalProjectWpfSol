using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using APIService;
using Model;
using static System.Net.Mime.MediaTypeNames;

namespace NewFinalProjectWpf
{
    public partial class MainWindow : Window
    {
        ApiService apiService;
        SportList sportlist;
        LeagueList leaguelist;
        TextBlock logo1;
        Button logo2;
        SpecialTeamsList SpecialTeamslist;
        TeamList Teamlist;
        Sport current;
        User currentuser;
        public MainWindow(User CurrentUser)
        {
            InitializeComponent();
            apiService = new ApiService();
            currentuser = CurrentUser;
            Start();
        }
        public async void Start()
        {
            // If Admin Show the add/insert/update
            if (!currentuser.Permission)
            {
                SportCon.Visibility = Visibility.Collapsed;
                SpecialCon.Visibility = Visibility.Collapsed;
                LeagueCon.Visibility = Visibility.Collapsed;
                UserButton.Visibility = Visibility.Collapsed;
            }
            //
            //Update Details
            Teamlist = await apiService.GetTeams();
            sportlist = await apiService.GetSports();
            leaguelist = await apiService.GetLeagues();
            SpecialTeamslist = await apiService.GetSpecialTeamss();
            current = sportlist.Find(x => x.SportName == sportlist[0].SportName);
            int currentid = sportlist[0].Id;
            //
            //Update Design
            ImageBrush imageBrushunder1 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\flame.png", UriKind.Relative))
            };
            PicUnder2.Background = imageBrushunder1;
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\Logo.png", UriKind.Relative))
            };
            ImageBrush imageBrushClose = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\Close.png", UriKind.Relative))
            };
            logo1 = new TextBlock() { Visibility = Visibility.Visible,Width = 50, Height = 50, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            logo1.Background = imageBrush;
            MAINGRID.Children.Add(logo1);
            logo2 = new Button() { Visibility = Visibility.Visible, Width = 50, BorderBrush = new SolidColorBrush(Colors.Transparent), Height = 50, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top };
            logo2.Background = imageBrushClose;
            logo2.Click += btnExit_Click;
            Grid.SetColumn(logo2, 2);
            MAINGRID.Children.Add(logo2);
            //
            // Update Sport Boxes
            foreach (var sport in sportlist)
            {
                MenuGrid.ColumnDefinitions.Add(new ColumnDefinition());
                int newcolumn = MenuGrid.ColumnDefinitions.Count - 3;
                string sportname = sport.SportName;
                Button b = new Button() { BorderBrush = new SolidColorBrush(Colors.Transparent),  Tag = sportname, Content = sportname, FontSize = 15, Background = new SolidColorBrush(Colors.Transparent),  Foreground = new SolidColorBrush(Colors.White), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center};
                b.Click += B_Click;
                Grid.SetColumn(b, newcolumn);
                MenuGrid.Children.Add(b);
            }
            DescriptionBox.Text = sportlist[0].SportDescription;
            //
            //Update Leagues Boxes
            int index = 1;
            for (int i = 1; i <= leaguelist.Count; i++)
            {
                if (leaguelist[i-1].SportID.Id == currentid)
                {
                    string buttonName = "lb" + index;
                    index++;
                    Button button = (Button)FindName(buttonName);
                    ImageBrush imageBrush111 = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\Back.jpg", UriKind.Relative))
                    };
                    button.Background = imageBrush111;
                    if (button != null)
                    {
                        button.Visibility = Visibility.Visible;
                        button.Content = leaguelist[i - 1].LeagueName;
                        button.Click += LeagueButtonClick;
                    }
                }
                
            }
            //
            // Update Special Teams Boxes
            LeagueList Specialleagues = new LeagueList();
            foreach (League item in leaguelist)
            {
                if (item.SportID.Id.ToString() == sportlist[0].Id.ToString())
                {
                    Specialleagues.Add(item);
                }
            }
            int index1 = 0;
            foreach (SpecialTeams item in SpecialTeamslist)
            {
                if (Specialleagues.Find(x => x.Id == item.LeagueID.Id) != null)
                {
                    index1++;
                    string sname = "sb" + index1;
                    Button button = (Button)FindName(sname);
                    button.Click += ButtonClickTeam;
                    button.Content = "🏆" + item.TeamName + "🏆" ;
                    button.Visibility = Visibility.Visible;
                }
            }
            //
        }
        // Refreash Lists
        public async Task RefreshSportList()
        {
            sportlist = await apiService.GetSports();

        }
        public async Task  RefreshLeagueList()
        {
            leaguelist = await apiService.GetLeagues();

        }
        public async Task  RefreshSTeamsList()
        {
            Teamlist = await apiService.GetTeams();
        }
        public async Task RefreshSpecialTeamsList()
        {
            SpecialTeamslist = await apiService.GetSpecialTeamss();
            
        }

        //
        //Click On The Close Button
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        //
        // Click On Button From The Menu 
        private async void B_Click(object sender, RoutedEventArgs e) 
        {
            await RefreshLeagueList();
            await RefreshSportList();
            await RefreshSTeamsList();
            await RefreshSpecialTeamsList();
            string sportbutton = (sender as Button).Content.ToString();
            current = sportlist.Find(x => x.SportName == sportbutton);
            DescriptionBox.Text = current.SportDescription;

            foreach (Button item in leaguewrap.Children)
            {
                item.Visibility = Visibility.Hidden;

            }
            int index = 1;
            for (int i = 1; i <= leaguelist.Count; i++)
            {
                if (leaguelist[i - 1].SportID.Id == current.Id)
                {
                    string buttonName = "lb" + index;
                    index++;
                    Button button = (Button)FindName(buttonName);
                    ImageBrush imageBrush111 = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\Back.jpg", UriKind.Relative))
                    };
                    button.Background = imageBrush111;
                    if (button != null)
                    {
                        button.Visibility = Visibility.Visible;
                        button.Content = leaguelist[i - 1].LeagueName;
                        button.Click += LeagueButtonClick;
                    }
                }

            }
            Sport s = sportlist.Find(x => x.SportName == sportbutton);
            int index1 = 0;
            foreach (Button item in StackSpecialTeams.Children)
            {
                item.Visibility = Visibility.Collapsed;
            }
            foreach (SpecialTeams item in SpecialTeamslist)
            {
                if (current.Id.ToString() == item.LeagueID.SportID.Id.ToString())
                {
                    index1++;
                    string sname = "sb" + index1;
                    Button button = (Button)FindName(sname);
                    button.Click += ButtonClickTeam;
                    button.Content = "🏆" + item.TeamName + "🏆";
                    button.Visibility = Visibility.Visible;
                }
            }




        }
        //
        //Del Insert Update Sport
        private async  void ButtonClickSportMenu(object sender, RoutedEventArgs e)
        {


            string head = (sender as MenuItem).Header.ToString();

            if (head == "Add")
            {
                UpdateSportStack.Visibility = Visibility.Collapsed;
                UpdateSportStackSec.Visibility = Visibility.Collapsed;
                DelSportStack.Visibility = Visibility.Collapsed;
                AddSportStack.Visibility = Visibility.Visible;
                DoneAddButton.Click += async delegate
                {
                    await RefreshSportList();
                    string sportname = SportTextBoxAdd.Text;
                    string sportdesc = SportTextBoxDescriptionAdd.Text;
                    if (sportname == "" || sportname == "Enter The Sport Name And Delete This Content" || sportdesc == "" || sportdesc == "Enter The Sport Description And Delete This Content")
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        AddSportStack.Visibility = Visibility.Collapsed;
                        MenuGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        int newcolumn = MenuGrid.ColumnDefinitions.Count - 3;
                        Button b = new Button()
                        {
                            BorderBrush = new SolidColorBrush(Colors.Transparent),
                            Tag = sportname,
                            Content = sportname,
                            FontSize = 15,
                            Background = new SolidColorBrush(Colors.Transparent),
                            Foreground = new SolidColorBrush(Colors.White),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        b.Click += B_Click;
                        Grid.SetColumn(b, newcolumn);
                        MenuGrid.Children.Add(b);
                        Sport sport = new Sport() { SportName = sportname, SportDescription = sportdesc };
                        await apiService.InsertSport(sport);


                    }


                };
            }
            else if (head == "Update")
            {

                UpdateSportStack.Visibility = Visibility.Visible;
                UpdateSportStackSec.Visibility = Visibility.Collapsed;
                DelSportStack.Visibility = Visibility.Collapsed;
                AddSportStack.Visibility = Visibility.Collapsed;
                DoneUpdateButton.Click += async delegate
                {
                    await RefreshSportList();
                    string sportname = TextBoxOldSport.Text;
                    if (sportname == "" || sportname == "Enter The Sport Name You Want To Update And Delete This Content" || sportlist.Find(x => x.SportName == sportname) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        Sport current = sportlist.Find(x => x.SportName == sportname);
                        string descold = DescriptionBox.Text;
                        string name = current.SportName;
                        string desc = current.SportDescription;

                        t1.Text = sportname;
                        t2.Text = desc;

                        UpdateSportStack.Visibility = Visibility.Collapsed;
                        UpdateSportStackSec.Visibility = Visibility.Visible;


                        DoneUpdateButtonContinue.Click += async delegate
                        {
                            await RefreshLeagueList();
                            await RefreshSportList();
                            await RefreshSTeamsList();
                            string newsport = t1.Text;
                            string newdesc = t2.Text;
                            if (newsport == "" || newdesc == "")
                            {
                                MessageBox.Show("Invalid Content");

                            }
                            else
                            {
                                current.SportDescription = newdesc;
                                await apiService.UpdateSport(current);
                                UpdateSportStackSec.Visibility = Visibility.Collapsed;
                                if (desc == descold)
                                {
                                    DescriptionBox.Text = newdesc;
                                }

                            }
                        };


                    };
                };
            }
            else if (head == "Delete")
            {
                UpdateSportStack.Visibility = Visibility.Collapsed;
                UpdateSportStackSec.Visibility = Visibility.Collapsed;
                DelSportStack.Visibility = Visibility.Visible;
                AddSportStack.Visibility = Visibility.Collapsed;
                DoneDeleteButton.Click += async delegate
                {
                    await RefreshSportList();
                    string sportname = TextBoxOldSportSec.Text;
                    if (sportname == "" || sportname == "Enter The Sport Name You Want To Delete And Delete This Content" || sportlist.Find(x => x.SportName == sportname) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelSportStack.Visibility = Visibility.Collapsed;
                        Sport s = sportlist.Find(x => x.SportName == sportname);
                        bool check = false;
                        foreach (var item in MenuGrid.Children)
                        {
                            if (check)
                            {
                                Grid.SetColumn(item as UIElement, (Grid.GetColumn(item as UIElement) - 1));
                            }
                            if (item is Button && (item as Button).Content.ToString() == sportname)
                            {
                                (item as Button).Visibility = Visibility.Collapsed;
                                check = true;
                            }
                           
                        }
                        MenuGrid.ColumnDefinitions.RemoveAt(MenuGrid.ColumnDefinitions.Count - 1);
                        await apiService.DeleteSport(s);
                    }
                };
            }
            await RefreshSportList();
        }  
        //
        //Del Insert Update Special Team
        public async void ButtonClickSpecialTeamMenu(object sender, RoutedEventArgs e)
        {
            string head = (sender as MenuItem).Header.ToString();
            if (head == "AddSpecialTeam")
            {
                DelSpecialTeamStack.Visibility = Visibility.Collapsed;
                UpdateSpecialTeamStack.Visibility = Visibility.Collapsed;
                UpdateSpecialTeamStackInfo.Visibility = Visibility.Collapsed;
                AddSpecialTeamStack.Visibility = Visibility.Visible;
                DoneAddButtonSpecial.Click += async delegate
                {
                    await RefreshLeagueList();
                    await RefreshSportList();
                    await RefreshSpecialTeamsList();
                    int teamid;
                    DateTime date;
                    int trophies;
                    int Wins;
                    int Golden;
                    int Players;
                    try
                    {
                        teamid = int.Parse(SpecialTeamIdBoxAdd.Text);
                        date = DateTime.Parse(SpecialTeamDateBoxAdd.Text);
                        trophies = int.Parse(SpecialTeamTrophiesBoxAdd.Text);
                        Wins = int.Parse(SpecialTeamWinsBoxAdd.Text);
                        Golden = int.Parse(SpecialTeamGoldenBoxAdd.Text);
                        Players = int.Parse(SpecialTeamYearPlayersBoxAdd.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid Content");
                        return;
                    }
                    if (Teamlist.Find(x => x.Id == teamid) == null && SpecialTeamslist.Find(x => x.Id == teamid) != null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        AddSpecialTeamStack.Visibility = Visibility.Collapsed;
                        Team t11 = Teamlist.Find(x => x.Id == teamid);
                        SpecialTeams t = new SpecialTeams() { TeamName = t11.TeamName, TeamColor = t11.TeamColor, LeagueID = t11.LeagueID, Id = teamid, FoundedDate = date, TotalTrophies = trophies, TotalWins = Wins, GoldenBalls = Golden, TotalYearPlayers = Players };
                        if (current.Id.ToString() == t11.LeagueID.SportID.Id.ToString())
                        {
                            Button b = new Button() { BorderBrush = new SolidColorBrush(Colors.Transparent), Content = "🏆" + t11.TeamName + "🏆",Foreground = new SolidColorBrush(Colors.White), FontSize = 20, Background = new SolidColorBrush(Colors.Transparent) };
                            b.Click += ButtonClickTeam;
                            StackSpecialTeams.Children.Add(b);
                        }
                        await apiService.InsertSpecialTeams(t);
                    }
                };
            }
            else if (head == "UpdateSpecialTeam")
            {
                DelSpecialTeamStack.Visibility = Visibility.Collapsed;
                AddSpecialTeamStack.Visibility = Visibility.Collapsed;
                UpdateSpecialTeamStackInfo.Visibility = Visibility.Collapsed;
                UpdateSpecialTeamStack.Visibility = Visibility.Visible;
                Team t;
                DoneUpdateButtonSpecial.Click += async delegate
                {
                    await RefreshLeagueList();
                    await RefreshSportList();
                    await RefreshSpecialTeamsList();
                    int teamid = int.Parse(SpecialTeamIdBoxUpdate.Text);
                    if (SpecialTeamslist.Find(x => x.Id == teamid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        UpdateSpecialTeamStackInfo.Visibility = Visibility.Visible;
                        UpdateSpecialTeamStack.Visibility = Visibility.Collapsed;
                        t = Teamlist.Find(x => x.Id == teamid);
                        SpecialTeams ts = SpecialTeamslist.Find(x => x.Id == teamid);
                        SpecialTeamIdBoxUpdateSec.Text = teamid.ToString();
                        SpecialTeamDateBoxUpdateSec.Text = ts.FoundedDate.ToString();
                        SpecialTeamTrophiesBoxUpdateSec.Text = ts.TotalTrophies.ToString();
                        SpecialTeamWinsBoxUpdateSec.Text = ts.TotalWins.ToString();
                        SpecialTeamGoldenBoxUpdateSec.Text = ts.GoldenBalls.ToString();
                        SpecialTeamYearPlayersBoxUpdateSec.Text = ts.TotalYearPlayers.ToString();
                        DoneUpdateButtonSpecialSec.Click += async delegate
                        {
                            await RefreshLeagueList();
                            await RefreshSportList();
                            await RefreshSpecialTeamsList();
                            DateTime newdate;
                            int newtrophies;
                            int newWins;
                            int newGolden;
                            int newPlayers;
                            try
                            {
                                newdate = DateTime.Parse(SpecialTeamDateBoxUpdateSec.Text);
                                newtrophies = int.Parse(SpecialTeamTrophiesBoxUpdateSec.Text);
                                newWins = int.Parse(SpecialTeamWinsBoxUpdateSec.Text);
                                newGolden = int.Parse(SpecialTeamGoldenBoxUpdateSec.Text);
                                newPlayers = int.Parse(SpecialTeamYearPlayersBoxUpdateSec.Text);
                            }
                            catch
                            {
                                MessageBox.Show("Invalid Content");
                                return;
                            }
                            UpdateSpecialTeamStackInfo.Visibility = Visibility.Collapsed;
                            SpecialTeams ts = SpecialTeamslist.Find(x => x.Id == t.Id);
                            ts.FoundedDate = newdate;
                            ts.TotalTrophies = newtrophies;
                            ts.TotalWins = newWins;
                            ts.GoldenBalls = newGolden;
                            ts.TotalYearPlayers = newPlayers;
                            await apiService.UpdateSpecialTeams(ts);
                        };
                    }
                };
            }
            else if (head == "DeleteSpecialTeam")
            {
                AddSpecialTeamStack.Visibility = Visibility.Collapsed;
                UpdateSpecialTeamStack.Visibility = Visibility.Collapsed;
                UpdateSpecialTeamStackInfo.Visibility = Visibility.Collapsed;
                DelSpecialTeamStack.Visibility = Visibility.Visible;
                DoneDelButtonSpecial.Click += async delegate
                {
                    await RefreshLeagueList();
                    await RefreshSportList();
                    await RefreshSpecialTeamsList();
                    int teamid = int.Parse(SpecialTeamIdBoxDel.Text);
                    if (SpecialTeamslist.Find(x => x.Id == teamid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelSpecialTeamStack.Visibility = Visibility.Collapsed;
                        SpecialTeams st = SpecialTeamslist.Find(x => x.Id == teamid);
                        if (st.LeagueID.SportID.Id.ToString() == current.Id.ToString())
                        {
                            foreach (var item in StackSpecialTeams.Children)
                            {
                                if (item is Button)
                                {
                                    Button b = (item as Button);
                                    if (b.Content.ToString() == "🏆" + st.TeamName + "🏆")
                                    {
                                        b.Visibility = Visibility.Hidden;
                                    }
                                }
                            }
                        }
                        await apiService.DeleteSpecialTeams(st);
                    }
                };
            }
            await RefreshLeagueList();
            await RefreshSportList();
            await RefreshSpecialTeamsList();
        }
        //
        //Del Insert Update League
        private async void ButtonClickLeagueMenu(object sender, RoutedEventArgs e)
        {


            string head = (sender as MenuItem).Header.ToString();

            if (head == "AddLeague")
            {
                AddLeagueStack.Visibility = Visibility.Visible;
                UpdateLeagueStack.Visibility = Visibility.Collapsed;
                UpdateLeagueStackSec.Visibility = Visibility.Collapsed;
                DelLeagueStack.Visibility = Visibility.Collapsed;
                LeagueDoneAddButton.Click += async delegate
                {
                    await RefreshLeagueList();
                    await RefreshSportList();
                    await RefreshSTeamsList();
                    string leaguename = LeagueTextBoxAdd.Text;
                    int sportid = int.Parse(LeaugeTextBoxSportIdAdd.Text);
                    string stringsportid = LeaugeTextBoxSportIdAdd.Text;

                    if (leaguename == "" || leaguename == "Enter The League Name And Delete This Content" || stringsportid == "Enter The Sport Id And Delete This Content" || sportlist.Find(x => x.Id == sportid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        AddLeagueStack.Visibility = Visibility.Collapsed;

                        Sport s = sportlist.Find(x => x.Id == sportid);
                        League league = new League() { LeagueName = leaguename, SportID = s };
                        foreach (var item in leaguewrap.Children)
                        {
                            if (item is Button)
                            {
                                if ((item as Button).Visibility == Visibility.Hidden)
                                {
                                    (item as Button).Content = league.LeagueName;
                                    ImageBrush imageBrush111 = new ImageBrush
                                    {
                                        ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\Back.jpg", UriKind.Relative))
                                    };
                                    (item as Button).Background = imageBrush111;
                                    (item as Button).Click += LeagueButtonClick;
                                    (item as Button).Visibility = Visibility.Visible;
                                    break;

                                }
                            }
                        }
                        await apiService.InsertLeague(league);


                    }


                };
            }
            else if (head == "UpdateLeague")
            {
                AddLeagueStack.Visibility = Visibility.Collapsed;
                UpdateLeagueStack.Visibility = Visibility.Visible;
                UpdateLeagueStackSec.Visibility = Visibility.Collapsed;
                DelLeagueStack.Visibility = Visibility.Collapsed;
                LeagueDoneUpdateButton.Click += async delegate
                {
                    await RefreshLeagueList();
                    await RefreshSportList();
                    await RefreshSTeamsList();
                    string leaguename = TextBoxOldLeague.Text;
                    string oldleague = leaguename;

                    if (leaguename == "" || leaguename == "Enter The leaguename Name You Want To Update And Delete This Content" || leaguelist.Find(x => x.LeagueName == leaguename) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        League current = leaguelist.Find(x => x.LeagueName == leaguename);
                        string name = current.LeagueName;
                        int currentid = current.SportID.Id;

                        t1l.Text = name;
                        t2l.Text = (currentid.ToString());

                        UpdateLeagueStack.Visibility = Visibility.Collapsed;
                        UpdateLeagueStackSec.Visibility = Visibility.Visible;


                        DoneUpdateButtonContinueLeague.Click += async delegate
                        {
                            await RefreshLeagueList();
                            await RefreshSportList();
                            await RefreshSTeamsList();
                            string newsleague = t1l.Text;
                            string newid = t2l.Text;
                            int newidint = int.Parse(newid);
                            if (newsleague == "" || newid == "" || sportlist.Find(x => x.Id == newidint) == null)
                            {
                                MessageBox.Show("Invalid Content");

                            }
                            else
                            {
                                current.LeagueName = newsleague;
                                current.SportID = sportlist.Find(x => x.Id == newidint);
                                UpdateLeagueStackSec.Visibility = Visibility.Collapsed;
                                foreach (Button item in leaguewrap.Children)
                                {

                                    if (item.Content.ToString() == oldleague)
                                    {
                                        item.Content = newsleague;
                                    }
                                }
                                if (currentid.ToString() != newid.ToString())
                                {
                                    foreach (Button item in leaguewrap.Children)
                                    {

                                        if (item.Content.ToString() == oldleague)
                                        {
                                            item.Visibility = Visibility.Collapsed;
                                        }
                                    }
                                }
                                await apiService.UpdateLeague(current);


                            }
                        };


                    };
                };
            }
            else if (head == "DeleteLeague")
            {
                AddLeagueStack.Visibility = Visibility.Collapsed;
                UpdateLeagueStack.Visibility = Visibility.Collapsed;
                UpdateLeagueStackSec.Visibility = Visibility.Collapsed;
                DelLeagueStack.Visibility = Visibility.Visible;
                LeagueDoneDeleteButton.Click += async delegate
                {
                    await RefreshLeagueList();
                    await RefreshSportList();
                    await RefreshSTeamsList();
                    string leguename = TextBoxOldLeagueSec.Text;
                    if (leguename == "" || leguename == "Enter The League Name You Want To Delete And Delete This Content" || leaguelist.Find(x => x.LeagueName == leguename) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelLeagueStack.Visibility = Visibility.Collapsed;
                        League l = leaguelist.Find(x => x.LeagueName == leguename);
                        
                        foreach (Button item in leaguewrap.Children)
                        {
                            if (item.Content.ToString() == leguename)
                            {
                                item.Visibility = Visibility.Collapsed;
                            }
                        }

                        await apiService.DeleteLeague(l);



                    }
                };
            }
            await RefreshLeagueList();
            await RefreshSportList();
            await RefreshSTeamsList();
        }
        // Click On A Team
        public async void ButtonClickTeam(object sender, RoutedEventArgs e)
        {
            string teamname = (sender as Button).Content.ToString();
            string emojiToRemove = "🏆";
            string newteamname = teamname.TrimStart(emojiToRemove.ToCharArray()).TrimEnd(emojiToRemove.ToCharArray());
            await RefreshLeagueList();
            await RefreshSportList();
            await RefreshSTeamsList();
            Team currentt = SpecialTeamslist.Find(x => x.TeamName == newteamname);
            logo1.Visibility = Visibility.Hidden;
            logo2.Visibility = Visibility.Hidden;
            TeamPage p = new TeamPage(currentt, true, currentuser);
            p.ClosePage += CloseWinTeam;
            TeamFrame.Visibility = Visibility.Visible;
            TeamFrame.Navigate(p);
        }
        //
        //Click On the Users Permitions
        public async void ButtonClickUsers(object sender, RoutedEventArgs e)
        {
            logo1.Visibility = Visibility.Hidden;
            logo2.Visibility = Visibility.Hidden;
            UsersPage p = new UsersPage();
            p.ClosePage += CloseWinUser;
            UserFrame.Visibility = Visibility.Visible;
            UserFrame.Navigate(p);

        }
        //
        //Click On a League Button
        public async void LeagueButtonClick(object sender, RoutedEventArgs e)
        {
            await RefreshLeagueList();
            await RefreshSportList();
            await RefreshSTeamsList();
            string leaguename = (sender as Button).Content.ToString();
            League currentl = leaguelist.Find(x => x.LeagueName == leaguename);
            logo1.Visibility = Visibility.Hidden;
            logo2.Visibility = Visibility.Hidden;
            LeaguePage p = new LeaguePage(currentl, currentuser);
            p.ClosePage += CloseWin;
            LeagueFrame.Visibility = Visibility.Visible;
            LeagueFrame.Navigate(p);

        }
        //
        //Close Window League Frame
        private async void CloseWin(object sender, EventArgs e)
        {
            LeagueFrame.Visibility = Visibility.Collapsed;
            await RefreshSTeamsList();
            logo1.Visibility = Visibility.Visible;
            logo2.Visibility = Visibility.Visible;

        }
        //
        // Close Window Team Frame
        private async void CloseWinTeam(object sender, EventArgs e)
        {
            TeamFrame.Visibility = Visibility.Collapsed;
            await RefreshSTeamsList();
            logo1.Visibility = Visibility.Visible;
            logo2.Visibility = Visibility.Visible;
        }
        //
        private async void CloseWinUser(object sender, EventArgs e)
        {
            UserFrame.Visibility = Visibility.Collapsed;
            logo1.Visibility = Visibility.Visible;
            logo2.Visibility = Visibility.Visible;
        }


    }
}
