using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private List<Point> snake = new List<Point>();
        private Point food = new Point();
        private Direction direction = Direction.Down;
        private int maxWidth;
        private int maxHeight;
        private int score;
        private bool gameOver;

        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private Label label1;

        public Form1()
        {
            InitializeComponent();

            lblGameOver = new Label
            {
                Size = new Size(200, 70),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            pbCanvas.Controls.Add(lblGameOver);

            InitializeGame();
        }

        private void InitializeGame()
        {
            maxWidth = pbCanvas.Width / 10; // Задаем размер клетки 10x10
            maxHeight = pbCanvas.Height / 10;
            snake.Clear();
            snake.Add(new Point(10, 5)); // Начальное положение змейки
            score = 0;
            gameOver = false;
            direction = Direction.Down;
            gameTimer.Interval = 100; // Скорость игры
            gameTimer.Tick += Update;
            gameTimer.Start();
            GenerateFood();

            lblGameOver.Visible = false;
        }

        private void GenerateFood()
        {
            Random random = new Random();
            food = new Point(random.Next(0, maxWidth), random.Next(0, maxHeight));
        }

        private void Update(object sender, EventArgs e)
        {
            if (gameOver)
            {
                gameTimer.Stop();
                if (MessageBox.Show("Игра окончена. Хотите начать снова?", "Змейка", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    InitializeGame();
                }
                else
                {
                    Close();
                }
            }

            MoveSnake();
            pbCanvas.Invalidate(); // Перерисовка PictureBox
        }

        private void MoveSnake()
        {
            for (int i = snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (direction)
                    {
                        case Direction.Up:
                            snake[i] = new Point(snake[i].X, snake[i].Y - 1);
                            break;
                        case Direction.Down:
                            snake[i] = new Point(snake[i].X, snake[i].Y + 1);
                            break;
                        case Direction.Left:
                            snake[i] = new Point(snake[i].X - 1, snake[i].Y);
                            break;
                        case Direction.Right:
                            snake[i] = new Point(snake[i].X + 1, snake[i].Y);
                            break;
                    }

                    // Проверка столкновения с границами
                    if (snake[i].X < 0 || snake[i].Y < 0 || snake[i].X >= maxWidth || snake[i].Y >= maxHeight)
                    {
                        gameOver = true;
                    }

                    // Проверка столкновения с телом змейки
                    for (int j = 1; j < snake.Count; j++)
                    {
                        if (snake[i].Equals(snake[j]))
                        {
                            gameOver = true;
                        }
                    }

                    // Проверка столкновения с едой
                    if (snake[i].Equals(food))
                    {
                        EatFood();
                    }
                }
                else
                {
                    snake[i] = snake[i - 1];
                }
            }
        }

        private void EatFood()
        {
            Point tail = snake[snake.Count - 1];
            snake.Add(new Point(tail.X, tail.Y));
            score += 10; // Увеличение счета
            GenerateFood();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!gameOver)
            {
                // Отрисовка змейки
                for (int i = 0; i < snake.Count; i++)
                {
                    Brush snakeColor = i == 0 ? Brushes.Black : Brushes.Green;
                    canvas.FillRectangle(snakeColor, new Rectangle(snake[i].X * 10, snake[i].Y * 10, 10, 10));
                }

                // Отрисовка еды
                canvas.FillRectangle(Brushes.Red, new Rectangle(food.X * 10, food.Y * 10, 10, 10));
            }
            else
            {
                lblGameOver.Text = $"Игра окончена\nВаш счет: {score}";
                lblGameOver.Location = new Point((pbCanvas.Width - lblGameOver.Width) / 2, (pbCanvas.Height - lblGameOver.Height) / 2);
                lblGameOver.Visible = true;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != Direction.Down)
                        direction = Direction.Up;
                    break;
                case Keys.Down:
                    if (direction != Direction.Up)
                        direction = Direction.Down;
                    break;
                case Keys.Left:
                    if (direction != Direction.Right)
                        direction = Direction.Left;
                    break;
                case Keys.Right:
                    if (direction != Direction.Left)
                        direction = Direction.Right;
                    break;
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
