using UnityEngine;

namespace Code.Persistence
{
    public class LocalPersistence
    {
        private const string KeyMaxScore = "MAX_SCORE";
        
        public void SetMaxScore(int score)
        {
            PlayerPrefs.SetInt(KeyMaxScore, score);
            PlayerPrefs.Save();
        }

        public int GetMaxScore()
        {
            return PlayerPrefs.GetInt(KeyMaxScore, 0);
        }
    }
}
