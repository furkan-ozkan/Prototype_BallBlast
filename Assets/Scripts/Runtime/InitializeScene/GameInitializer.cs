using HappyEvents;
using UnityEngine;
using Scripts.ServiceProvider;
using UnityEngine.SceneManagement;

namespace Runtime.InitializeScene
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private EventBusAsset _globalEventBus;
        private void Start()
        {
            ServiceProvider.Register(_globalEventBus);
            SceneManager.LoadScene(1);
        }
    }
}