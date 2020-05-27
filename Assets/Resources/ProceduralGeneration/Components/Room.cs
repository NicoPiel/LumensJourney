using Unity.Mathematics;
using UnityEngine;

namespace ProceduralGeneration.Components
{
    public class Room
    {
        public string RoomType { get; set; }
        
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        
        private int width;
        private int height;

        public Room (int x1, int x2, int y1, int y2)
        {
            XCoordinate = x1;
            YCoordinate = y1;
            RoomType = "";
            
            width = math.abs(x2 - x1);
            height = math.abs(y2 - y1);

            Debug.Log($"{width}x{height} room created.");
        }
    }
}