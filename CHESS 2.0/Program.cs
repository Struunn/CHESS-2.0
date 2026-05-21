
// 2D arrays that keep track of certain square states
bool[,] canBeCaptured = new bool[8, 8];     // Keeps track of squares a piece can capture
bool[,] canBeMovedTo = new bool[8, 8];      // Keeps track of squares a piece can move to
bool[,] selectedSquare = new bool[8, 8];    // Keeps track of the square that is currently the selected piece

/// Resets the canBeCaptured, canBeMovedTo, and selectedSquare arrays
void ResetMoveData()
{
    for (int i = 0; i < 8; i++)
    {
        for (int j = 0; j < 8; j++)
        {
            canBeMovedTo[i, j] = false;
            canBeCaptured[i, j] = false;
            selectedSquare[i, j] = false;
        }
    }
}

/// Sets up the board in the starting setup
Piece?[,] board = new Piece?[,] { 
        { Piece._Rook__, Piece.Knight_, Piece.Bishop_, Piece._Queen_, Piece._King__, Piece.Bishop_, Piece.Knight_, Piece._Rook__ },
        { Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_ },
        { Piece._Rooki_, Piece.Knighti, Piece.Bishopi, Piece.Queeni_, Piece._Kingi_, Piece.Bishopi, Piece.Knighti, Piece._Rooki_ }};

/// Checks if the given square is occupied by a white piece
bool IsWhitePiece(Piece? piece)
{
    return
        piece == Piece._Pawni_ ||
        piece == Piece._Rooki_ ||
        piece == Piece.Knighti ||
        piece == Piece.Bishopi ||
        piece == Piece.Queeni_ ||
        piece == Piece._Kingi_;
}

/// Checks if the given square is occupied by a black piece
bool IsBlackPiece(Piece? piece)
{
    return
        piece == Piece._Pawn__ ||
        piece == Piece._Rook__ ||
        piece == Piece.Knight_ ||
        piece == Piece.Bishop_ ||
        piece == Piece._Queen_ ||
        piece == Piece._King__;
}

// Keeps track of the position of the selected (green) square
int selectedRow = 4;
int selectedColumn = 4;

/// Draws a square
void DrawSquare(int row, int col, bool canBeCaptured, bool canBeMovedTo, bool kingIsInCheck, bool selectedSquare) // Draws the square
{
    Piece? square = board[row, col];

    bool isSelected = (row == selectedRow && col == selectedColumn);

    bool isWhiteSquare;
    if (((col + row) & 1) == 0) // Checks if the square is white or not
    {
        isWhiteSquare = true;
    }
    else
    {
        isWhiteSquare = false;
    }

    Console.BackgroundColor = isSelected? ConsoleColor.Green :      // Sets the backgroundcolor depending on the different conditions
                              selectedSquare? ConsoleColor.Yellow :
                              kingIsInCheck? ConsoleColor.Blue :
                              canBeCaptured? ConsoleColor.Red :
                              canBeMovedTo? ConsoleColor.DarkYellow :
                              isWhiteSquare? ConsoleColor.White : ConsoleColor.DarkGray;

    if (square == Piece._Rooki_ || // Checks which side the piece on the square belongs to, changes its color accordingly
        square == Piece.Knighti ||
        square == Piece.Bishopi ||
        square == Piece.Queeni_ ||
        square == Piece._Kingi_ ||
        square == Piece._Pawni_)    // If it's white:
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    else if (square == Piece._______) // If it's none:
    {
        Console.ForegroundColor = isSelected? ConsoleColor.Green : isWhiteSquare? ConsoleColor.White : ConsoleColor.DarkGray;
    }
    else // If it's black:
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
    }
    
    // Draws the whole square
    if (row == 0) // Prevents it going out of bounds when row = 0
    {
        Console.SetCursorPosition(col * 7, row);
    }
    else
    {
        Console.SetCursorPosition(col * 7, row * 3 - 1);
    }
    Console.Write("       ");
    Console.SetCursorPosition(col * 7, row * 3);
    Console.Write(square);
    Console.SetCursorPosition(col * 7, 1 + row * 3);
    Console.Write("       ");
    Console.SetCursorPosition(col * 7, row * 3);
}


// ----------------------------- METHODS FOR PIECE MOVES -----------------------------

// Keeps track of both kings position and check(mate) state
int whiteKingRowPos = 7;
int whiteKingColPos = 4;
int blackKingRowPos = 0;
int blackKingColPos = 4;
bool whiteKingInCheck = false;
bool blackKingInCheck = false;
bool checkmate = false;

/// Sets the pieces position to the new selected position
void SetPiecePos(Piece piece, int tempSelectedRow, int tempSelectedColumn, int selectedRowOffset, int selectedColOffset, bool isWhite)
{
    board[tempSelectedRow, tempSelectedColumn] = Piece._______; // Sets old place to empty
    board[selectedRow + selectedRowOffset, selectedColumn + selectedColOffset] = piece; // Sets new place to piece
    DrawBoard(isWhite); // Updates every square
}

/// Sets the square to moveable
void CanBeMovedToPos(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    int row = tempSelectedRow + rowOffset;
    int col = tempSelectedColumn + colOffset;
    canBeMovedTo[row, col] = true;
    DrawSquare(row, col, false, true, false, false); // Redraws square to be able to move to
}

/// Sets the square to captureable
void CanBeCapturedPos(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn, int enPassantOffset)
{
    int row = tempSelectedRow + rowOffset;
    int col = tempSelectedColumn + colOffset;
    canBeCaptured[row, col] = true;
    DrawSquare(row, col, true, false, false, false); // Redraws square to be able to capture to
}

/// Gets the position and the piece that is on the requested square
Piece? GetPosition(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    Piece? position;
    int row = tempSelectedRow + rowOffset;
    int col = tempSelectedColumn + colOffset;

    if (row >= 0 && row < 8 && col >= 0 && col < 8) // Checks if it's within bounds
    {
        return position = board[row, col]; // Returns the piece on the requested square location
    }
    else
    {
        return null;
    }
}

/// Checks if the Piece on the square is white or not. Also sets the current piece position to be the selectedsquare
bool IsWhite(Piece piece, int tempSelectedRow, int tempSelectedColumn)
{
    // Set the current position to be the selectedsquare
    selectedSquare[tempSelectedRow, tempSelectedColumn] = true;

    if (GetPosition(0, 0, tempSelectedRow, tempSelectedColumn) == piece) // Determines which side the pawn belongs to
    {
        return true;
    }
    else
    {
        return false;
    }
}

