using UnityEngine;

namespace Runtime.Cannon.Data
{
    [CreateAssetMenu(fileName = "CannonSettings", menuName = "BallBlast/Cannon Settings")]
    public class CannonSettings : ScriptableObject
    {
        [Header("Movement")]
        public float screenDeadZone = 120f;
        public float smoothTime = 0.1f;
        public float maxSpeed = 50f;
        
    }
}