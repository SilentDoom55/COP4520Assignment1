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
            String input;
            Console.WriteLine("Please input N");
            input = Console.ReadLine();
            if (input != null)
            {
                n = Int32.Parse(input);
                DateTime start = DateTime.Now;
                bool[] temp = Sieve(n);
                TimeSpan diff = DateTime.Now - start;
                PrintResults(temp, diff.TotalMilliseconds.ToString());
            }
            else
            {
                Console.WriteLine("Invalid input, restart the program");
            }
        }

        // Taken from the pseudo code at https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes
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

        //Overloaded so that the additional variables can be input at a later time
        public static void PrintResults(bool[] list, String time)
        {
            PrintResults(list, time, 0, 0, 0);
        }
        // Prints the results in the format requested
        public static void PrintResults(bool[] list, String time, int primeCount, int topPrimeCount, int primeSum)
        {
            int i;
            ArrayList topPrimes = new ArrayList();
            for(i = 2; i < list.Length; i++)
            {
                if(list[i] == true)
                {
                    primeCount++;
                    primeSum += i;
                    if(topPrimeCount < 10)
                    {
                        topPrimeCount++;
                        topPrimes.Add(i);
                    }
                    else
                    {
                        topPrimes.RemoveAt(0);
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
