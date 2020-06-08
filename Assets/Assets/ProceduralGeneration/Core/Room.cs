using UnityEngine;

namespace Assets.ProceduralGeneration.Core
{
    public class Room : MonoBehaviour
    {
        public static GameObject GenerateRoomObject(string name, Rect rect, Transform parent, Vector2 position)
        {
            var newObject = new GameObject
            {
                name = name,
                tag = "Room",
                layer = LayerMask.NameToLayer("Rooms")
            };
            
            newObject.transform.SetParent(parent.transform);
            newObject.transform.rotation = Quaternion.identity;
            newObject.transform.position = position;

            var roomCollider = newObject.AddComponent<BoxCollider2D>();
            roomCollider.size = new Vector2(rect.width + 1, rect.height + 1);
            roomCollider.offset = new Vector2(rect.width / 2, rect.height / 2);
            roomCollider.isTrigger = true;

            return newObject;
        }
    }
}
