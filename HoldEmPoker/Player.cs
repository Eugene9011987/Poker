using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace HoldEmPoker
{
    class Player : IComparable<Player>
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsLoose = false;        // loose stage
        public bool IsDefault = false;      // game over for player

        public bool AllIn = false;

        private int account;                // amount of money
        public int Account
        {
            get { return account; }
            set { account = value; }
        }

        private int rate;                   // max rate on table
        public int Rate
        {
            get { return rate; }
            set { rate = value; }
        }

        private int contribution;
        public int Contribution
        {
            get { return contribution; }
            set { contribution = value; }
        }


        private Combination cmb;            // combination of cards in game
        public Combination Comb
        {
            get { return cmb; }
            set { cmb = value; }
        }

        public List<Card> pairOfCards = new List<Card>();     // pair of cards on hands
        public List<Card> combinationOfCards;

        public Player()
        {
        }

        public Player(string nm)
        {
            Name = nm;
            Comb = Combination.HightCard;
            combinationOfCards = new List<Card>();
            account = 100;
            pairOfCards = new List<Card>(2);
        }

        public void DefineCombination(Card[] s)           // s - set of cards on table
        {
            combinationOfCards = new List<Card>();
            Card[] set = new Card[s.Length + 2];

            set[0] = pairOfCards[0];
            set[1] = pairOfCards[1];

            for (int i = 0; i < s.Length; i++)
            {
                set[i + 2] = s[i];
            }

            Comb = Combination.HightCard;

            Array.Sort(set);
            Array.Reverse(set);

            ////////////////////////////////////// define FLASH
            int countC = 0;
            int countB = 0;
            int countT = 0;
            int countP = 0;

            List<Card> buffSetColor = new List<Card>();

            for (int i = 0; i < set.Length; i++)
            {
                if (set[i].Name[2].Equals('c'))
                {
                    countC++;
                }
                else if (set[i].Name[2].Equals('b'))
                {
                    countB++;
                }
                else if (set[i].Name[2].Equals('t'))
                {
                    countT++;
                }
                else
                {
                    countP++;
                }
            }

            if (countC > 4)
            {
                Comb = Combination.Flush;
                foreach (var item in set)
                {
                    if (item.Name[2].Equals('c'))
                    {
                        buffSetColor.Add(item);
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    combinationOfCards.Add(buffSetColor[i]);
                }
            }
            else if (countB > 4)
            {
                Comb = Combination.Flush;
                foreach (var item in set)
                {
                    if (item.Name[2].Equals('b'))
                    {
                        buffSetColor.Add(item);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    combinationOfCards.Add(buffSetColor[i]);
                }
            }
            else if (countT > 4)
            {
                Comb = Combination.Flush;
                foreach (var item in set)
                {
                    if (item.Name[2].Equals('t'))
                    {
                        buffSetColor.Add(item);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    combinationOfCards.Add(buffSetColor[i]);
                }
            }
            else if (countP > 4)
            {
                Comb = Combination.Flush;
                foreach (var item in set)
                {
                    if (item.Name[2].Equals('p'))
                    {
                        buffSetColor.Add(item);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    combinationOfCards.Add(buffSetColor[i]);
                }
            }

            ////////////////////////////////////////////////////////////////////////  define STREET-FLASH
            if (Comb == Combination.Flush)
            {
                int cntSequence = 0;
                List<Card> tempSet = new List<Card>();          // для хранения последовательных карт цвета

                for (int i = 0; i < buffSetColor.Count - 1; i++)
                {
                    if (Convert.ToInt32(buffSetColor[i].Name.Substring(0, 2)) - Convert.ToInt32(buffSetColor[i + 1].Name.Substring(0, 2)) == 1)
                    {
                        cntSequence++;
                        tempSet.Add(buffSetColor[i]);
                        if (tempSet.Count == 4)
                        {
                            tempSet.Add(buffSetColor[i + 1]);
                            break;
                        }
                    }
                    else if (Convert.ToInt32(buffSetColor[i].Name.Substring(0, 2)) - Convert.ToInt32(buffSetColor[i + 1].Name.Substring(0, 2)) != 0)
                    {
                        cntSequence = 0;
                        tempSet.Clear();
                    }
                }
                if (cntSequence > 3 && Convert.ToInt32(tempSet[0].Name.Substring(0, 2)) == 14 && Convert.ToInt32(tempSet[4].Name.Substring(0, 2)) == 10)
                {
                    Comb = Combination.RoyalFlush;
                    combinationOfCards = tempSet;
                    return;
                }

                else if (cntSequence > 3)
                {
                    Comb = Combination.StraightFlush;
                    combinationOfCards = tempSet;
                    return;
                }

                else if (cntSequence > 2 && Convert.ToInt32(buffSetColor[0].Name.Substring(0, 2)) == 14 && Convert.ToInt32(buffSetColor[buffSetColor.Count - 1].Name.Substring(0, 2)) == 2 && Convert.ToInt32(tempSet[0].Name.Substring(0, 2)) == 5)   // A as 1
                {
                    combinationOfCards.Clear();
                    for (int i = buffSetColor.Count - 4; i < buffSetColor.Count; i++)
                    {
                        combinationOfCards.Add(buffSetColor[i]);
                    }
                    Comb = Combination.StraightFlush;
                    combinationOfCards.Add(buffSetColor[0]);
                    return;
                }
            }

            ///////////////////////////////////////////////// define KARE

            //  define KARE, FULL HOUSE, SET, 2 PAIRS, PAIR
            bool[] massCountOfPair = new bool[6];

            int maxAmount = 0;

            for (int i = 0; i < set.Length - 1; i++)
            {
                int amount = 0;

                for (int j = i + 1; j < set.Length; j++)
                {
                    if (set[i].CompareTo(set[j]) == 0)
                    {
                        amount++;
                        massCountOfPair[i] = true;
                    }
                }
                if (amount > maxAmount)
                {
                    maxAmount = amount;
                }
            }

            if (maxAmount == 3)                                     // if kare
            {
                List<Card> tempKare = new List<Card>();
                Card cardForAdd;
                int indexLastElementOfKare = 0;

                for (int i = 0; i < set.Length; i++)
                {
                    if (set[i].CompareTo(set[3]) == 0)
                    {
                        tempKare.Add(set[i]);
                        indexLastElementOfKare = i;
                    }
                }
                if (set[0].CompareTo(set[3]) > 0)
                {
                    cardForAdd = set[0];
                }
                else
                {
                    cardForAdd = set[indexLastElementOfKare + 1];
                }

                tempKare.Add(cardForAdd);
                combinationOfCards = tempKare;
                Comb = Combination.FourOfAKind;
                return;
            }

            // определение кол-ва пар for full house
            int countOfPair = 0;
            for (int i = 0; i < massCountOfPair.Length; i++)
            {
                if (massCountOfPair[i] == true)
                {
                    countOfPair++;
                }
            }

            ///////////////////////////////////////////////////////////////////// Full House
            if (maxAmount == 2 && countOfPair > 2)
            {
                int cntThree = 0;
                int cntTwo = 0;
                List<Card> cardsThree = new List<Card>();
                List<Card> cardsTwo = new List<Card>();

                for (int i = 0; i < set.Length - 1; i++)
                {
                    if (set[i].CompareTo(set[i + 1]) == 0)
                    {
                        cardsThree.Add(set[i]);
                        cntThree++;
                    }
                    else
                    {
                        cntThree = 0;
                        cardsThree.Clear();
                    }
                    if (cntThree == 2)
                    {
                        cardsThree.Add(set[i + 1]);
                        break;
                    }
                }
                combinationOfCards.AddRange(cardsThree);

                for (int i = 0; i < set.Length - 1; i++)
                {
                    if (set[i].CompareTo(set[i + 1]) == 0 && set[i].CompareTo(cardsThree[0]) != 0)
                    {
                        cardsTwo.Add(set[i]);
                        cntTwo++;
                    }
                    else
                    {
                        cntTwo = 0;
                        cardsTwo.Clear();
                    }
                    if (cntTwo > 0)
                    {
                        cardsTwo.Add(set[i + 1]);
                        break;
                    }
                }
                combinationOfCards.AddRange(cardsTwo);
                Comb = Combination.FullHouse;
                return;
            }

            /////////////////////////////////////////////////////////////////// Флеш
            if (Comb == Combination.Flush)
            {
                return;
            }

            /////////////////////////////////////////////////////////////// Стрит
            int countSequence = 0;
            List<Card> cardsForStreet = new List<Card>();

            for (int i = 0; i < set.Length - 1; i++)
            {
                if (Convert.ToInt32(set[i].Name.Substring(0, 2)) - Convert.ToInt32(set[i + 1].Name.Substring(0, 2)) == 1)
                {
                    countSequence++;
                    cardsForStreet.Add(set[i]);
                    if (countSequence == 4)
                    {
                        cardsForStreet.Add(set[i + 1]);
                        break;
                    }
                }
                else if (Convert.ToInt32(set[i].Name.Substring(0, 2)) - Convert.ToInt32(set[i + 1].Name.Substring(0, 2)) != 0)
                {
                    countSequence = 0;
                    cardsForStreet.Clear();
                }
            }
            if (countSequence > 3)
            {
                Comb = Combination.Straight;
                combinationOfCards = cardsForStreet;
                return;
            }
            else if (countSequence > 2 && Convert.ToInt32(set[0].Name.Substring(0, 2)) == 14 && Convert.ToInt32(set[set.Length - 1].Name.Substring(0, 2)) == 2 && Convert.ToInt32(cardsForStreet[0].Name.Substring(0, 2)) == 5)
            {
                cardsForStreet.Add(set[set.Length - 1]);
                cardsForStreet.Add(set[0]);
                combinationOfCards = cardsForStreet;
                Comb = Combination.Straight;
                return;
            }

            ///////////////////////////////////////////////////////////////////// Сет
            if (maxAmount == 2)
            {
                int cntThree = 0;
                List<Card> cardsThree = new List<Card>();

                for (int i = 0; i < set.Length - 1; i++)
                {
                    if (set[i].CompareTo(set[i + 1]) == 0)
                    {
                        cardsThree.Add(set[i]);
                        cntThree++;
                    }
                    else
                    {
                        cntThree = 0;
                        cardsThree.Clear();
                    }
                    if (cntThree == 2)
                    {
                        cardsThree.Add(set[i + 1]);
                        break;
                    }
                }

                int cnt = 0;
                for (int i = 0; i < set.Length; i++)
                {
                    if (set[i].CompareTo(cardsThree[0]) != 0)
                    {
                        if (cnt == 2)
                        {
                            break;
                        }
                        cardsThree.Add(set[i]);
                        cnt++;
                    }
                }
                combinationOfCards = cardsThree;
                Comb = Combination.ThreeOfAKind;
                return;
            }

            ////////////////////////////////////////////////// Две пары
            if (countOfPair > 1)
            {
                int cntTwo = 0;
                int cntTwo2 = 0;
                List<Card> cardsTwo = new List<Card>();
                List<Card> cardsTwo2 = new List<Card>();

                for (int i = 0; i < set.Length - 1; i++)
                {
                    if (set[i].CompareTo(set[i + 1]) == 0)
                    {
                        cardsTwo.Add(set[i]);
                        cntTwo++;
                    }
                    else
                    {
                        cntTwo = 0;
                        cardsTwo.Clear();
                    }
                    if (cntTwo > 0)
                    {
                        cardsTwo.Add(set[i + 1]);
                        break;
                    }
                }
                combinationOfCards.AddRange(cardsTwo);

                for (int i = 0; i < set.Length - 1; i++)
                {
                    if (set[i].CompareTo(set[i + 1]) == 0 && set[i].CompareTo(cardsTwo[0]) != 0)
                    {
                        cardsTwo2.Add(set[i]);
                        cntTwo2++;
                    }
                    else
                    {
                        cntTwo2 = 0;
                        cardsTwo2.Clear();
                    }
                    if (cntTwo2 > 0)
                    {
                        cardsTwo2.Add(set[i + 1]);
                        break;
                    }
                }
                combinationOfCards.AddRange(cardsTwo2);

                int cou = 0;
                for (int i = 0; i < set.Length; i++)
                {
                    if (cou == 1)
                    {
                        break;
                    }
                    if (set[i].CompareTo(cardsTwo[0]) != 0 && set[i].CompareTo(cardsTwo2[0]) != 0)
                    {
                        combinationOfCards.Add(set[i]);
                        cou++;
                    }
                }
                Comb = Combination.TwoPairs;
                return;
            }

            ////////////////////////////////////////////////////////////// Пара
            if (countOfPair > 0)
            {
                int cntTwo = 0;

                List<Card> cardsTwo = new List<Card>();

                for (int i = 0; i < set.Length - 1; i++)
                {
                    if (set[i].CompareTo(set[i + 1]) == 0)
                    {
                        cardsTwo.Add(set[i]);
                        cntTwo++;
                    }
                    else
                    {
                        cntTwo = 0;
                        cardsTwo.Clear();
                    }
                    if (cntTwo > 0)
                    {
                        cardsTwo.Add(set[i + 1]);
                        break;
                    }
                }
                combinationOfCards.AddRange(cardsTwo);

                int cou = 0;
                for (int i = 0; i < set.Length; i++)
                {
                    if (cou == 3)
                    {
                        break;
                    }
                    if (set[i].CompareTo(cardsTwo[0]) != 0)
                    {
                        combinationOfCards.Add(set[i]);
                        cou++;
                    }
                }
                Comb = Combination.Pair;
                return;
            }

            // nothing
            int coun = 0;
            for (int i = 0; i < set.Length; i++)
            {
                if (coun == 5)
                {
                    break;
                }
                combinationOfCards.Add(set[i]);
                coun++;
            }
            Comb = Combination.HightCard;
        }

        public int CompareTo(Player other)
        {
            if (this.Comb > other.Comb)
            {
                return 1;
            }
            if (this.Comb < other.Comb)
            {
                return -1;
            }
            else
            {
                for (int i = 0; i < combinationOfCards.Count; i++)
                {
                    if (this.combinationOfCards[i].CompareTo(other.combinationOfCards[i]) > 0)
                    {
                        return 1;
                    }
                    else if (this.combinationOfCards[i].CompareTo(other.combinationOfCards[i]) < 0)
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }

        public void ShowCombination(Form1 form, List<Card> set)               // эффект при определении комбинации
        {
            if (IsFind(set[0]))
            {
                form.pictureBox1.Location = new Point(form.pictureBox1.Location.X, 203);
            }
            if (IsFind(set[1]))
            {
                form.pictureBox2.Location = new Point(form.pictureBox2.Location.X, 203);
            }
            if (IsFind(set[2]))
            {
                form.pictureBox3.Location = new Point(form.pictureBox3.Location.X, 203);
            }
            if (set.Count > 3)
            {
                if (IsFind(set[3]))
                {
                    form.pictureBox4.Location = new Point(form.pictureBox4.Location.X, 203);
                }
            }
            if (set.Count > 4)
            {
                if (IsFind(set[4]))
                {
                    form.pictureBox5.Location = new Point(form.pictureBox5.Location.X, 203);
                }
            }

            if (Name.Equals("Player"))
            {
                if (IsFind(pairOfCards[0]))
                {
                    form.picturePl1.Location = new Point(form.picturePl1.Location.X, 391);
                }
                if (IsFind(pairOfCards[1]))
                {
                    form.picturePl2.Location = new Point(form.picturePl2.Location.X, 391);
                }
            }
            if (Name.Equals("Computer1"))
            {
                if (IsFind(pairOfCards[0]))
                {
                    form.pictureBot11.Location = new Point(form.pictureBot11.Location.X, 18);
                }
                if (IsFind(pairOfCards[1]))
                {
                    form.pictureBot12.Location = new Point(form.pictureBot12.Location.X, 18);
                }
            }
            if (Name.Equals("Computer2"))
            {
                if (IsFind(pairOfCards[0]))
                {
                    form.pictureBot21.Location = new Point(form.pictureBot21.Location.X, 18);
                }
                if (IsFind(pairOfCards[1]))
                {
                    form.pictureBot22.Location = new Point(form.pictureBot22.Location.X, 18);
                }
            }
        }

        private bool IsFind(Card card)            // есть ли карта в наборе игрока
        {
            foreach (var item in combinationOfCards)
            {
                if (card.Name.Equals(item.Name))
                {
                    return true;
                }
            }
            return false;
        }

        public string AcceptSolution(Form1 frm, int seqOfGame, List<Card> set, ref int rt)  //TODO: Best AI (artificial intelligence)
        {
            Thread.Sleep(500);
            Random rnd = new Random();
            int someSolution = rnd.Next(1, 5);
            int powerOfSolution = someSolution;
            string s = "";

            if (seqOfGame == 0)                          // префлоп
            {
                if (pairOfCards[0].Name.Substring(0, 2).Equals(pairOfCards[1].Name.Substring(0, 2))) // если пара
                {
                    powerOfSolution += 3;
                    if (Convert.ToInt32(pairOfCards[0].Name.Substring(0, 2)) > 10)    // если пара из крупных карт
                    {
                        powerOfSolution += 2;
                    }
                }
                else if (pairOfCards[0].Name.Substring(2, 1).Equals(pairOfCards[1].Name.Substring(2, 1)))  // если одна масть
                {
                    powerOfSolution += 2;
                }
                if (Account + Rate <= rt)
                {
                    Rate = Rate + Account;
                    Account = 0;
                    AllIn = true;
                    s = "all in";
                }
                else if (powerOfSolution > 5)
                {
                    if (Account + Rate > someSolution * 5 + rt)
                    {
                        Account -= (rt - Rate) + someSolution * 5;
                        rt = rt + someSolution * 5;
                        Rate = rt;
                        s = "raise";
                    }
                    else
                    {
                        AllIn = true;
                        Rate = Account + Rate;
                        Account = 0;
                        s = "raise all in";
                        if (Rate > rt)
                        {
                            rt = Rate;
                        }
                    }
                }
                else if (Rate == rt)
                {
                    s = "check";
                }
                else if (powerOfSolution > 2)
                {
                    if (Account + Rate > rt)
                    {
                        Account -= rt - Rate;
                        Rate = rt;
                        s = "call";
                    }
                    else
                    {
                        AllIn = true;
                        Rate = Account + Rate;
                        Account = 0;
                        s = "all in";
                    }
                }
                else
                {
                    if (Name.Equals("Computer1"))
                    {
                        frm.pictureBot11.Image = null;
                        frm.pictureBot11.Refresh();
                        frm.pictureBot12.Image = null;
                        frm.pictureBot12.Refresh();
                    }

                    if (Name.Equals("Computer2"))
                    {
                        frm.pictureBot21.Image = null;
                        frm.pictureBot21.Refresh();
                        frm.pictureBot22.Image = null;
                        frm.pictureBot22.Refresh();
                    }

                    IsLoose = true;
                    s = "fold";
                }

                if (Name.Equals("Computer1"))
                {
                    frm.label2.Text = s;
                    frm.label2.Refresh();
                }
                else if (Name.Equals("Computer2"))
                {
                    frm.label3.Text = s;
                    frm.label3.Refresh();
                }

                ShowRate(frm);

                Thread.Sleep(200);
                return s;
            }
            else                     // если есть общие карты на столе
            {
                DefineCombination(set.ToArray());
                powerOfSolution += (byte)Comb;

                if (Account + Rate <= rt)
                {
                    Rate = Rate + Account;
                    Account = 0;
                    AllIn = true;
                    s = "all in";
                }
                else if (powerOfSolution > 5)  // > 5
                {
                    if (Account + Rate > someSolution * 5 + rt)
                    {
                        Account -= (rt - Rate) + someSolution * 5;
                        rt = rt + someSolution * 5;
                        Rate = rt;
                        s = "raise";
                    }
                    else
                    {
                        AllIn = true;
                        Rate = Account + Rate;
                        Account = 0;
                        s = "raise all in";
                        if (Rate > rt)
                        {
                            rt = Rate;
                        }
                    }
                }
                else if (Rate == rt)
                {
                    s = "check";
                }
                else if (powerOfSolution > 2)   // > 2
                {
                    if (Account + Rate > rt)
                    {
                        Account -= rt - Rate;
                        Rate = rt;
                        s = "call";
                    }
                    else
                    {
                        AllIn = true;
                        Rate = Account + Rate;
                        Account = 0;
                        s = "all in";
                    }
                }
                else
                {
                    IsLoose = true;
                    s = "fold";
                }

                if (Name.Equals("Computer1"))
                {
                    frm.label2.Text = s;
                    frm.label2.Refresh();
                }
                else if (Name.Equals("Computer2"))
                {
                    frm.label3.Text = s;
                    frm.label3.Refresh();
                }

                if (s.Equals("fold"))
                {
                    if (Name.Equals("Computer1"))
                    {
                        frm.pictureBot11.Image = null;
                        frm.pictureBot12.Image = null;
                        frm.pictureBot11.Refresh();
                        frm.pictureBot12.Refresh();
                    }
                    else if (Name.Equals("Computer2"))
                    {

                        frm.pictureBot21.Image = null;
                        frm.pictureBot22.Image = null;
                        frm.pictureBot21.Refresh();
                        frm.pictureBot22.Refresh();

                    }
                }
                ShowRate(frm);

                Thread.Sleep(200);
                return s;
            }
        }

        public void ShowRate(Form1 frm)                       // view rate and account
        {
            if (Name.Equals("Player"))
            {
                if (Rate != 0)
                {
                    frm.lblPlRate.Text = Rate.ToString();
                }

                frm.lblPlRate.Refresh();
                frm.lblPlAcc.Text = Account.ToString();
                frm.lblPlAcc.Refresh();
            }
            else if (Name.Equals("Computer1"))
            {
                if (Rate != 0)
                {
                    frm.lblComp1Rate.Text = Rate.ToString();
                }
                frm.lblComp1Rate.Refresh();
                frm.lblCom1Acc.Text = Account.ToString();
                frm.lblCom1Acc.Refresh();
            }
            else if (Name.Equals("Computer2"))
            {
                if (Rate != 0)
                {
                    frm.lblComp2Rate.Text = Rate.ToString();
                }
                frm.lblComp2Rate.Refresh();
                frm.lblCom2Acc.Text = Account.ToString();
                frm.lblCom2Acc.Refresh();
            }
            Thread.Sleep(100);
        }
    }
}