/// Checks directions until it gets blocked / can capture
Piece? CanMoveUntilBlocked(int rowOffset, int colOffset, bool isWhite, int tempSelectedRow, int tempSelectedColumn, bool checkPiece, Piece currentPiece, bool onlyOnce, bool checkIfCanBlock)
{
    // Bools to hold the current state of the king checks
    bool whiteInCheck = whiteKingInCheck;
    bool blackInCheck = blackKingInCheck;

    for (int i = 1; i < 8; i++)
    {
        int row = tempSelectedRow + rowOffset * i;
        int col = tempSelectedColumn + colOffset * i;

        if (row < 0 || row > 7 || col < 0 || col > 7) // Stop if out of bounds
        {
            if (!checkPiece)
            {
                board[tempSelectedRow, tempSelectedColumn] = currentPiece;
            }
            return null;
        }

        if (!checkPiece)
        {
            board[tempSelectedRow, tempSelectedColumn] = Piece._______;
        }

        Piece? target = board[row, col];

        if (target == Piece._______) // Checks if the square is empty
        {
            if (!checkPiece) // checks if it needs to run every square or only the capturing ones
            {
                board[row, col] = currentPiece; // Sets square to the current piece

                if (isWhite? !WhiteKingCheck(0, 0, false) : !BlackKingCheck(0, 0, false)) // Checks if their sides king is in check now that they're in a different position
                {
                    board[row, col] = Piece._______; // Sets square back to empty

                    if (checkIfCanBlock) // Checks if it only return a bool or if it's an actual piece move
                    {
                        board[tempSelectedRow, tempSelectedColumn] = currentPiece; // Sets the current square back to the current piece
                        SetCheck(whiteInCheck, blackInCheck);
                        return Piece.CanBlock;
                    }
                    else
                    {
                        CanBeMovedToPos(rowOffset * i, colOffset * i, tempSelectedRow, tempSelectedColumn);
                    }
                }
                
                board[row, col] = Piece._______; // Sets the square back to empty in case it hasn't
                SetCheck(whiteInCheck, blackInCheck);
            }

            if (onlyOnce) // Checks if it needs to run only one time
            { break; }
        }
        else
        {
            if (isWhite? !IsWhitePiece(target) : !IsBlackPiece(target)) // Checks if it can capture on the square
            {
                Piece? captureablePiece = board[row, col]; // Stores the piece on that square
                board[row, col] = currentPiece; // Replaces the stored piece with the current piece

                if (checkPiece) // checks if it needs to run every square or only the capturing ones
                {
                    board[row, col] = captureablePiece; // Returns the stored piece to it's proper square
                    return board[row, col];
                }
                else if (isWhite? !WhiteKingCheck(0, 0, false) : !BlackKingCheck(0, 0, false)) // Checks if their sides king is in check now that they're in a different position
                {
                    board[row, col] = captureablePiece; // Returns the stored piece to it's proper square

                    if (checkIfCanBlock) // Checks if it only return a bool or if it's an actual piece move
                    {
                        board[tempSelectedRow, tempSelectedColumn] = currentPiece; // Returns the current piece to the start square
    
                        SetCheck(whiteInCheck, blackInCheck);
                        return Piece.CanBlock;
                    }
                    else
                    {
                        CanBeCapturedPos(rowOffset * i, colOffset * i, tempSelectedRow, tempSelectedColumn, 0);
                    }
                }
                board[row, col] = captureablePiece; // Returns the stored piece to it's proper square in case it hadn't
                SetCheck(whiteInCheck, blackInCheck);
            }
            break;
        }
    }

    if (!checkPiece)
    {
        board[tempSelectedRow, tempSelectedColumn] = currentPiece; // Returns the current piece to the start square in case it hadn't
    }
    return null;
}

/// Checks whether the piece in the selected direction is a piece that can see the king, and thus whether the king is in check
Piece? CheckDirection(int rowDirection, int colDirection, bool isWhite, int kingRowPos, int kingColPos, int kingRowOffset, int kingColOffset, Piece queen, Piece rook, Piece bishop, Piece knight, bool onlyOnce)
{
    // Gets the piece that the direction ends up hitting
    Piece? piece = CanMoveUntilBlocked(rowDirection, colDirection, isWhite, kingRowPos + kingRowOffset, kingColPos + kingColOffset, true, Piece._Kingi_, onlyOnce, false);

    if ((rowDirection == 0 || colDirection == 0) && (piece == queen || piece == rook)) // Checks if the direction was straight and if it hit Queen or Rook
    {
        if (isWhite)
        {
            whiteKingInCheck = true;
        }
        else
        {
            blackKingInCheck = true;
        }
    }
    else if ((rowDirection == 2 || rowDirection == -2 || colDirection == 2 || colDirection == -2) && (piece == knight)) // Checks if it had special movement and if it hit Knight
    {
        if (isWhite)
        {
            whiteKingInCheck = true;
        }
        else
        {
            blackKingInCheck = true;
        }
    }
    else if (((rowDirection == 1 || rowDirection == -1) && (colDirection == 1 || colDirection == -1)) && (piece == queen || piece == bishop))// Checks if the direction was diagonal and if it hit Queen or Bishop
    {
        if (isWhite)
        {
            whiteKingInCheck = true;
        }
        else
        {
            blackKingInCheck = true;
        }
    }
    return piece;
}

