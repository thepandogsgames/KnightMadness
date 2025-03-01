using Code.Events;
using Code.Utilities.Enums;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class ScoreUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameController gameController;
        private IEventManager _eventManager;
        private int _score;

        private void Start()
        {
            _eventManager = gameController.GetEventManager();
            _eventManager.Subscribe(EventTypeEnum.AddPoint, OnScoreAdded);
            _eventManager.Subscribe(EventTypeEnum.GameStarted, OnGameStarted);
            _eventManager.Subscribe(EventTypeEnum.PlayerEaten, OnPlayerEaten);
            gameObject.SetActive(false);
        }

        private void OnScoreAdded()
        {
            _score++;
            scoreText.text = _score.ToString();
        }

        private void OnGameStarted()
        {
            _score = 0;
            scoreText.text = _score.ToString();
            gameObject.SetActive(true);
        }

        private void OnPlayerEaten()
        {
            gameObject.SetActive(false);
        }
    }
}
