#define TEST
#undef TEST

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Пятнашки
{
    public partial class GameField : Form
    {
        const Int16 ButtonSizePlusDistanceBetweenButtons = 76;
        enum Route { Up, Right, Down, Left };
        Button[] button_Array;
        byte tmp = 0;
        byte[,] Game_Matrix;
        byte[,] Sample_Matrix_1 = new byte[,] { {0,1,2,3 }, { 4,5,6,7}, {8,9,10,11 }, {12,13,14,15 } };
        byte[,] Sample_Matrix_2 = new byte[,] { { 1,2,3,4 }, {5,6,7,8 }, { 9,10,11,12 }, { 13,14,15,0 } };
        public GameField()
        {
            InitializeComponent();
            Rebuild_Field();
        }
        void Rebuild_Field()
        {
            button_Array = new Button[16]{null,button1,button2,button3,
                button4,button5,button6,button7,
                button8,button9,button10,button11,
                button12,button13,button14,button15};
            do
            {
                Game_Matrix = new byte[4, 4];
                fill_Game_Matrix();
            } while (!IsSolvable());
            button_Array = null;
            GC.Collect();
        }
        bool IsSolvable()
        {
            byte[] a = new byte[16];
            byte index = 0,inversions = 0;
            /*Только для случая, когда Game_Matrix[0,0] равно нулю*/
            foreach (byte b in Game_Matrix)
            {
                a[index] = b;
                index++;
            }
            for (int i = 1; i < 16; ++i)
            {
                for (int j = 1; j < i; j++)
                {
                    if (a[j] > a[i]) { inversions++;}
                }
            }
            inversions++;
            return (inversions % 2 == 0);
        }
        void fill_Game_Matrix()
        {
            List<int> existing_numbers = new List<int>();
            existing_numbers.Add(0);
            Random rnd = new Random((int)DateTime.Now.Ticks);
            byte tmp_number;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == 0 && j == 0) { Game_Matrix[i, j] = 0; continue; }
                    tmp_number = (byte)rnd.Next(1, 16);
                    if (!existing_numbers.Contains(tmp_number))
                    {
                        Game_Matrix[i, j] = tmp_number;
                        button_Array[tmp_number].Location = new Point(12 + ButtonSizePlusDistanceBetweenButtons * j, 12 + ButtonSizePlusDistanceBetweenButtons * i);
                        existing_numbers.Add(tmp_number);
                    }
                    else
                    {
                        do
                        {
                            tmp_number = (byte)rnd.Next(1, 16);
                        } while (existing_numbers.Contains(tmp_number));
                        Game_Matrix[i, j] = tmp_number;
                        button_Array[tmp_number].Location = new Point(12 + ButtonSizePlusDistanceBetweenButtons * j, 12 + ButtonSizePlusDistanceBetweenButtons * i);
                        existing_numbers.Add(tmp_number);
                    }
                }
            }
        }
        void Matrix_Swap(int i, int j, Route route)
        {
            tmp = Game_Matrix[i, j];
            switch (route)
            {
                case Route.Up : Game_Matrix[i, j] = Game_Matrix[i-1, j]; Game_Matrix[i-1, j] = tmp; break;
                case Route.Down: Game_Matrix[i, j] = Game_Matrix[i + 1, j]; Game_Matrix[i + 1, j] = tmp; break;
                case Route.Left: Game_Matrix[i, j] = Game_Matrix[i, j - 1]; Game_Matrix[i, j - 1] = tmp; break;
                case Route.Right: Game_Matrix[i, j] = Game_Matrix[i, j + 1]; Game_Matrix[i, j + 1] = tmp; break;
            }
            if (Game_Matrix.IsEquals(Sample_Matrix_1) || Game_Matrix.IsEquals(Sample_Matrix_2)) { Congratulations(); }
        }
        void Congratulations()
        {
            MessageBox.Show("Поздравляем, вы выиграли!","Победа",MessageBoxButtons.OK);
            Rebuild_Field();
        }
        new void Move(Button sender, Route route)
        {
            switch (route)
            {
                case Route.Up: for (int i = 1; i <= ButtonSizePlusDistanceBetweenButtons; i++) { Thread.Sleep(2); sender.Location = new Point(sender.Location.X, sender.Location.Y-1); } break;
                case Route.Down: for (int i = 1; i <= ButtonSizePlusDistanceBetweenButtons; i++) { Thread.Sleep(2); sender.Location = new Point(sender.Location.X, sender.Location.Y+1); } break;
                case Route.Right: for (int i = 1; i <= ButtonSizePlusDistanceBetweenButtons; i++) { Thread.Sleep(2); sender.Location = new Point(sender.Location.X + 1, sender.Location.Y); } break;
                case Route.Left: for (int i = 1; i <= ButtonSizePlusDistanceBetweenButtons; i++) { Thread.Sleep(2); sender.Location = new Point(sender.Location.X - 1, sender.Location.Y); } break;
            }
            GC.Collect();
        }
        #region Click event
        void Button_Click_Controller(object sender, EventArgs e)
        {
            int i = (((Button)((Button)sender)).Location.Y - 12) / ButtonSizePlusDistanceBetweenButtons;
            int j = (((Button)sender).Location.X - 12) / ButtonSizePlusDistanceBetweenButtons;
            if (i > 0 && i < 3 && j > 0 && j < 3)
            {
                if (Game_Matrix[i + 1, j] == 0) { Move(((Button)sender), Route.Down); Matrix_Swap(i, j, Route.Down); }
                else if (Game_Matrix[i - 1, j] == 0) { Move(((Button)sender), Route.Up); Matrix_Swap(i, j, Route.Up); }
                else if (Game_Matrix[i, j + 1] == 0) { Move(((Button)sender), Route.Right); Matrix_Swap(i, j, Route.Right); }
                else if (Game_Matrix[i, j - 1] == 0) { Move(((Button)sender), Route.Left); Matrix_Swap(i, j, Route.Left); }
            }
            else if (i == 0 && j == 0)
            {
                if (Game_Matrix[i + 1, j] == 0) { Move(((Button)sender), Route.Down); Matrix_Swap(i, j, Route.Down); }
                else if (Game_Matrix[i, j + 1] == 0) { Move(((Button)sender), Route.Right); Matrix_Swap(i, j, Route.Right); }
            }
            else if (i == 0 && j == 3)
            {
                if (Game_Matrix[i + 1, j] == 0) { Move(((Button)sender), Route.Down); Matrix_Swap(i, j, Route.Down); }
                else if (Game_Matrix[i, j - 1] == 0) { Move(((Button)sender), Route.Left); Matrix_Swap(i, j, Route.Left); }
            }
            else if (i == 0 && (j == 1 || j == 2))
            {
                if (Game_Matrix[i + 1, j] == 0) { Move(((Button)sender), Route.Down); Matrix_Swap(i, j, Route.Down); }
                else if (Game_Matrix[i, j - 1] == 0) { Move(((Button)sender), Route.Left); Matrix_Swap(i, j, Route.Left); }
                else if (Game_Matrix[i, j + 1] == 0) { Move(((Button)sender), Route.Right); Matrix_Swap(i, j, Route.Right); }
            }
            else if (i == 3 && j == 0)
            {
                if (Game_Matrix[i - 1, j] == 0) { Move(((Button)sender), Route.Up); Matrix_Swap(i, j, Route.Up); }
                else if (Game_Matrix[i, j + 1] == 0) { Move(((Button)sender), Route.Right); Matrix_Swap(i, j, Route.Right); }
            }
            else if (i == 3 && j == 3)
            {
                if (Game_Matrix[i - 1, j] == 0) { Move(((Button)sender), Route.Up); Matrix_Swap(i, j, Route.Up); }
                else if (Game_Matrix[i, j - 1] == 0) { Move(((Button)sender), Route.Left); Matrix_Swap(i, j, Route.Left); }
            }
            else if (i == 3 && (j == 1 || j == 2))
            {
                if (Game_Matrix[i - 1, j] == 0) { Move(((Button)sender), Route.Up); Matrix_Swap(i, j, Route.Up); }
                else if (Game_Matrix[i, j - 1] == 0) { Move(((Button)sender), Route.Left); Matrix_Swap(i, j, Route.Left); }
                else if (Game_Matrix[i, j + 1] == 0) { Move(((Button)sender), Route.Right); Matrix_Swap(i, j, Route.Right); }
            }
            else if (j == 3 && (i == 1 || i == 2))
            {
                if (Game_Matrix[i, j - 1] == 0) { Move(((Button)sender), Route.Left); Matrix_Swap(i, j, Route.Left); }
                else if (Game_Matrix[i - 1, j] == 0) { Move(((Button)sender), Route.Up); Matrix_Swap(i, j, Route.Up); }
                else if (Game_Matrix[i + 1, j] == 0) { Move(((Button)sender), Route.Down); Matrix_Swap(i, j, Route.Down); }
            }
            else if (j == 0 && (i == 1 || i == 2))
            {
                if (Game_Matrix[i, j + 1] == 0) { Move(((Button)sender), Route.Right); Matrix_Swap(i, j, Route.Right); }
                else if (Game_Matrix[i - 1, j] == 0) { Move(((Button)sender), Route.Up); Matrix_Swap(i, j, Route.Up); }
                else if (Game_Matrix[i + 1, j] == 0) { Move(((Button)sender), Route.Down); Matrix_Swap(i, j, Route.Down); }
            }
        }
        #endregion
    }
    public static class puzzle_15_extesions
    {
        public static bool IsEquals(this byte[,] Matrix, byte[,] Sample_Matrix)
        {
            if (Matrix.GetLength(0) != Sample_Matrix.GetLength(0) || Matrix.GetLength(1) != Sample_Matrix.GetLength(1)) { throw new ArgumentException(); }
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] != Sample_Matrix[i, j]) { return false; }
                }
            }
            return true;
        } 
    }
}
