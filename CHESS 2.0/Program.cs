
// Places the pieces in the starting setup
bool[,] canBeCaptured = new bool[8, 8];
bool[,] canBeMovedTo = new bool[8, 8];

void ResetMoveData()
{
    for (int i = 0; i < 8; i++)
    {
        for (int j = 0; j < 8; j++)
        {
            canBeMovedTo[i, j] = false;
            canBeCaptured[i, j] = false;
        }
    }
}

Piece[,] board = new Piece[,] { 
                        { Piece._Rook__, Piece.Knight_, Piece.Bishop_, Piece._Queen_, Piece._King__, Piece.Bishop_, Piece.Knight_, Piece._Rook__ },
                        { Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_ },
                        { Piece._Rooki_, Piece.Knighti, Piece.Bishopi, Piece.Queeni_, Piece._Kingi_, Piece.Bishopi, Piece.Knighti, Piece._Rooki_ }};

bool isWhitePiece(Piece? piece)
{
    return
        piece == Piece._Pawni_ ||
        piece == Piece._Rooki_ ||
        piece == Piece.Knighti ||
        piece == Piece.Bishopi ||
        piece == Piece.Queeni_ ||
        piece == Piece._Kingi_;
}
bool isBlackPiece(Piece? piece)
{
    return
        piece == Piece._Pawn__ ||
        piece == Piece._Rook__ ||
        piece == Piece.Knight_ ||
        piece == Piece.Bishop_ ||
        piece == Piece._Queen_ ||
        piece == Piece._King__;
}

// Keeps track of the position of the selected square
int selectedRow = 3;
int selectedColumn = 3;
void DrawSquare(int Row, int Col, bool canBeCaptured, bool canBeMovedTo) // Draws the square
{
    bool isSelected = (Row == selectedRow && Col == selectedColumn);
    bool isWhite;
    if ((Col + Row) % 2 == 0)
    {
        isWhite = true;
    }
    else
    {
        isWhite = false;
    }
    Console.BackgroundColor = isSelected ? ConsoleColor.Green : 
        canBeCaptured? ConsoleColor.Red : 
        canBeMovedTo? ConsoleColor.DarkYellow : 
        isWhite ? ConsoleColor.White : ConsoleColor.DarkGray;

    if (board[Row, Col] == Piece._Rooki_ ||     // Checks if piece belongs to White, changes its color accordingly
        board[Row, Col] == Piece.Knighti ||
        board[Row, Col] == Piece.Bishopi ||
        board[Row, Col] == Piece.Queeni_ ||
        board[Row, Col] == Piece._Kingi_ ||
        board[Row, Col] == Piece._Pawni_)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    else if (board[Row, Col] == Piece._______)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
    }

    // Draws the whole square
    if (Row == 0)
    {
        Row = 1;
        Console.SetCursorPosition(Col * 7, Row - 1);
        Row = 0;
    }
    else
    {
        Console.SetCursorPosition(Col * 7, Row * 3 - 1);
    }
    Console.Write("       ");
    Console.SetCursorPosition(Col * 7, Row * 3);
    Console.Write(board[Row, Col]);
    Console.SetCursorPosition(Col * 7, 1 + Row * 3);
    Console.Write($" {Row}, {Col}  ");
    Console.SetCursorPosition(Col * 7, Row * 3);
}


// ----------------------------- METHODS FOR PIECE MOVES -----------------------------

void SetPiecePos(Piece piece, int tempSelectedRow, int tempSelectedColumn)
{
    board[tempSelectedRow, tempSelectedColumn] = Piece._______;
    board[selectedRow, selectedColumn] = piece;
    DrawBoard();
}

void CanBeMovedToPos(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    canBeMovedTo[tempSelectedRow + rowOffset, tempSelectedColumn + colOffset] = true;
    DrawSquare(tempSelectedRow + rowOffset, tempSelectedColumn + colOffset, false, true);
}

void CanBeCapturedPos(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    if (tempSelectedColumn + colOffset >= 0 && tempSelectedColumn + colOffset <= 7 &&
        tempSelectedRow + rowOffset >= 0 && tempSelectedRow + rowOffset <= 7) // Makes sure it doesn't go out of bounds
    {
        canBeCaptured[tempSelectedRow + rowOffset, tempSelectedColumn + colOffset] = true;
        DrawSquare(tempSelectedRow + rowOffset, tempSelectedColumn + colOffset, true, false);
    }
}

Piece? GetPosition(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    Piece? position;
    int Row = tempSelectedRow + rowOffset;
    int Col = tempSelectedColumn + colOffset;

    if (Row >= 0 && Row < 8 && Col >= 0 && Col < 8)
    {
        return position = board[Row, Col];
    }
    else
    {
        return null;
    }
}

