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
        private static int threadCount = 8;
        public static ArrayList parPrimesTemp = new ArrayList();
        public static ArrayList parallelPrimes = ArrayList.Synchronized(parPrimesTemp);
        public static bool[] sieve;
        private static readonly object sieveLock = new object();
        public static long totalPrimes = 0;
        public static long sumPrimes = 0;
        public static Thread[] aThreads;
        public static void Main()
        {
            int n;
            String input;
            Console.WriteLine("Please input N");
            input = Console.ReadLine();
            if (input != null)
            {
                n = Int32.Parse(input);
                sieve = new bool[n];
                //Console.WriteLine("\nParallel (8):");
                //ParallelController(n);

                //threadCount = 1;\
                //parallelPrimes.Clear();
                //Console.WriteLine("\nParallel (1):");
                //ParallelController(n);

                Console.WriteLine("\nSeries Sieve:");
                SeriesSieveController(n);

                Console.WriteLine("\nParallel Sieve: ");
                AtkinController(n);
            }
            else
            {
                Console.WriteLine("Invalid input, restart the program");
            }
        }

        public static void ParallelController(int n)
        {
            DateTime parstart = DateTime.Now;
            Parallel(n);
            TimeSpan pardiff = DateTime.Now - parstart;
            ParallelPrintResults(pardiff.TotalMilliseconds.ToString());
        }

        public static void Parallel(int n)
        {
            int start = 2;
            Thread[] threads = new Thread[threadCount];
            int range = (n - start) / threadCount;
            for (int i = 0; i < threadCount; i++)
            {
                int tStart = start;
                if (tStart + range > n || (tStart + range != n && i + 1 == threadCount))
                    range = n - tStart;
                int tRange = range;
                int tI = i;
                threads[i] = new Thread(new ThreadStart(() => GeneratePrimes(tStart, tRange, tI)));
                //Console.WriteLine("Thread Started: " + i + " From " + start + " to " + (start + range));
                start += range;
                threads[i].Start();
            }
            for (int i = 0; i < threadCount; i++)
                threads[i].Join();
        }

        public static void GeneratePrimes(int start, int range, int thread)
        {
            bool isPrime = true;
            int end = start + range;
           
            for (int i = start; i <= end; i++)
            {
                for (int j = 2; j <= i; j++)
                {
                    if (i != j && i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    //Console.WriteLine("Thread " + thread + " added " + i);
                    if(parallelPrimes.IndexOf(i) == -1)
                        parallelPrimes.Add(i);
                }
                isPrime = true;
            }
        }

        public static void ParallelPrintResults(String time)
        {
            int sum = 0;
            Console.WriteLine("Execution Time: " + time);
            Console.WriteLine("Total Number of Primes: " + parallelPrimes.Count);
            foreach (int i in parallelPrimes)
                sum += i;
            Console.WriteLine("Sum of All Primes Found: " + sum);
            Console.WriteLine("Top 10 Primes: ");
            parallelPrimes.Sort();
            for (int i = parallelPrimes.Count - 10; i < parallelPrimes.Count; i++)
            {
                if (i < 0)
                    i = 0;
                Console.Write(parallelPrimes[i] + " ");
            }
                
            Console.Write("\n");
        }

        public static void SeriesSieveController(int n)
        {
            DateTime start = DateTime.Now;
            bool[] temp = SeriesSieve(n);
            TimeSpan diff = DateTime.Now - start;
            SeriesPrintResults(temp, diff.TotalMilliseconds.ToString());
            //Compare(temp);
        }

        // Taken from the pseudo code at https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes
        public static bool[] SeriesSieve(int n)
        {
            bool[] list = new bool[n];
            int i, j, sqrtN = (int)Math.Sqrt((double)n) + 1;
            for (i = 0; i < n; i++)
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

        // Prints the results in the format requested
        public static void SeriesPrintResults(bool[] list, String time)
        {
            int i, primeCount = 0, topPrimeCount = 0;
            long primeSum = 0;
            DateTime start = DateTime.Now;
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
            TimeSpan end = DateTime.Now - start;
            double tempEnd = Double.Parse(end.TotalMilliseconds.ToString()); 
            double tempTime = Double.Parse(time) + tempEnd;
            Console.WriteLine("Execution Time: " + tempTime);
            Console.WriteLine("Total Number of Primes: " + primeCount);
            Console.WriteLine("Sum of All Primes Found: " + primeSum);
            Console.WriteLine("Top 10 Primes: ");
            for (i = 0; i < 10; i++)
                Console.Write(topPrimes[i] + " ");
            Console.Write("\n");
        }

        public static void Compare(bool[] list)
        {
            Console.WriteLine("Beginning Compare");
            int j = 0;
            for(int i = 2; i < list.Count(); i++)
            {
                if(list[i])
                {
                    Console.WriteLine(i + " " + parallelPrimes[j]);
                    j++;
                    if (j == parallelPrimes.Count)
                        break;
                }
            }
        }
        
        public static void AtkinController(int n)
        {
            aThreads = new Thread[threadCount];
            sieve[2] = true;
            sieve[3] = true;
            sieve[5] = true;
            DateTime start = DateTime.Now;
            for (int i = 0; i < threadCount; i++)
            {
                aThreads[i] = new Thread(new ThreadStart(() => AtkinSieve(n, i)));
                aThreads[i].Start();
            }
            TimeSpan end = DateTime.Now - start;
            AtkinPrintResults(end.TotalMilliseconds.ToString());
        }

        //Sieve of Atkin
        public static void AtkinSieve(int n, int threadNum)
        {
            for (int x = 1 + threadNum; x * x < n; x += threadCount)
            {
                for (int y = 1; y * y < n; y++)
                {
                    int u = (4 * x * x) + (y * y);
                    if (u <= n && (u % 12 == 1 || u % 12 == 5))
                    {
                        lock (sieveLock)
                        {
                            sieve[u] = true;
                        }
                    }

                    u = (3 * x * x) + (y * y);
                    if (u <= n && u % 12 == 7)
                    {
                        lock (sieveLock)
                        {
                            sieve[u] = true;
                        }
                    }

                    u = (3 * x * x) - (y * y);
                    if (x > y && u <= n && u % 12 == 11)
                    {
                        lock (sieveLock)
                        {
                            sieve[u] = true;
                        }
                    }
                }
            }

            for (int r = 5 + threadNum; r * r < n; r += threadCount)
            {
                lock (sieveLock)
                {
                    if (sieve[r])
                    {
                        for (int i = r * r; i < n; i += r * r)
                        {
                            if (sieve[i] == true)
                            {
                                sieve[i] = false;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < threadCount; i++)
                aThreads[i].Join();
        }
        public static void AtkinPrintResults(String time)
        {
            int i, primeCount = 0, topPrimeCount = 0;
            long primeSum = 0;
            ArrayList topPrimes = new ArrayList();
            for (i = 2; i < sieve.Length; i++)
            {
                if (sieve[i] == true)
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
            for (i = topPrimes.Count - 10; i < topPrimes.Count; i++)
            {
                if (i < 0)
                    i = 0;
                Console.Write(topPrimes[i] + " ");
            }
            Console.Write("\n");
        }
    }
}