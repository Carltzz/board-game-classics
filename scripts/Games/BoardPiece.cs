using Godot;
using System.Collections.Generic;
using System.Linq;

namespace BoardClassics.Games
{
  /// <summary>
  /// Represents a piece on a CheckeredBoard
  /// </summary>
  public partial class BoardPiece : Area2D
  {
    [Export(PropertyHint.Enum, "The type of piece on the board")]
    public BoardPieceType PieceType;

    [Export(PropertyHint.ResourceType, "The texture for the white piece")]
    public Texture2D WhiteTexture;

    [Export(PropertyHint.ResourceType, "The texture for the black piece")]
    public Texture2D BlackTexture;

    [Export(hintString: "The sprite to represent the piece")]
    protected Sprite2D sprite;

    /// <summary>
    /// The board that this piece belongs to
    /// </summary>
    public CheckeredBoard Board => Tile.Board;

    /// <summary>
    /// The tile that this piece is on
    /// </summary>
    public BoardTile Tile { get; set; }

    /// <summary>
    /// Whether this piece is selected
    /// </summary>
    protected bool selected;

    /// <summary>
    /// The global mouse position of where the piece was selected
    /// </summary>
    protected Vector2 dragStart;

    /// <summary>
    /// The old position of the piece before it was selected
    /// </summary>
    protected Vector2 oldPosition;

    /// <summary>
    /// The closest tile that the piece is currently hovering over
    /// </summary>
    protected BoardTile closestTile;

    /// <summary>
    /// The valid moves that the piece can make
    /// </summary>
    protected IEnumerable<Move> validMoves;

    /// <inheritdoc />
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
      if (@event.IsActionPressed("Drag"))
      {
        Tile.ZIndex = 100; // Lift the piece above all other pieces
        selected = true;
        oldPosition = Position;
        dragStart = GetGlobalMousePosition();
        validMoves = Board.GetValidMoves(this);
        HighlightValidMoves(true);
      }

      if (@event.IsActionReleased("Drag") && selected)
      {
        Tile.ZIndex = 99; // Lower the piece back to its original Z index
        selected = false;
        SnapToClosetValidMove();
      }

      HighlightClosestTile();
    }

    /// <summary>
    /// Snaps the piece to the closest valid tile that it can move to
    /// </summary>
    private void SnapToClosetValidMove()
    {
      BoardTile tile = null;
      Move? foundMove = null;

      foreach (Move move in validMoves)
      {
        Board.GetTile(move.X, move.Y).SetHighlight(false);
        Vector2I pos = closestTile.GetBoardPosition();
        if (pos.X == move.X && pos.Y == move.Y)
        {
          tile = closestTile;
          foundMove = move;
        }
      }

      tile?.Board.ExecuteMove((Move)foundMove);
      if (tile != closestTile) closestTile?.SetHighlight(false);
      Position = Vector2.Zero;
    }

    /// <summary>
    /// Highlights the closest tile that the piece is currently hovering over
    /// </summary>
    private void HighlightClosestTile()
    {
      if (!selected) return;

      // Find all the tiles that the piece collides with
      var overlaps = GetOverlappingAreas().OfType<BoardTile>();

      float closestDistance = float.PositiveInfinity;
      BoardTile newClosestTile = null;

      foreach (BoardTile tile in overlaps)
      {
        float distance = GetGlobalMousePosition().DistanceTo(tile.GlobalPosition);
        if (distance < closestDistance)
        {
          closestDistance = distance;
          newClosestTile = tile;
        }
      }

      if (newClosestTile == closestTile) return;

      closestTile?.SetHighlight(false);
      newClosestTile?.SetHighlight(true);
      closestTile = newClosestTile;
    }

    /// <inheritdoc />
    public override void _Process(double delta)
    {
      if (selected)
      {
        Vector2 dragDelta = GetGlobalMousePosition() - dragStart;
        Position = oldPosition + (dragDelta / GetParent<BoardTile>().Scale);
      }
    }

    /// <summary>
    /// Sets the highlights of the valid moves that the piece can make
    /// </summary>
    /// <param name="newValue">What value to set the highlight to</param>
    protected void HighlightValidMoves(bool newValue)
    {
      if (validMoves == null) return;
      foreach (Move move in validMoves)
      {
        BoardTile tile = Board.GetTile(move.X, move.Y);
        tile.SetHighlight(newValue);
      }
    }

    /// <summary>
    /// Swaps the positions of two pieces
    /// </summary>
    /// <param name="newPiece">The new piece being moved</param>
    /// <param name="oldPiece">The piece occupying the tile to newPiece is moving to</param>
    public static void SwapPositions(BoardPiece newPiece, BoardPiece oldPiece)
    {
      (oldPiece.Tile, newPiece.Tile) = (newPiece.Tile, oldPiece.Tile);
    }

    /// <summary>
    /// Sets the texture of the piece based on the PieceType
    /// </summary>
    public virtual void SetTexture()
    {
      if (PieceType == BoardPieceType.None)
      {
        sprite.Texture = null;
        return;
      }

      sprite.Texture = PieceType == BoardPieceType.White ? WhiteTexture : BlackTexture;
    }

    /// <summary>
    /// Resets the piece to its default state
    /// </summary>
    public virtual void Reset() { }
  }
}