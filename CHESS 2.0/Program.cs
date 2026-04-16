
using System.Security.AccessControl;

string ANSI(Piece Text, int color) // Text Color    (Credit to Harry, he helped with ANSI)
{
    Piece text = Text;
    string ansi = $"\u001b[38;2;{(color >> 16) & 0xff};{(color >> 8) & 0xff};{color & 0xff}m";
    return ansi + text;
}

// Places the pieces in the starting setup
Piece[,] board = new Piece[,] { 
                        { Piece.Rook, Piece.Knight, Piece.Bishop, Piece.King, Piece.Queen, Piece.Bishop, Piece.Knight, Piece.Rook },
                        { Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn },
                        { Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____ },
                        { Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____ },
                        { Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____ },
                        { Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____, Piece.____ },
                        { Piece.Pawni, Piece.Pawni, Piece.Pawni, Piece.Pawni, Piece.Pawni, Piece.Pawni, Piece.Pawni, Piece.Pawni },
                        { Piece.Rooki, Piece.Knighti, Piece.Bishopi, Piece.Kingi, Piece.Queeni, Piece.Bishopi, Piece.Knighti, Piece.Rooki }};

// Keeps track of the position of the selected square
int selectedRow = 3;
int selectedColumn = 3;
void DrawSquare(int Row, int Col, bool isWhite) // White square
{
    bool isSelected = (Row == selectedRow && Col == selectedColumn);
    Console.BackgroundColor = isSelected? ConsoleColor.Green : isWhite? ConsoleColor.White : ConsoleColor.DarkGray;
    Console.WriteLine(board[Row, Col]);
    Console.ResetColor();
}   

// Start of game loop
while (true)
{
    Console.Clear();
    for (int length = 0; length < 8;  length++) // Draws the board
    {
        for (int height = 0; height < 8; height++)
        {
            Console.SetCursorPosition(3 + length * 8, 2 + height * 3);
            if (board[height, length] == Piece.Rooki ||
                board[height, length] == Piece.Knighti ||
                board[height, length] == Piece.Bishopi ||
                board[height, length] == Piece.Queeni ||
                board[height, length] == Piece.Kingi ||
                board[height, length] == Piece.Pawni)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (board[height, length] == Piece.____)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if ((length + height) % 2 == 0)
            {
                DrawSquare(height, length, true);
            }
            else
            {
                DrawSquare(height, length, false);
            }
        }
    }

    // Move the selected square using arrow keys
    ConsoleKey Navigate = Console.ReadKey(true).Key;
    switch (Navigate)
    {
        case ConsoleKey.UpArrow:
            selectedRow--;
            break;

        case ConsoleKey.DownArrow:
            selectedRow++;
            break;

        case ConsoleKey.LeftArrow:
            selectedColumn--;
            break;

        case ConsoleKey.RightArrow:
            selectedColumn++;
            break;

        case ConsoleKey.Enter:
            
            break;
    }
}
enum Piece
{
    Rooki, Rook, Knighti, Knight, Bishopi, Bishop, Kingi, King, Queeni, Queen, Pawni, Pawn, ____
}
