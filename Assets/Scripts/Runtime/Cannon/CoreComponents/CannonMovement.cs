using Runtime.Cannon.Data;
using UnityEngine;

namespace Runtime.Cannon.CoreComponents
{
    public class CannonMovement
    {
        private readonly CannonSettings _settings;
        private readonly Camera _camera;
        
        private float _targetX;
        private float _currentVelocity;

        public CannonMovement(CannonSettings settings, Camera camera)
        {
            _settings = settings;
            _camera = camera;
        }

        public void SetTargetFromScreen(Vector2 screenPosition, float currentWorldZ)
        {
            float clampedX = Mathf.Clamp(
                screenPosition.x,
                _settings.screenDeadZone,
                Screen.width - _settings.screenDeadZone
            );

            Vector3 screenPoint = new Vector3(clampedX, 0, currentWorldZ - _camera.transform.position.z);
            Vector3 worldPoint = _camera.ScreenToWorldPoint(screenPoint);
            
            _targetX = worldPoint.x;
        }

        public void UpdatePosition(Transform cannonTransform)
        {
            float newX = Mathf.SmoothDamp(
                cannonTransform.position.x,
                _targetX,
                ref _currentVelocity,
                _settings.smoothTime,
                _settings.maxSpeed
            );

            cannonTransform.position = new Vector3(
                newX,
                cannonTransform.position.y,
                cannonTransform.position.z
            );
        }
    }
}