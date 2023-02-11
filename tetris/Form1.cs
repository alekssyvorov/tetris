using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tetris
{
    public partial class Form1 : Form
    {
        const int a = 8;
        const int b = 16;
        int size;
        //добавляем переменную в которой будем складывать количество убранных линий
        int linesRemovs;
        int[,] map = new int[b, a];
        Shape currentShape;
        int score;
        int interval;
        public Form1()
        {
            InitializeComponent();
            //Включаем сразу двойную буферизацию DoubleBuffered - True
            //Что бы не было мерцаний
            Init();
        }
        public void Init()
        {
            size = 45;
            score = 0;
            linesRemovs = 0;
            currentShape = new Shape(3, 0);
            label1.Text = "Score: " + score;
            label2.Text = "Lines: " + linesRemovs;
            interval = 300;
            this.KeyDown += new KeyEventHandler(keyFunc);
            timer1.Interval = interval;
            timer1.Tick += new EventHandler(update);//создаем с помощью помощника функцию update
            timer1.Start();
            
            Invalidate();
            
        }
        public void SliceMap()
        {
            int count = 0;
            int removeLines = 0;
            for (int i = 0; i < b; i++)
            {
                count = 0;
                for (int j = 0; j < a; j++)
                {
                    if(map[i,j] != 0)
                    count++;
                }
                if(count == a)
                {
                    //если число = 8, то увеличиваем число текущих линий и смещаем всю карту вниз
                    removeLines++;
                    for (int k = i; k >= 1; k--)
                    {
                        for (int n = 0; n < a; n++)
                        {
                            map[k, n] = map[k-1, n];
                        }
                    }
                }
            }
            //увеличиваем количество очков
            for (int i = 0; i < removeLines; i++)  
            {
                score += 10 * (i+1);
            }
            linesRemovs += removeLines;
            //и это же в функции init

            //if(linesRemovs % 5 == 0)
            //{
            //    if (interval > 60)
            //        interval -= 10;
            //}

            label1.Text = "Score: " + score;
            label2.Text = "Lines: " + linesRemovs;
        }
        private void keyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (!Intersection())
                    {
                        ResetArea();
                        currentShape.RotateShape();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Space:
                    timer1.Interval = 10;
                    break;
                case Keys.Right:
                    if (!CollideHor(1))
                    {
                        ResetArea();
                        currentShape.MoveRight();
                        Merge();
                        Invalidate();
                    }

                    break;
                case Keys.Left:
                    if (!CollideHor(-1))
                    {
                        ResetArea();
                        currentShape.MoveLeft();
                        Merge();
                        Invalidate();
                    }
                    break;
            }
        }

        private void update(object sender, EventArgs e)
        {
            ResetArea();
            SliceMap();
            if (!Collide())
            {
                currentShape.MoveDown();
            }
            else
            {
                Merge();
                SliceMap();
                timer1.Interval = interval;
                currentShape.ResetShape(3, 0);
                if (Collide())
                {
                    for (int i = 0; i < b; i++)
                    {
                        for (int j = 0; j < a; j++)
                        {
                            map[i, j] = 0;
                        }
                    }
                    timer1.Tick -= new EventHandler(update);
                    timer1.Stop();
                    Init();
                }
            }
            Merge();
            Invalidate();//что бы каждый кадр отрисовывался на холсте
            //теперь пишем функцию Marge
            
        }

        public void Merge()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if(currentShape.matrix[i-currentShape.y, j-currentShape.x] != 0)
                    map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x];
                }
            }
        }
        //Обрезаем хвостик фигуры и добавляем обработчики нажатия клавиш
        public void ResetArea()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if(i>=0 && j>=0 && i < b && j < a)
                    {
                        if(currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        map[i, j] = 0;
                    }
                    
                }
            }
        }

        public void DrawMap(Graphics e)
        {
            for (int i = 0; i < b; i++)
            {
                for (int j = 0; j < a; j++)
                {
                    if (map[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(50 + j * size+1, 50 + i * size+1, size-1, size-1));
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Green, new Rectangle(50 + j * size + 1, 50 + i * size + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(50 + j * size + 1, 50 + i * size + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(50 + j * size + 1, 50 + i * size + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(50 + j * size + 1, 50 + i * size + 1, size - 1, size - 1));
                    }
                }
            }
        }

        public void DrawGrid(Graphics g)
        {
            for (int i = 0; i <= b; i++)
            {
                g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + a * size, 50 + i * size));
            }
            for (int i = 0; i <= a; i++)
            {
                g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + b * size));
            }
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawMap(e.Graphics);
            ShoeNextShape(e.Graphics);

            //И после этого в Paint пропишем функцию OnPaint
            //Отсортировал события в алфавитном порядке и нашел Paint
            //и теперь создаю новый класс, в котором буду рисовать фигуры
        }

        //метод складывания фигур
        public bool Collide()
        {
            for (int i = currentShape.y + currentShape.sizeMatrix-1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x+currentShape.sizeMatrix; j++)
                {
                    if(currentShape.matrix[i-currentShape.y,j-currentShape.x] != 0)
                    {
                        if (i + 1 == b)
                            return true;
                        if (map[i + 1, j] != 0)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool CollideHor(int dir)//-1 это влево, +1 - это вправо
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                            return true;
                        if (map[i, j+1*dir] != 0)
                        {
                            if(j-currentShape.x+1*dir>=currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if(currentShape.matrix[i-currentShape.y, j - currentShape.x + 1 * dir] == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //что бы нельзя было поворачивать фигуру есть поле занято
        public bool Intersection()
        {
            for (int i = currentShape.y; i < currentShape.y+currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x+currentShape.sizeMatrix; j++)
                {
                    if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                        return true;
                }
            }
            return false;
        }

        public void ShoeNextShape(Graphics e)
        {
            for (int i = 0; i < currentShape.sizeMatrix; i++)
            {
                for (int j = 0; j < currentShape.sizeMatrix; j++)
                {
                    if (currentShape.nextMatrix[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(500 + j * (size) + 1, 100 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Green, new Rectangle(500 + j * (size) + 1, 100 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(500 + j * (size) + 1, 100 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(500 + j * (size) + 1, 100 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(500 + j * (size) + 1, 100 + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }
    }
}
