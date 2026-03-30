using TMPro;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Simple.Scripts
{
    public class InputHandler : MonoBehaviour
    {
        public TMP_Text TMPText;
        public TMP_InputField InputField;

        public void Start()
        {
            InputField.onValueChanged.AddListener(text => { TMPText.SetText(text); });
        }
    }
}