using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldEmPoker
{
    class Card : IComparable<Card>
    {
        private string name;

        public string Name                  // Example: "02b" - 2 bubna
        {
            get { return name; }
            set { name = value; }
        }

        public Card(string nm)
        {
            Name = nm;
        }

        public int CompareTo(Card other)
        {
            return Convert.ToInt32(name.Substring(0, 2)).CompareTo(Convert.ToInt32(other.name.Substring(0, 2)));
        }
    }
}
