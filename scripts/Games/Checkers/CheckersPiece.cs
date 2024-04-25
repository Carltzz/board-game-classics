using Godot;

namespace BoardClassics.Games.Checkers
{
  /// <summary>
  /// Provides a specific implementation for a checkers piece
  /// </summary>
  public partial class CheckersPiece : BoardPiece
  {
    [Export(PropertyHint.ResourceType, "The texture to represent a white king")]
    public Texture2D WhiteKingTexture;

    [Export(PropertyHint.ResourceType, "The texture to represent a black king")]
    public Texture2D BlackKingTexture;

    /// <summary>
    /// Whether this piece is a king
    /// </summary>
    public bool King { get; private set; }

    /// <inheritdoc />
    public override void _Ready()
    {
      SetTexture();
    }

    /// <summary>
    /// Reference to the checkers game instance
    /// </summary>
    private CheckersGame Game => CheckersGame.Instance;

    /// <inheritdoc />
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
      if (!Game.IsTurn(this)) return;
      base._InputEvent(viewport, @event, shapeIdx);
    }

    /// <inheritdoc />
    public override void SetTexture()
    {
      if (King)
      {
        sprite.Texture = PieceType == BoardPieceType.White ? WhiteKingTexture : BlackKingTexture;
        return;
      }

      base.SetTexture();
    }

    /// <summary>
    /// Applies the king status to this piece
    /// </summary>
    public void Promote()
    {
      King = true;
      SetTexture();
    }

    /// <inheritdoc />
    public override void Reset()
    {
      King = false;
      SetTexture();
    }
  }
}