using UnityEngine;
using UnityEngine.InputSystem;

namespace HappyInputs.Scripts.Core.Data
{
    [CreateAssetMenu(fileName = "HappyInputActionData", menuName = "Happy Inputs/Happy Input Action Data")]
    public class InputActionData : ScriptableObject
    {
        public string actionName;
        public InputActionReference actionReference;
        public InputType inputType = InputType.Press;
        public float holdDuration = 0.5f; // For hold type
        public float doubleTapWindow = 0.3f; // For double tap
    }
}