


using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab7.Task2.Views
{
    public partial class Game2048 : Window
    {
        private readonly int[,] board = new int[4, 4];
        private readonly Border[,] tiles = new Border[4, 4];
        private readonly TextBlock[,] labels = new TextBlock[4, 4];
        private readonly Random rand = new Random();
        private int score = 0;
        private int highScore = 0;
        private bool hasWon = false; // Прапорець для одноразового сповіщення про перемогу

        public Game2048()
        {
            InitializeComponent();
            InitGrid();
            StartNewGame();
        }

        private void InitGrid()
        {
            GameGrid.Children.Clear();
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    var textBlock = new TextBlock
                    {
                        FontSize = 22,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var border = new Border
                    {
                        Margin = new Thickness(4),
                        CornerRadius = new CornerRadius(5),
                        Child = textBlock
                    };

                    tiles[r, c] = border;
                    labels[r, c] = textBlock;
                    GameGrid.Children.Add(border);
                }
            }
        }

        private void StartNewGame()
        {
            score = 0;
            hasWon = false;
            Array.Clear(board, 0, board.Length);
            AddTile();
            AddTile();
            UpdateUI();
        }

        private void AddTile()
        {
            var emptyCells = new List<(int, int)>();
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (board[r, c] == 0) emptyCells.Add((r, c));

            if (emptyCells.Count > 0)
            {
                var (r, c) = emptyCells[rand.Next(emptyCells.Count)];
                board[r, c] = rand.NextDouble() < 0.9 ? 2 : 4;
            }
        }

        private void UpdateUI()
        {
            lblScore.Text = score.ToString();
            if (score > highScore) highScore = score;
            lblHighScore.Text = highScore.ToString();

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    int val = board[r, c];
                    labels[r, c].Text = val > 0 ? val.ToString() : "";
                    tiles[r, c].Background = GetTileColor(val);
                    labels[r, c].Foreground = val <= 4 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#776E65")) : Brushes.White;
                }
            }
        }

        private Brush GetTileColor(int value) => value switch
        {
            0 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CDC1B4")),
            2 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEE4DA")),
            4 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDE0C8")),
            8 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2B179")),
            16 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59563")),
            32 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F67C5F")),
            64 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F65E3B")),
            128 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDCF72")),
            256 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDCC61")),
            512 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDC850")),
            1024 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDC53F")),
            2048 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDC22E")),
            _ => Brushes.Black
        };

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool moved = e.Key switch
            {
                Key.Left => MoveLeft(),
                Key.Right => MoveRight(),
                Key.Up => MoveUp(),
                Key.Down => MoveDown(),
                _ => false
            };

            if (moved)
            {
                AddTile();
                UpdateUI();

                // Перевірка на перемогу (плитка 2048)
                CheckWinCondition();

                // Перевірка на поразку (немає ходів)
                if (IsGameOver())
                {
                    MessageBox.Show("Гра закінчена! Немає доступних ходів.", "Поразка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CheckWinCondition()
        {
            if (hasWon) return;

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (board[r, c] == 2048)
                    {
                        hasWon = true;
                        MessageBox.Show("Вітаємо! Ви досягли плитки 2048 і перемогли!", "Перемога", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
            }
        }

        private bool IsGameOver()
        {
            // Якщо є хоч одна порожня клітинка — гра триває
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (board[r, c] == 0) return false;
                }
            }

            // Якщо є сусідні однакові плитки по горизонталі чи вертикалі — гра триває
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (c < 3 && board[r, c] == board[r, c + 1]) return false;
                    if (r < 3 && board[r, c] == board[r + 1, c]) return false;
                }
            }

            return true;
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private bool MoveLeft()
        {
            bool moved = false;
            for (int r = 0; r < 4; r++)
            {
                int[] row = { board[r, 0], board[r, 1], board[r, 2], board[r, 3] };
                if (SlideAndMerge(row)) moved = true;
                for (int c = 0; c < 4; c++) board[r, c] = row[c];
            }
            return moved;
        }

        private bool MoveRight()
        {
            bool moved = false;
            for (int r = 0; r < 4; r++)
            {
                int[] row = { board[r, 3], board[r, 2], board[r, 1], board[r, 0] };
                if (SlideAndMerge(row)) moved = true;
                for (int c = 0; c < 4; c++) board[r, 3 - c] = row[c];
            }
            return moved;
        }

        private bool MoveUp()
        {
            bool moved = false;
            for (int c = 0; c < 4; c++)
            {
                int[] col = { board[0, c], board[1, c], board[2, c], board[3, c] };
                if (SlideAndMerge(col)) moved = true;
                for (int r = 0; r < 4; r++) board[r, c] = col[r];
            }
            return moved;
        }

        private bool MoveDown()
        {
            bool moved = false;
            for (int c = 0; c < 4; c++)
            {
                int[] col = { board[3, c], board[2, c], board[1, c], board[0, c] };
                if (SlideAndMerge(col)) moved = true;
                for (int r = 0; r < 4; r++) board[3 - r, c] = col[r];
            }
            return moved;
        }

        private bool SlideAndMerge(int[] line)
        {
            bool moved = false;
            int pos = 0;
            for (int i = 0; i < 4; i++)
            {
                if (line[i] != 0)
                {
                    if (pos != i) { line[pos] = line[i]; line[i] = 0; moved = true; }
                    pos++;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (line[i] != 0 && line[i] == line[i + 1])
                {
                    line[i] *= 2;
                    score += line[i];
                    line[i + 1] = 0;
                    moved = true;
                }
            }

            pos = 0;
            for (int i = 0; i < 4; i++)
            {
                if (line[i] != 0)
                {
                    if (pos != i) { line[pos] = line[i]; line[i] = 0; moved = true; }
                    pos++;
                }
            }

            return moved;
        }
    }
}
