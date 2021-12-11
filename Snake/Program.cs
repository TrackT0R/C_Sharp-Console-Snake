using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.InitCursor(Console.CursorLeft, Console.CursorTop);
            
            while (true) {
                Game.Start(30, 10);

                while (true) {
                    var key = Console.ReadKey(true).Key; //true - не выводить символ в консоль

                    if (Game.Over) break;
                    if (key == ConsoleKey.Escape) {
                        Game.Stop();
                        break;
                    }

                    Game.KeyPress(key);
                }

                Console.Write("Do you want to restart or exit game? (r/e) : ");
                var k = Console.ReadKey(true).Key;
                while (k != ConsoleKey.E && k != ConsoleKey.R)
                    k = Console.ReadKey(true).Key;

                Console.Write(k);

                if (k == ConsoleKey.E) {
                    Game.ConsoleClean();
                    return;
                }

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("                                                    ");
                Console.WriteLine("                                                    ");
            }
        }
    }
}
