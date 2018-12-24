using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Misc
{
    public class UnrepeatedRandom
    {
        List<int> Numbers = new List<int>();
        Random rand;

        int from = 0, to = 0;
        public UnrepeatedRandom()
        {
                
        }

        public UnrepeatedRandom(int from,int to)
        {
            this.from = from;
            this.to = to;
            refill(from, to);
        }

        public void Update(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public int Next()
        {
            if (Numbers.Count == 0)
            {
                refill(from, to);
            }
            int randIndex = rand.Next(0, Numbers.Count);
            int randResult = Numbers[randIndex];
            Numbers.RemoveAt(randIndex);
            return randResult;
        }

        private void refill(int from, int to)
        {
            rand = new Random();
            Numbers.Clear();
            for (int i = from; i < to; i++)
            {
                Numbers.Add(i);
            }
        }

    }
}
