using Code.Events;
using Code.Persistence;
using Code.Utilities.Enums;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private TextMeshProUGUI maxScoreText;
        private IEventManager _eventManager;
        private LocalPersistence _localPersistence;

        private void Start()
        {
            _eventManager = gameController.GetEventManager();
            _localPersistence = gameController.GetLocalPersistence();
            maxScoreText.text = _localPersistence.GetMaxScore().ToString();
            _eventManager.Subscribe(EventTypeEnum.PawnsHidden, OnPawnsHidden);
        }

        public void OnPlayButtonPressed()
        {
            gameObject.SetActive(false);
            _eventManager.TriggerEventAsync(EventTypeEnum.GameStarted);
        }

        private void OnPawnsHidden()
        {
            maxScoreText.text = _localPersistence.GetMaxScore().ToString();
            gameObject.SetActive(true);
        }
    }
}