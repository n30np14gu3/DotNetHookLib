using System;
using System.Threading;
using ExampleLib;

namespace ExampleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExampleClass2Hook example = new ExampleClass2Hook("Ivan");

            for (int i = 0;; i++)
            {
                Console.WriteLine(example.GetHello());
                example.UpdateName($"Ivan_{i}");
                Thread.Sleep(1000);
            }
        }
    }
}
