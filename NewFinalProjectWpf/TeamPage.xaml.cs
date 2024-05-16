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
using static System.Net.Mime.MediaTypeNames;

namespace NewFinalProjectWpf
{
    public partial class TeamPage : Page
    {
        public event EventHandler ClosePage;
        Team CurrentTeam;
        Button BackB;
        bool Isspecial;
        OffencesList offenceslist;
        ApiService apiService;
        TeamList Teamlist;
        LeagueList leaguelist;
        League CurrentLeague;
        PlayerList Playerslist;
        User currentuser;
        public TeamPage(Team currentTeam, bool IsSpecial, User us)
        {
            InitializeComponent();
            CurrentTeam = currentTeam;
            Isspecial = IsSpecial;
            apiService = new ApiService();
            currentuser = us;
            Start();
        }
        //Start
        public async void Start()
        {
            // If Admin Show the add/insert/update
            if (!currentuser.Permission)
            {
                OffenceCon.Visibility = Visibility.Collapsed;
                PlayerCon.Visibility = Visibility.Collapsed;
                TeamCon.Visibility = Visibility.Collapsed;
            }
            //
            //Update Main Details
            offenceslist = await apiService.GetOffencess();
            leaguelist = await apiService.GetLeagues();
            Playerslist = await apiService.GetPlayers();
            Teamlist = await apiService.GetTeams();
            CurrentLeague = CurrentTeam.LeagueID;
            //
            //Design
            ImageBrush imageBrushunder1 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\flame.png", UriKind.Relative))
            };
            PicUnder2.Background = imageBrushunder1;
            if (Isspecial)
            {
                Topic.Text = "🌟" + CurrentTeam.TeamName + "🌟";
            }
            else
            {
                Topic.Text = CurrentTeam.TeamName;
            }
            ImageBrush imageBrush111 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\coolA.jpg", UriKind.Relative))
            };
            PageThis.Background = imageBrush111;
            BackB = new Button() {Height = 50,Width = 50, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Top};
            ImageBrush imageBrush11 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\BackAr.png", UriKind.Relative))
            };
            BackB.Background = imageBrush11;
            BackB.Click += ButtonClickBack;
            MainGrid.Children.Add(BackB);
            //
            //TeamStats
            CurrentLeagueTB.Text = CurrentTeam.LeagueID.LeagueName;
            CurrentTeamColorTB.Text = CurrentTeam.TeamColor.ToString();
            //
            //PlayersList
            foreach (Player item in Playerslist)
            {
                if (item.TeamID.Id.ToString() == CurrentTeam.Id.ToString())
                {
                    TextBlock t = new TextBlock() {Text = item.Id.ToString()+ " - " + item.PlayerName.ToString(),Foreground = new SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center, FontSize = 20};
                    PlayersStack.Children.Add(t);
                }
            }
            //
            //Offences
            foreach (Offences item in offenceslist)
            {
                if (item.Tid.Id.ToString() == CurrentTeam.Id.ToString())
                {
                    TextBlock t = new TextBlock() { Text =  "ID - " +  item.Id.ToString() + " - " + item.OffenceName.ToString() + " With lvl: " + item.OffenceLevel.ToString(), Foreground = new SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center, FontSize = 17, Margin = new Thickness(10) };
                    OffencesStack.Children.Add(t);
                }
            }
            //
        }
        //
        //Refreashes Lists
        public async Task RefreshOffences()
        {
            offenceslist =  await apiService.GetOffencess();
        }
        public async Task RefreshTeamsList()
        {
            Teamlist = await apiService.GetTeams();
        }
        public async Task RefreshPlayersList()
        {
            Playerslist = await apiService.GetPlayers();
        }
        //
        //When Click On The Back Page Arrow
        public void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            BackB.Visibility = Visibility.Hidden;
            ClosePage?.Invoke(this, EventArgs.Empty);
        }
        //
        //Insert Update Delete Team
        public async void ButtonClickTeamMenu(object sender, RoutedEventArgs e)
        {
            string head = (sender as MenuItem).Header.ToString();
            if (head == "AddTeam")
            {
                AddSTeamStack.Visibility = Visibility.Visible;
                DelTeamStack.Visibility = Visibility.Collapsed;
                UpdateSTeamStack.Visibility = Visibility.Collapsed;
                UpdateSTeamStackInfo.Visibility = Visibility.Collapsed;
                TeamAddButton.Click += async delegate
                {
                    await RefreshTeamsList();
                    string teamname = TeamNameBoxADD.Text;
                    int teamleagueid = int.Parse(TeamLeagueBoxADD.Text);
                    string teamcolor = TeamColorBoxADD.Text;
                    if (teamname == "" || teamcolor == "" || teamname == "Enter The Team Name And Delete This Content" || teamcolor == "Enter The Team Color And Delete This Content" || leaguelist.Find(x => x.Id == teamleagueid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        AddSTeamStack.Visibility = Visibility.Collapsed;
                        Team t = new Team() { TeamName = teamname, LeagueID = leaguelist.Find(x => x.Id == teamleagueid), TeamColor = teamcolor };
                        await apiService.InsertTeam(t);
                    }
                };
            }
            else if (head == "UpdateTeam")
            {
                AddSTeamStack.Visibility = Visibility.Collapsed;
                DelTeamStack.Visibility = Visibility.Collapsed;
                UpdateSTeamStackInfo.Visibility = Visibility.Collapsed;
                UpdateSTeamStack.Visibility = Visibility.Visible;
                TeamUpdateButton.Click += async delegate
                {
                    await RefreshTeamsList();
                    string teamname = TeamNameBoxUpdate.Text;
                    string oldname = teamname;
                    if (teamname == "" || teamname == "Enter The Team Name And Delete This Content" || Teamlist.Find(x => x.TeamName == teamname) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        Team t = Teamlist.Find(x => x.TeamName == teamname);
                        int oldid = t.Id;
                        TeamNameBoxUpdateSec.Text = teamname;
                        TeamLeagueBoxUpdateSec.Text = t.LeagueID.Id.ToString();
                        TeamColorBoxUpdateSec.Text = t.TeamColor;
                        UpdateSTeamStackInfo.Visibility = Visibility.Visible;
                        UpdateSTeamStack.Visibility = Visibility.Collapsed;
                        TeamUpdateButtonSec.Click += async delegate
                        {
                            await RefreshTeamsList();
                            string teamname = TeamNameBoxUpdateSec.Text;
                            int teamleagueid = int.Parse(TeamLeagueBoxUpdateSec.Text);
                            string teamcolor = TeamColorBoxUpdateSec.Text;
                            if (teamname == "" || teamcolor == "" || teamname == "Enter The Team Name And Delete This Content" || teamcolor == "Enter The Team Color And Delete This Content" || leaguelist.Find(x => x.Id == teamleagueid) == null)
                            {
                                MessageBox.Show("Invalid Content");
                            }
                            else
                            {
                                UpdateSTeamStackInfo.Visibility = Visibility.Collapsed;
                                t.TeamName = teamname;
                                t.LeagueID = leaguelist.Find(x => x.Id == teamleagueid);
                                t.TeamColor = teamcolor;
                                Topic.Text = teamname;
                                CurrentLeagueTB.Text = t.LeagueID.LeagueName.ToString();
                                CurrentTeamColorTB.Text = t.TeamColor.ToString();
                                await apiService.UpdateTeam(t);
                            }
                        };
                    }
                };
            }
            else if (head == "DeleteTeam")
            {
                AddSTeamStack.Visibility = Visibility.Collapsed;
                UpdateSTeamStack.Visibility = Visibility.Collapsed;
                UpdateSTeamStackInfo.Visibility = Visibility.Collapsed;
                DelTeamStack.Visibility = Visibility.Visible;
                TeamDeleteButton.Click += async delegate
                {
                    await RefreshTeamsList();
                    string teamname = TeamNameBoxDelete.Text;
                    if (Teamlist.Find(x => x.TeamName == teamname) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelTeamStack.Visibility = Visibility.Collapsed;
                        Team t = Teamlist.Find(x => x.TeamName == teamname);
                        if (t.Id == CurrentTeam.Id)
                        {
                            BackB.Visibility = Visibility.Hidden;
                            ClosePage?.Invoke(PageThis, EventArgs.Empty);
                        }
                        await apiService.DeleteTeam(t);
                    }
                };
            }
            await RefreshTeamsList();
        }
        //
        //Insert Update Delete player
        public async void ButtonClickPlayerMenu(object sender, RoutedEventArgs e)
        {
            string head = (sender as MenuItem).Header.ToString();
            if (head == "AddPlayer")
            {
                DelPlayerStack.Visibility = Visibility.Collapsed;
                UpdatePlayersStack.Visibility = Visibility.Collapsed;
                UpdatePlayersStackInfo.Visibility = Visibility.Collapsed;
                AddPlayersStack.Visibility = Visibility.Visible;
                PlayerAddButton.Click += async delegate
                {
                    await RefreshPlayersList();
                    await RefreshTeamsList();
                    string playername = PlayerNameBoxADD.Text;
                    int teamid = int.Parse(PlayerTeamIdBoxADD.Text);
                    if (playername == ""  || playername == "Enter The Player Name And Delete This Content"  || Teamlist.Find(x => x.Id.ToString() == teamid.ToString()) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        AddPlayersStack.Visibility = Visibility.Collapsed;
                        Player p = new Player() {PlayerName = playername, TeamID = Teamlist.Find(x => x.Id == teamid) };
                        Player pl = (Playerslist.Last());
                        int num = pl.Id + 1;
                        if (CurrentTeam.Id.ToString() == p.TeamID.Id.ToString())
                        {
                            TextBlock t = new TextBlock() { Text = num + " - " + p.PlayerName.ToString(), Foreground = new SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center, FontSize = 20 };
                            PlayersStack.Children.Add(t);
                        }
                        await apiService.InsertPlayer(p);
                    }
                };
            }
            else if (head == "UpdatePlayer")
            {
                DelPlayerStack.Visibility = Visibility.Collapsed;
                UpdatePlayersStack.Visibility = Visibility.Visible;
                UpdatePlayersStackInfo.Visibility = Visibility.Collapsed;
                AddPlayersStack.Visibility = Visibility.Collapsed;
                PlayersUpdateButton.Click += async delegate
                {
                    await RefreshPlayersList();
                    await RefreshTeamsList();
                    string playername = PlayersNameBoxUpdate.Text;
                    string oldname = playername;
                    if (playername == "" || playername == "Enter The Player Name And Delete This Content" || Playerslist.Find(x => x.PlayerName == playername) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        Player p = Playerslist.Find(x => x.PlayerName == playername);
                        int oldteamid = p.TeamID.Id;
                        PlayersNameBoxUpdateSec.Text = playername;
                        PlayersTeamIdBoxUpdateSec.Text = p.TeamID.Id.ToString();
                        UpdatePlayersStack.Visibility = Visibility.Collapsed;
                        UpdatePlayersStackInfo.Visibility = Visibility.Visible;
                        PlayersUpdateButtonSec.Click += async delegate
                        {
                            await RefreshPlayersList();
                            await RefreshTeamsList();
                            string playername = PlayersNameBoxUpdateSec.Text;
                            int teamid = int.Parse(PlayersTeamIdBoxUpdateSec.Text);
                            if (playername == "" || playername == "Enter The Player Name And Delete This Content" || Teamlist.Find(x => x.Id == teamid) == null)
                            {
                                MessageBox.Show("Invalid Content");
                            }
                            else
                            {
                                UpdatePlayersStackInfo.Visibility = Visibility.Collapsed;
                                p.PlayerName = playername;
                                p.TeamID = Teamlist.Find(x => x.Id == teamid);
                                if (oldteamid != teamid)
                                {
                                    foreach (TextBlock item in PlayersStack.Children)
                                    {
                                        if (item.Text.Contains(oldname))
                                        {
                                            item.Visibility = Visibility.Collapsed;
                                        }
                                    }
                                }
                                if (oldname != playername)
                                {
                                    foreach (TextBlock item in PlayersStack.Children)
                                    {
                                        if (item.Text.Contains(oldname))
                                        {
                                            item.Text =p.Id.ToString() + " - " + playername;
                                        }
                                    }
                                }

                                await apiService.UpdatePlayer(p);
                            }
                        };
                    }
                };
            }
            else if (head == "DeletePlayer")
            {
                DelPlayerStack.Visibility = Visibility.Visible;
                UpdatePlayersStack.Visibility = Visibility.Collapsed;
                UpdatePlayersStackInfo.Visibility = Visibility.Collapsed;
                AddPlayersStack.Visibility = Visibility.Collapsed;
                PlayerDeleteButton.Click += async delegate
                {
                    await RefreshPlayersList();
                    await RefreshTeamsList();
                    string playername = PlayerNameBoxDelete.Text;
                    if (Playerslist.Find(x => x.PlayerName == playername) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelPlayerStack.Visibility = Visibility.Collapsed;
                        Player p = Playerslist.Find(x => x.PlayerName == playername);
                        if (p.TeamID.Id == CurrentTeam.Id)
                        {
                            foreach (TextBlock item in PlayersStack.Children)
                            {
                                if (item.Text.Contains(playername))
                                {
                                    item.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                        await apiService.DeletePlayer(p);
                    }
                };
            }
            await RefreshPlayersList();
            await RefreshTeamsList();
        }
        //
        //Insert Update Delete Offences
        public async void ButtonClickOffencesMenu(object sender, RoutedEventArgs e)
        {
            string head = (sender as MenuItem).Header.ToString();
            if (head == "AddOffence")
            {
                AddOffencesStack.Visibility = Visibility.Visible;
                UpdateOffencesStack.Visibility = Visibility.Collapsed;
                UpdateOffencesStackInfo.Visibility = Visibility.Collapsed;
                DelOffencesStack.Visibility = Visibility.Collapsed;
                OffenceAddButton.Click += async delegate
                {
                    await RefreshOffences();
                    string offencename = OffenceNameBoxADD.Text;
                    int teamid = int.Parse(OffenceTeamIdBoxADD.Text);
                    int lvl = int.Parse(OffencelvlBoxADD.Text);
                    if (offencename == "" || offencename == "Enter The Offence Name And Delete This Content" || Teamlist.Find(x => x.Id.ToString() == teamid.ToString()) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        AddOffencesStack.Visibility = Visibility.Collapsed;
                        Offences o = new Offences() { OffenceName = offencename, Tid = Teamlist.Find(x => x.Id == teamid) , OffenceLevel = lvl};
                        Offences ol = (offenceslist.Last());
                        int num = ol.Id + 1;
                        if (CurrentTeam.Id.ToString() == o.Tid.Id.ToString())
                        {
                            TextBlock t = new TextBlock() { Text = "ID - " + num + " - " + o.OffenceName.ToString() + " With lvl: " + o.OffenceLevel.ToString(), Foreground = new SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center, FontSize = 20 };
                            OffencesStack.Children.Add(t);
                        }
                        await apiService.InsertOffences(o);
                    }
                };
            }
            else if (head == "UpdateOffence")
            {
                AddOffencesStack.Visibility = Visibility.Collapsed;
                UpdateOffencesStack.Visibility = Visibility.Visible;
                UpdateOffencesStackInfo.Visibility = Visibility.Collapsed;
                DelOffencesStack.Visibility = Visibility.Collapsed;
                OffenceUpdateButton.Click += async delegate
                {
                    await RefreshOffences();
                    string offencename = OffenceNameBoxUpdate.Text;
                    string oldname = offencename;

                    if (offencename == "" || offencename == "Enter The Offence Name And Delete This Content" || offenceslist.Find(x => x.OffenceName == offencename) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        Offences o = offenceslist.Find(x => x.OffenceName == offencename);
                        int oldlvl = o.OffenceLevel;
                        int oldteamid = o.Tid.Id;
                        OffenceNameBoxUpdateSec.Text = offencename;
                        OffenceTeamIdBoxUpdateSec.Text = o.Tid.Id.ToString();
                        OffencelvlBoxUpdateSec.Text = o.OffenceLevel.ToString();
                        UpdateOffencesStack.Visibility = Visibility.Collapsed;
                        UpdateOffencesStackInfo.Visibility = Visibility.Visible;
                        OffenceUpdateButtonSec.Click += async delegate
                        {
                            await RefreshOffences();
                            string offencename = OffenceNameBoxUpdateSec.Text;
                            int teamid = int.Parse(OffenceTeamIdBoxUpdateSec.Text);
                            int lvl = int.Parse(OffencelvlBoxUpdateSec.Text);
                            if (offencename == "" || offencename == "Enter The Offence Name And Delete This Content" || Teamlist.Find(x => x.Id == teamid) == null)
                            {
                                MessageBox.Show("Invalid Content");
                            }
                            else
                            {
                                UpdateOffencesStackInfo.Visibility = Visibility.Collapsed;
                                o.OffenceName = offencename;
                                o.Tid = Teamlist.Find(x => x.Id == teamid);
                                o.OffenceLevel = lvl;
                                if (oldteamid != teamid)
                                {
                                    foreach (TextBlock item in OffencesStack.Children)
                                    {
                                        if (item.Text.Contains(oldname))
                                        {
                                            item.Visibility = Visibility.Collapsed;
                                        }
                                    }
                                }
                                if (oldname != offencename || lvl != oldlvl)
                                {
                                    foreach (TextBlock item in OffencesStack.Children)
                                    {
                                        if (item.Text.Contains(oldname))
                                        {
                                            item.Text = "ID - " + o.Id.ToString() + " - " + o.OffenceName.ToString() + " With lvl: " + o.OffenceLevel.ToString();
                                        }
                                    }
                                }

                                await apiService.UpdateOffences(o);
                            }
                        };
                    }
                };
            }
            else if (head == "DeleteOffence")
            {
                AddOffencesStack.Visibility = Visibility.Collapsed;
                UpdateOffencesStack.Visibility = Visibility.Collapsed;
                UpdateOffencesStackInfo.Visibility = Visibility.Collapsed;
                DelOffencesStack.Visibility = Visibility.Visible;
                OffenceDeleteButton.Click += async delegate
                {
                    await RefreshOffences();
                    string offencename = OffenceNameBoxDelete.Text;
                    if (offenceslist.Find(x => x.OffenceName == offencename) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelOffencesStack.Visibility = Visibility.Collapsed;
                        Offences o = offenceslist.Find(x => x.OffenceName == offencename);
                        if (o.Tid.Id == CurrentTeam.Id)
                        {
                            foreach (TextBlock item in OffencesStack.Children)
                            {
                                if (item.Text.Contains(offencename))
                                {
                                    item.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                        await apiService.DeleteOffences(o);
                    }
                };
            }
            await RefreshOffences();
        }
        //
    }
}
