using System;
using System.Timers;
using System.Threading;

namespace Tamagotchi
{
    public delegate void TamagotchiEvent(string message);

    internal class Tamagoch
    {
        private const int Feed = 0;
        private const int Walk = 1;
        private const int Sleep = 2;
        private const int Heal = 3;
        private const int Play = 4;

        private const int Healthy = 0;
        private const int Sick = 1;
        private const int Dead = 2;

        private System.Timers.Timer requestTimer;
        private System.Timers.Timer lifeTimer;
        private Random random;
        private int previousRequest;
        private int ignorCount;
        private int status;
        private int currentRequest;
        private const int MAX_IGNORED = 3;
        private bool hasPreviousRequest;
        private bool waitingForAnswer;

        public string Name { get; private set; }
        public event TamagotchiEvent ThisRequest;
        public event TamagotchiEvent StatusChanged;
        public event TamagotchiEvent Death;

        public Tamagoch(string name)
        {
            Name = name;
            random = new Random();
            ignorCount = 0;
            status = Healthy;
            hasPreviousRequest = false;
            previousRequest = -1;
            waitingForAnswer = false;

            requestTimer = new System.Timers.Timer(GetRandomInterval(10000, 11000));
            requestTimer.Elapsed += MakeRequest;
            requestTimer.AutoReset = true;


            lifeTimer = new System.Timers.Timer(GetRandomInterval(60000, 120000));
            lifeTimer.Elapsed += LifeEnd;
            lifeTimer.AutoReset = false;
        }

        public void Start()
        {
            if (status == Dead) { return}
            ;

            Console.Clear();
            DrawTamagotchi();
            Console.WriteLine($"\n{Name} \"born! Take care of him!");
            Console.WriteLine("\nWaiting for the first request...");

            requestTimer.Start();
            lifeTimer.Start();
        }

        private int GetRandomInterval(int min, int max)
        {
            return random.Next(min, max);
        }

        private void MakeRequest(object sender, ElapsedEventArgs e)
        {
            if (status == Dead || waitingForAnswer) { return}
            ;

            waitingForAnswer = true;

            if (status == Sick)
            {
                currentRequest = Heal;
                ShowRequest("I feel sick! Treat me!");
                return;
            }

            int newRequest;
            do
            {
                newRequest = random.Next(0, 5);
            } while (hasPreviousRequest && newRequest == previousRequest);

            previousRequest = newRequest;
            hasPreviousRequest = true;
            currentRequest = newRequest;

            string message = GetRequestMessage(newRequest);
            ShowRequest(message);
        }

        private string GetRequestMessage(int requestType)
        {
            if (requestType == Feed)
                return "I'm hungry! Feed me!";
            else if (requestType == Walk)
                return "I want to go for a walk!";
            else if (requestType == Sleep)
                return "I'm tired! Put me to sleep!";
            else if (requestType == Play)
                return "I'm bored! Play with me!";
            else if (requestType == Heal)
                return "I feel sick! Heal me!";
            else
                return "Pay attention to me!";
        }


        private void ShowRequest(string message)
        {
            if (ThisRequest != null)
                ThisRequest(message);

            DrawTamagotchi();
            Console.WriteLine($"\n{Name} каже: {message}");
            Console.WriteLine("\n Do the request? (y/n): ");


            string answer = Console.ReadLine();

            bool satisfied = answer == "y" || answer == "так";

            HandleRequest(satisfied);
            waitingForAnswer = false;

        }

        private void HandleRequest(bool satisfied)
        {
            if (satisfied)
            {
                ignorCount = 0;

                if (status == Sick && currentRequest == Heal)
                {
                    status = Healthy;
                    DrawTamagotchi();
                    Console.WriteLine($"\n{Name} recovered! ");
                    if (StatusChanged != null)
                        StatusChanged($"{Name} recovered!");
                }
                else if (status == Healthy)
                {
                    DrawTamagotchi();
                    string response = GetResponseMessage(currentRequest);
                    Console.WriteLine($"\n{Name}: {response}");
                }
            }
            else
            {
                ignorCount++;
                DrawTamagotchi();
                Console.WriteLine($"\n{Name} sorry.. (Ignor: {ignorCount}/{MAX_IGNORED})");

                if (ignorCount >= MAX_IGNORED && status == Healthy)
                {
                    status = Sick;
                    DrawTamagotchi();
                    Console.WriteLine($"\n{Name} sick! ");
                    if (StatusChanged != null)
                        StatusChanged($"{Name} sick!");

                    requestTimer.Stop();
                    Thread.Sleep(2000);
                    waitingForAnswer = false;
                    MakeRequest(null, null);
                    requestTimer.Start();
                }
                else if (status == Sick && currentRequest == Heal)
                {
                    Die();
                }
            }

            Console.WriteLine("\nWaiting for the next request..");
        }

        private string GetResponseMessage(int requestType)
        {
            if (requestType == Feed)
                return "Very tasty! Thank you! ";
            else if (requestType == Walk)
                return "How wonderful! Thank you! ";
            else if (requestType == Sleep)
                return "Thank you for putting me to bed";
            else if (requestType == Play)
                return "Fun! Thank you! ";
            else
                return "Thank you! ";
        }

        private void LifeEnd(object sender, ElapsedEventArgs e)
        {
            if (status != Dead)
            {
                requestTimer.Stop();
                DrawTamagotchi();
                Console.WriteLine($"\n{Name} lived a long and happy life! :) ");
                Console.WriteLine("\nGame over");
                status = Dead;
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private void Die()
        {
            status = Dead;
            requestTimer.Stop();
            lifeTimer.Stop();

            DrawTamagotchi();
            Console.WriteLine($"\n{Name} died :(");
            if (Death != null)
                Death($"{Name} помер");

            Console.WriteLine($"\n{Name} died due to lack of care...");
            Console.WriteLine("Game over");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private void DrawTamagotchi()
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("          TAMAGOTCHI: {Name} ");
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();

            if (status == Healthy)
            {
                Console.WriteLine("|            ///                      |");
                Console.WriteLine("|           (0‿0)                    |");
                Console.WriteLine("|         // | | \\                   |");
                Console.WriteLine("|            ---                  |");
                Console.WriteLine("|          //   \\                    |");
                Console.WriteLine("|                                    | ");
                Console.WriteLine("|        Status: Healthy              |");
            }
            else if (status == Sick)
            {
                Console.WriteLine("|             |||                        |");
                Console.WriteLine("|            (-_-)                       |");
                Console.WriteLine("|          // | | \\                     |");
                Console.WriteLine("|             ---                         |");
                Console.WriteLine("|           //   \\                      |");
                Console.WriteLine("|                                        |");
                Console.WriteLine("|        Status: Sick                   |");
            }
            else if (status == Dead)
            {
                Console.WriteLine("|             ///                        |");
                Console.WriteLine("|            (x_x)                       |");
                Console.WriteLine("|           / | | \\                     |");
                Console.WriteLine("|             ---                        |");
                Console.WriteLine("|           /    \\                       |");
                Console.WriteLine("|                                        |");
                Console.WriteLine("|        Status: Died                |");
            }

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"    Ігнорувань: {ignorCount}/{MAX_IGNORED} ");
            Console.ResetColor();
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();
        }
    }
};
