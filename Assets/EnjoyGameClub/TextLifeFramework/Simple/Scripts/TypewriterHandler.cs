using EnjoyGameClub.TextLifeFramework.Core;
using EnjoyGameClub.TextLifeFramework.Core.Animation.Special;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Simple.Scripts
{
    public class TypewriterHandler : MonoBehaviour
    {
        private TextLife _textLife;
        private Typewriter _typewriter;

        private void Awake()
        {
            _textLife = GetComponent<TextLife>();
            // _typewriter = _textLife.GetProcess<Typewriter>();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) _typewriter.CompleteImmediately();

            if (Input.GetKeyDown(KeyCode.Alpha2)) _typewriter.Reset();

            if (Input.GetKeyDown(KeyCode.Alpha3)) _typewriter.ShowText("<shake>Hello world!<@shake>");
        }
    }
}