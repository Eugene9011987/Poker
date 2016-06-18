using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace HoldEmPoker
{
    class StackOfCards
    {
        List<Card> stackLOfCards;

        int count;

        public StackOfCards()
        {
            count = 52;                                 // amount of cards
            stackLOfCards = new List<Card>(52);
            stackLOfCards.Add(new Card("02c"));         // c - cirva
            stackLOfCards.Add(new Card("02b"));         // b - bubna
            stackLOfCards.Add(new Card("02t"));         // t - tref
            stackLOfCards.Add(new Card("02p"));         // p - piki

            stackLOfCards.Add(new Card("03c"));
            stackLOfCards.Add(new Card("03b"));
            stackLOfCards.Add(new Card("03t"));
            stackLOfCards.Add(new Card("03p"));

            stackLOfCards.Add(new Card("04c"));
            stackLOfCards.Add(new Card("04b"));
            stackLOfCards.Add(new Card("04t"));
            stackLOfCards.Add(new Card("04p"));

            stackLOfCards.Add(new Card("05c"));
            stackLOfCards.Add(new Card("05b"));
            stackLOfCards.Add(new Card("05t"));
            stackLOfCards.Add(new Card("05p"));

            stackLOfCards.Add(new Card("06c"));
            stackLOfCards.Add(new Card("06b"));
            stackLOfCards.Add(new Card("06t"));
            stackLOfCards.Add(new Card("06p"));

            stackLOfCards.Add(new Card("07c"));
            stackLOfCards.Add(new Card("07b"));
            stackLOfCards.Add(new Card("07t"));
            stackLOfCards.Add(new Card("07p"));

            stackLOfCards.Add(new Card("08c"));
            stackLOfCards.Add(new Card("08b"));
            stackLOfCards.Add(new Card("08t"));
            stackLOfCards.Add(new Card("08p"));

            stackLOfCards.Add(new Card("09c"));
            stackLOfCards.Add(new Card("09b"));
            stackLOfCards.Add(new Card("09t"));
            stackLOfCards.Add(new Card("09p"));

            stackLOfCards.Add(new Card("10c"));
            stackLOfCards.Add(new Card("10b"));
            stackLOfCards.Add(new Card("10t"));
            stackLOfCards.Add(new Card("10p"));

            stackLOfCards.Add(new Card("11c"));         // valet
            stackLOfCards.Add(new Card("11b"));
            stackLOfCards.Add(new Card("11t"));
            stackLOfCards.Add(new Card("11p"));

            stackLOfCards.Add(new Card("12c"));          // qeen
            stackLOfCards.Add(new Card("12b"));
            stackLOfCards.Add(new Card("12t"));
            stackLOfCards.Add(new Card("12p"));

            stackLOfCards.Add(new Card("13c"));         // king
            stackLOfCards.Add(new Card("13b"));
            stackLOfCards.Add(new Card("13t"));
            stackLOfCards.Add(new Card("13p"));

            stackLOfCards.Add(new Card("14c"));         // tuz
            stackLOfCards.Add(new Card("14b"));
            stackLOfCards.Add(new Card("14t"));
            stackLOfCards.Add(new Card("14p"));
        }

        public Card TakeCard()                          // take card from stack
        {
            Random rnd = new Random();
            int r = rnd.Next(0, count);
            Thread.Sleep(300);

            Card temp = stackLOfCards[r];

            stackLOfCards.Remove(temp);
            count--;

            return temp;
        }
    }
}
