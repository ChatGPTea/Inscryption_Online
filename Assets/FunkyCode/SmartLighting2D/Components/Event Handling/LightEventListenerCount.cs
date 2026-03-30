using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightEventListenerCount : MonoBehaviour
    {
        [SerializeField] public List<Light2D> lights = new();

        private LightCollider2D lightCollider;

        private void OnEnable()
        {
            lights.Clear();

            lightCollider = GetComponent<LightCollider2D>();

            lightCollider?.AddEvent(CollisionEvent);
        }

        private void OnDisable()
        {
            lightCollider?.RemoveEvent(CollisionEvent);
        }

        private void CollisionEvent(LightCollision2D collision)
        {
            switch (collision.state)
            {
                case LightCollision2D.State.OnCollisionEnter:
                    lights.Add(collision.light);
                    break;

                case LightCollision2D.State.OnCollisionExit:
                    lights.Remove(collision.light);
                    break;
            }
        }
    }
}