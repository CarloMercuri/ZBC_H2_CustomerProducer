using System;
using System.Threading;

namespace ConsumerProducer
{
    internal class Program
    {
        static ProductBuffer Buffer;

        static void Main(string[] args)
        {
            Buffer = new ProductBuffer();

            Console.WriteLine("Select which mode: ");

            Console.WriteLine("1 - Without Monitor etc...");
            Console.WriteLine("2 - With Monitor.");

            int choice = GetUserInputInteger("Make a selection:");

            if (choice == 1)
            {
                RunEager();   
            }
            else if (choice == 2)
            {
                RunWithMonitor();
            }


            Console.ReadKey();
        }

        static void RunWithMonitor()
        {
            Thread producerThread = new Thread(new ThreadStart(ProducerM));
            producerThread.Name = "Producer Monitor Thread";
            producerThread.Start();

            Thread.Sleep(100);

            Thread consumerThread = new Thread(new ThreadStart(ConsumerM));
            consumerThread.Name = "Consumer Monitor Thread";
            consumerThread.Start();
        }

        static void RunEager()
        {
            Thread producerThread = new Thread(new ThreadStart(ProducerE));
            producerThread.Name = "Producer Eager Thread";
            producerThread.Start();

            Thread.Sleep(100);

            Thread consumerThread = new Thread(new ThreadStart(ConsumerE));
            consumerThread.Name = "Consumer Eager Thread";
            consumerThread.Start();
        }

        static void ProducerE()
        {
            while (true)
            {
                if (!Buffer.TryInsertProduct())
                {
                    Console.WriteLine("Producer taking a break.");
                    Thread.Sleep(3000);
                }
                else
                {
                    Console.WriteLine($"Producer inserted a product. Products on buffer :{Buffer.CountProductsRemaining()}");
                }

                Thread.Sleep(100/15);
            }
        }

        static void ConsumerE()
        {
            while (true)
            {
                if (Buffer.TryGetProduct(out Product product))
                {
                    Console.WriteLine($"Customer retreived a product. Products on buffer: {Buffer.CountProductsRemaining()}");
                }
                else
                {
                    Console.WriteLine("Buffer empty. Customer taking a break.");
                    Thread.Sleep(3000);
                }

                Thread.Sleep(1000);
            }
        }

        static void ProducerM()
        {
            while (true)
            {
                if (Monitor.TryEnter(Buffer, -1))
                {
                    while (Buffer.TryInsertProduct())
                    {
                        Console.WriteLine($"Producer inserted product. Items on buffer: {Buffer.CountProductsRemaining()}");
                    }

                    // Now it's full.

                    Console.WriteLine("Producer waiting now.");

                    Monitor.Pulse(Buffer);

                    Monitor.Exit(Buffer);


                    Thread.Sleep(3000);
                }
            }
        }

        static void ConsumerM()
        {
            while (true)
            {
                if (Monitor.TryEnter(Buffer, -1))
                {
                    while (Buffer.TryGetProduct(out Product product))
                    {
                        Console.WriteLine($"Customer retreived a product. Items on buffer: {Buffer.CountProductsRemaining()}");
                    }

                    // Now it's full.
                    Console.WriteLine("Consumer waiting now.");

                    Monitor.Pulse(Buffer);

                    Monitor.Exit(Buffer);

                    Thread.Sleep(3000);
                }
            }
        }

        /// <summary>
        /// Requests the user to enter an integer with the corresponding request string, and
        /// makes sure the input is sanitized
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        static int GetUserInputInteger(string phrase = "")
        {
            string userInput = "";

            while (true)
            {
                if (phrase != "")
                {
                    Console.WriteLine(phrase);
                }

                userInput = Console.ReadLine();

                // Empty input (only pressed enter for example)
                if (userInput.Length <= 0)
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }

                // Check that it only contains numbers
                if (!IsInputOnlyDigits(userInput))
                {
                    Console.WriteLine("Invalid input: must only contain numbers");
                    continue;
                }
                else
                {
                    break;
                }
            }

            return int.Parse(userInput);
        }

        /// <summary>
        /// Returns true if the string only contains digits
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static bool IsInputOnlyDigits(string input)
        {
            foreach (char c in input)
            {
                // check that it's a number (unicode)
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
