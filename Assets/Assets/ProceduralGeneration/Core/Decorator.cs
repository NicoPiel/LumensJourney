using Assets.PickUps.Scripts;
using Unity.Burst;
using UnityEngine;

namespace Assets.ProceduralGeneration.Core
{
    [BurstCompile]
    public class Decorator : MonoBehaviour
    {
        private Generator _generator;

        private void Start()
        {
            _generator = GetComponent<Generator>();
        }

        public bool Decorate()
        {
            if (_generator.Generated)
            {
                Rect firstRoom = _generator.GetRooms()[0];

                var pickUp = (GameObject) Instantiate(UnityEngine.Resources.Load("PickUp"),
                    new Vector3(firstRoom.x + firstRoom.width / 2 + 1, firstRoom.y + firstRoom.height / 2 + 1),
                    Quaternion.identity,
                    _generator.GetParent().transform);
                
                var slime = (GameObject) Instantiate(UnityEngine.Resources.Load("Slime"),
                    new Vector3(firstRoom.x + firstRoom.width / 2, firstRoom.y + firstRoom.height / 2),
                    Quaternion.identity,
                    _generator.GetParent().transform);

                pickUp.GetComponent<PickUpScript>().SetPickUpItem("Red Potion");
                
                return true;
            }

            return false;
        }
    }
}

