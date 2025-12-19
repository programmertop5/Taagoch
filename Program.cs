using System;
using System.Threading;

namespace Tamagotchi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Tamagotchi Game";

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("         Ласкаво просимо до гри         ");
            Console.WriteLine("             TAMAGOTCHI!                ");
            Console.WriteLine("---------------------------------------");
            Console.Write("\nВведіть ім'я для вашого тамагочі: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
                name = "Тамагочі";

            Tamagoch t = new Tamagoch(name);

            t.ThisRequest += delegate (string message) { };

            t.StatusChanged += delegate (string message) { };

            t.Death += delegate (string message) { };

            t.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
