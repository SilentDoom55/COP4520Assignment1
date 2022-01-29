using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace COP4520Assignment1
{
    internal class Program
    {
        private static int threadCount = 8;
        public static bool[] primesPara;
        public static void Main()
        {
            // n = 10^8
            int n = 100000000;
            primesPara = new bool[n];

            //initializing all numbers as prime except for evens
            for (int i = 0; i < n; i++)
            {
                if(i % 2 == 0 && i != 2)
                {
                    primesPara[i] = false;
                }
                else
                {
                    primesPara[i] = true;
                }
            }

            SieveController(n);
        }

        public static void SieveController(int n)
        {
            // Initializing variables
            int start = 2;
            Thread[] threads = new Thread[threadCount];
            int range = (n - start) / threadCount;

            // Initializing the segments
            int[] starts = new int[threadCount];
            starts[0] = start;
            int[] ends = new int[threadCount];
            ends[0] = start + range;
            
            // Calculating the segments from 2 to n
            for (int i = 1; i < threadCount; i++)
            {
                starts[i] = ends[i - 1];
                if (start + range > n || (start + range != n && i + 1 == threadCount))
                    range = n - starts[i];
                ends[i] = starts[i] + range;
            }

            // Starting timer
            DateTime startPara = DateTime.Now;

            // Creating and running threads
            for (int i = 0; i < threadCount; i++)
            {
                int tI = i;
                threads[tI] = new Thread(new ThreadStart(() => SegmentedSieve(n, starts[tI], ends[tI])));
                threads[tI].Start();
                if (tI == 7)
                    break;
            }
            // Waiting for all threads to finish
            for (int i = 0; i < threadCount; i++)
                threads[i].Join();

            // Stopping Timer
            TimeSpan diffPara = DateTime.Now - startPara;

            // Printing the results
            BasicParaPrintResults(diffPara.TotalMilliseconds.ToString());
        }

        // Performs a Sieve of Eratosthenes for a specified segment
        public static void SegmentedSieve(int n, int min, int max)
        {
            // Math.Sqrt is O(1)
            int sqrt = (int)Math.Sqrt(max) + 1;

            for(int i = 2; i < n && i < sqrt; i++)
            {
                int temp = i * i;

                // Verifies starting at a number higher than min and fixes off by one errors
                if (temp < min)
                    temp = ((min + i - 1) / i) * i;

                // If the start point and incrementer are both even, skip
                // There will never be a prime and they are already marked as non-prime
                if (i % 2 == 0 && temp % 2 == 0)
                    continue;

                for (int j = temp; j < max; j += i)
                {
                    primesPara[j] = false;
                }
            }
        }

        // Prints the results to primes.txt
        public static void BasicParaPrintResults(String time)
        {
            int i, primeCount = 0, topPrimeCount = 0;
            long primeSum = 0;
            ArrayList topPrimes = new ArrayList();
            for (i = 2; i < primesPara.Length; i++)
            {
                if (primesPara[i] == true)
                {
                    primeCount++;
                    primeSum += i;
                    if (topPrimeCount < 10)
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
            using StreamWriter file = new("primes.txt");
            file.Write("Execution Time: " + time + "ms");
            file.Write("\tTotal Number of Primes: " + primeCount);
            file.Write("\tSum of All Primes Found: " + primeSum);
            file.WriteLine("\nTop 10 Primes: ");
            for (i = 0; i < 10; i++)
                file.Write(topPrimes[i] + " ");
        }
    }
}