/// Checks if the white king is in check
bool WhiteKingCheck(int rowOffset, int colOffset, bool checkForCheckmate)
{
    bool somethingCanBlock = false;

    /// To avoid repetition and unnecessary block of text
    Piece? CheckDirectionShort(int rowDirection, int colDirection, bool onlyOnce)
    {
         return CheckDirection(rowDirection, colDirection, true, whiteKingRowPos, whiteKingColPos, rowOffset, colOffset, Piece._Queen_, Piece._Rook__, Piece.Bishop_,Piece.Knight_, onlyOnce);
    }

    whiteKingInCheck = false;

    // Checks every direction to see if any piece can see the king, thus the king being in check
    CheckDirectionShort(1, 0, false);
    CheckDirectionShort(-1, 0, false);
    CheckDirectionShort(0, 1, false);
    CheckDirectionShort(0, -1, false);
    CheckDirectionShort(1, 1, false);
    CheckDirectionShort(-1, -1, false);
    CheckDirectionShort(-1, 1, false);
    CheckDirectionShort(1, -1, false);
    CheckDirectionShort(2, 1, true);
    CheckDirectionShort(2, -1, true);
    CheckDirectionShort(-2, 1, true);
    CheckDirectionShort(-2, -1, true);
    CheckDirectionShort(1, 2, true);
    CheckDirectionShort(1, -2, true);
    CheckDirectionShort(-1, 2, true);
    CheckDirectionShort(-1, -2, true);

    // King specific checks to make sure the kings cannot go next to eachother
    if (CheckDirectionShort(1, 0, true) == Piece._King__ ||
        CheckDirectionShort(-1, 0, true) == Piece._King__ ||
        CheckDirectionShort(0, 1, true) == Piece._King__ ||
        CheckDirectionShort(0, -1, true) == Piece._King__ ||
        CheckDirectionShort(1, 1, true) == Piece._King__ ||
        CheckDirectionShort(-1, -1, true) == Piece._King__ ||
        CheckDirectionShort(-1, 1, true) == Piece._King__ ||
        CheckDirectionShort(1, -1, true) == Piece._King__)
    {
        whiteKingInCheck = true;
    }

    // Checks for pawns to check whether or not a pawn is checking the king
    if (whiteKingRowPos + rowOffset - 1 >= 0 && whiteKingColPos + colOffset + 1 < 8 && whiteKingColPos + colOffset - 1 >= 0)
    {
        if (board[whiteKingRowPos + rowOffset - 1, whiteKingColPos + colOffset + 1] == Piece._Pawn__ ||
        board[whiteKingRowPos + rowOffset - 1, whiteKingColPos + colOffset - 1] == Piece._Pawn__)
        {
            whiteKingInCheck = true;
        }
    }

    CheckMate(checkForCheckmate, Piece._Pawni_, Piece._Rooki_, Piece.Bishopi, Piece.Queeni_, Piece.Knighti, Piece._Kingi_, whiteKingInCheck);
    
    return whiteKingInCheck;
}

/// Checks if the black king is in check
bool BlackKingCheck(int rowOffset, int colOffset, bool checkForCheckmate)
{
    /// To avoid repetition and unnecessary block of text
    Piece? CheckDirectionShort(int rowDirection, int colDirection, bool onlyOnce)
    {
        return CheckDirection(rowDirection, colDirection, false, blackKingRowPos, blackKingColPos, rowOffset, colOffset, Piece.Queeni_, Piece._Rooki_, Piece.Bishopi, Piece.Knighti, onlyOnce);
    }

    blackKingInCheck = false;

    // Checks every direction to see if any piece can see the king, thus the king being in check
    CheckDirectionShort(1, 0, false);
    CheckDirectionShort(-1, 0, false);
    CheckDirectionShort(0, 1, false);
    CheckDirectionShort(0, -1, false);
    CheckDirectionShort(1, 1, false);
    CheckDirectionShort(-1, -1, false);
    CheckDirectionShort(-1, 1, false);
    CheckDirectionShort(1, -1, false);
    CheckDirectionShort(2, 1, true);
    CheckDirectionShort(2, -1, true);
    CheckDirectionShort(-2, 1, true);
    CheckDirectionShort(-2, -1, true);
    CheckDirectionShort(1, 2, true);
    CheckDirectionShort(1, -2, true);
    CheckDirectionShort(-1, 2, true);
    CheckDirectionShort(-1, -2, true);

    // King specific checks to make sure the kings cannot go next to eachother
    if (CheckDirectionShort(1, 0, true) == Piece._Kingi_ ||
        CheckDirectionShort(-1, 0, true) == Piece._Kingi_ ||
        CheckDirectionShort(0, 1, true) == Piece._Kingi_ ||
        CheckDirectionShort(0, -1, true) == Piece._Kingi_ ||
        CheckDirectionShort(1, 1, true) == Piece._Kingi_ ||
        CheckDirectionShort(-1, -1, true) == Piece._Kingi_ ||
        CheckDirectionShort(-1, 1, true) == Piece._Kingi_ ||
        CheckDirectionShort(1, -1, true) == Piece._Kingi_)
    {
        blackKingInCheck = true;
    }

    // Checks for pawns to check whether or not a pawn is checking the king
    if (blackKingRowPos + rowOffset + 1 < 8 && blackKingColPos + colOffset + 1 < 8 && blackKingColPos + colOffset - 1 >= 0)
    {
        if (board[blackKingRowPos + rowOffset + 1, blackKingColPos + colOffset + 1] == Piece._Pawni_ ||
        board[blackKingRowPos + rowOffset + 1, blackKingColPos + colOffset - 1] == Piece._Pawni_)
        {
            blackKingInCheck = true;
        }
    }

    CheckMate(checkForCheckmate, Piece._Pawn__, Piece._Rook__, Piece.Bishop_, Piece._Queen_, Piece._Queen_, Piece._King__, blackKingInCheck);

    return blackKingInCheck;
}

/// Checks for check
void CheckForCheck(bool isWhite)
{
    WhiteKingCheck(0, 0, true);
    BlackKingCheck(0, 0, true);
    DrawBoard(isWhite); // Makes sure the board gets updated with the new data
}

/// Sets both kings check to previous state
void SetCheck(bool whiteInCheck, bool blackInCheck)
{
    if (whiteInCheck)
    {
        whiteKingInCheck = true;
    }
    else
    {
        whiteKingInCheck = false;
    }
    if (blackInCheck)
    {
        blackKingInCheck = true;
    }
    else
    {
        blackKingInCheck = false;
    }
}

/// Checks for checkmate
void CheckMate(bool checkForCheckMate, Piece pawn, Piece rook, Piece bishop, Piece queen, Piece knight, Piece king, bool kingInCheck)
{
    bool somethingCanBlock = false;

    if (checkForCheckMate)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[j, i] == pawn)
                {
                    if (PawnMove(j, i, true))
                    {
                        somethingCanBlock = true;
                    }
                }
                else if (board[j, i] == rook)
                {
                    if (RookMove(j, i, true))
                    {
                        somethingCanBlock = true;
                    }
                }
                else if (board[j, i] == bishop)
                {
                    if (BishopMove(j, i, true))
                    {
                        somethingCanBlock = true;
                    }
                }
                else if (board[j, i] == queen)
                {
                    if (QueenMove(j, i, true))
                    {
                        somethingCanBlock = true;
                    }
                }
                else if (board[j, i] == knight)
                {
                    if (KnightMove(j, i, true))
                    {
                        somethingCanBlock = true;
                    }
                }
                else if (board[j, i] == king)
                {
                    if (KingMove(j, i, true))
                    {
                        somethingCanBlock = true;
                    }
                }
            }
        }
        if (!somethingCanBlock && kingInCheck) // Checkmate
        {
            Console.SetCursorPosition(0, 25);

            if (king == Piece._Kingi_) // If white loses
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("----------------");
                Console.WriteLine("Black side wins!");
                Console.WriteLine("----------------");
                Console.WriteLine("Press E to exit");

                while (!checkmate)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.E)
                    {
                        checkmate = true;
                    }
                }
            }
            else // If black loses
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("----------------");
                Console.WriteLine("White side wins!");
                Console.WriteLine("----------------");
                Console.WriteLine("Press E to exit");

                while (!checkmate)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.E)
                    {
                        checkmate = true;
                    }
                }
            }
        }
        else if (!somethingCanBlock && !kingInCheck) // Stalemate
        {
            Console.SetCursorPosition(0, 25);

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("----------------");
            Console.WriteLine("   STALEMATE    ");
            Console.WriteLine("----------------");
            Console.WriteLine("Press E to exit");

            while (!checkmate)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.E)
                {
                    checkmate = true;
                }
            }
        }
    }
}

