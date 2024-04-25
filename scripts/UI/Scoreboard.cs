using Godot;

namespace BoardClassics.UI
{
  public partial class Scoreboard : PanelContainer
  {
    [Export]
    private Label Score;

    [Export]
    private Label Captures;

    private int numPoints;
    private int numCaptures;

    public void AwardPoints(int points)
    {
      numPoints += points;
      Score.Text = numPoints.ToString();
    }

    public void SetScore(int newScore)
    {
      numPoints = newScore;
      Score.Text = newScore.ToString();
    }

    public void AwardCapture()
    {
      numCaptures++;
      Captures.Text = numCaptures.ToString();
    }

    public void SetCaptures(int newCaptures)
    {
      numCaptures = newCaptures;
      Captures.Text = newCaptures.ToString();
    }

    public void Reset()
    {
      Score.Text = "0";
      Captures.Text = "0";
      numPoints = 0;
      numCaptures = 0;
    }
  }
}