using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightRoom2D : MonoBehaviour
    {
        public enum RoomType
        {
            Collider,
            Sprite
        }

        public static List<LightRoom2D> List = new();

        public int lightLayer;
        public Color color = Color.black;

        public LightingRoomShape shape = new();

        public SpriteMeshObject spriteMeshObject = new();

        public void Awake()
        {
            Initialize();
        }

        public void OnEnable()
        {
            List.Add(this);

            LightingManager2D.Get();

            shape.SetTransform(transform);
        }

        public void OnDisable()
        {
            List.Remove(this);
        }

        public void Initialize()
        {
            shape.ResetLocal();
        }
    }
}