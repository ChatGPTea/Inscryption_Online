/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using EnjoyGameClub.TextLifeFramework.Core.SerializedObject;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation.LiteMotion
{
    public enum DataType
    {
        Linear = -1,
        Curve = -3,
        Random = -5
    }

    public enum CharacterBehaviourType
    {
        MoveX = 1,
        MoveY = 1 << 2,
        MoveZ = 1 << 3,
        RotateX = 1 << 4,
        RotateY = 1 << 5,
        RotateZ = 1 << 6,
        ScaleX = 1 << 7,
        ScaleY = 1 << 8,
        Color = 1 << 9,
        Alpha = 1 << 10
    }

    [Serializable]
    public class LiteMotionBehaviour : PersistentScriptableObject
    {
        private static Dictionary<int, Action<LiteMotionBehaviour, Character, float>> _behaviourDictionary =
            new()
            {
                // Linear
                [(int)DataType.Linear * (int)CharacterBehaviourType.MoveX] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Move(Vector3.right * behaviour.intensity * time);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.MoveY] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Move(Vector3.up * behaviour.intensity * time);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.MoveZ] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Move(Vector3.forward * behaviour.intensity * time);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.RotateX] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Rotate(Vector3.right * behaviour.intensity * time, behaviour.Pivot);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.RotateY] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Rotate(Vector3.up * behaviour.intensity * time, behaviour.Pivot);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.RotateZ] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Rotate(Vector3.forward * behaviour.intensity * time, behaviour.Pivot);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.ScaleX] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Scale(Vector2.right * behaviour.intensity * time);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.ScaleY] =
                    (behaviour, character, time) =>
                    {
                        character.Transform.Scale(Vector2.up * behaviour.intensity * time);
                    },
                [(int)DataType.Linear * (int)CharacterBehaviourType.Color] =
                    (behaviour, character, time) => { character.Color.SetColor(behaviour.color); },
                [(int)DataType.Linear * (int)CharacterBehaviourType.Alpha] =
                    (behaviour, character, time) => { character.Color.SetAlpha(behaviour.intensity); },
                // Curve
                [(int)DataType.Curve * (int)CharacterBehaviourType.MoveX] = (behaviour, character, time) =>
                {
                    character.Transform.Move(Vector3.right * behaviour.intensity *
                                             behaviour.animationCurve.Evaluate(time));
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.MoveY] = (behaviour, character, time) =>
                {
                    character.Transform.Move(Vector3.up * behaviour.intensity *
                                             behaviour.animationCurve.Evaluate(time));
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.MoveZ] = (behaviour, character, time) =>
                {
                    character.Transform.Move(Vector3.forward * behaviour.intensity *
                                             behaviour.animationCurve.Evaluate(time));
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.RotateX] = (behaviour, character, time) =>
                {
                    character.Transform.Rotate(Vector3.right * behaviour.intensity *
                                               behaviour.animationCurve.Evaluate(time), behaviour.Pivot);
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.RotateY] = (behaviour, character, time) =>
                {
                    character.Transform.Rotate(Vector3.up * behaviour.intensity *
                                               behaviour.animationCurve.Evaluate(time), behaviour.Pivot);
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.RotateZ] = (behaviour, character, time) =>
                {
                    character.Transform.Rotate(Vector3.forward * behaviour.intensity *
                                               behaviour.animationCurve.Evaluate(time), behaviour.Pivot);
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.ScaleX] = (behaviour, character, time) =>
                {
                    character.Transform.Scale(Vector3.one + Vector3.right * behaviour.intensity *
                        behaviour.animationCurve.Evaluate(time));
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.ScaleY] = (behaviour, character, time) =>
                {
                    character.Transform.Scale(Vector3.one + Vector3.up * behaviour.intensity *
                        behaviour.animationCurve.Evaluate(time));
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.Color] = (behaviour, character, time) =>
                {
                    character.Color.SetColor(behaviour.gradient.Evaluate(Mathf.PingPong(time, 1)));
                },
                [(int)DataType.Curve * (int)CharacterBehaviourType.Alpha] = (behaviour, character, time) =>
                {
                    character.Color.SetAlpha(behaviour.animationCurve.Evaluate(time));
                },
                // Random
                [(int)DataType.Random * (int)CharacterBehaviourType.MoveX] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Move(Vector3.right * val);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.MoveY] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Move(Vector3.up * val);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.MoveZ] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Move(Vector3.forward * val);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.RotateX] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Rotate(Vector3.right * val
                        ,
                        behaviour.Pivot);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.RotateY] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Rotate(Vector3.up * val,
                        behaviour.Pivot);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.RotateZ] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));

                    character.Transform.Rotate(Vector3.forward * val,
                        behaviour.Pivot);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.ScaleX] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Scale(
                        Vector3.one + Vector3.right * val);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.ScaleY] = (behaviour, character, time) =>
                {
                    var val = Mathf.Lerp(behaviour.Range.x, behaviour.Range.y,
                        Mathf.PerlinNoise(behaviour.randomSeed,
                            time));
                    character.Transform.Scale(Vector3.one + Vector3.up * val);
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.Color] = (behaviour, character, time) =>
                {
                    character.Color.SetColor(Color.HSVToRGB(Mathf.PerlinNoise(behaviour.randomSeed, time),
                        1, 1));
                },
                [(int)DataType.Random * (int)CharacterBehaviourType.Alpha] = (behaviour, character, time) =>
                {
                    character.Color.SetAlpha(Mathf.PerlinNoise(behaviour.randomSeed, time));
                }
            };

        public DataType dataType = DataType.Linear;
        public CharacterBehaviourType behaviourType = CharacterBehaviourType.MoveX;

        // Linear and Curve
        public float intensity = 1;

        public float characterTimeOffset;

        // Only curve
        public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        // Only Random
        public float randomSeed;

        // Only Color Linear
        public Color color = Color.white;

        // Only Color Curve
        public Gradient gradient = new();

        public Vector2 Pivot = new(0.5f, 0.5f);

        // Only Random without color
        public Vector2 Range = new(0, 1);
        private Action<LiteMotionBehaviour, Character, float> _behaviour;
        private int _lastCode = 0;

        public void Execute(Character character)
        {
            var currentCode = (int)dataType * (int)behaviourType;
            if (currentCode != _lastCode) ChangeBehaviour(currentCode);

            character.Time.Time += characterTimeOffset * -0.001f * character.CharIndex;
            _behaviour?.Invoke(this, character,
                character.Time.Time);
        }

        private void ChangeBehaviour(int code)
        {
            _behaviour = _behaviourDictionary[code];
        }
    }
}