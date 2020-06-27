using Core;
using UnityEngine;

namespace Assets.Hub
{
    public class HubLoaderScript : MonoBehaviour
    {
        private void Start()
        {
            GameManager.GetEventHandler().onHubEntered.Invoke();
        }
    }
}
