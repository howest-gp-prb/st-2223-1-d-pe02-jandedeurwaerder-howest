using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prb.PE2.GuessingGame.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int NumberOfThrows = 3;
        List<string> allPlayers = new List<string>();
        Random rnd = new Random();
        int[] playerGuesses = new int[2];
        string[] playerNames = new string[2];
        int[] playerIndexes = new int[2];
        int[] randomGuesses = new int[NumberOfThrows];
        int[] scores = new int[2];
        string winnerName;
        int winnerIndex;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnNewRound.IsEnabled = false;
            DoStartup();
            DoSeeding();
        }
        private void DoStartup()
        {
            btnAddName.IsEnabled = false;
            PrepareForNewGame();
        }
        private void PrepareForNewGame()
        {
            txtFirstPlayerGuess.Text = "";
            txtSecondPlayerGuess.Text = "";
            btnPlay.IsEnabled = false;
            btnQuickPick.IsEnabled = false;
            txtFirstPlayerGuess.IsEnabled = false;
            txtSecondPlayerGuess.IsEnabled = false;
        }
        private void DoSeeding()
        {
            allPlayers.Add("Wim");
            allPlayers.Add("Els");
            allPlayers.Add("Guust");
            allPlayers.Add("Louis");
            allPlayers.Add("Martha");
            allPlayers.Add("Lizzy");
            PopulateListbox();
            btnNewRound.IsEnabled = true;
        }
        private void PopulateListbox()
        {
            lstPlayers.ItemsSource = allPlayers;
            lstPlayers.Items.Refresh();
        }
        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnAddName.IsEnabled = txtName.Text.Trim().Length > 0;
        }
        private void BtnAddName_Click(object sender, RoutedEventArgs e)
        {
            string newPlayer = txtName.Text.Trim();
            if (CheckPlayerInUse(newPlayer))
            {
                MessageBox.Show($"De naam {newPlayer} is reeds in gebruik!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            allPlayers.Add(newPlayer);
            btnNewRound.IsEnabled = EnoughPlayers();
            PopulateListbox();
            txtName.Clear();
            txtName.Focus();
        }
        private bool CheckPlayerInUse(string newName)
        {
            foreach (string player in allPlayers)
            {
                if (player.ToUpper() == newName.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
        private bool EnoughPlayers()
        {
            return allPlayers.Count > 1;
        }

        private void BtnNewRound_Click(object sender, RoutedEventArgs e)
        {
            PickPlayers();
            PrepareNewRound();
        }
        private void PickPlayers()
        {
            playerIndexes[0] = rnd.Next(0, allPlayers.Count);
            playerIndexes[1] = playerIndexes[0];
            while (playerIndexes[0] == playerIndexes[1])
            {
                playerIndexes[1] = rnd.Next(0, allPlayers.Count);
            }
            PreparePlayers();
        }
        private void PreparePlayers()
        {
            playerNames[0] = allPlayers[playerIndexes[0]];
            playerNames[1] = allPlayers[playerIndexes[1]];
            lblResult.Content = "";
            lblFirstPlayerName.Content = playerNames[0];
            lblSecondPlayerName.Content = playerNames[1];
            lblFirstPlayerScore.Content = 0;
            lblSecondPlayerScore.Content = 0;
        }
        private void PrepareNewRound()
        {
            btnPlay.IsEnabled = true;
            btnQuickPick.IsEnabled = true;
            txtFirstPlayerGuess.IsEnabled = true;
            txtSecondPlayerGuess.IsEnabled = true;
            txtFirstPlayerGuess.Focus();
        }
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            ClearStats();
            try
            {
                CheckInput();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            GameLogic();
            DisplayResults();
            RearangePlayers();
            PrepareForNewGame();
        }
        private void ClearStats()
        {
            lblResult.Content = "";
            lblFirstPlayerScore.Content = "";
            lblSecondPlayerScore.Content = "";
            lblWinner.Content = "";
        }
        private void CheckInput()
        {
            int.TryParse(txtFirstPlayerGuess.Text.Trim(), out playerGuesses[0]);
            int.TryParse(txtSecondPlayerGuess.Text.Trim(), out playerGuesses[1]); ;
            if (playerGuesses[0] <= 0 || playerGuesses[0] >= 100)
                throw new Exception("Voer een getal in van 1 tot 99");
            if (playerGuesses[1] <= 0 || playerGuesses[1] >= 100)
                throw new Exception("Voer een getal in van 1 tot 99");
        }
        private void GameLogic()
        {
            PickRandomNumbers();
            int differencePlayer1, differencePlayer2;
            scores = new int[2];
            for (int i = 0; i < NumberOfThrows; i++)
            {
                differencePlayer1 = Math.Abs(playerGuesses[0] - randomGuesses[i]);
                differencePlayer2 = Math.Abs(playerGuesses[1] - randomGuesses[i]);
                if (differencePlayer1 < differencePlayer2)
                {
                    scores[0]++;
                }
                else if (differencePlayer1 > differencePlayer2)
                {
                    scores[1]++;
                }
            }
            if (scores[0] > scores[1])
            {
                winnerName = playerNames[0];
                winnerIndex = playerIndexes[0];
            }
            else if (scores[1] > scores[0])
            {
                winnerName = playerNames[1];
                winnerIndex = playerIndexes[1];
            }
            else
            {
                winnerName = "Niemand";
                winnerIndex = -1;
            }

        }
        private void PickRandomNumbers()
        {
            for (int i = 0; i < NumberOfThrows; i++)
            {
                randomGuesses[i] = rnd.Next(1, 100);
            }
        }
        private void DisplayResults()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < NumberOfThrows; i++)
            {
                sb.Append($"{randomGuesses[i]} ");
            }
            lblResult.Content = sb.ToString();

            //string result = "";
            //for (int i = 0; i < NumberOfThrows; i++)
            //{
            //    result += randomGuesses[i] + " ";
            //}
            //lblResult.Content = result;

            lblFirstPlayerScore.Content = $"({playerGuesses[0]}) {scores[0]}";
            lblSecondPlayerScore.Content = $"({playerGuesses[1]}) {scores[1]}";
            lblWinner.Content = $"{winnerName} heeft gewonnen";

        }
        private void RearangePlayers()
        {
            if (winnerIndex != -1)
            {
                if (winnerIndex == playerIndexes[0] && playerIndexes[0] > playerIndexes[1])
                {                    
                    string tempName = allPlayers[playerIndexes[0]];
                    allPlayers[playerIndexes[0]] = allPlayers[playerIndexes[1]];
                    allPlayers[playerIndexes[1]] = tempName;
                }
                else if (winnerIndex == playerIndexes[1] && playerIndexes[1] > playerIndexes[0])
                {
                    string tempName = allPlayers[playerIndexes[1]];
                    allPlayers[playerIndexes[1]] = allPlayers[playerIndexes[0]];
                    allPlayers[playerIndexes[0]] = tempName;
                }
                PopulateListbox();
            }
        }
        private void BtnQuickPick_Click(object sender, RoutedEventArgs e)
        {
            playerGuesses[0] = rnd.Next(1, 100);
            playerGuesses[1] = rnd.Next(1, 100);
            txtFirstPlayerGuess.Text = playerGuesses[0].ToString();
            txtSecondPlayerGuess.Text = playerGuesses[1].ToString();
        }
    }
}
