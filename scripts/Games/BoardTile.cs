using Godot;

namespace BoardClassics.Games
{
  public abstract partial class BoardTile : Area2D
  {
    [Export(PropertyHint.Enum, "The shading of the tile on the board")]
    public BoardTileType TileType;

    [Export(PropertyHint.ResourceType, "The texture for the light tile")]
    public Texture2D LightTexture;

    [Export(PropertyHint.ResourceType, "The texture for the dark tile")]
    public Texture2D DarkTexture;

    [Export(hintString: "The sprite to represent the tile")]
    protected Sprite2D sprite;

    [Export(hintString: "The scene to instantiate a board piece")]
    private PackedScene boardPieceScene;

    /// <summary>
    /// The color to modulate the sprite when highlighted
    /// </summary>

    private static readonly Color MODULATED_COLOR = new Color(0, 192, 222, 0.3f);

    /// <summary>
    /// The default color of the sprite
    /// </summary>
    private static readonly Color DEFAULT_COLOR = new Color(1, 1, 1, 1);

    /// <summary>
    /// The board that this tile belongs to
    /// </summary>
    public CheckeredBoard Board => board;
    protected CheckeredBoard board;

    /// <summary>
    /// The x position of this tile on the board
    /// </summary>
    protected Vector2I boardPosition;

    /// <summary>
    /// The piece that is currently occupying this tile
    /// </summary>
    public BoardPiece OccupiedBy => occupiedBy;
    protected BoardPiece occupiedBy;

    /// <summary>
    /// Whether this tile is highlighted
    /// </summary>
    protected bool highlighted;

    /// <summary>
    /// Replaces the current piece with the given piece
    /// </summary>
    /// <param name="piece">The new piece to occupy this tile</param>
    public abstract void ReplaceWith(BoardPiece piece);

    /// <inheritdoc />
    public override void _Ready()
    {
      ZIndex = 99;
      sprite.Texture = TileType == BoardTileType.Light ? LightTexture : DarkTexture;
    }

    /// <summary>
    /// Adds a piece to this tile and instantiates a new piece if one doesn't exist
    /// </summary>
    /// <param name="pieceType">The type of piece to add</param>
    public void AddPiece(BoardPieceType pieceType)
    {
      if (occupiedBy != null)
      {
        occupiedBy.PieceType = pieceType;
        occupiedBy.Reset();
        return;
      }

      BoardPiece newPiece = boardPieceScene.Instantiate<BoardPiece>();
      newPiece.PieceType = pieceType;
      newPiece.Tile = this;
      AddChild(newPiece);
      occupiedBy = newPiece;
    }

    /// <summary>
    /// Sets the initial position of the tile
    /// </summary>
    /// <param name="board">The board the tile belongs to</param>
    /// <param name="x">The x coordinate of the tile</param>
    /// <param name="y">The y coordinate of the tile</param>
    public void SetInitialPosition(CheckeredBoard board, int x, int y)
    {
      this.board = board;
      boardPosition = new Vector2I(x, y);
    }

    /// <summary>
    /// Gets the board position of this tile
    /// </summary>
    public Vector2I GetBoardPosition()
    {
      return boardPosition;
    }

    /// <summary>
    /// Sets the size of the tile
    /// </summary>
    /// <param name="width">The new width of the tile</param>
    /// <param name="height">The new height of the tile</param>
    public void SetSize(float width, float height)
    {
      if (width <= 0 || height <= 0)
      {
        GD.PushWarning("Tile dimensions must be positive");
        return;
      }

      float scaleX = width / LightTexture.GetWidth();
      float scaleY = height / LightTexture.GetHeight();
      Scale = new Vector2(scaleX, scaleY);
    }

    /// <summary>
    /// Sets the highlight of the tile
    /// </summary>
    /// <param name="newValue">Whether the tile is highlighted or not</param>
    public void SetHighlight(bool newValue)
    {
      highlighted = newValue;
      if (highlighted) sprite.SelfModulate = MODULATED_COLOR;
      else sprite.SelfModulate = DEFAULT_COLOR;
    }
  }
}