using Code.Events;
using Code.Utilities.Enums;
using UnityEngine;

namespace Code.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        private IEventManager _eventManager;

        private void Start()
        {
            _eventManager = gameController.GetEventManager();
            _eventManager.Subscribe(EventTypeEnum.PawnsHidden, OnPawnsHidden);
        }

        public void OnPlayButtonPressed()
        {
            gameObject.SetActive(false);
            _eventManager.TriggerEventAsync(EventTypeEnum.GameStarted);
        }

        private void OnPawnsHidden()
        {
            gameObject.SetActive(true);
        }
    }
}