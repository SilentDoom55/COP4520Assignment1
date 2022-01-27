using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace COP4520Assignment1
{
    internal class Program
    {
        public static void Main()
        {
            int n;
            Console.WriteLine("Please input N");
            n = Int32.Parse(Console.ReadLine());
            PrintResults(Sieve(n), 10.0);
        }

        public static bool[] Sieve(int n)
        {
            bool[] list = new bool[n];
            int i, j, sqrtN = (int)Math.Sqrt((double)n) + 1;
            for (i = 0; i < n - 2; i++)
                list[i] = true;

            for(i = 2; i < sqrtN; i++)
            {
                if(list[i] == true)
                {
                    for(j = i*i; j < n; j += i)
                    {
                        list[j] = false;
                    }
                }
            }
            return list;
        }

        public static void PrintResults(bool[] list, double time)
        {
            int i, j, primeCount = 0, topPrimeCount = 0, primeSum = 0;
            ArrayList topPrimes = new ArrayList();
            for(i = 2; i < list.Length; i++)
            {
                if(list[i] == true)
                {
                    primeCount++;
                    primeSum += i;
                    if(topPrimeCount < 10)
                    {
                        topPrimes.Add(i);
                    }
                    else
                    {
                        topPrimes.Remove(0);
                        topPrimes.Add(i);
                    }
                }
            }

            Console.WriteLine("Execution Time: " + time);
            Console.WriteLine("Total Number of Primes: " + primeCount);
            Console.WriteLine("Sum of All Primes Found: " + primeSum);
            Console.WriteLine("Top 10 Primes: ");
            for (i = 0; i < 10; i++)
                Console.Write(topPrimes[i] + " ");
        }
    }
}
