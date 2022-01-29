using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace COP4520Assignment1
{
    internal class Program
    {
        private static int threadCount = 8;
        public static bool[] primesSeries;
        public static bool[] primesPara;
        public static void Main()
        {
            int n;
            String input;
            Console.WriteLine("Please input N");
            input = Console.ReadLine();
            if (input != null)
            {
                n = Int32.Parse(input);
                primesSeries = new bool[n];
                primesPara = new bool[n];
                for (int i = 0; i < n; i++)
                {
                    if(i % 2 == 0 && i != 2)
                    {
                        primesSeries[i] = false;
                        primesPara[i] = false;
                    }
                    else
                    {
                        primesSeries[i] = true;
                        primesPara[i] = true;
                    }
                }
                BasicSieveController(n);
                //\\Compare();
            }
            else
            {
                Console.WriteLine("Invalid input, restart the program");
            }
        }

        public static void BasicSieveController(int n)
        {
            DateTime startSeries = DateTime.Now;
            SeriesSieve(n);
            TimeSpan diffSeries = DateTime.Now - startSeries;
            BasicSeriesPrintResults(diffSeries.TotalMilliseconds.ToString());

            int start = 2;
            Thread[] threads = new Thread[threadCount];
            int range = (n - start) / threadCount;

            int[] starts = new int[threadCount];
            starts[0] = start;
            int[] ends = new int[threadCount];
            ends[0] = start + range;
            for (int i = 1; i < threadCount; i++)
            {
                starts[i] = ends[i - 1];
                if (start + range > n || (start + range != n && i + 1 == threadCount))
                    range = n - starts[i];
                ends[i] = starts[i] + range;
            }
            DateTime startPara = DateTime.Now;
            for (int i = 0; i < threadCount; i++)
            {
                int tI = i;
                //Console.WriteLine("Thread Started: " + i + " From " + starts[tI] + " to " + ends[tI]);
                threads[tI] = new Thread(new ThreadStart(() => SegmentedSieve(n, starts[tI], ends[tI], tI)));
                threads[tI].Start();
                if (tI == 7)
                    break;
            }
            for (int i = 0; i < threadCount; i++)
                threads[i].Join();
            TimeSpan diffPara = DateTime.Now - startPara;
            BasicParaPrintResults(diffPara.TotalMilliseconds.ToString());
        }

        public static void SeriesSieve(int n)
        {
            int sqrt = (int)Math.Sqrt(n);
            for(int i = 2; i <= sqrt; i++)
            {
                if(primesSeries[i])
                {
                    for(int j = i * 2; j < n; j += i)
                    {
                        primesSeries[j] = false;
                    }
                }
            }
        }

        public static void SegmentedSieve(int n, int min, int max, int thread)
        {
            //Console.WriteLine("Thread: " + thread + "  min: " + min + "  max: " + max);
            int sqrt = (int)Math.Sqrt(max) + 1;
            for(int i = 2; i < n && i < sqrt; i++)
            {
                int temp = i * i;

                if (temp < min)
                    temp = ((min + i - 1) / i) * i;

                for (int j = temp; j < max; j += i)
                {
                    primesPara[j] = false;
                }
            }
            Console.WriteLine("Completed Thread : " + thread);
        }

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
            Console.WriteLine("Execution Time: " + time);
            Console.WriteLine("Total Number of Primes: " + primeCount);
            Console.WriteLine("Sum of All Primes Found: " + primeSum);
            Console.WriteLine("Top 10 Primes: ");
            for (i = 0; i < 10; i++)
                Console.Write(topPrimes[i] + " ");
            Console.Write("\n");
        }
        public static void BasicSeriesPrintResults(String time)
        {
            int i, primeCount = 0, topPrimeCount = 0;
            long primeSum = 0;
            ArrayList topPrimes = new ArrayList();
            for (i = 2; i < primesSeries.Length; i++)
            {
                if (primesSeries[i] == true)
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
            Console.WriteLine("Execution Time: " + time);
            Console.WriteLine("Total Number of Primes: " + primeCount);
            Console.WriteLine("Sum of All Primes Found: " + primeSum);
            Console.WriteLine("Top 10 Primes: ");
            for (i = 0; i < 10; i++)
                Console.Write(topPrimes[i] + " ");
            Console.Write("\n");
        }
        public static void Compare()
        {
            for (int i = 0; i < 100000000; i++)
            {
                if (primesPara[i] != primesSeries[i])
                    Console.WriteLine("ERROR AT: " + i);
            }
        }
    }
}