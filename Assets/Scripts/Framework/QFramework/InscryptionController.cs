using QFramework;
using UnityEngine;

namespace Inscryption
{
    public abstract class InscryptionController : MonoBehaviour, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return Inscription.Interface;
        }
    }
}