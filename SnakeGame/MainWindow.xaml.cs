using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private List<Point> snakePositions = new List<Point>();
        private Brush snakeColor = Brushes.Green;
        private Brush appleColor = Brushes.Red;
        private DispatcherTimer timer = new DispatcherTimer();
        private Random rand = new Random();
        private Point snakeFirstPosition = new Point(200, 200);
        private Point current = new Point();
        private int direction = 0;
        private int previous = 0;
        private int snakeSize = 10;
        private int snakeLength = 80;
        private int snakeSpeed = 5;
        private int score = 0;
        private int counter = 0;
        private static Point apple;
        
        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += GameTick;
            timer.Interval = TimeSpan.FromMilliseconds(snakeSpeed);
            timer.Start();
            this.KeyDown += KeyPressed;
            AddSnake(snakeFirstPosition);
            current = snakeFirstPosition;
            AddApple();
        }

    private void AddSnake(Point current)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = snakeColor;
            rect.Width = snakeSize;
            rect.Height = snakeSize;
            Canvas.SetLeft(rect, current.X);
            Canvas.SetTop(rect, current.Y);
            int count = GameArea.Children.Count;
            GameArea.Children.Add(rect);
            snakePositions.Add(current);
            if (count > snakeLength)
            {
                GameArea.Children.RemoveAt(count - snakeLength + 1);
                snakePositions.RemoveAt(count - snakeLength - 1);
            }
        }

    private void AddApple()
        {
            apple = new Point(rand.Next(10, 490), rand.Next(10, 490));
            Ellipse appleShape = new Ellipse();
            appleShape.Fill = appleColor;
            appleShape.Width = 10;
            appleShape.Height = 10;
            Canvas.SetLeft(appleShape, apple.X);
            Canvas.SetTop(appleShape, apple.Y);
            GameArea.Children.Insert(0, appleShape);
        }
        private void MoveSnake()
        {
            switch (direction)
            {
                case (int)Direction.directions.Down:
                    current.Y += 1;
                    AddSnake(current);
                    break;
                case (int)Direction.directions.Up:
                    current.Y += -1;
                    AddSnake(current);
                    break;
                case (int)Direction.directions.Left:
                    current.X += -1;
                    AddSnake(current);
                    break;
                case (int)Direction.directions.Right:
                    current.X += 1;
                    AddSnake(current);
                    break;
            }
            if ((current.X < 7) || (current.X > 518) || (current.Y < 7) || (current.Y > 495))
            {
                GameOver();
            }
            for (int i = 0; i < (snakePositions.Count - snakeSize * 2); i++)
            {
                Point point = new Point(snakePositions[i].X, snakePositions[i].Y);
                if ((Math.Abs(point.X - current.X) < snakeSize) && (Math.Abs(point.Y - current.Y) < snakeSize))
                {
                    GameOver();
                    break;
                }
            }
        }
        private void AddScore()
        {
            if ((Math.Abs(apple.X - current.X) < snakeSize) && (Math.Abs(apple.Y - current.Y) < snakeSize))
            {
                ScoreSound();
                score = score + 1;
                snakeLength = snakeLength + 10;
                Score.Text = "SCORE: " + score.ToString();
                GameArea.Children.RemoveAt(0);
                AddApple();
            }
        }

        private void GameTick(object sender, EventArgs e)
        {
            MoveSnake();
            AddScore();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (previous != (int)Direction.directions.Up)
                        direction = (int)Direction.directions.Down;
                    break;
                case Key.Up:
                    if (previous != (int)Direction.directions.Down)
                        direction = (int)Direction.directions.Up;
                    break;
                case Key.Left:
                    if (previous != (int)Direction.directions.Right)
                        direction = (int)Direction.directions.Left;
                    break;
                case Key.Right:
                    if (previous != (int)Direction.directions.Left)
                        direction = (int)Direction.directions.Right;
                    break;
                case Key.P:
                    PauseGame();
                    break;

            }
            previous = direction;
        }

        private void GameOver()
        {
            GameOverSound();
            MessageBox.Show("Game Over! Your score: " + score.ToString(), "You Lose", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void PauseGame()
        {
            counter++;
            if(counter%2 == 0)
            {
                timer.Start();
                Score.Text = "SCORE: " + score.ToString();
            }
            else
            {
                timer.Stop();
                Score.Text = "PAUSED";
            }
        }

        private void ScoreSound()
        {
            var scoreSound = new SoundPlayer(Sounds.point);
            scoreSound.Play();
        }

        private void GameOverSound()
        {
            var gameOverSound = new SoundPlayer(Sounds.gameover);
            gameOverSound.Play();
        }
    }
}

