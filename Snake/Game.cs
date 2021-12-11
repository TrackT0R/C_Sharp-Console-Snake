using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snake
{
    static class Game
    {
        #region Types
        enum Elem
        {
            Up, Down, Left, Right, Null, Wall, Food
        }
        class point
        {
            public int x, y;
            public point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        #endregion

        #region Data
        public static bool Over { get; private set; } = false;

        static Elem[,] field { get; set; }
        static List<Elem> MoveQueue = new List<Elem>();
        static Timer timer;
        static point head, tail;
        static point Cursor;
        static int count { get; set; }
        static int period { get; set; }
        #endregion

        public static void InitCursor(int x, int y)
        {
            Cursor = new point(x, y);
        }

        public static void Start(int x, int y)
        {
            count = 0;
            period = 500;
            Over = false;

            field = new Elem[x + 2, y + 2];

            for (int i = 0; i < x + 2; i++)
                for (int j = 0; j < y + 2; j++)
                    field[i, j] = (i % (x + 1) == 0 || j % (y + 1) == 0) ? Elem.Wall : Elem.Null;

            head = new point((x + 1) / 2 + 2, (y + 1) / 2);
            tail = new point((x + 1) / 2 - 2, (y + 1) / 2);

            for (int i = tail.x; i <= head.x; i++)
                field[i, tail.y] = Elem.Right;

            PutFood();
            Console.CursorVisible = false;

            TimerCallback tm = new TimerCallback(timer_Tick);
            timer = new Timer(tm, null, 0, 500);
        }

        public static void Stop()
        {
            Over = true;
            timer.Change(-1, -1);
            Console.WriteLine("Game Over!             ");
        }

        public static void ConsoleClean()
        {
            Console.SetCursorPosition(Cursor.x, Cursor.y);

            var s = new String(' ', Math.Max(field.GetLength(0), 52));
            for (int i = 0; i < field.GetLength(1) + 3; i++)
                Console.WriteLine(s);

            Console.SetCursorPosition(Cursor.x, Cursor.y - 1);
            Console.CursorVisible = true;
        }

        public static void KeyPress(ConsoleKey key)
        {
            if (MoveQueue.Count >= 2)
                return;

            switch (key) {
                case ConsoleKey.UpArrow:
                    if (MoveQueue.Count == 0 && (field[head.x, head.y] == Elem.Up || field[head.x, head.y] == Elem.Down) ||
                        MoveQueue.Count == 1 && (MoveQueue[0] == Elem.Up || MoveQueue[0] == Elem.Down)) return;
                    MoveQueue.Add(Elem.Up);
                    break;
                case ConsoleKey.DownArrow:
                    if (MoveQueue.Count == 0 && (field[head.x, head.y] == Elem.Up || field[head.x, head.y] == Elem.Down) ||
                        MoveQueue.Count == 1 && (MoveQueue[0] == Elem.Up || MoveQueue[0] == Elem.Down)) return;
                    MoveQueue.Add(Elem.Down);
                    break;
                case ConsoleKey.RightArrow:
                    if (MoveQueue.Count == 0 && (field[head.x, head.y] == Elem.Left || field[head.x, head.y] == Elem.Right) ||
                        MoveQueue.Count == 1 && (MoveQueue[0] == Elem.Left || MoveQueue[0] == Elem.Right)) return;
                    MoveQueue.Add(Elem.Right);
                    break;
                case ConsoleKey.LeftArrow:
                    if (MoveQueue.Count == 0 && (field[head.x, head.y] == Elem.Left || field[head.x, head.y] == Elem.Right) ||
                        MoveQueue.Count == 1 && (MoveQueue[0] == Elem.Left || MoveQueue[0] == Elem.Right)) return;
                    MoveQueue.Add(Elem.Left);
                    break;
            }
        }



        static void FieldPrint()
        {
            Console.SetCursorPosition(Cursor.x, Cursor.y);
            for (int i = 0; i < field.GetLength(1); i++) {
                for (int j = 0; j < field.GetLength(0); j++)
                    Console.Write(field[j, i] == Elem.Wall ? '#' : field[j, i] == Elem.Null ? ' ' : field[j, i] == Elem.Food ? '+' : '@');
                Console.WriteLine();
            }
            Console.WriteLine($"Score : {count}");
        }

        static void timer_Tick(object obj)
        {
            lock (field) {
                #region MoveQueue check
                if (MoveQueue.Count > 0) {
                    field[head.x, head.y] = MoveQueue[0];
                    MoveQueue.RemoveAt(0);
                }
                #endregion

                var duration = field[head.x, head.y];

                #region Snake head move
                switch (field[head.x, head.y]) {
                    case Elem.Up: head.y--; break;
                    case Elem.Down: head.y++; break;
                    case Elem.Right: head.x++; break;
                    case Elem.Left: head.x--; break;
                }
                #endregion

                #region Where is snake head after move
                switch (field[head.x, head.y]) {
                    #region Moving forward
                    case Elem.Null:
                        field[head.x, head.y] = duration;

                        var t = new point(tail.x, tail.y);
                        switch (field[tail.x, tail.y]) {
                            case Elem.Up: tail.y--; break;
                            case Elem.Down: tail.y++; break;
                            case Elem.Right: tail.x++; break;
                            case Elem.Left: tail.x--; break;
                        }
                        field[t.x, t.y] = Elem.Null;
                        break;
                    #endregion

                    #region Eating
                    case Elem.Food:
                        field[head.x, head.y] = duration;
                        count++;
                        if (period > 100) {
                            period -= 20;
                            timer.Change(period, period);
                        }
                        PutFood();
                        break;
                    #endregion

                    #region Crash
                    default:
                        Stop();
                        return;
                        #endregion
                }
                #endregion
            }
            FieldPrint();
        }

        static void PutFood()
        {
            var rnd = new Random();
            int x, y;
            do {
                x = rnd.Next(1, field.GetLength(0));
                y = rnd.Next(1, field.GetLength(1));
            } while (field[x, y] != Elem.Null);

            field[x, y] = Elem.Food;
        }
    }
}
