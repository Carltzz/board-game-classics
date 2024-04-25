using Godot;

namespace BoardClassics.Games.Checkers
{
  /// <summary>
  /// Provides the main UI for a checkers game
  /// </summary>
  public partial class CheckersUI : CanvasLayer
  {
    [Export(hintString: "The button to reset the game")]
    private Button ResetButton;

    [Export(hintString: "The button to undo the last move")]
    private Button UndoButton;

    [Export(hintString: "The button to exit the game")]
    private Button ExitButton;

    /// <summary>
    /// Reference to the checkers game instance
    /// </summary>
    private CheckersGame Game => CheckersGame.Instance;

    /// <inheritdoc />
    public override void _Ready()
    {
      ResetButton.Pressed += () => CheckersGame.Instance.ResetGame();
      ExitButton.Pressed += GoMainMenu;
      UndoButton.Pressed += () => CheckersGame.Instance.UndoMove();
    }

    /// <summary>
    /// Returns to the main menu
    /// </summary>
    public void GoMainMenu()
    {
      GetTree().ChangeSceneToFile("res://scenes/mainMenu.tscn");
    }
  }
}