bool IsWhite(Piece piece, int tempSelectedRow, int tempSelectedColumn)
{
    bool isWhite;
    if (GetPosition(0, 0, tempSelectedRow, tempSelectedColumn) == piece) // Determines which side the pawn belongs to
    {
        return isWhite = true;
    }
    else
    {
        return isWhite = false;
    }
}


// ----------------------------- PIECE MOVES -----------------------------

void PawnMove(int tempSelectedRow, int tempSelectedColumn) // ----- PAWN -----
{
    bool isWhite = IsWhite(Piece._Pawni_, tempSelectedRow, tempSelectedColumn);
    while (true)
    {
        if (isWhite)
        {
            Piece? movePos1 = GetPosition(-1, 0, tempSelectedRow, tempSelectedColumn);
            if (movePos1 != null && movePos1 == Piece._______) // Shows normal moves
            {
                CanBeMovedToPos(-1, 0, tempSelectedRow, tempSelectedColumn);
                Piece? movePos2 = GetPosition(-2, 0, tempSelectedRow, tempSelectedColumn);
                if (tempSelectedRow == 6 && movePos2 != null && movePos2 == Piece._______)
                {
                    CanBeMovedToPos(-2, 0, tempSelectedRow, tempSelectedColumn);
                }
            }

            Piece? attackPos1 = GetPosition(-1, -1, tempSelectedRow, tempSelectedColumn);
            if (attackPos1 != null && attackPos1 != Piece._______ && !isWhitePiece(attackPos1)) // Show capture moves
            {
                CanBeCapturedPos(-1, -1, tempSelectedRow, tempSelectedColumn);
            }
            Piece? attackPos2 = GetPosition(-1, 1, tempSelectedRow, tempSelectedColumn);
            if (attackPos2 != null && attackPos2 != Piece._______ && !isWhitePiece(attackPos2))
            {
                CanBeCapturedPos(-1, 1, tempSelectedRow, tempSelectedColumn);
            }
        }
        else
        {
            Piece? movePos1 = GetPosition(1, 0, tempSelectedRow, tempSelectedColumn);
            if (movePos1 != null && movePos1 == Piece._______) // Shows normal moves
            {
                CanBeMovedToPos(1, 0, tempSelectedRow, tempSelectedColumn);
                Piece? movePos2 = GetPosition(2, 0, tempSelectedRow, tempSelectedColumn);
                if (tempSelectedRow == 1 && movePos2 != null && movePos2 == Piece._______)
                {
                    CanBeMovedToPos(2, 0, tempSelectedRow, tempSelectedColumn);
                }
            }

            Piece? attackPos1 = GetPosition(1, 1, tempSelectedRow, tempSelectedColumn);
            if (attackPos1 != null && attackPos1 != Piece._______ && !isBlackPiece(attackPos1)) // Show capture moves
            {
                CanBeCapturedPos(1, 1, tempSelectedRow, tempSelectedColumn);
            }
            Piece? attackPos2 = GetPosition(1, -1, tempSelectedRow, tempSelectedColumn);
            if (attackPos2 != null && attackPos2 != Piece._______ && !isBlackPiece(attackPos2))
            {
                CanBeCapturedPos(1, -1, tempSelectedRow, tempSelectedColumn);
            }
        }

        // ----- Move Pawn -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the pawn if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn] == true) // Step forward
            {
                SetPiecePos(isWhite? Piece._Pawni_ : Piece._Pawn__, tempSelectedRow, tempSelectedColumn);
                Promotion();
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn] == true) // Capture
            {
                SetPiecePos(isWhite ? Piece._Pawni_ : Piece._Pawn__, tempSelectedRow, tempSelectedColumn);
                Promotion();
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }

        // ----- Promotion -----
        void Promotion()
        {
            void ReplacePiece(Piece whitePiece, Piece blackPiece, int i)
            {
                board[isWhite ? 0 : 7, i] = isWhite ? whitePiece : blackPiece;
                SetPiecePos(isWhite ? whitePiece : blackPiece, tempSelectedRow, tempSelectedColumn);
            }
            
            for (int i = 0; i < 7; i++)
            {
                if (board[0, i] == Piece._Pawni_ || board[7, i] == Piece._Pawn__)
                {
                    Console.ResetColor();
                    string[] options = {"Queen", "Rook", "Bishop", "Knight"};
                    int selected = 0;

                    while (board[0, i] == Piece._Pawni_ || board[7, i] == Piece._Pawn__)
                    {
                        Console.SetCursorPosition(0, 25);
                        for (int j = 0; j < options.Length; j++)
                        {
                            if (j == selected)
                            {
                                Console.WriteLine($"> {options[j]}");
                            }
                            else
                            {
                                Console.WriteLine($"  {options[j]}");

                            }
                        }

                        ConsoleKey Choice = Console.ReadKey(true).Key;
                        switch (Choice)
                        {
                            case ConsoleKey.DownArrow:
                                selected++;
                                break;

                            case ConsoleKey.UpArrow:
                                selected--;
                                break;

                            case ConsoleKey.Enter:
                                switch (selected)
                                {
                                    case 0:
                                        ReplacePiece(Piece.Queeni_, Piece._Queen_, i);
                                        break;

                                    case 1:
                                        ReplacePiece(Piece._Rooki_, Piece._Rook__, i);
                                        break;

                                    case 2:
                                        ReplacePiece(Piece.Bishopi, Piece.Bishop_, i);
                                        break;

                                    case 3:
                                        ReplacePiece(Piece.Knighti, Piece.Knight_, i);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}


void RookMove(int tempSelectedRow, int tempSelectedColumn) // ----- ROOK -----
{
    bool isWhite = IsWhite(Piece._Rooki_, tempSelectedRow, tempSelectedColumn);
    bool blockedUp = false;
    while (true)
    {
        if (isWhite)
        {

            for (int i = 0; i < 8; i++)
            {
                if (tempSelectedRow + i < 8)
                {
                    bool blocked = false;
                    if (board[tempSelectedRow + i, tempSelectedColumn] == Piece._______)
                    {
                        CanBeMovedToPos(i, 0, tempSelectedRow, tempSelectedColumn);
                    }
                    else
                    {
                        blocked = true;
                    }
                }
                if (tempSelectedRow - i >= 0)
                {
                    if (board[tempSelectedRow - i, tempSelectedColumn] != Piece._______)
                    {
                        blockedUp = true;
                    }
                    if (board[tempSelectedRow - i, tempSelectedColumn] == Piece._______ && !blockedUp)
                    {
                        CanBeMovedToPos(-i, 0, tempSelectedRow, tempSelectedColumn);
                    }
                }
                if (tempSelectedColumn + i < 8 && board[tempSelectedRow, tempSelectedColumn + i] == Piece._______)
                {
                    CanBeMovedToPos(0, i, tempSelectedRow, tempSelectedColumn);
                }
                if (tempSelectedColumn - i >= 0 && board[tempSelectedRow, tempSelectedColumn - i] == Piece._______)
                {
                    CanBeMovedToPos(0, -i, tempSelectedRow, tempSelectedColumn);
                }
            }
        }

        // ----- Move Rook -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the rook if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn] == true) // Move
            {
                SetPiecePos(isWhite ? Piece._Rooki_ : Piece._Rook__, tempSelectedRow, tempSelectedColumn);
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn] == true) // Capture
            {
                SetPiecePos(isWhite ? Piece._Rooki_ : Piece._Rook__, tempSelectedRow, tempSelectedColumn);
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }
    }
}

// ----------------------------- -----------------------------

void DrawBoard()
{
    ResetMoveData();
    Console.ResetColor();
    Console.Clear();
    for (int length = 0; length < 8; length++)
    {
        for (int height = 0; height < 8; height++)
        {
            Console.SetCursorPosition(3 + length * 10, 2 + height * 3);
            
            DrawSquare(height, length, canBeCaptured[height, length], canBeMovedTo[height, length]);
        }
    }
}
void UpdateSquare(int offsetRow, int offsetCol)
{
    DrawSquare(selectedRow + offsetRow, selectedColumn + offsetCol, false, false);
}
ConsoleKey MoveSelectedSquare() // Navigate the board
{
    ConsoleKey Navigate = Console.ReadKey(true).Key;
    switch (Navigate)
    {
        case ConsoleKey.UpArrow:
            if (selectedRow > 0)
            {
                selectedRow--;
                UpdateSquare(1, 0);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.DownArrow:
            if (selectedRow < 7)
            {
                selectedRow++;
                UpdateSquare(-1, 0);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.LeftArrow:
            if (selectedColumn > 0)
            {
                selectedColumn--;
                UpdateSquare(0, 1);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.RightArrow:
            if (selectedColumn < 7)
            {
                selectedColumn++;
                UpdateSquare(0, -1);
                UpdateSquare(0, 0);  
            }
            break;
    }
    return Navigate;
}

DrawBoard();

// Start of game loop
while (true)
{
    if (MoveSelectedSquare() == ConsoleKey.Enter)
    {
        if (board[selectedRow, selectedColumn] == Piece._Pawni_ ||
            board[selectedRow, selectedColumn] == Piece._Pawn__)
        {
            PawnMove(selectedRow, selectedColumn);
        }
        if (board[selectedRow, selectedColumn] == Piece._Rooki_ ||
            board[selectedRow, selectedColumn] == Piece._Rook__)
        {
            RookMove(selectedRow, selectedColumn);
        }
    }
}
enum Piece // List of possible pieces
{
    _Rooki_, _Rook__, Knighti, Knight_, Bishopi, Bishop_, _Kingi_, _King__, Queeni_, _Queen_, _Pawni_, _Pawn__, _______
}