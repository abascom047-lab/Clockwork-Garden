using Assets.scripts.Clockwork.Parts;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserLostState : IDispenserState {
		private event Action Completed;

		private readonly Transform _transform;

		private DispenserRotationMovement _rotationMovement;
		private DispenserForwardMovement _forwardMovement;

		private bool _isMoving = false;

		public DispenserLostState(ClockworkPartPropulsionController propulsionControl, Transform transform) {
			_transform = transform;

			_rotationMovement = new DispenserRotationMovement(propulsionControl, _transform);
			_forwardMovement = new DispenserForwardMovement(propulsionControl, _transform);
		}

		public void OnEnable() {
			_rotationMovement.Enable();
			_forwardMovement.Enable();

			_rotationMovement.Completed += OnRotationMovementCompleted;
			_forwardMovement.Completed += OnForwardMovementCompleted;
		}

		public void OnDisable() {
			_rotationMovement.Disable();
			_forwardMovement.Disable();

			_rotationMovement.Completed -= OnRotationMovementCompleted;
			_forwardMovement.Completed -= OnForwardMovementCompleted;
		}

		public void Update(bool grounded) {
			_rotationMovement.Update(grounded);
			_forwardMovement.Update(grounded);
		}

		public void Start() {
			if (_isMoving)
				return;

			_isMoving = true;
			_rotationMovement.Activate(Quaternion.LookRotation(_transform.right));
		}

		public void Stop() {
			_isMoving = false;

			_rotationMovement.Deactivate();
			_forwardMovement.Deactivate();
		}

		private void OnRotationMovementCompleted() {
			_forwardMovement.Activate(3);
		}

		private void OnForwardMovementCompleted() {
			_isMoving = false;
			Completed?.Invoke();
		}
	}
}
