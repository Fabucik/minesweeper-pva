using System;

class Minesweeper
{
    static char[,] board;
    static bool[,] revealed;
    static bool[,] flagged;
    static int rows, cols, mineCount;
    static bool gameOver = false;

    static void Main(string[] args)
    {
        SelectDifficulty();
        InitializeBoard();
        PlaceMines();
        CalculateAdjacentMines();

        while (!gameOver)
        {
            Console.Clear();
            RenderBoard();
            ProcessInput();
        }

        Console.WriteLine("Game Over!");
    }

    static void SelectDifficulty()
    {
        Console.WriteLine("Select Difficulty:");
        Console.WriteLine("1. Easy (8x8, 10 mines)");
        Console.WriteLine("2. Medium (16x16, 40 mines)");
        Console.WriteLine("3. Hard (24x24, 99 mines)");
        Console.Write("Enter choice (1-3): ");

        int choice = int.Parse(Console.ReadLine() ?? "1");
        switch (choice)
        {
            case 1: // zacátečník
                rows = 8;
                cols = 8;
                mineCount = 10;
                break;
            case 2: // pokrocily
                rows = 16;
                cols = 16;
                mineCount = 40;
                break;
            case 3: // expert
                rows = 24;
                cols = 24;
                mineCount = 99;
                break;
            default:
                Console.WriteLine("Invalid choice! Defaulting to Easy.");
                rows = 8; cols = 8; mineCount = 10;
                break;
        }
    }

    static void InitializeBoard()
    {
        board = new char[rows, cols];
        revealed = new bool[rows, cols];
        flagged = new bool[rows, cols];

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                board[r, c] = ' ';
    }

    static void PlaceMines()
    {
        Random rand = new Random();
        int placed = 0;

        while (placed < mineCount)
        {
            int r = rand.Next(rows);
            int c = rand.Next(cols);
            if (board[r, c] != 'M')
            {
                board[r, c] = 'M';
                placed++;
            }
        }
    }

    static void CalculateAdjacentMines()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] == 'M') continue;

                int count = 0;
                // delta row a delta column oznacuje, ktere policko vedle kontrolujeme. 
                // dr = -1; dl = -1; kontrolujeme levy horni roh
                // dr = 0; dl = -1; kontrolujeme levou stranu, atd
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        int nr = r + dr, nc = c + dc;
                        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && board[nr, nc] == 'M')
                            count++;
                    }
                }

                board[r, c] = count > 0 ? count.ToString()[0] : ' ';
            }
        }
    }

    static void RenderBoard()
    {
        Console.WriteLine("  " + string.Join(" ", Enumerable.Range(0, cols)));
        for (int r = 0; r < rows; r++)
        {
            Console.Write(r.ToString("D") + " ");
            for (int c = 0; c < cols; c++)
            {
                if (revealed[r, c])
                    Console.Write(board[r, c] + " ");
                else if (flagged[r, c])
                    Console.Write("F ");
                else
                    Console.Write(". ");
            }
            Console.WriteLine();
        }
    }

    static void ProcessInput()
    {
        Console.Write("Enter command (r <row> <col> to reveal, f <row> <col> to flag): ");
        string[] input = Console.ReadLine()?.Split();
        if (input == null || input.Length < 3)
        {
            return;
        }

        char command = input[0][0];
        int row = int.Parse(input[1]);
        int col = int.Parse(input[2]);

        if (command == 'r')
        {
            RevealCell(row, col);
            if (CheckWinCondition())
            {
                Console.Clear();
                RenderBoard();
                Console.WriteLine("Congratulations, you won!");
                gameOver = true;
            }
        }
        else if (command == 'f')
        {
            flagged[row, col] = !flagged[row, col];
        }
    }

    static void RevealCell(int row, int col)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || revealed[row, col]) return;

        revealed[row, col] = true;

        if (board[row, col] == 'M')
        {
            gameOver = true;
            return;
        }

        if (board[row, col] == ' ')
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    RevealCell(row + dr, col + dc);
                }
            }
        }
    }

    static bool CheckWinCondition()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (!revealed[r, c] && board[r, c] != 'M')
                {
                    return false;
                }
            }
        }
        return true;
    }
}
