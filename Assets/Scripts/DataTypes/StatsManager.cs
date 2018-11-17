using UnityEngine.UI;


public class StatsManager
{
    public Text scoreText;
    public Text turnCountText;

    private int scoreValue = 0;
    private int turnCountValue = 0;


    public StatsManager() {}


    private void SetScoreValue(int scoreValue)
    {
        this.scoreValue = scoreValue;
        this.scoreText.text = string.Format("Score: {0:D4}", this.scoreValue);
    }

    public void IncrementScore(int deltaScoreValue)
    {
        this.SetScoreValue(this.scoreValue + deltaScoreValue);
    }

    public void IncrementTurnCount()
    {
        this.turnCountText.text = string.Format("Turn: {0:D4}", ++this.turnCountValue);
    }
}
