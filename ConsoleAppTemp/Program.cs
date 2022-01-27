using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace ConsoleAppTemp
{

    class  Shooter
    {
        public string name = "Underfined";
        public int trials ;
        public Shooter() { trials = 5;  }

    }

    class Line
    {
        public string name = "Underfined";
        public List<Shooter> shooter;

        public void Act0(List<Line> lines, int i)   //Первые в ряду
        {
            Console.WriteLine(lines[i].name + ", " + lines[i].shooter[0].name + ": Занял направление!");
        }

        public void Act1(List<Line> lines, int i)   //Следующие в ряду
        {
            
            Thread t = Thread.CurrentThread;
            Console.WriteLine(lines[i].name + ", " + t.Name + ", " + lines[i].shooter[0].name + ": Занял направление!");
            
        }

        public void Act2(List<Line> lines, int i)
        {

            Thread t = Thread.CurrentThread;
            Console.WriteLine(lines[i].name + ", " + t.Name  + ": Подготовиться к стрельбе!");

        }

        public void Act3(List<Line> lines, int i)
        {

            Thread t = Thread.CurrentThread;
            Console.WriteLine(lines[i].name + ", " + t.Name + ", " + lines[i].shooter[0].name + ": К стрельбе готов!");

        }

        public void Act4(List<Line> lines, int i)
        {

            Thread t = Thread.CurrentThread;
            Console.WriteLine(lines[i].name + ", " + t.Name + ": Произвести стрельбу!");

        }

        public List<Line> Act5(List<Line> lines, int i)
        {

            Thread t = Thread.CurrentThread;
            Console.WriteLine(lines[i].name + ", " + t.Name + ", " + lines[i].shooter[0].name + ": Стрельбу закончил!");
            lines[i].shooter[0].trials = lines[i].shooter[0].trials - 1;
            lines[i].shooter.Add(lines[i].shooter[0]);
            lines[i].shooter.RemoveAt(0);

            return lines;
        }



    }

    

    class Program
    {
        static int secondsCount = 0;   //Общее время
        static int secondsCombatCount1 = 0; //Время стрельбы с первым инструктором
        static int secondsCombatCount2 = 0;  //Время стрельбы с втором инструктором

        //Второй поток
        public static void SecondInstructorControl(object x)
        {
            System.Timers.Timer timer2 = new System.Timers.Timer(1000);     //Таймер для 2 инструктора
            timer2.Elapsed += Tick2;
            timer2.Enabled = true;
            timer2.AutoReset = true;

            Random random = new Random();
            Thread t = Thread.CurrentThread;
            t.Name = "Инструктор 2";
            
            var lines = (List<Line>)x ;
            bool[] finishes = new bool[lines.Count];    //Объявления о окончании стрельбы всех стрелков на направлении
            int countfinishes = 0;
            bool first= true;                           //проверка первого ряда

            while (countfinishes < lines.Count / 2)
            {
                for (int i = lines.Count / 2; i < lines.Count - 1; i++)
                {
                    if (finishes[i] == false)
                    {
                        if (lines[i].shooter[0].trials != 0)
                        {
                            if (first != true)
                            {
                                Thread.Sleep(random.Next(3000, 10000)); //Длительность действия для Act1
                                lines[i].Act1(lines, i);
                            }
                            Thread.Sleep(random.Next(2000, 6000));     //Длительность действия для Act2
                            lines[i].Act2(lines, i);
                            Thread.Sleep(random.Next(1000, 4000));     //Длительность действия для Act3
                            lines[i].Act3(lines, i);
                            Thread.Sleep(random.Next(1000, 2000));     //Длительность действия для Act4
                            lines[i].Act4(lines, i);
                            timer2.Start();                            //Таймер стрельбы
                            Thread.Sleep(random.Next(5000, 15000));     //Длительность действия для Act5
                            lines[i].Act5(lines, i);
                            timer2.Stop();

                        }
                        else
                        {
                            finishes[i] = true;
                            countfinishes++;
                        }
                    }
                }
                first = false;
               
            }
            Console.WriteLine(t.Name + ": Стрельба завершена!");
            
            
        } 

        public static void Tick(object sender, ElapsedEventArgs e)      //Для общего таймера
        {
            secondsCount++;
        }

        public static void Tick1(object sender, ElapsedEventArgs e)     //Для таймера стрельбы под руководством первого инструктора
        {
            secondsCombatCount1++;
        }
        public static void Tick2(object sender, ElapsedEventArgs e)     //Для таймера стрельбы под руководством второго инструктора
        {
            secondsCombatCount2++;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("--------------Тестовое задание: Тир-------------");

            const int countshooter = 13;    //количество стрелков
            const int maxlines = 6;         //количество направлений


            System.Timers.Timer timer = new System.Timers.Timer(1000);      // Общий таймер
            timer.Elapsed += Tick;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();

            Random random = new Random();
            Shooter[] shooters = new Shooter[countshooter];
            int j = 0;      //индекс для lines

            List<Line> lines = new List<Line>();
            bool addLineslock = false;      //Блокировка добавления направлений в список на повторных циклах




            //Распределение стрелков по направлениям
            for (int i = 0; i < countshooter; i++)
            {

                if (addLineslock == false)
                {
                    lines.Add(new Line());
                    lines[j].shooter = new List<Shooter>();
                }

                shooters[i] = new Shooter() { name = $"Стрелок {i + 1}" };

                if (j >= maxlines)
                {
                    j = 0;

                    lines[j].name = $"Направление {j + 1}";

                    lines[j].shooter.Add(shooters[i]);

                    addLineslock = true;
                    j++;
                }
                else
                {
                    lines[j].name = $"Направление {j + 1}";
                    lines[j].shooter.Add(shooters[i]);

                    if (addLineslock == false)
                    {
                        Thread.Sleep(random.Next(3000, 10000));
                        lines[j].Act0(lines, j);
                    }

                    j++;
                }
            }


            //Добавление 2 потока
            Thread myThread = new Thread(new ParameterizedThreadStart(SecondInstructorControl));
            myThread.Start(lines);

            //продолжение
            System.Timers.Timer timer1 = new System.Timers.Timer(1000);     //Таймер для 1 инструктора
            timer1.Elapsed += Tick1;
            timer1.Enabled = true;
            timer1.AutoReset = true;

            Thread t = Thread.CurrentThread;
            t.Name = "Инструктор 1";
            bool[] finishes = new bool[lines.Count];    //Объявления о окончании стрельбы всех стрелков на направлении
            int countfinishes = 0;
            bool first = true;

            while (countfinishes < lines.Count / 2)
            {
                for (int i = 0; i < lines.Count / 2; i++)
                {
                    if (finishes[i] == false)
                    {
                        if (lines[i].shooter[0].trials != 0)
                        {
                            if (first != true)
                            {
                                Thread.Sleep(random.Next(3000, 10000));     //Длительность действия для Act1
                                lines[i].Act1(lines, i);
                            }
                            Thread.Sleep(random.Next(2000, 6000));     //Длительность действия для Act2
                            lines[i].Act2(lines, i);
                            Thread.Sleep(random.Next(1000, 4000));     //Длительность действия для Act3
                            lines[i].Act3(lines, i);
                            Thread.Sleep(random.Next(1000, 2000));     //Длительность действия для Act4
                            lines[i].Act4(lines, i);

                            timer1.Start();                            //Таймер стрельбы
                            Thread.Sleep(random.Next(5000, 15000));     //Длительность действия для Act5
                            lines[i].Act5(lines, i);
                            timer1.Stop();

                        }
                        else
                        {
                            finishes[i] = true;
                            countfinishes++;
                        }
                    }
                }

                first = false;
            }
            Console.WriteLine(t.Name + ": Стрельба завершена!");

            while (myThread.IsAlive == true) { Thread.Sleep(1000); }      //Ожидание закрытия второго потока
             
            Console.WriteLine("Сумма затраченного времени: " + secondsCount / 60 + " мин. " + secondsCount % 60 + "сек. ");
            if (secondsCombatCount1 < secondsCombatCount2)
            {
                Console.Write("Длительность стрельб: " + secondsCombatCount2 / 60 + " мин. " + secondsCombatCount2 % 60 + " сек.");
            }
            else
            {
                Console.Write("Длительность стрельб: " + secondsCombatCount1 / 60 + " мин. " + secondsCombatCount1 % 60 + " сек.");
            }
            Console.ReadKey();
        }
    }
}
