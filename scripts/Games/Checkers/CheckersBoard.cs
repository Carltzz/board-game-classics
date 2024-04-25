using Godot;
using System.Collections.Generic;

namespace BoardClassics.Games.Checkers
{
  /// <summary>
  /// Provides functionality for a checkers specific board
  /// </summary>
  public partial class CheckersBoard : CheckeredBoard
  {
    /*
     * Checkers pieces here have no variants, so we don't need to define additional
     * piece types. We can use the BoardPieceType enum from the base class.
     */

    /// <summary>
    /// The y coordinate where the black half of the board ends on a standard checkers board
    /// </summary>
    public const int BLACK_HALF_END = 3;

    /// <summary>
    /// The y coordinate where the white half of the board starts on a standard checkers board
    /// </summary>
    public const int WHITE_HALF_START = 4;

    /// <summary>Reference to checkers game instance</summary>
    private CheckersGame Game => CheckersGame.Instance;

    /// <inheritdoc />
    protected override void AddInitialPiece(int x, int y)
    {
      if (!InBounds(x, y))
      {
        GD.PushWarning("Cannot add piece in out of bounds position");
        return;
      }

      var pieceType = BoardPieceType.None;
      if (IsInitialBlackPiece(x, y)) pieceType = BoardPieceType.Black;
      else if (IsInitialWhitePiece(x, y)) pieceType = BoardPieceType.White;
      tiles[x, y].AddPiece(pieceType);
    }

    /// <inheritdoc />
    public override IEnumerable<Move> GetValidMoves(BoardPiece piece)
    {
      Vector2I pos = piece.Tile.GetBoardPosition();

      Move initial = new()
      {
        CapturedPieces = new List<BoardPiece>(),
        InitialX = pos.X,
        InitialY = pos.Y,
        Piece = piece,
        X = pos.X,
        Y = pos.Y,
      };

      List<Move> moves = new();
      CheckersPiece boardPiece = (CheckersPiece)piece;

      // Black pieces move down, white pieces move up
      int dy = boardPiece.PieceType == BoardPieceType.Black ? 1 : -1;

      // Begin the search for all possible moves
      CheckAllDirections(moves, boardPiece, initial, pos.X, pos.Y, dy, false);
      return moves;
    }

    /// <inheritdoc />
    public override void ExecuteMove(Move move)
    {
      CheckersPiece checkersPiece = (CheckersPiece)move.Piece;

      // We can promote the piece if it reaches the opposite end of the board
      if (checkersPiece.PieceType == BoardPieceType.Black && move.Y == 7) { checkersPiece.Promote(); move.Promoted = true; }
      if (checkersPiece.PieceType == BoardPieceType.White && move.Y == 0) { checkersPiece.Promote(); move.Promoted = true; }

      // Remove all the captured pieces from the board
      move.CapturedPieces.ForEach(captured => Game.CapturePiece(captured));

      // Move the piece to the target tile
      move.Target.ReplaceWith(move.Piece);

      // Register the move and end the turn
      Game.PushHistory(move);
      Game.EndTurn();
    }

    /// <inheritdoc />
    public override void RevertMove(Move move)
    {
      // Move the piece back to its original position
      BoardTile originalTile = GetTile(move.InitialX, move.InitialY);
      originalTile.ReplaceWith(move.Piece);

      // Restore captured pieces
      foreach (CheckersPiece captured in move.CapturedPieces)
      {
        captured.PieceType = move.Piece.PieceType == BoardPieceType.Black ? BoardPieceType.White : BoardPieceType.Black;
        captured.SetTexture();
      }

      // Demote the piece if it was promoted by this move
      if (move.Promoted)
      {
        CheckersPiece checkersPiece = (CheckersPiece)move.Piece;
        checkersPiece.Reset();
      }
    }

    /// <summary>
    /// The black pieces will start on the top 3 rows on dark tiles
    /// </summary>
    /// <param name="x">The x coordinate to check</param>
    /// <param name="y">The y coordinate to check</param>
    /// <returns>Whether the (x,y) coordinate represents a starting black piece</returns>
    private bool IsInitialBlackPiece(int x, int y)
    {
      return y < BLACK_HALF_END && tiles[x, y].TileType == BoardTileType.Dark;
    }

    /// <summary>
    /// The white pieces will start on the bottom 3 rows on dark tiles
    /// </summary>
    /// <param name="x">The x coordinate to check</param>
    /// <param name="y">The y coordinate to check</param>
    /// <returns>Whether the (x,y) coordinate represents a starting white piece</returns>
    private bool IsInitialWhitePiece(int x, int y)
    {
      return y > WHITE_HALF_START && tiles[x, y].TileType == BoardTileType.Dark;
    }

