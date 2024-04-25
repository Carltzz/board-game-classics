using Godot;

namespace BoardClassics.UI
{
  public partial class MainMenu : Control
  {
    #region MAIN MENU
    [Export(hintString: "The to select which game to play")]
    public Button PlayButton;

    [Export(hintString: "The button to access the options menu")]
    public Button OptionsButton;

    [Export(hintString: "The button to access the about menu")]
    public Button AboutButton;

    [Export(hintString: "The button to exit the game")]
    public Button ExitButton;
    #endregion

    #region SELECT GAME MODE
    [Export(hintString: "The button to play checkers")]
    public Button PlayCheckersButton;

    [Export(hintString: "The button to go back to the main menu")]
    public Button SelectGameBackButton;
    #endregion

    /// <summary>
    /// Reference to the main menu control
    /// </summary>
    private Control menuMain;

    /// <summary>
    /// Reference to the select game mode control
    /// </summary>
    private Control menuPickGame;

    /// <inheritdoc />
    public override void _Ready()
    {
      menuMain = GetNode<Control>("MainMenu");
      menuPickGame = GetNode<Control>("SelectGameMode");

      PlayButton.Pressed += OnPlayButtonPressed;
      ExitButton.Pressed += OnExitButtonPressed;
      SelectGameBackButton.Pressed += OnSelectGameBackButtonPressed;
      PlayCheckersButton.Pressed += StartCheckers;
    }

    /// <summary>
    /// Called when the play button is pressed
    /// </summary>
    public void OnPlayButtonPressed()
    {
      menuMain.Visible = false;
      menuPickGame.Visible = true;
    }

    /// <summary>
    /// Called when the select game back button is pressed
    /// </summary>
    public void OnSelectGameBackButtonPressed()
    {
      menuMain.Visible = true;
      menuPickGame.Visible = false;
    }

    /// <summary>
    /// Called when the exit button is pressed
    /// </summary>
    public void OnExitButtonPressed()
    {
      GetTree().Quit();
    }

    /// <summary>
    /// Starts the checkers game
    /// </summary>
    public void StartCheckers()
    {
      GetTree().ChangeSceneToFile("res://scenes/checkers.tscn");
    }
  }
}