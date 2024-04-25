using Godot;

namespace BoardClassics.Games.Checkers
{
  /// <summary>
  /// Provides a specific implementation for a checkers board tile
  /// </summary>
  public partial class CheckersBoardTile : BoardTile
  {
    /// <inheritdoc />
    public override void ReplaceWith(BoardPiece piece)
    {
      CheckersBoardTile newTile = this;
      CheckersBoardTile oldTile = (CheckersBoardTile)piece.Tile;
      BoardPiece newPiece = piece;
      BoardPiece oldPiece = occupiedBy;

      if (oldPiece.PieceType != BoardPieceType.None)
      {
        GD.PushWarning("Tile is already occupied");
        return;
      }

      newTile.occupiedBy = newPiece;
      oldTile.occupiedBy = oldPiece;
      BoardPiece.SwapPositions(newPiece, oldPiece);

      newTile.RemoveChild(oldPiece);
      oldTile.RemoveChild(newPiece);
      newTile.AddChild(newPiece);
      oldTile.AddChild(oldPiece);
    }
  }
}