    /// <summary>
    /// Auxiliary method to add all possible moves for a piece
    /// </summary>
    /// <param name="moves">The current list of valid moves</param>
    /// <param name="piece">The piece being moved on the board</param>
    /// <param name="move">If there are multiple jumps, this represents the previous move</param>
    /// <param name="x">The virtual x position of the piece</param>
    /// <param name="y">The virtual y position of the piece</param>
    /// <param name="dy">The forward direction of the piece</param>
    /// <param name="mustJump">Whether the piece can only perform jump moves</param>
    private void CheckAllDirections(List<Move> moves, CheckersPiece piece, Move move, int x, int y, int dy, bool mustJump)
    {
      // Normal pieces can only move in their local forward direction
      moves.AddRange(GetMoves(moves, piece, move, x, y, 1, dy, mustJump));
      moves.AddRange(GetMoves(moves, piece, move, x, y, -1, dy, mustJump));

      // Kings can move in both directions
      if (piece.King)
      {
        moves.AddRange(GetMoves(moves, piece, move, x, y, 1, -dy, mustJump));
        moves.AddRange(GetMoves(moves, piece, move, x, y, -1, -dy, mustJump));
      }
    }

    /// <summary>
    /// Auxiliary method to search for all possible moves in a given direction
    /// </summary>
    /// <param name="moves">The current list of valid moves</param>
    /// <param name="piece">The piece being moved on the board</param>
    /// <param name="move">If there are multiple jumps, this represents the previous move</param>
    /// <param name="x">The virtual x position of the piece</param>
    /// <param name="y">The virtual y position of the piece</param>
    /// <param name="dx">The target x movement of the piece</param>
    /// <param name="dy">The target y movement of the piece</param>
    /// <param name="mustJump">Whether the move has to be a jump move</param>
    /// <returns>All the possible moves it the player begins moving in the (dx,dy) direction</returns>
    private List<Move> GetMoves(List<Move> moves, BoardPiece piece, Move move, int x, int y, int dx, int dy, bool mustJump)
    {
      // The piece itself can't be out of bounds. This should not have occurred
      if (!InBounds(x, y))
      {
        GD.PushWarning("Piece detected out of bounds");
        return moves;
      }

      // Calculate the new position of the piece
      int newX = x + dx;
      int newY = y + dy;

      // If the target position is off the board, we can stop
      if (!InBounds(newX, newY)) return moves;

      var target = (CheckersPiece)GetPiece(newX, newY);

      // We shouldn't move to a spot occupied by a piece of the same type, we can stop
      if (target.PieceType == piece.PieceType) return moves;

      // We have found an empty square, but this isn't a jump move
      if (target.PieceType == BoardPieceType.None)
      {
        // If we have to jump, then this isn't a valid move
        if (mustJump) return moves;

        Move newMove = CloneMove(move, newX, newY);
        moves.Add(newMove);
        return moves;
      }

      // We have found a piece to capture, so calculate the jump position
      int captureX = newX + dx;
      int captureY = newY + dy;

      // If we have to jump, but the capture position is out of bounds, we can stop
      if (!InBounds(captureX, captureY)) return moves;

      var jumpPiece = (CheckersPiece)GetPiece(captureX, captureY);

      // We must ensure the capture position is empty and that our previous move(s) didn't capture this piece
      if (jumpPiece.PieceType == BoardPieceType.None && !move.CapturedPieces.Contains(target))
      {
        Move newMove = CloneMove(move, captureX, captureY);
        newMove.CapturedPieces.Add(target);
        moves.Add(newMove);

        // If we can jump again, we must do so. Here we must set mustJump to true
        CheckAllDirections(moves, (CheckersPiece)piece, newMove, captureX, captureY, dy, true);
      }

      // Once the recursion is done, we can return the list of moves
      return moves;
    }

    /// <summary>
    /// Auxiliary method to clone a move
    /// </summary>
    /// <param name="move">The move to clone</param>
    /// <param name="newX">The new x position of the piece</param>
    /// <param name="newY">The new y position of the piece</param>
    /// <returns></returns>
    private Move CloneMove(Move move, int newX, int newY)
    {
      Move newMove = new()
      {
        Target = GetTile(newX, newY),
        CapturedPieces = new List<BoardPiece>(move.CapturedPieces),
        InitialX = move.InitialX,
        InitialY = move.InitialY,
        Piece = move.Piece,
        X = newX,
        Y = newY,
      };

      return newMove;
    }
  }
}