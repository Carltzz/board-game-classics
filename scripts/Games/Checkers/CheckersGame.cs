using System.Collections.Generic;
using System.Linq;
using BoardClassics.UI;
using Godot;

namespace BoardClassics.Games.Checkers
{
  /// <summary>
  /// Provides the main game logic for a checkers game
  /// </summary>
  public partial class CheckersGame : Node
  {
    /// <summary>
    /// The player whose turn it is
    /// </summary>
    private BoardPieceType currentTurn;

    /// <summary>
    /// Reference to the checkers board
    /// </summary>
    [Export]
    private CheckersBoard Board;

    /// <summary>
    /// Reference to the scoreboard for the white player
    /// </summary>
    [Export]
    private Scoreboard WhiteScoreboard;

    /// <summary>
    /// Reference to the scoreboard for the black player
    /// </summary>
    [Export]
    private Scoreboard BlackScoreboard;

    /// <summary>
    /// The number of black pieces left on the board
    /// </summary>
    private int blackPiecesLeft;

    /// <summary>
    /// The number of white pieces left on the board
    /// </summary>
    private int whitePiecesLeft;

    /// <summary>
    /// The history of moves made in the game
    /// </summary>
    private Stack<Move> history;

    /// <summary>
    /// The static instance of the game
    /// </summary>
    public static CheckersGame Instance => instance;
    private static CheckersGame instance;

    /// <inheritdoc />
    public override void _Ready()
    {
      instance = this;
      history = new Stack<Move>();
      currentTurn = BoardPieceType.White;
      BeginTurn();
    }

    /// <summary>
    /// Highlights any moves the player can play
    /// </summary>
    /// <returns>True if there are any playable moves</returns>
    public bool HighlightMoveable()
    {
      bool anyValid = false;

      Board.ForEach(tile =>
      {
        if (tile.OccupiedBy.PieceType == currentTurn)
        {
          if (Board.GetValidMoves(tile.OccupiedBy).Any())
          {
            tile.SetHighlight(true);
            anyValid = true;
            return;
          }
        }

        tile.SetHighlight(false);
      });

      return anyValid;
    }

    /// <summary>
    /// Begins the turn for the current player. The game will end if the player has no moves
    /// </summary>
    public void BeginTurn()
    {
      // Highlight the moves
      if (!HighlightMoveable())
      {
        // No moves were highlighted which means the player can't move. GG.
        EndGame(currentTurn == BoardPieceType.White ? BoardPieceType.Black : BoardPieceType.White);
      }
    }

    /// <summary>
    /// Captures a piece and awards points to the capturing player
    /// </summary>
    /// <param name="piece">The piece being captured</param>
    public void CapturePiece(BoardPiece piece)
    {
      if (piece.PieceType == BoardPieceType.Black)
      {
        blackPiecesLeft--;
        WhiteScoreboard.AwardPoints(5);
        WhiteScoreboard.AwardCapture();
      }
      else
      {
        whitePiecesLeft--;
        BlackScoreboard.AwardPoints(5);
        BlackScoreboard.AwardCapture();
      }

      RemovePiece(piece);
    }

    /// <summary>
    /// Removes a piece from the board
    /// </summary>
    /// <param name="piece">The piece to remove from the board</param>
    public void RemovePiece(BoardPiece piece)
    {
      if (piece.PieceType == BoardPieceType.None)
      {
        GD.PushWarning("Cannot remove piece that is not on the board");
        return;
      }
      piece.PieceType = BoardPieceType.None;
      piece.Reset();
    }

    /// <summary>
    /// Adds a move to the history of the game
    /// </summary>
    /// <param name="move">The move to add to the history</param>
    public void PushHistory(Move move)
    {
      history.Push(move);
    }

    /// <summary>
    /// Ends the current turn and begins the next
    /// </summary>
    public void EndTurn()
    {
      currentTurn = currentTurn == BoardPieceType.White ? BoardPieceType.Black : BoardPieceType.White;
      BeginTurn();
    }

    /// <summary>
    /// Undoes the last move made in the game
    /// </summary>
    public void UndoMove()
    {
      if (history.Count == 0) return;
      Move move = history.Pop();
      Board.RevertMove(move);
      EndTurn();
    }

    /// <summary>
    /// Checks the state of the game to see if it has ended
    /// </summary>
    public void CheckState()
    {
      if (blackPiecesLeft == 0)
      {
        EndGame(BoardPieceType.White);
      }
      else if (whitePiecesLeft == 0)
      {
        EndGame(BoardPieceType.Black);
      }
    }

    /// <summary>
    /// Ends the game and displays the winner
    /// </summary>
    /// <param name="winner">The winner of the game</param>
    public void EndGame(BoardPieceType winner)
    {
      GD.Print("Game Over");
      GD.Print("Winner: " + winner);
      ResetGame();
    }

    /// <summary>
    /// Resets the game to its initial state
    /// </summary>
    public void ResetGame()
    {
      history.Clear();
      WhiteScoreboard.Reset();
      BlackScoreboard.Reset();
      Board.Reset();
    }

    /// <summary>
    /// Checks if it is the turn of the specified piece
    /// </summary>
    /// <param name="piece">The piece to check</param>
    /// <returns>Whether that piece is allowed to move</returns>
    public bool IsTurn(CheckersPiece piece)
    {
      return piece.PieceType == currentTurn;
    }
  }
}