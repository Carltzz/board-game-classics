using System;
using System.Collections.Generic;
using Godot;

namespace BoardClassics.Games
{
  /// <summary>
  /// Represents a checkered board for a board game
  /// </summary>

  public abstract partial class CheckeredBoard : Area2D
  {
    [Export(PropertyHint.Range, "The number of rows on the checkered board")]
    public int Rows = 8;

    [Export(PropertyHint.Range, "The number of columns on the checkered board")]
    public int Cols = 8;

    [Export(hintString: "The scene to use for the board tiles")]
    protected PackedScene boardTileScene;

    [Export(hintString: "The collider for the board")]
    protected CollisionShape2D collider;

    /// <summary>
    /// The tiles on the board
    /// </summary>
    protected BoardTile[,] tiles;

    /// <summary>
    /// The bounds of the board
    /// </summary>
    protected RectangleShape2D bounds;

    /// <summary>
    /// Adds the initial piece to the board that occupies the given position
    /// </summary>
    protected abstract void AddInitialPiece(int x, int y);

    /// <summary>
    /// Gets the valid moves for the given piece
    /// </summary>
    /// <param name="piece">The target piece to move</param>
    /// <returns>A list of all possible moves the piece can make</returns>
    public abstract IEnumerable<Move> GetValidMoves(BoardPiece piece);

    /// <summary>
    /// Executes the given move on the board
    /// </summary>
    /// <param name="piece">The move to perform</param>
    public abstract void ExecuteMove(Move move);

    /// <summary>
    /// Reverts the given move on the board
    /// </summary>
    /// <param name="move">The move to undo</param>
    public abstract void RevertMove(Move move);

    /// <inheritdoc />
    public override void _Ready()
    {
      tiles = new BoardTile[Rows, Cols];
      bounds = (RectangleShape2D)collider.Shape;

      for (int i = 0; i < Rows; i++)
      {
        for (int j = 0; j < Cols; j++)
        {
          AddTile(i, j);
          AddInitialPiece(i, j);
        }
      }
    }

    /// <summary>
    /// Adds a tile to the board at the given position
    /// </summary>
    /// <param name="x">The x position to add the tile</param>
    /// <param name="y">The y position to add the tile</param>
    protected void AddTile(int x, int y)
    {
      bool dark = (x + y) % 2 == 1;
      float width = bounds.Size.X / Rows;
      float height = bounds.Size.Y / Cols;
      float adjustedX = x - Cols / 2;
      float adjustedY = y - Rows / 2;

      BoardTileType tileType = dark ? BoardTileType.Dark : BoardTileType.Light;
      BoardTile tile = boardTileScene.Instantiate<BoardTile>();

      tile.TileType = tileType;
      tile.Position = new Vector2(adjustedX * width, adjustedY * height);
      tile.SetInitialPosition(this, x, y);
      tile.SetSize(width, height);

      tiles[x, y] = tile;
      AddChild(tile);
    }

    /// <summary>
    /// Checks if the given position is within the bounds of the board
    /// </summary>
    /// <param name="x">The x position to check</param>
    /// <param name="y">The y position to check</param>
    /// <returns></returns>
    public bool InBounds(int x, int y)
    {
      return x >= 0 && x < Cols && y >= 0 && y < Rows;
    }

    /// <summary>
    /// Gets the piece at the given position on the board
    /// </summary>
    /// <param name="x">The x position to look for</param>
    /// <param name="y">The y position to look for</param>
    /// <returns>The piece found at that position, 
    /// or null if the position is out of bounds</returns>
    public BoardPiece GetPiece(int x, int y)
    {
      if (!InBounds(x, y))
      {
        GD.PushWarning("Searching for piece out of bounds");
        return null;
      }
      return tiles[x, y].OccupiedBy;
    }

    /// <summary>
    /// Gets the tile at the given position on the board
    /// </summary>
    /// <param name="x">The x position to look for</param>
    /// <param name="y">The y position to look for</param>
    /// <returns>The tile found at that position,
    /// or null if the position is out of bounds</returns>
    public BoardTile GetTile(int x, int y)
    {
      if (!InBounds(x, y))
      {
        GD.PushWarning("Searching for tile out of bounds");
        return null;
      }
      return tiles[x, y];
    }

    /// <summary>
    /// Iterates over all the tiles on the board
    /// </summary>
    /// <param name="action">The action to apply to all tiles</param>
    public void ForEach(Action<BoardTile> action)
    {
      for (int i = 0; i < Rows; i++)
      {
        for (int j = 0; j < Cols; j++)
        {
          action(tiles[i, j]);
        }
      }
    }

    /// <summary>
    /// Resets the board to its initial state
    /// </summary>
    public void Reset()
    {
      for (int i = 0; i < 8; i++)
      {
        for (int j = 0; j < 8; j++)
        {
          AddInitialPiece(i, j);
        }
      }
    }
  }
}