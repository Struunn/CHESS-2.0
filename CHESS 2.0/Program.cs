
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
string WhSq(int Row, int Col) // White square
{
    Console.ResetColor();
    Console.BackgroundColor = ConsoleColor.White;
    bool isSelected = (Row == selectedRow && Col == selectedColumn);
    string whiteSquare = ANSI(board[Row, Col], isSelected ? 0x85FFB2 : 0xFFFFFF);
    return whiteSquare;
}   
string BlSq(int Row, int Col) // Black square
{   
    Console.ResetColor();
    Console.BackgroundColor = ConsoleColor.DarkGray;
    bool/*?*/                   isSelected = (Row == selectedRow && Col == selectedColumn);
    string blackSquare = ANSI(board[Row, Col], isSelected? 0x85FFB2 : 0xfffff);
    return blackSquare;
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
            if (board[selectedRow, selectedColumn] == Piece.Rooki ||
                board[selectedRow, selectedColumn] == Piece.Knighti ||
                board[selectedRow, selectedColumn] == Piece.Bishopi ||
                board[selectedRow, selectedColumn] == Piece.Queeni ||
                board[selectedRow, selectedColumn] == Piece.Kingi ||
                board[selectedRow, selectedColumn] == Piece.Pawni)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (board[selectedRow, selectedColumn] == Piece.____)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Black;
            }
            if ((length + height) % 2 == 0)
            {
                Console.Write($"{WhSq(height, length)}\t");
            }
            else
            {
                Console.Write($"{BlSq(height, length)}\t");
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
    }
}
enum Piece
{
    Rooki, Rook, Knighti, Knight, Bishopi, Bishop, Kingi, King, Queeni, Queen, Pawni, Pawn, ____
}
