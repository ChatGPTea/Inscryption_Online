using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode
{
    public struct LightCollision2D
    {
        public enum State
        {
            OnCollision,
            OnCollisionEnter,
            OnCollisionExit
        }

        public Light2D light;
        public LightCollider2D collider;

        public List<Vector2> points;
        public State state;

        public LightCollision2D(State state)
        {
            light = null;

            collider = null;

            points = null;

            this.state = state;
        }
    }
}