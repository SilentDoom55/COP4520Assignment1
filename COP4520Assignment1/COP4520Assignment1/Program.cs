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
        private static int threadCount = 1;
        public static List<int> parallelPrimes = new List<int>();
        public static void Main()
        {
            int n;
            String input;
            Console.WriteLine("Please input N");
            input = Console.ReadLine();
            if (input != null)
            {
                n = Int32.Parse(input);
                Console.WriteLine("Serial:");
                SerialSieveController(n);
                Console.WriteLine("Parallel:");
                ParallelSieveController(n);
            }
            else
            {
                Console.WriteLine("Invalid input, restart the program");
            }
        }

        public static void ParallelSieveController(int n)
        {
            DateTime parstart = DateTime.Now;
            ParallelSieve(n);
            TimeSpan pardiff = DateTime.Now - parstart;
            ParallelPrintResults(pardiff.TotalMilliseconds.ToString());
        }

        public static void ParallelSieve(int n)
        {
            Program program = new Program();
            int start = 2;
            Thread[] threads = new Thread[threadCount];
            int range = (n - start) / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                int startl = start;
                threads[i] = new Thread(new ThreadStart(() => GeneratePrimes(startl, range)));
                start += range;
                threads[i].Start();
            }
            for (int i = 0; i < threadCount; i++)
                threads[i].Join();
        }

        public static void GeneratePrimes( int start, int range)
        {
            bool isPrime = true;
            int end = start + range;
            for (int i = start; i <= end; i++)
            {
                for (int j = 2; j <= end; j++)
                {
                    if (i != j && i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    parallelPrimes.Add(i);
                }
                isPrime = true;
            }
        }

        public static void ParallelPrintResults(String time)
        {
            Console.WriteLine("Execution Time: " + time);
            Console.WriteLine("Total Number of Primes: " + parallelPrimes.Count);
            Console.WriteLine("Sum of All Primes Found: " + parallelPrimes.Sum());
            Console.WriteLine("Top 10 Primes: ");
            parallelPrimes.Sort();
            for (int i = parallelPrimes.Count - 10; i < parallelPrimes.Count; i++)
                Console.Write(parallelPrimes[i] + " ");
        }

        public static void SerialSieveController(int n)
        {
            DateTime start = DateTime.Now;
            bool[] temp = SerialSieve(n);
            TimeSpan diff = DateTime.Now - start;
            SerialPrintResults(temp, diff.TotalMilliseconds.ToString());
        }

        // Taken from the pseudo code at https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes
        public static bool[] SerialSieve(int n)
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
        public static void SerialPrintResults(bool[] list, String time)
        {
            SerialPrintResults(list, time, 0, 0, 0);
        }
        // Prints the results in the format requested
        public static void SerialPrintResults(bool[] list, String time, int primeCount, int topPrimeCount, int primeSum)
        {
            int i;
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
    }
    
}
