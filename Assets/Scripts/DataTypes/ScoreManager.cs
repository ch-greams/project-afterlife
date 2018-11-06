using UnityEngine.UI;


public class ScoreManager
{
    public Text scoreText;

    public int scoreValue = 0;


    public ScoreManager() {}


    private void SetScoreValue(int scoreValue)
    {
        this.scoreValue = scoreValue;
        this.scoreText.text = string.Format("Score: {0:D4}", this.scoreValue);
    }

    public void IncrementScore(int deltaScoreValue)
    {
        this.SetScoreValue(this.scoreValue + deltaScoreValue);
    }
}