bool isWhiteTurn = true;
void NewTurn()
{
    if (isWhiteTurn)
    {
        isWhiteTurn = false; // If it is white's turn, make it black's turn
    }
    else
    {
        isWhiteTurn = true; // If it is black's turn, make it white's turn
    }
}


// ----------------------------- PIECE MOVES -----------------------------

// Keeps track of Rook and King first moves for castling purposes
bool whiteRook1HasMoved = false;
bool whiteRook2HasMoved = false;
bool blackRook1HasMoved = false;
bool blackRook2HasMoved = false;
bool whiteKingHasMoved = false;
bool blackKingHasMoved = false;

// Keeps track of En Passant
int? whiteEnPassantCol = null;
int? blackEnPassantCol = null;

/// Controls the movement of the Pawn
bool PawnMove(int tempSelectedRow, int tempSelectedColumn, bool checkIfPieceCanBlock) // ----- PAWN -----
{
    /// So... The pawn kinda got out of hand
    /// I'll try to comment it to the best of my ability
    /// Good luck

    bool isWhite = IsWhite(Piece._Pawni_, tempSelectedRow, tempSelectedColumn);
    bool pieceCanblock = false;

    // Bools to hold the current state of the king checks
    bool whiteInCheck = whiteKingInCheck;
    bool blackInCheck = blackKingInCheck;

    int rowOffset;
    int colOffset;

    /// Gets the piece on the requested square
    Piece? TargetSquare(int RowOffset, int ColOffset)
    {
        rowOffset = RowOffset;
        colOffset = ColOffset;

        if (tempSelectedRow + rowOffset > 7 || tempSelectedRow + rowOffset < 0 ||   // Makes sure it stays in bounds
            tempSelectedColumn + colOffset > 7 || tempSelectedColumn + colOffset < 0)
        {
            return null;
        }

        Piece? targetSquare = board[tempSelectedRow + RowOffset, tempSelectedColumn + ColOffset];
        return targetSquare;
    }

    /// Handles the promotion
    void Promotion()
    {
        /// Replaces the pawn with the selected promotion piece
        void ReplacePiece(Piece whitePiece, Piece blackPiece, int col)
        {
            board[isWhite? 0 : 7, col] = isWhite ? whitePiece : blackPiece;
            SetPiecePos(isWhite? whitePiece : blackPiece, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
        }

        // Options for promotion
        string[] options = {$"{(isWhite? Piece.Queeni_ : Piece._Queen_)}",
                            $"{(isWhite? Piece._Rooki_ : Piece._Rook__)}",
                            $"{(isWhite? Piece.Bishopi : Piece.Bishop_)}",
                            $"{(isWhite? Piece.Knighti : Piece.Knight_)}"};

        // Keeps track of selected option
        int selected = 0;

        for (int i = 0; i < 8; i++) // To check each column
        {
            if (board[0, i] == Piece._Pawni_ || board[7, i] == Piece._Pawn__) // Checks if pawn is at either end of the board
            {
                Console.ResetColor();

                while (board[0, i] == Piece._Pawni_ || board[7, i] == Piece._Pawn__)
                {
                    // Loops the selection menu, preventing going out of bounds
                    if (selected < 0)
                    {
                        selected = 3;
                    }
                    else if (selected > 3)
                    {
                        selected = 0;
                    }

                    Console.SetCursorPosition(0, 25);
                    for (int j = 0; j < options.Length; j++)
                    {
                        if (j == selected) // Prints selected option
                        {
                            Console.BackgroundColor = isWhite? ConsoleColor.DarkCyan : ConsoleColor.DarkMagenta;
                            Console.WriteLine($"> {options[j]}");
                        }
                        else // Prints nonselected options
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.WriteLine($"  {options[j]}");
                        }
                    }

                    // Navigation of promotion menu
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

    /// Resets En Passant variables
    void ResetEnPassant()
    {
        // Reset variables to make sure en passant works properly
        whiteEnPassantCol = null;
        blackEnPassantCol = null;
    }

    /// Runs the capture logic for the pawn to check if it can capture
    void PawnCapture(int sideCapture)
    {
        /// Runs the capturing logic
        void Capture(Piece? captureablePiece)
        {
            if (checkIfPieceCanBlock) // Checks if it needs to just return a bool or actual move
            {
                board[isWhite ? tempSelectedRow - 1 : tempSelectedRow + 1, tempSelectedColumn + sideCapture] = captureablePiece;
                SetCheck(whiteInCheck, blackInCheck);
                pieceCanblock = true;
                return;
            }
            else
            {
                CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn, isWhite ? -1 : 1);
            }
        }

        if ((isWhite? tempSelectedRow - 1 < 0 : tempSelectedRow + 1 > 7) || tempSelectedColumn + sideCapture < 0 || tempSelectedColumn + sideCapture > 7)
        {
            return; // If out of bounds, stop
        }

        Piece? captureablePiece = TargetSquare(isWhite? -1 : 1, sideCapture); // Holds the captureable piece for later use
        board[isWhite? tempSelectedRow - 1 : tempSelectedRow + 1, tempSelectedColumn + sideCapture] = isWhite? Piece._Pawni_ : Piece._Pawn__; // Sets the square of the captureable piece to Pawn

        if (isWhite? !WhiteKingCheck(0, 0, false) : !BlackKingCheck(0, 0, false)) // Checks if the king is in check when the Pawn is in the new position
        {
            board[isWhite? tempSelectedRow - 1 : tempSelectedRow + 1, tempSelectedColumn + sideCapture] = captureablePiece; // Sets the square of the captureable piece back to the captureable piece

            if (tempSelectedColumn + sideCapture == whiteEnPassantCol) // Checks if en passant is possible
            {
                Capture(captureablePiece);
            }
            else if (tempSelectedColumn + sideCapture == blackEnPassantCol)
            {
                Capture(captureablePiece);
            }
            else
            {
                Capture(captureablePiece);
            }
        }

        board[isWhite? tempSelectedRow - 1 : tempSelectedRow + 1, tempSelectedColumn + sideCapture] = captureablePiece; // Sets the square of the captureable piece back to the captureable piece
        SetCheck(whiteInCheck, blackInCheck);
        return;
    }

    bool Move(int squaresForward)
    {
        board[isWhite ? tempSelectedRow - squaresForward : tempSelectedRow + squaresForward, tempSelectedColumn] = isWhite ? Piece._Pawni_ : Piece._Pawn__; // Sets the empty square to Pawn

        if (isWhite ? !WhiteKingCheck(0, 0, false) : !BlackKingCheck(0, 0, false)) // Checks if the king is in check when the Pawn is in the new position
        {
            board[isWhite ? tempSelectedRow - 1 : tempSelectedRow + 1, tempSelectedColumn] = Piece._______; // Sets the target square back to empty

            if (checkIfPieceCanBlock) // Checks if it needs to just return a bool or actual move
            {
                board[tempSelectedRow, tempSelectedColumn] = isWhite ? Piece._Pawni_ : Piece._Pawn__; // Sets start square back to pawn
                SetCheck(whiteInCheck, blackInCheck);
                return true;
            }
            else
            {
                CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
        }
        board[isWhite ? tempSelectedRow - squaresForward : tempSelectedRow + squaresForward, tempSelectedColumn] = Piece._______; // Sets the target square back to empty
        SetCheck(whiteInCheck, blackInCheck);
        return false;
    }

    while (true)
    {
        board[tempSelectedRow, tempSelectedColumn] = Piece._______; // Sets start square into empty

        if (TargetSquare(isWhite? -1 : 1, 0) == Piece._______) // Checks if the square right in front of the pawn is free
        {
            if (Move(1))
            {
                return true;
            }
        }

        if ((TargetSquare(isWhite? -1 : 1, 0) == Piece._______) && (TargetSquare(isWhite? -2 : 2, 0) == Piece._______) && (isWhite? tempSelectedRow == 6 : tempSelectedRow == 1)) // Checks if the square 2 in front of the pawn is free and if it's on its starting row
        {
            if (Move(2))
            {
                return true;
            }
        }
        board[tempSelectedRow, tempSelectedColumn] = isWhite? Piece._Pawni_ : Piece._Pawn__; // Sets start square back to Pawn

        if ((TargetSquare(isWhite? -1 : 1, 1) != Piece._______ && TargetSquare(isWhite ? -1 : 1, 1) != null) ||     // Checks if a piece is front right
           ((isWhite? tempSelectedColumn + 1 == whiteEnPassantCol : tempSelectedColumn + 1 == blackEnPassantCol) && // Checks if en passant is possible
           (isWhite? board[tempSelectedRow, tempSelectedColumn + 1] == Piece._Pawn__ : board[tempSelectedRow, tempSelectedColumn + 1] == Piece._Pawni_)))
        {
            if ((isWhite? !IsWhitePiece(TargetSquare(isWhite? -1 : 1, 1)) : !IsBlackPiece(TargetSquare(isWhite? -1 : 1, 1))) && TargetSquare(isWhite? -1 : 1, 1) != null)  // Checks if the target piece is enemy
            {
                PawnCapture(1);

                if (checkIfPieceCanBlock) // Checks if it needs to return a bool
                {
                    return pieceCanblock;
                }
            }
        }
        if ((TargetSquare(isWhite? -1 : 1, -1) != Piece._______ && TargetSquare(isWhite ? -1 : 1, -1) != null) ||   // Checks if a piece is front left
           ((isWhite? tempSelectedColumn - 1 == whiteEnPassantCol : tempSelectedColumn - 1 == blackEnPassantCol) && //Checks if en passant is possible
           (isWhite? board[tempSelectedRow, tempSelectedColumn - 1] == Piece._Pawn__ : board[tempSelectedRow, tempSelectedColumn - 1] == Piece._Pawni_))) 
        {
            if ((isWhite? !IsWhitePiece(TargetSquare(isWhite? -1 : 1, -1)) : !IsBlackPiece(TargetSquare(isWhite? -1 : 1, -1))) && TargetSquare(isWhite ? -1 : 1, -1) != null) // Checks if the target piece is enemy
            {
                PawnCapture(-1);

                if (checkIfPieceCanBlock) // Checks if it needs to return a bool
                {
                    return pieceCanblock;
                }
            }
        }

        if (checkIfPieceCanBlock) // Checks if it needs to return a bool
        {
            return false;
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                ResetEnPassant();

                if ((isWhite? tempSelectedRow == 6 : tempSelectedRow == 1) && (isWhite? selectedRow == 4 : selectedRow == 3)) // Checks if Pawn made a two square move
                {
                    if (isWhite)
                    {
                        blackEnPassantCol = tempSelectedColumn;
                    }
                    else
                    {
                        whiteEnPassantCol = tempSelectedColumn;
                    }
                }

                SetPiecePos(isWhite? Piece._Pawni_ : Piece._Pawn__, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                Promotion();
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                if (selectedColumn == whiteEnPassantCol || selectedColumn == blackEnPassantCol) // Checks if Pawn took en passant
                {
                    // Remove piece captured by en passant
                    if (isWhite)
                    {
                        board[selectedRow + 1, selectedColumn] = Piece._______;
                    }
                    else
                    {
                        board[selectedRow - 1, selectedColumn] = Piece._______;
                    }
                }

                ResetEnPassant();
                SetPiecePos(isWhite? Piece._Pawni_ : Piece._Pawn__, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                Promotion();
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard(isWhite);
                return false;
            }
        }
    }
}

/// Controls the movement of the Rook
bool RookMove(int tempSelectedRow, int tempSelectedColumn, bool checkIfPieceCanBlock) // ----- ROOK -----
{
    bool isWhite = IsWhite(Piece._Rooki_, tempSelectedRow, tempSelectedColumn);

    /// Keeps track of whether a rook has moved
    void FirsRookMove()
    {
        if (tempSelectedRow == 7 && tempSelectedColumn == 0)
        {
            whiteRook1HasMoved = true;
        }
        if (tempSelectedRow == 7 && tempSelectedColumn == 7)
        {
            whiteRook2HasMoved = true;
        }
        if (tempSelectedRow == 0 && tempSelectedColumn == 0)
        {
            blackRook1HasMoved = true;
        }
        if (tempSelectedRow == 0 && tempSelectedColumn == 7)
        {
            blackRook2HasMoved = true;
        }
    }

    /// To avoid repetition and unnecessary block of text
    Piece? CanMoveUntilBlockedShort(int rowDirection, int colDirection, bool checkIfCanBlock)
    {
        return CanMoveUntilBlocked(rowDirection, colDirection, isWhite, tempSelectedRow, tempSelectedColumn, false, isWhite? Piece._Rooki_ : Piece._Rook__, false, checkIfCanBlock);
    }

    while (true)
    {
        if (checkIfPieceCanBlock) // Checks if it needs to return a bool
        {
            // Check if the Rook can do anything to block check
            if (CanMoveUntilBlockedShort(1, 0, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(-1, 0, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(0, 1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(0, -1, true) == Piece.CanBlock)
            {
                return true;
            }
            return false;
        }
        else
        {
            // Runs directional movement
            CanMoveUntilBlockedShort(1, 0, false);
            CanMoveUntilBlockedShort(-1, 0, false);
            CanMoveUntilBlockedShort(0, 1, false);
            CanMoveUntilBlockedShort(0, -1, false);
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                FirsRookMove();
                SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                FirsRookMove();
                SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard(isWhite);
                return false;
            }
        }
    }
}

/// Controls the movement of the Bishop
bool BishopMove(int tempSelectedRow, int tempSelectedColumn, bool checkIfPieceCanBlock) // ----- BISHOP -----
{
    bool isWhite = IsWhite(Piece.Bishopi, tempSelectedRow, tempSelectedColumn);

    /// To avoid repetition and unnecessary block of text
    Piece? CanMoveUntilBlockedShort(int rowDirection, int colDirection, bool checkIfCanBlock)
    {
        return CanMoveUntilBlocked(rowDirection, colDirection, isWhite, tempSelectedRow, tempSelectedColumn, false, isWhite? Piece.Bishopi : Piece.Bishop_, false, checkIfCanBlock);
    }

    while (true)
    {
        if (checkIfPieceCanBlock) // Checks if it needs to return a bool
        {
            // Check if the Bishop can do anything to block check
            if (CanMoveUntilBlockedShort(1, 1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(-1, -1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(-1, 1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(1, -1, true) == Piece.CanBlock)
            {
                return true;
            }
            return false;
        }
        else
        {
            // Runs directional movement
            CanMoveUntilBlockedShort(1, 1, false);
            CanMoveUntilBlockedShort(-1, -1, false);
            CanMoveUntilBlockedShort(-1, 1, false);
            CanMoveUntilBlockedShort(1, -1, false);
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite ? Piece.Bishopi : Piece.Bishop_, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite ? Piece.Bishopi : Piece.Bishop_, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard(isWhite);
                return false;
            }
        }
    }
}

/// Controls the movement of the Queen
bool QueenMove(int tempSelectedRow, int tempSelectedColumn, bool checkIfPieceCanBlock) // ----- QUEEN -----
{
    bool isWhite = IsWhite(Piece.Queeni_, tempSelectedRow, tempSelectedColumn);

    /// To avoid repetition and unnecessary block of text
    Piece? CanMoveUntilBlockedShort(int rowDirection, int colDirection, bool checkIfCanBlock)
    {
        return CanMoveUntilBlocked(rowDirection, colDirection, isWhite, tempSelectedRow, tempSelectedColumn, false, isWhite? Piece.Queeni_ : Piece._Queen_, false, checkIfCanBlock);
    }

    while (true)
    {
        if (checkIfPieceCanBlock) // Checks if it needs to return a bool
        {
            // Check if the Queen can do anything to block check
            if (CanMoveUntilBlockedShort(1, 0, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(-1, 0, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(0, 1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(0, -1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(1, 1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(-1, -1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(-1, 1, true) == Piece.CanBlock ||
                CanMoveUntilBlockedShort(1, -1, true) == Piece.CanBlock)
            {
                return true;
            }
            return false;
        }
        else
        {
            // Runs directional movement
            CanMoveUntilBlockedShort(1, 0, false);
            CanMoveUntilBlockedShort(-1, 0, false);
            CanMoveUntilBlockedShort(0, 1, false);
            CanMoveUntilBlockedShort(0, -1, false);
            CanMoveUntilBlockedShort(1, 1, false);
            CanMoveUntilBlockedShort(-1, -1, false);
            CanMoveUntilBlockedShort(-1, 1, false);
            CanMoveUntilBlockedShort(1, -1, false);
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite? Piece.Queeni_ : Piece._Queen_, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite? Piece.Queeni_ : Piece._Queen_, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard(isWhite);
                return false;
            }
        }
    }
}

/// Controls the movement of the Knight
bool KnightMove(int tempSelectedRow, int tempSelectedColumn, bool checkIfPieceCanBlock)// ----- KNIGHT -----
{
    bool isWhite = IsWhite(Piece.Knighti, tempSelectedRow, tempSelectedColumn);

    /// Checks the movement of the Knight
    Piece? Move(int rowOffset, int colOffset, bool checkIfCanBlock)
    {
        // Bools to hold the current state of the king checks
        bool whiteInCheck = whiteKingInCheck;
        bool blackInCheck = blackKingInCheck;

        bool stop = false;

        int row = tempSelectedRow + rowOffset;
        int col = tempSelectedColumn + colOffset;

        if (row < 0 || row > 7 || col < 0 || col > 7) // Check if out of bounds
        {
            stop = true;
        }

        if (!stop) // If not out of bounds
        {
            board[tempSelectedRow, tempSelectedColumn] = Piece._______; // Sets start square to empty
            Piece? target = board[row, col];

            if (target == Piece._______)
            {
                board[row, col] = isWhite? Piece.Knighti : Piece.Knight_; // Sets target square to Knight

                if (isWhite? !WhiteKingCheck(0, 0, false) : !BlackKingCheck(0, 0, false)) // Checks if King is in check with new Knight position
                {
                    board[row, col] = Piece._______; // Sets target square back to empty

                    if (checkIfCanBlock) // Checks if it needs to return a bool
                    {
                        board[tempSelectedRow, tempSelectedColumn] = isWhite ? Piece.Knighti : Piece.Knight_; // Sets start square back to Knight
                        SetCheck(whiteInCheck, blackInCheck);
                        return Piece.CanBlock;
                    }
                    else
                    {
                        CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
                    }
                }

                board[row, col] = Piece._______; // Sets target square back to empty
                SetCheck(whiteInCheck, blackInCheck);
            }
            else if (isWhite? !IsWhitePiece(target) : !IsBlackPiece(target)) // Checks if target square is enemy piece
            {
                Piece? captureablePiece = board[row, col]; // Stores target squares piece
                board[row, col] = isWhite ? Piece.Knighti : Piece.Knight_; // Sets target square to Knight

                if (isWhite? !WhiteKingCheck(0, 0, false) : !BlackKingCheck(0, 0, false)) // Checks if King is in check with new Knight position
                {
                    board[row, col] = captureablePiece; // Sets target square back to enemy piece

                    if (checkIfCanBlock) // Checks if it needs to return a bool
                    {
                        board[tempSelectedRow, tempSelectedColumn] = isWhite ? Piece.Knighti : Piece.Knight_; // Sets start square back to Knight
                        SetCheck(whiteInCheck, blackInCheck);
                        return Piece.CanBlock;
                    }
                    else
                    {
                        CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn, 0);
                    }
                }

                board[row, col] = captureablePiece; // Sets target square back to enemy piece
                SetCheck(whiteInCheck, blackInCheck);
            }
            board[tempSelectedRow, tempSelectedColumn] = isWhite? Piece.Knighti : Piece.Knight_; // Sets start square back to Knight
        }
        return null;
    }

    while (true)
    {
        if (checkIfPieceCanBlock) // Checks if it needs to return a bool
        {
            // Check if the Knight can do anything to block check
            if (Move(2, 1, true) == Piece.CanBlock ||
                Move(2, -1, true) == Piece.CanBlock ||
                Move(-2, 1, true) == Piece.CanBlock ||
                Move(-2, -1, true) == Piece.CanBlock ||
                Move(1, 2, true) == Piece.CanBlock ||
                Move(1, -2, true) == Piece.CanBlock ||
                Move(-1, 2, true) == Piece.CanBlock ||
                Move(-1, -2, true) == Piece.CanBlock)
            {
                return true;
            }
            return false;
        }
        else
        {
            // Runs directional movement
            Move(2, 1, false);
            Move(2, -1, false);
            Move(-2, 1, false);
            Move(-2, -1, false);
            Move(1, 2, false);
            Move(1, -2, false);
            Move(-1, 2, false);
            Move(-1, -2, false);
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite? Piece.Knighti : Piece.Knight_, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite? Piece.Knighti : Piece.Knight_, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard(isWhite);
                return false;
            }
        }
    }
}

/// Controls the movement of the King
bool KingMove(int tempSelectedRow, int tempSelectedColumn, bool checkIfCanMoveOutCheck) // ----- KING -----
{
    bool isWhite = IsWhite(Piece._Kingi_, tempSelectedRow, tempSelectedColumn);

    /// Kings movement
    Piece? Move(int rowOffset, int colOffset, bool checkIfCanBlock)
    {
        // Bools to hold the current state of the king checks
        bool whiteInCheck = whiteKingInCheck;
        bool blackInCheck = blackKingInCheck;

        bool stop = false;

        int row = tempSelectedRow + rowOffset;
        int col = tempSelectedColumn + colOffset;        

        if (row < 0 || row > 7 || col < 0 || col > 7) // Check if out of bounds
        {
            stop = true;
        }

        if (!stop)
        {
            board[tempSelectedRow, tempSelectedColumn] = Piece._______; // Sets start square to empty
            Piece? target = board[row, col];

            if (target == Piece._______)
            {
                board[row, col] = isWhite? Piece._Kingi_ : Piece._King__; // Sets target square to King

                if (isWhite? !WhiteKingCheck(rowOffset, colOffset, false) : !BlackKingCheck(rowOffset, colOffset, false)) // Checks if King is in check with new King position
                {
                    board[row, col] = Piece._______; // Sets target square back to empty

                    if (checkIfCanBlock) // Checks if it needs to return a bool
                    {
                        board[tempSelectedRow, tempSelectedColumn] = isWhite ? Piece._Kingi_ : Piece._King__; // Sets start square back to King
                        SetCheck(whiteInCheck, blackInCheck);
                        return Piece.CanBlock;
                    }
                    else
                    {
                        CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
                    }
                }

                board[row, col] = Piece._______; // Sets target square back to empty
                SetCheck(whiteInCheck, blackInCheck);
            }
            else if (isWhite? !IsWhitePiece(target) : !IsBlackPiece(target)) // Checks if target square is enemy piece
            {
                Piece? captureablePiece = board[row, col]; // Stores target squares piece
                board[row, col] = isWhite ? Piece._Kingi_ : Piece._King__; // Sets target square to King

                if (isWhite? !WhiteKingCheck(rowOffset, colOffset, false) : !BlackKingCheck(rowOffset, colOffset, false)) // Checks if King is in check with new King position
                {
                    board[row, col] = captureablePiece; // Sets target square back to enemy piece

                    if (checkIfCanBlock) // Checks if it needs to return a bool
                    {
                        board[tempSelectedRow, tempSelectedColumn] = isWhite ? Piece._Kingi_ : Piece._King__; // Sets start square back to King
                        SetCheck(whiteInCheck, blackInCheck);
                        return Piece.CanBlock;
                    }
                    else
                    {
                        CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn, 0);
                    }
                }
                board[row, col] = captureablePiece; // Sets target square back to enemy piece
                SetCheck(whiteInCheck, blackInCheck);
            }
            board[tempSelectedRow, tempSelectedColumn] = isWhite ? Piece._Kingi_ : Piece._King__; // Sets start square back to King
        }
        return null;
    }

    /// Keeps track of whether the king has moved
    void KingHasMoved()
    {
        if (isWhite)
        {
            whiteKingHasMoved = true;
        }
        else
        {
            blackKingHasMoved = true;
        }
    }

    /// Updates the variables that hold the kings coordinates
    void KingsPosTracker()
    {
        if (isWhite)
        {
            whiteKingRowPos = selectedRow;
            whiteKingColPos = selectedColumn;
        }
        else
        {
            blackKingRowPos = selectedRow;
            blackKingColPos = selectedColumn;
        }
    }

    while (true)
    {
        if (checkIfCanMoveOutCheck) // Checks if it needs to return a bool
        {
            // Check if the King can move out of check
            if (Move(1, 0, true) == Piece.CanBlock ||
                Move(-1, 0, true) == Piece.CanBlock ||
                Move(0, 1, true) == Piece.CanBlock ||
                Move(0, -1, true) == Piece.CanBlock ||
                Move(1, 1, true) == Piece.CanBlock ||
                Move(-1, -1, true) == Piece.CanBlock ||
                Move(1, -1, true) == Piece.CanBlock ||
                Move(-1, 1, true) == Piece.CanBlock)
            {
                return true;
            }
            return false;
        }
        else
        {
            // Runs directional movement
            Move(1, 0, false);
            Move(-1, 0, false);
            Move(0, 1, false);
            Move(0, -1, false);
            Move(1, 1, false);
            Move(-1, -1, false);
            Move(1, -1, false);
            Move(-1, 1, false);
        }

        // Checks if castling is possible
        if (isWhite? !whiteKingHasMoved : !blackKingHasMoved) // Checks if king hasn't moved
        {
            if (isWhite? !whiteRook1HasMoved : !blackRook1HasMoved) // Checks if left Rook hasn't moved
            {
                if ((canBeMovedTo[tempSelectedRow, tempSelectedColumn - 1]) && // Checks if can move one square left
                    (board[tempSelectedRow, tempSelectedColumn - 2] == Piece._______) && // Checks if 2 squares left is free
                    (board[tempSelectedRow, tempSelectedColumn - 3] == Piece._______) && // Checks if 3 squares left is free
                    (isWhite? board[7, 0] == Piece._Rooki_ : board[0, 0] == Piece._Rook__)) // Checks if Rook is in left corner
                {
                    Move(0, -2, false);
                }
            }
            if (isWhite? !whiteRook2HasMoved : !blackRook2HasMoved) // Checks if right Rook hasn't moved
            {
                if ((canBeMovedTo[tempSelectedRow, tempSelectedColumn + 1]) && // Checks if can move one square right
                    (board[tempSelectedRow, tempSelectedColumn + 2] == Piece._______) && // Checks if 2 squares right is free
                    (isWhite ? board[7, 7] == Piece._Rooki_ : board[0, 7] == Piece._Rook__)) // Checks if Rook is in right corner
                {
                    Move(0, 2, false);
                }
            }
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                //Castling movement
                if (selectedColumn == tempSelectedColumn + 2)
                {
                    // Moves Rook to proper position when castling
                    SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, isWhite? 7 : 0, 7, 0, -1, isWhite);
                }
                if (selectedColumn == tempSelectedColumn - 2)
                {
                    // Moves Rook to proper position when castling
                    SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, isWhite ? 7 : 0, 0, 0, 1, isWhite);
                }

                KingsPosTracker();
                KingHasMoved();
                SetPiecePos(isWhite? Piece._Kingi_ : Piece._King__, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();
                return false;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                KingsPosTracker();
                KingHasMoved();
                SetPiecePos(isWhite? Piece._Kingi_ : Piece._King__, tempSelectedRow, tempSelectedColumn, 0, 0, isWhite);
                CheckForCheck(isWhite);
                NewTurn();

                return false;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard(isWhite);
                return false;
            }
        }
    }
}

// ----------------------------- -----------------------------

/// Draws the board
void DrawBoard(bool isWhite)
{
    ResetMoveData();
    Console.ResetColor();
    Console.Clear();

    for (int length = 0; length < 8; length++) // Row
    {
        for (int height = 0; height < 8; height++) // Column
        {
            Console.SetCursorPosition(3 + length * 10, 2 + height * 3);
            
            DrawSquare(height, length, canBeCaptured[height, length], canBeMovedTo[height, length],
                       (whiteKingInCheck && board[height, length] == Piece._Kingi_) || (blackKingInCheck && board[height, length] == Piece._King__),
                       selectedSquare[height, length]);
        }
    }
}

/// Updates the square to be redrawn with the current data
void UpdateSquare(int rowOffset, int colOffset)
{
    int row = selectedRow + rowOffset;
    int col = selectedColumn + colOffset;

    DrawSquare(selectedRow + rowOffset, selectedColumn + colOffset, false, false,
              (whiteKingInCheck && board[row, col] == Piece._Kingi_) || (blackKingInCheck && board[row, col] == Piece._King__), selectedSquare[row, col]);
}

/// Moves the selected square around
ConsoleKey MoveSelectedSquare() // Navigate the board
{
    ConsoleKey Navigate = Console.ReadKey(true).Key;
    switch (Navigate)
    {
        case ConsoleKey.UpArrow: // Moves up
            if (selectedRow > 0)
            {
                selectedRow--;
                UpdateSquare(1, 0);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.DownArrow: // Moves down
            if (selectedRow < 7)
            {
                selectedRow++;
                UpdateSquare(-1, 0);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.LeftArrow: // Moves left
            if (selectedColumn > 0)
            {
                selectedColumn--;
                UpdateSquare(0, 1);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.RightArrow: // Moves right
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

// Start of game loop
DrawBoard(true); // Draws the board at the start
while (!checkmate)
{
    if (MoveSelectedSquare() == ConsoleKey.Enter)
    {
        Piece? selectedMoveSquare = board[selectedRow, selectedColumn];

        if ((selectedMoveSquare == Piece._Pawni_ && isWhiteTurn)  || // Triggers Pawn movement code
            (selectedMoveSquare == Piece._Pawn__ && !isWhiteTurn))
        {
            PawnMove(selectedRow, selectedColumn, false);
        }

        if ((selectedMoveSquare == Piece._Rooki_ && isWhiteTurn) || // Trigger Rook movement code
            (selectedMoveSquare == Piece._Rook__ && !isWhiteTurn))
        {
            RookMove(selectedRow, selectedColumn, false);
        }

        if ((selectedMoveSquare == Piece.Bishopi && isWhiteTurn) || // Triggers Bishop movement code
            (selectedMoveSquare == Piece.Bishop_ && !isWhiteTurn))
        {
            BishopMove(selectedRow, selectedColumn, false);
        }

        if ((selectedMoveSquare == Piece.Queeni_ && isWhiteTurn) || // Triggers Queen movement code
            (selectedMoveSquare == Piece._Queen_ && !isWhiteTurn))
        {
            QueenMove(selectedRow, selectedColumn, false);
        }

        if ((selectedMoveSquare == Piece.Knighti && isWhiteTurn) || // Triggers Knight movement code
            (selectedMoveSquare == Piece.Knight_ && !isWhiteTurn))
        {
            KnightMove(selectedRow, selectedColumn, false);
        }

        if ((selectedMoveSquare == Piece._Kingi_ && isWhiteTurn) || // Triggers King movement code
            (selectedMoveSquare == Piece._King__ && !isWhiteTurn))
        {
            KingMove(selectedRow, selectedColumn, false);
        }
    }
}

enum Piece // List of possible pieces
{
    _Rooki_, _Rook__, Knighti, Knight_, Bishopi, Bishop_, _Kingi_, _King__, Queeni_, _Queen_, _Pawni_, _Pawn__, _______, CanBlock // Canblock only exists to return information in certain situations
}