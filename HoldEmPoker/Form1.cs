using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace HoldEmPoker
{
    public partial class Form1 : Form
    {
        Player pl1 = new Player("Player");
        Player bot1 = new Player("Computer1");
        Player bot2 = new Player("Computer2");
        StackOfCards stack;
        List<Card> set;                         // set of cards on table

        List<Player> players = new List<Player>();
        List<Player> winners;
        int sequenceOfGame = 0;                  // pre-flop, flop, tern or river
        int indexOfPlayer = 0;                   // self index (I) of position of players
        int indexOfFirstPlayer = 0;              // player after dealler button, whom first word
        int solutionOfPlayer = 0;                // Trade before Player or after, 0 - before, 1 - after
        int bank = 0;
        int maxRate = 10;                        // max rate of players on table

        string riseIsOccured = "";               // for define index of Player(I), when done rise

        int indexOfRisedPlayer = -1;             // index of rised player in new array of players (0 - if was rise). Trade begin since player, who stand after him 

        public Form1()
        {
            InitializeComponent();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Game()                              //TODO: increase rate                                  
        {                                               //TODO: очередь раздачи, после баттона диллера
            if (sequenceOfGame == 0)
            {
                set = new List<Card>();

                pl1.pairOfCards = new List<Card>();
                bot1.pairOfCards = new List<Card>();
                bot2.pairOfCards = new List<Card>();

                pl1.IsLoose = false;
                bot1.IsLoose = false;
                bot2.IsLoose = false;

                InitialAligment();                                  // выравнивание позиций карт

                players = new List<Player>();

                if (!pl1.IsDefault)
                {
                    players.Add(pl1);
                }
                if (!bot1.IsDefault)
                {
                    players.Add(bot1);
                }
                if (!bot2.IsDefault)
                {
                    players.Add(bot2);
                }

                if (players.Count == 1)
                {
                    MessageBox.Show(players[0].Name + " VICTORY!!!");
                    button1.Visible = true;
                    return;
                }

                indexOfFirstPlayer++;                  // перемещение баттона диллера
                if (indexOfFirstPlayer > players.Count - 1)
                {
                    indexOfFirstPlayer = 0;
                }
                ChangeSequenceOfPlayers(indexOfFirstPlayer);

                bank = 0;

                maxRate = 10;
                if (players[players.Count - 2].Account > maxRate / 2)
                {
                    players[players.Count - 2].Account -= maxRate / 2;      // small blind
                    players[players.Count - 2].Rate = maxRate / 2;
                }
                else
                {
                    players[players.Count - 2].AllIn = true;
                    players[players.Count - 2].Rate = players[players.Count - 2].Account;
                    players[players.Count - 2].Account = 0;
                }

                if (players[players.Count - 1].Account > maxRate)
                {
                    players[players.Count - 1].Account -= maxRate;         // big blind 
                    players[players.Count - 1].Rate = maxRate;
                }
                else
                {
                    players[players.Count - 1].AllIn = true;
                    players[players.Count - 1].Rate = players[players.Count - 2].Account;
                    players[players.Count - 1].Account = 0;
                }

                foreach (var item in players)
                {
                    item.ShowRate(this);
                }

                stack = new StackOfCards();                         // колода карт

                if (!pl1.IsDefault)
                {
                    pl1.pairOfCards.Add(stack.TakeCard());              // 2 карты для игрока
                    pl1.pairOfCards.Add(stack.TakeCard());
                }

                // 2 карты для бота
                if (!bot1.IsDefault)
                {
                    bot1.pairOfCards.Add(stack.TakeCard());
                    bot1.pairOfCards.Add(stack.TakeCard());
                }

                // 2 карты для бота 2
                if (!bot2.IsDefault)
                {
                    bot2.pairOfCards.Add(stack.TakeCard());
                    bot2.pairOfCards.Add(stack.TakeCard());
                }
                winners = new List<Player>();

                //////////////////////////////////////визуальная раздача игроку и ботам рубашкой вверх

                if (!pl1.IsDefault)
                {
                    picturePl2.Visible = false;
                    picturePl1.Image = Image.FromFile("Poker\\back.png");
                    picturePl1.Refresh();
                    Thread.Sleep(300);
                }

                if (!bot1.IsDefault)
                {
                    pictureBot12.Visible = false;
                    pictureBot11.Image = Image.FromFile("Poker\\back.png");
                    pictureBot11.Refresh();
                    Thread.Sleep(300);
                }

                if (!bot2.IsDefault)
                {
                    pictureBot22.Visible = false;
                    pictureBot21.Image = Image.FromFile("Poker\\back.png");
                    pictureBot21.Refresh();
                    Thread.Sleep(300);
                }

                if (!pl1.IsDefault)
                {
                    picturePl2.Image = Image.FromFile("Poker\\back.png");
                    picturePl2.Visible = true;
                    picturePl2.Refresh();
                    Thread.Sleep(300);
                }

                if (!bot1.IsDefault)
                {
                    pictureBot12.Image = Image.FromFile("Poker\\back.png");
                    pictureBot12.Visible = true;
                    pictureBot12.Refresh();
                    Thread.Sleep(300);
                }

                if (!bot2.IsDefault)
                {
                    pictureBot22.Image = Image.FromFile("Poker\\back.png");
                    pictureBot22.Visible = true;
                    pictureBot22.Refresh();
                }
                ///////////////////////////////////////////// открытие карт игрока

                if (!pl1.IsDefault)
                {
                    Thread.Sleep(1000);
                    picturePl2.Visible = false;
                    picturePl1.Image = Image.FromFile("Poker\\" + pl1.pairOfCards[0].Name + ".png");
                    picturePl1.Refresh();

                    picturePl2.Image = Image.FromFile("Poker\\" + pl1.pairOfCards[1].Name + ".png");
                    picturePl2.Visible = true;
                    picturePl2.Refresh();
                }
            }
            else if (sequenceOfGame == 1)   ///////////////////////////////////                  ФЛОП
            {
                Thread.Sleep(300);

                players = new List<Player>();

                if (!pl1.IsDefault)
                {
                    players.Add(pl1);
                }
                if (!bot1.IsDefault)
                {
                    players.Add(bot1);
                }
                if (!bot2.IsDefault)
                {
                    players.Add(bot2);
                }

                ChangeSequenceOfPlayers(indexOfFirstPlayer);      // оставить последовательность относительно баттона диллера

                set.Add(stack.TakeCard());
                pictureBox1.Image = Image.FromFile("Poker\\" + set[0].Name + ".png");
                pictureBox1.Refresh();

                set.Add(stack.TakeCard());
                pictureBox2.Image = Image.FromFile("Poker\\" + set[1].Name + ".png");
                pictureBox2.Refresh();

                set.Add(stack.TakeCard());
                pictureBox3.Image = Image.FromFile("Poker\\" + set[2].Name + ".png");
                pictureBox3.Refresh();
            }

            else if (sequenceOfGame == 2)                 //////////////////////////////////           ТЕНРН
            {
                Thread.Sleep(300);

                players = new List<Player>();

                if (!pl1.IsDefault)
                {
                    players.Add(pl1);
                }
                if (!bot1.IsDefault)
                {
                    players.Add(bot1);
                }
                if (!bot2.IsDefault)
                {
                    players.Add(bot2);
                }

                ChangeSequenceOfPlayers(indexOfFirstPlayer);              // оставить последовательность относительно баттона диллера

                set.Add(stack.TakeCard());
                pictureBox4.Image = Image.FromFile("Poker\\" + set[3].Name + ".png");
                pictureBox4.Refresh();
            }

            else if (sequenceOfGame == 3)                         /////////////////////////////////////////////// РИВЕР
            {
                Thread.Sleep(300);

                players = new List<Player>();

                if (!pl1.IsDefault)
                {
                    players.Add(pl1);
                }
                if (!bot1.IsDefault)
                {
                    players.Add(bot1);
                }
                if (!bot2.IsDefault)
                {
                    players.Add(bot2);
                }

                ChangeSequenceOfPlayers(indexOfFirstPlayer);              // оставить последовательность относительно баттона диллера

                set.Add(stack.TakeCard());
                pictureBox5.Image = Image.FromFile("Poker\\" + set[4].Name + ".png");
                pictureBox5.Refresh();
            }

            Trade();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Trade()
        {
            LABEL1:

            Player p = new Player();

            if (solutionOfPlayer == 0)       ///////////////////////////////////////////////// before word of Player (I)
            {
                for (int i = indexOfRisedPlayer + 1; i <= indexOfPlayer; i++)  // если был райз, слово игроку после игрока, сделавшего райз
                {

                    if (players[i].Name.Equals("Computer1") && !players[i].IsLoose)
                    {
                        if (RemainderOnePlayer(ref p))
                        {
                            solutionOfPlayer = 1;
                            break;
                        }
                        if (!players[i].AllIn)
                        {
                            string s = players[i].AcceptSolution(this, sequenceOfGame, set, ref maxRate);

                            if (s.Equals("raise") || s.Equals("raise all in"))
                            {
                                ChangeSequenceOfPlayers(i);               // если райз - сделать этого игрока первым в списке торгов
                                riseIsOccured = "";
                                indexOfRisedPlayer = 0;
                                goto LABEL1;
                            }
                        }
                    }

                    else if (players[i].Name.Equals("Computer2") && !players[i].IsLoose)
                    {
                        if (RemainderOnePlayer(ref p))
                        {
                            solutionOfPlayer = 1;
                            break;
                        }

                        if (!players[i].AllIn)
                        {
                            string s = players[i].AcceptSolution(this, sequenceOfGame, set, ref maxRate);

                            if (s.Equals("raise") || s.Equals("raise all in"))
                            {
                                ChangeSequenceOfPlayers(i);               // если райз - сделать этого игрока первым в списке торгов
                                riseIsOccured = "";
                                indexOfRisedPlayer = 0;
                                goto LABEL1;
                            }
                        }
                    }

                    else if (players[i].Name.Equals("Player"))
                    {
                        if (RemainderOnePlayer(ref p))
                        {
                            solutionOfPlayer = 1;
                            break;
                        }
                        indexOfPlayer = i;

                        if (players[i].IsLoose || players[i].AllIn || players[i].IsDefault)
                        {
                            solutionOfPlayer = 1;                   // если Игрок упал - переход ко 2-й части торгов без его участия
                        }
                        else
                        {
                            AcceptRateButton.Visible = true;
                            AcceptRateButton.Refresh();
                            UpRateButton.Visible = true;
                            UpRateButton.Refresh();
                            DownRateButton.Visible = true;
                            DownRateButton.Refresh();
                            textBoxRate.Visible = true;
                            textBoxRate.Text = 0.ToString();
                            textBoxRate.Refresh();
                            riseButton.Visible = true;
                            riseButton.Refresh();
                            FallDownButton.Visible = true;
                            FallDownButton.Refresh();
                            break;
                        }
                    }
                }
                if (pl1.IsDefault)
                {
                    solutionOfPlayer = 1;
                    indexOfRisedPlayer = 2;
                }
            }
            //////////////////////////////////////////////////////////////////////////////////

            if (solutionOfPlayer == 1)          ///////////////////////////////// after word of Player (I)
            {

                if (!riseIsOccured.Equals(""))                // if was Rise of Player
                {
                    ChangeSequenceOfPlayers(Convert.ToInt32(riseIsOccured));
                    riseIsOccured = "";
                    solutionOfPlayer = 1;
                    indexOfRisedPlayer = -1;
                    goto LABEL1;
                }

                for (int i = indexOfPlayer + 1; i < players.Count; i++)
                {
                    if (players[i].Name.Equals("Computer1") && !players[i].IsLoose)
                    {
                        if (RemainderOnePlayer(ref p))
                        {
                            solutionOfPlayer = 1;
                            break;
                        }
                        if (!players[i].AllIn)
                        {
                            string s = players[i].AcceptSolution(this, sequenceOfGame, set, ref maxRate);

                            if (s.Equals("raise") || s.Equals("raise all in"))
                            {
                                ChangeSequenceOfPlayers(i);               // если райз - сделать этого игрока первым в списке торгов
                                riseIsOccured = "";
                                indexOfRisedPlayer = 0;

                                solutionOfPlayer = 0;

                                goto LABEL1;
                            }
                        }
                    }

                    else if (players[i].Name.Equals("Computer2") && !players[i].IsLoose)
                    {
                        if (RemainderOnePlayer(ref p))
                        {
                            solutionOfPlayer = 1;
                            break;
                        }

                        if (!players[i].AllIn)
                        {
                            string s = players[i].AcceptSolution(this, sequenceOfGame, set, ref maxRate);

                            if (s.Equals("raise") || s.Equals("raise all in"))
                            {
                                ChangeSequenceOfPlayers(i);               // если райз - сделать этого игрока первым в списке торгов
                                riseIsOccured = "";
                                indexOfRisedPlayer = 0;

                                solutionOfPlayer = 0;

                                goto LABEL1;
                            }
                        }
                    }
                }

                for (int i = 0; i < players.Count; i++)
                {
                    bank += players[i].Rate;
                    players[i].Contribution += players[i].Rate;
                }

                lblBank.Text = bank.ToString();
                lblBank.Refresh();

                lblPlRate.Text = "";
                lblComp1Rate.Text = "";
                lblComp2Rate.Text = "";
                lblPlRate.Refresh();
                lblComp1Rate.Refresh();
                lblComp2Rate.Refresh();

                if (RemainderOnePlayer(ref p))                          // if all players fold but one
                {
                    MessageBox.Show(p.Name + " WON!");
                    sequenceOfGame = -1;

                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].Name.Equals(p.Name))
                        {
                            players[i].Account += players[i].Contribution;
                        }
                        else
                        {
                            if (p.Contribution >= players[i].Contribution)
                            {
                                p.Account += players[i].Contribution;
                            }
                            else
                            {
                                p.Account += p.Contribution;
                                players[i].Account += players[i].Contribution - p.Contribution;
                            }
                        }
                    }

                    p.ShowRate(this);

                    foreach (var item in players)
                    {
                        if (item.Account <= 0)
                        {
                            item.IsDefault = true;
                            MessageBox.Show(item.Name + " LOST");
                        }
                        item.AllIn = false;
                        item.Contribution = 0;
                    }
                }

                else if (sequenceOfGame == 3)
                {
                    sequenceOfGame = -1;

                    label2.Text = "";
                    label3.Text = "";
                    label2.Refresh();
                    label3.Refresh();

                    winners = DefineWinners(set, players);
                    ShowWinners(set, winners);

                    if (winners.Count == 1)
                    {
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (players[i].Name.Equals(winners[0].Name))
                            {
                                players[i].Account += players[i].Contribution;
                            }
                            else
                            {
                                if (winners[0].Contribution >= players[i].Contribution)
                                {
                                    winners[0].Account += players[i].Contribution;
                                }
                                else
                                {
                                    winners[0].Account += winners[0].Contribution;
                                    players[i].Account += players[i].Contribution - winners[0].Contribution;
                                }
                            }
                        }
                        winners[0].ShowRate(this);
                    }

                    else if (winners.Count == 2)  
                    {
                        int countOfPlayersInGame = 0;
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (!players[i].IsLoose && !players[i].IsDefault)
                            {
                                countOfPlayersInGame++;
                            }
                        }

                        if (countOfPlayersInGame == 3)
                        {
                            Player losedPlayer = null;

                            for (int i = 0; i < players.Count; i++)   // find looser
                            {
                                if (!winners.Contains(players[i]))
                                {
                                    losedPlayer = players[i];
                                }
                            }
                            if (winners[0].Contribution > winners[1].Contribution)
                            {
                                Player temporary = winners[0];
                                winners[0] = winners[1];
                                winners[1] = temporary;
                            }

                            for (int i = 0; i < winners.Count; i++)
                            {
                                winners[i].Account += winners[i].Contribution;
                            }

                            if (winners[0].Contribution >= losedPlayer.Contribution / 2)
                            {
                                winners[0].Account += losedPlayer.Contribution / 2;
                                losedPlayer.Contribution -= losedPlayer.Contribution / 2;
                            }
                            else
                            {
                                winners[0].Account += winners[0].Contribution;
                                losedPlayer.Contribution -= winners[0].Contribution;
                            }

                            if (winners[1].Contribution >= losedPlayer.Contribution)
                            {
                                winners[1].Account += losedPlayer.Contribution;
                                losedPlayer.Contribution = 0;
                            }
                            else
                            {
                                winners[1].Account += winners[1].Contribution;
                                losedPlayer.Contribution -= winners[1].Contribution;
                            }

                            losedPlayer.Account += losedPlayer.Contribution; ;

                            for (int i = 0; i < winners.Count; i++)
                            {
                                winners[i].ShowRate(this);
                            } 
                        }
                        else
                        {
                            foreach (var item in winners)
                            {
                                item.Account += item.Contribution;
                                item.ShowRate(this);
                            }
                        }
                    }

                    else if (winners.Count == 3)
                    {
                        foreach (var item in winners)
                        {
                            item.Account += item.Contribution;
                            item.ShowRate(this);
                        }
                    }

                    foreach (var item in players)
                    {
                        if (item.Account <= 0)
                        {
                            item.IsDefault = true;
                            MessageBox.Show(item.Name + " LOST");
                        }
                        item.AllIn = false;
                        item.Contribution = 0;
                    }
                }

                foreach (var pl in players)
                {
                    pl.Rate = 0;                       // default rates
                }

                maxRate = 0;

                lblPlAcc.Text = pl1.Account.ToString();
                lblPlAcc.Refresh();
                lblCom1Acc.Text = bot1.Account.ToString();
                lblPlAcc.Refresh();
                lblCom2Acc.Text = bot2.Account.ToString();
                lblPlAcc.Refresh();

                riseIsOccured = "";
                solutionOfPlayer = 0;
                indexOfRisedPlayer = -1;

                label2.Text = "";
                label3.Text = "";
                label2.Refresh();
                label3.Refresh();

                sequenceOfGame++;
                Game();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            pl1 = new Player("Player");
            bot1 = new Player("Computer1");
            bot2 = new Player("Computer2");
            Game();
        }

        private void ShowWinners(List<Card> set, List<Player> winners)
        {
            Thread.Sleep(1000);
            Aligment();
            string stringOfWinners = "";

            foreach (var item in winners)
            {
                if (!item.IsLoose)
                {
                    item.ShowCombination(this, set);
                    stringOfWinners += item.Name + " ";
                }
            }

            MessageBox.Show(stringOfWinners + " WON!!!");
        }

        private List<Player> DefineWinners(List<Card> set, List<Player> players)
        {
            List<Player> pls = new List<Player>();

            foreach (var player in players)
            {
                player.DefineCombination(set.ToArray());

                if (player.Name.Equals("Player") && !player.IsLoose)
                {
                    label1.Text = player.Comb.ToString();
                    label1.Refresh();
                    pls.Add(player);
                }
                else if (player.Name.Equals("Computer1") && !player.IsLoose)
                {
                    Thread.Sleep(500);
                    pictureBot11.Image = Image.FromFile("Poker\\" + bot1.pairOfCards[0].Name + ".png");
                    pictureBot11.Refresh();
                    pictureBot12.Image = Image.FromFile("Poker\\" + bot1.pairOfCards[1].Name + ".png");
                    pictureBot12.Visible = true;
                    pictureBot12.Refresh();

                    label2.Text = player.Comb.ToString();
                    label2.Refresh();
                    pls.Add(player);
                }
                else if (player.Name.Equals("Computer2") && !player.IsLoose)
                {
                    Thread.Sleep(500);
                    pictureBot21.Image = Image.FromFile("Poker\\" + bot2.pairOfCards[0].Name + ".png");
                    pictureBot21.Refresh();
                    pictureBot22.Image = Image.FromFile("Poker\\" + bot2.pairOfCards[1].Name + ".png");
                    pictureBot22.Visible = true;
                    pictureBot22.Refresh();
                    Thread.Sleep(1000);

                    label3.Text = player.Comb.ToString();
                    label3.Refresh();
                    pls.Add(player);
                }
            }

            pls.Sort();
            pls.Reverse();

            List<Player> winners = new List<Player>();

            winners.Add(pls[0]);

            for (int i = 1; i < pls.Count; i++)
            {
                if (pls[i].CompareTo(pls[0]) == 0 && !pls[i].IsLoose)
                {
                    winners.Add(pls[i]);
                }
            }
            return winners;
        }

        private void InitialAligment()
        {
            picturePl1.Image = null;
            picturePl2.Image = null;

            pictureBot11.Image = null;
            pictureBot12.Image = null;

            pictureBot21.Image = null;
            pictureBot22.Image = null;

            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;

            Aligment();

            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            lblBank.Text = "";
            lblBank.Refresh();
        }

        private void Aligment()
        {
            lblPlRate.Text = "";
            lblPlRate.Refresh();
            lblComp1Rate.Text = "";
            lblComp1Rate.Refresh();
            lblComp2Rate.Text = "";
            lblComp2Rate.Refresh();

            //выравнивание позиций карт
            picturePl1.Location = new Point(206, 411);
            picturePl1.Refresh();
            picturePl2.Location = new Point(230, 411);
            picturePl2.Refresh();

            pictureBot11.Location = new Point(69, 38);
            pictureBot11.Refresh();
            pictureBot12.Location = new Point(93, 38);
            pictureBot12.Refresh();

            pictureBot21.Location = new Point(392, 38);
            pictureBot21.Refresh();
            pictureBot22.Location = new Point(416, 38);
            pictureBot22.Refresh();

            pictureBox1.Location = new Point(12, 223);
            pictureBox1.Refresh();
            pictureBox2.Location = new Point(113, 223);
            pictureBox2.Refresh();
            pictureBox3.Location = new Point(214, 223);
            pictureBox3.Refresh();
            pictureBox4.Location = new Point(315, 223);
            pictureBox4.Refresh();
            pictureBox5.Location = new Point(416, 223);
            pictureBox5.Refresh();
        }

        private void UpRateButton_Click(object sender, EventArgs e)
        {
            int a = 0;
            try
            {
                a = Convert.ToInt32(textBoxRate.Text);
            }
            catch (Exception)
            {
                a = 0;
            }
            if (pl1.Account + pl1.Rate > a + maxRate)
            {
                textBoxRate.Text = (a + 5).ToString();
            }
        }

        private void DownRateButton_Click(object sender, EventArgs e)
        {
            int a = 0;
            try
            {
                a = Convert.ToInt32(textBoxRate.Text);
            }
            catch (Exception)
            {
                a = 0;
            }

            if (a > 0)
            {
                textBoxRate.Text = (a - 5).ToString();
            }
        }

        private void AcceptRateButton_Click(object sender, EventArgs e)
        {
            if (pl1.Account + pl1.Rate > maxRate)
            {
                pl1.Account -= maxRate - pl1.Rate;
                pl1.Rate = maxRate;
            }
            else
            {
                pl1.AllIn = true;
                pl1.Rate = pl1.Account + pl1.Rate;
                pl1.Account = 0;
            }

            AcceptRateButton.Visible = false;
            AcceptRateButton.Refresh();
            UpRateButton.Visible = false;
            UpRateButton.Refresh();
            DownRateButton.Visible = false;
            DownRateButton.Refresh();
            textBoxRate.Visible = false;
            textBoxRate.Refresh();

            riseButton.Visible = false;
            riseButton.Refresh();

            FallDownButton.Visible = false;
            FallDownButton.Refresh();

            solutionOfPlayer = 1;

            pl1.ShowRate(this);

            Trade();
        }

        private void ChangeSequenceOfPlayers(int indexFirstPlayer)               // indexFirstPlayer - index of player from which begin new sequence
        {
            List<Player> termList = new List<Player>();

            for (int i = indexFirstPlayer; i < players.Count; i++)
            {
                termList.Add(players[i]);
            }

            for (int i = 0; i < indexFirstPlayer; i++)
            {
                termList.Add(players[i]);
            }

            for (int i = 0; i < termList.Count; i++)
            {
                if (termList[i].Name.Equals("Player"))
                {
                    indexOfPlayer = i;
                    break;
                }
            }
            players = termList;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pl1.IsLoose = true;

            picturePl1.Image = null;
            picturePl1.Refresh();
            picturePl2.Image = null;
            picturePl2.Refresh();

            AcceptRateButton.Visible = false;
            AcceptRateButton.Refresh();
            UpRateButton.Visible = false;
            UpRateButton.Refresh();
            DownRateButton.Visible = false;
            DownRateButton.Refresh();
            textBoxRate.Visible = false;
            textBoxRate.Refresh();
            riseButton.Visible = false;
            riseButton.Refresh();
            FallDownButton.Visible = false;
            FallDownButton.Refresh();

            solutionOfPlayer = 1;

            pl1.ShowRate(this);

            Trade();
        }

        private bool RemainderOnePlayer(ref Player p)
        {
            int countOfLoosers = 0;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].IsLoose)
                {
                    countOfLoosers++;
                }
            }

            if (countOfLoosers == players.Count - 1)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (!players[i].IsLoose)
                    {
                        p = players[i];
                    }
                }
                return true;
            }
            return false;
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            if (pl1.Rate == maxRate)
            {
                lblPlRate.Text = pl1.Rate.ToString();
                lblPlRate.Refresh();

                if (pl1.IsLoose)
                {
                    picturePl1.Image = null;
                    picturePl1.Refresh();
                    picturePl2.Image = null;
                    picturePl2.Refresh();
                }

                if (bot1.IsLoose)
                {
                    pictureBot11.Image = null;
                    pictureBot11.Refresh();
                    pictureBot12.Image = null;
                    pictureBot12.Refresh();
                }

                if (bot2.IsLoose)
                {
                    pictureBot21.Image = null;
                    pictureBot21.Refresh();
                    pictureBot22.Image = null;
                    pictureBot22.Refresh();
                }

                AcceptRateButton.Visible = false;
                AcceptRateButton.Refresh();
                UpRateButton.Visible = false;
                UpRateButton.Refresh();
                DownRateButton.Visible = false;
                DownRateButton.Refresh();
                textBoxRate.Visible = false;
                textBoxRate.Refresh();
                riseButton.Visible = false;
                riseButton.Refresh();
                FallDownButton.Visible = false;
                FallDownButton.Refresh();

                solutionOfPlayer = 1;

                Trade();
            }
        }

        private void riseButton_Click(object sender, EventArgs e)
        {
            int a;
            try
            {
                a = Convert.ToInt32(textBoxRate.Text);
            }
            catch (Exception)
            {
                a = 0;
            }

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Name.Equals("Player"))
                {
                    if (a > 0)  // если был райз
                    {
                        riseIsOccured = i.ToString(); ;
                    }
                    // сделать этого игрока первым в списке торгов
                    break;
                }
            }

            if (pl1.Account + pl1.Rate > a + maxRate)
            {
                pl1.Account -= (maxRate - pl1.Rate) + a;
                maxRate = maxRate + a;
                pl1.Rate = maxRate;
            }
            else
            {
                pl1.AllIn = true;
                pl1.Rate = pl1.Account + pl1.Rate;
                pl1.Account = 0;
                if (pl1.Rate > maxRate)
                {
                    maxRate = pl1.Rate;
                }
            }

            lblPlRate.Text = pl1.Rate.ToString();
            lblPlRate.Refresh();

            AcceptRateButton.Visible = false;
            AcceptRateButton.Refresh();
            UpRateButton.Visible = false;
            UpRateButton.Refresh();
            DownRateButton.Visible = false;
            DownRateButton.Refresh();
            textBoxRate.Visible = false;
            textBoxRate.Refresh();
            riseButton.Visible = false;
            riseButton.Refresh();
            FallDownButton.Visible = false;
            FallDownButton.Refresh();

            solutionOfPlayer = 1;

            pl1.ShowRate(this);

            Trade();
        }
    }
}
