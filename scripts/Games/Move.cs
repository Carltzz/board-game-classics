using System.Collections.Generic;

namespace BoardClassics.Games
{
  /// <summary>
  /// Represents a move made by a player in a board game
  /// </summary>
  public struct Move
  {
    /// <summary>
    /// The initial x coordinate of the piece prior to the move
    /// </summary>
    public int InitialX;

    /// <summary>
    /// The initial y coordinate of the piece prior to the move
    /// </summary>
    public int InitialY;

    /// <summary>
    /// The piece being moved
    /// </summary>
    public BoardPiece Piece;

    /// <summary>
    /// The x coordinate of the piece after the move
    /// </summary>
    public int X;

    /// <summary>
    /// The y coordinate of the piece after the move
    /// </summary>
    public int Y;

    /// <summary>
    /// The pieces captured during the move
    /// </summary>
    public List<BoardPiece> CapturedPieces;

    /// <summary>
    /// The target tile of the move
    /// </summary>
    public BoardTile Target;

    /// <summary>
    /// Whether the piece was promoted during the move
    /// </summary>
    public bool Promoted;
  }
}