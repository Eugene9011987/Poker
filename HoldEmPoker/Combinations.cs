using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldEmPoker
{
    enum Combination : byte
    {
        HightCard,
        Pair,
        TwoPairs,            
        ThreeOfAKind,              
        Straight,            
        Flush,             
        FullHouse,
        FourOfAKind,               
        StraightFlush,       
        RoyalFlush    
    }
}
