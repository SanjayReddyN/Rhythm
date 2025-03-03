using System;

namespace RhythmGame.Core
{
    public class ScoreManager
    {
        private int score;

        public ScoreManager()
        {
            score = 0;
        }

        public void AddScore(int points)
        {
            score += points;
        }

        public void ResetScore()
        {
            score = 0;
        }

        public int GetScore()
        {
            return score;
        }
    }
}