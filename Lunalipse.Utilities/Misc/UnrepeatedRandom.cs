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
        int currentIndex = 0;

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
            refill(from, to);
        }

        public int Next()
        {
            currentIndex++;
            if (currentIndex == Numbers.Count)
            {
                Shuffle(Numbers, rand);
                currentIndex = 0;
            }
            int randResult = Numbers[currentIndex];
            return randResult;
        }

        public int Previous()
        {
            currentIndex--;
            if (currentIndex == 0)
            {
                currentIndex = Numbers.Count - 1;
            }
            int randResult = Numbers[currentIndex];
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
            Shuffle(Numbers, rand);
        }

        public void Shuffle<T>(IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
