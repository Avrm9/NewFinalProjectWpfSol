using APIService;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class LeaguePage : Page
    {
        public event EventHandler ClosePage;
        ApiService apiService;
        League CurrentLeague;
        SpecialTeamsList SpecialTeamslist;
        TeamList Teamlist;
        LeagueList leaguelist;
        MatchSumList MatchSumlist;
        Button b11;
        User currentuser;
        public LeaguePage(League c, User us)
        {
            InitializeComponent();
            apiService = new ApiService();
            CurrentLeague = c;
            currentuser = us;
            start();
        }
        //Start
        public async void start()
        {
            // If Admin Show the add/insert/update
            if (!currentuser.Permission)
            {
                TeamCon.Visibility = Visibility.Collapsed;
                SpecialCon.Visibility = Visibility.Collapsed;
                MatchCon.Visibility = Visibility.Collapsed;
            }
            //
            //Update Main Details
            Teamlist = await apiService.GetTeams();
            MatchSumlist = await apiService.GetMatchSums();
            SpecialTeamslist = await apiService.GetSpecialTeamss();
            leaguelist = await apiService.GetLeagues();
            //
            //Design things
            ImageBrush imageBrushunder = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\under.png", UriKind.Relative))
            };
            PicUnder.Background = imageBrushunder;
            ImageBrush imageBrush111 = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\cool.jpg", UriKind.Relative))
            };
            PageThis.Background = imageBrush111;
            b11 = new Button() { BorderThickness = new Thickness(0),Width = 50, Height = 50, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top};
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\..\\..\\NewFinalProjectWpfSol\\NewFinalProjectWpf\\Images\\BackArrow.png", UriKind.Relative))
            };
            b11.Background = imageBrush;
            b11.Click += ButtonClickBack;
            MainGrid.Children.Add(b11);
            HeaderBlock.Text = CurrentLeague.LeagueName + " League";
            //
            //Update Teams Details
            foreach (Team team in Teamlist)
            {
                if (team.LeagueID.Id == CurrentLeague.Id)
                {
                    Button b = new Button() { BorderBrush = new SolidColorBrush(Colors.Transparent), Content = team.TeamName, FontSize = 20, Background = new SolidColorBrush(Colors.Transparent) };
                    b.Click += ButtonClickTeam;
                    StackTeams.Children.Add(b);
                    if (SpecialTeamslist.Find(x => x.Id == team.Id) != null)
                    {
                        Button bS = new Button() { BorderBrush = new SolidColorBrush(Colors.Transparent), Content = "🏆" + team.TeamName + "🏆", FontSize = 20, Background = new SolidColorBrush(Colors.Transparent) };
                        bS.Click += ButtonClickTeam;
                        StackSpecialTeams.Children.Add(bS);
                    }
                }
            }
            //
            //Update Matches Details
            foreach (MatchSum item in MatchSumlist)
            {
                if (CurrentLeague.Id == item.LeagueID.Id)
                {
                    TextBlock b = new TextBlock() { Text = " Winner - " + item.WinnerTeam.TeamName.ToString() + "  Within " + item.MatchTime + " H\n" + "        " + item.MatchDate, FontSize = 20, Foreground = new SolidColorBrush(Colors.Black) };
                    StackMatch.Children.Add(b);
                }
            }
            //
        }
        //
        //Refreash Lists
        public async Task RefreshSTeamsList()
        {
            SpecialTeamslist = await apiService.GetSpecialTeamss();
        }
        public async Task RefreshTeamsList()
        {
            Teamlist = await apiService.GetTeams();
        }
        public async Task RefreshMatchList()
        {
            MatchSumlist = await apiService.GetMatchSums();
        }
        //
        //Close Current Page When Click Back
        public async void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            b11.Visibility = Visibility.Hidden;
            ClosePage?.Invoke(this, EventArgs.Empty);
        }
        //
        //Click On A Team
        public async void ButtonClickTeam(object sender, RoutedEventArgs e)
        {
            string teamname = (sender as Button).Content.ToString();
            Team currentt;
            TeamPage p;;
            if (teamname.Contains("🏆"))
            {
                string emojiToRemove = "🏆";
                string newteamname = teamname.TrimStart(emojiToRemove.ToCharArray()).TrimEnd(emojiToRemove.ToCharArray());
                await RefreshSTeamsList();
                currentt = SpecialTeamslist.Find(x => x.TeamName == newteamname);
                p =  new TeamPage(currentt, true, currentuser);
            }
            else
            {
                string newteamname = teamname;
                await RefreshTeamsList();
               
                currentt = Teamlist.Find(x => x.TeamName == newteamname);
                p = new TeamPage(currentt, false, currentuser);
            }
            // LOGOS AND IMAGES VISIBILITY
            b11.Visibility = Visibility.Hidden;
            p.ClosePage += CloseWinToLeague;
            TeamFrame.Visibility = Visibility.Visible;
            TeamFrame.Navigate(p);
        }
        //
        //Close Windows
        private async void CloseWin(object sender, EventArgs e)
        {
            TeamFrame.Visibility = Visibility.Collapsed;
            b11.Visibility = Visibility.Visible;
        }
        private async void CloseWinToLeague(object sender, EventArgs e)
        {
            TeamFrame.Visibility = Visibility.Collapsed;
            b11.Visibility = Visibility.Visible;
        }
        //
        //Insert Update Delete Team
        public async void  ButtonClickTeamMenu(object sender, RoutedEventArgs e)
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
                        if (CurrentLeague.Id == teamleagueid)
                        {
                            Button b = new Button() { BorderBrush = new SolidColorBrush(Colors.Transparent), Content = t.TeamName, FontSize = 20, Background = new SolidColorBrush(Colors.Transparent) };
                            b.Click += ButtonClickTeam;
                            StackTeams.Children.Add(b);
                        }
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
                                if (CurrentLeague.Id == teamleagueid)
                                {
                                    foreach (var item in StackTeams.Children)
                                    {
                                        if (item is Button)
                                        {
                                            Button b = (item as Button);
                                            if (b.Content.ToString() == oldname)
                                            {
                                                b.Content = teamname;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in StackTeams.Children)
                                    {
                                        if (item is Button)
                                        {
                                            Button b = (item as Button);
                                            if (b.Content.ToString() == oldname)
                                            {
                                                b.Visibility = Visibility.Hidden;
                                            }
                                        }
                                    }
                                }
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
                        if (t.LeagueID.Id.ToString() == CurrentLeague.Id.ToString())
                        {
                            foreach (var item in StackTeams.Children)
                            {
                                if (item is Button)
                                {
                                    Button b = (item as Button);
                                    if (b.Content.ToString() == teamname)
                                    {
                                        b.Visibility = Visibility.Hidden;
                                    }
                                }
                            }
                        }
                        await apiService.DeleteTeam(t);
                    }
                };
            }
            await RefreshTeamsList();
        }
        //
        //Insert Update Delete Special Team
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
                    await RefreshSTeamsList();
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
                        Team t1 = Teamlist.Find(x => x.Id == teamid);
                        SpecialTeams t = new SpecialTeams() { TeamName = t1.TeamName,TeamColor = t1.TeamColor,LeagueID = t1.LeagueID,Id = teamid, FoundedDate = date, TotalTrophies = trophies, TotalWins = Wins, GoldenBalls = Golden, TotalYearPlayers = Players };
                        if (CurrentLeague.Id.ToString() == t1.LeagueID.Id.ToString())
                        {
                            Button b = new Button() { BorderBrush = new SolidColorBrush(Colors.Transparent), Content = "🏆" + t1.TeamName + "🏆", FontSize = 20, Background = new SolidColorBrush(Colors.Transparent) };
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
                    await RefreshSTeamsList();
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
                        SpecialTeams ts = SpecialTeamslist.Find(x =>x.Id == teamid);
                        SpecialTeamIdBoxUpdateSec.Text = teamid.ToString();
                        SpecialTeamDateBoxUpdateSec.Text = ts.FoundedDate.ToString();
                        SpecialTeamTrophiesBoxUpdateSec.Text = ts.TotalTrophies.ToString();
                        SpecialTeamWinsBoxUpdateSec.Text = ts.TotalWins.ToString();
                        SpecialTeamGoldenBoxUpdateSec.Text = ts.GoldenBalls.ToString();
                        SpecialTeamYearPlayersBoxUpdateSec.Text = ts.TotalYearPlayers.ToString();
                        DoneUpdateButtonSpecialSec.Click += async delegate
                        {
                            await RefreshSTeamsList();
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
                DoneDelButtonSpecial.Click += async  delegate
                {
                    await RefreshSTeamsList();
                    int teamid = int.Parse(SpecialTeamIdBoxDel.Text);
                    if (SpecialTeamslist.Find(x => x.Id == teamid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelSpecialTeamStack.Visibility = Visibility.Collapsed;
                        SpecialTeams st = SpecialTeamslist.Find(x => x.Id == teamid);
                        if (st.LeagueID.Id.ToString() == CurrentLeague.Id.ToString())
                        {
                            foreach (var item in StackSpecialTeams.Children)
                            {
                                if (item is Button)
                                {
                                    Button b = (item as Button);
                                    if (b.Content.ToString() == "🏆" +st.TeamName + "🏆")
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
            await RefreshSTeamsList();
        }
        //
        //Insert Update Delete MacthSum
        public async void ButtonClickMatchMenu(object sender, RoutedEventArgs e)
        {
            string head = (sender as MenuItem).Header.ToString();
            if (head == "AddMatch")
            {
                AddMatchStack.Visibility = Visibility.Visible;
                DelMatchStack.Visibility= Visibility.Collapsed;
                UpdateMatchStack.Visibility = Visibility.Collapsed;
                UpdateMatchStackInfo.Visibility = Visibility.Collapsed;
                DoneAddButtonMatch.Click += async delegate
                {
                    await RefreshMatchList();
                    DateTime date;
                    Team winnerteam;
                    League league;
                    int MatchTime;
                    try
                    {
                        date = DateTime.Parse(MatchDateBoxAdd.Text);
                        winnerteam = Teamlist.Find(x => x.Id == int.Parse(MatchWinnerTeamAdd.Text));
                        league = leaguelist.Find(x => x.Id == int.Parse(MatchLeagueIdAdd.Text));
                        MatchTime = int.Parse(MatchTimeAdd.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid Content");
                        return;
                    }
                    AddMatchStack.Visibility = Visibility.Collapsed;
                    MatchSum match = new MatchSum() {  MatchDate = date, WinnerTeam = winnerteam, LeagueID = league, MatchTime=MatchTime };
                    if (CurrentLeague.Id.ToString() == match.LeagueID.Id.ToString())
                    {
                        MatchSum item = MatchSumlist.Last();
                        int id = item.Id + 1;
                        TextBlock b = new TextBlock(){Text = " Winner - " + match.WinnerTeam.TeamName.ToString() + "  Within " + match.MatchTime + " H\n" + "        " + match.MatchDate, FontSize = 20, Foreground = new SolidColorBrush(Colors.Black) };
                        StackMatch.Children.Add(b);
                    }
                    await apiService.InsertMatchSum(match);
                };
            }
            else if (head == "UpdateMatch")
            {
                AddMatchStack.Visibility = Visibility.Collapsed;
                DelMatchStack.Visibility = Visibility.Collapsed;
                MatchSum m;
                UpdateMatchStack.Visibility = Visibility.Visible;
                UpdateMatchStackInfo.Visibility = Visibility.Collapsed;
                DoneUpdateButtonMatch.Click += async delegate
                {
                    await RefreshMatchList();
                    int matchid = int.Parse(MatchIdBoxUpdate.Text);
                    if (MatchSumlist.Find(x => x.Id == matchid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        UpdateMatchStack.Visibility = Visibility.Collapsed;
                        UpdateMatchStackInfo.Visibility = Visibility.Visible;
                        m = MatchSumlist.Find(x => x.Id == matchid);
                        int oldle = m.LeagueID.Id;
                        string oldteam = m.WinnerTeam.ToString();
                        MatchIdBoxUpdateSec.Text = m.Id.ToString();
                        MatchDateBoxUpdateSec.Text = m.MatchDate.ToString();
                        MatchWinnerTeamUpdateSec.Text = m.WinnerTeam.Id.ToString();
                        MatchLeagueIdUpdateSec.Text = m.LeagueID.Id.ToString();
                        MatchTimeUpdateSec.Text = m.MatchTime.ToString();
                        DoneUpdateButtonMatchSec.Click += async delegate
                        {
                            await RefreshMatchList();
                            DateTime date;
                            Team winnerteam;
                            League league;
                            int MatchTime;
                            try
                            {
                                date = DateTime.Parse(MatchDateBoxUpdateSec.Text);
                                winnerteam = Teamlist.Find(x => x.Id == int.Parse(MatchWinnerTeamUpdateSec.Text));
                                league = leaguelist.Find(x => x.Id == int.Parse(MatchLeagueIdUpdateSec.Text));
                                MatchTime = int.Parse(MatchTimeUpdateSec.Text);
                            }
                            catch
                            {
                                MessageBox.Show("Invalid Content");
                                return;
                            }
                            UpdateMatchStackInfo.Visibility = Visibility.Collapsed;
                            m.MatchDate = date;
                            m.WinnerTeam = winnerteam;
                            m.MatchTime = MatchTime;
                            m.LeagueID = league;
                            if (m.LeagueID.Id == CurrentLeague.Id)
                            {
                                foreach (var item in StackMatch.Children)
                                {
                                    if (item is TextBlock)
                                    {
                                        TextBlock b = (item as TextBlock);
                                        if (b.Text.ToString().Contains(oldteam))
                                        {
                                            b.Text = " Winner - " + m.WinnerTeam.TeamName.ToString() + "  Within " + m.MatchTime + " H\n" + "        " + m.MatchDate;
                                        }
                                    }
                                }
                            }
                            if (league.Id != oldle)
                            {
                                foreach (var item in StackMatch.Children)
                                {
                                    if (item is TextBlock)
                                    {
                                        TextBlock b = (item as TextBlock);
                                        if (b.Text.ToString().Contains(oldteam))
                                        {
                                            b.Visibility = Visibility.Collapsed;
                                        }
                                    }
                                }
                            }
                            await apiService.UpdateMatchSum(m);
                        };
                    }
                };
            }
            else if (head == "DeleteMatch")
            {
                AddMatchStack.Visibility = Visibility.Collapsed;
                UpdateMatchStack.Visibility = Visibility.Collapsed;
                UpdateMatchStackInfo.Visibility = Visibility.Collapsed;
                DelMatchStack.Visibility = Visibility.Visible;
                DoneDelButtonMatch.Click += async delegate
                {
                    await  RefreshMatchList();
                    int matchid = int.Parse(MatchIdBoxDel.Text);
                    if (MatchSumlist.Find(x => x.Id == matchid) == null)
                    {
                        MessageBox.Show("Invalid Content");
                    }
                    else
                    {
                        DelMatchStack.Visibility = Visibility.Collapsed;
                        MatchSum m = MatchSumlist.Find(x => x.Id == matchid);
                        if (m.LeagueID.Id.ToString() == CurrentLeague.Id.ToString())
                        {
                            foreach (var item in StackMatch.Children)
                            {
                                if (item is TextBlock)
                                {
                                    TextBlock b = (item as TextBlock);
                                    if (b.Text.ToString().Contains(m.WinnerTeam.ToString()))
                                    {
                                        b.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                        await apiService.DeleteMatchSum(m);
                    }
                };
            }
            await  RefreshMatchList();
        } 
        //
    }
}