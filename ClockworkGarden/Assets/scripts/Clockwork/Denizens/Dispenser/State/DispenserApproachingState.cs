using Assets.scripts.Clockwork.Parts;
using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserApproachingState : IDispenserState {
		public event Action Completed;
		public event Action Failed;
		public event Action Lost;

		private readonly Transform _transform;

		private DispenserRotationMovement _rotationMovement;
		private DispenserTowardsMovement _towardsMovement;

		private Transform _targetTransform;
		private bool _isMoving = false;

		public DispenserApproachingState(ClockworkPartPropulsionController propulsionControl, Transform transform) {
			_transform = transform;

			_rotationMovement = new DispenserRotationMovement(propulsionControl, _transform);
			_towardsMovement = new DispenserTowardsMovement(propulsionControl, _transform);
		}

		public void OnEnable() {
			_rotationMovement.Enable();
			_towardsMovement.Enable();

			_rotationMovement.Completed += OnRotationMovementCompleted;
			_towardsMovement.Completed += OnTowardsMovementCompleted;
			_towardsMovement.Canceled += OnMovementCanceled;
		}

		public void OnDisable() {
			_rotationMovement.Disable();
			_towardsMovement.Disable();

			_rotationMovement.Completed -= OnRotationMovementCompleted;
			_towardsMovement.Completed -= OnTowardsMovementCompleted;
			_towardsMovement.Canceled -= OnMovementCanceled;
		}

		public void Update(bool grounded) {
			_rotationMovement.Update(grounded);
			_towardsMovement.Update(grounded);
		}

		public void Start(Transform targetTransform) {
			_targetTransform = targetTransform;
			Start();
		}

		public void Start() {
			if (_isMoving)
				return;

			if (_targetTransform == null) {
				Failed?.Invoke();
				return;
			}

			_isMoving = true;
			_rotationMovement.Activate(Tools.GetLookAtRotation(_transform.position, _targetTransform.position, true));
		}

		public void Stop() {
			_isMoving = false;

			_rotationMovement.Deactivate();
			_towardsMovement.Deactivate();
		}

		private void OnRotationMovementCompleted() {
			if (_targetTransform == null) {
				_isMoving = false;
				Failed?.Invoke();
				return;
			}

			_towardsMovement.Activate(_targetTransform.position, 7.5f);
		}

		private void OnTowardsMovementCompleted() {
			if (_targetTransform == null) {
				_isMoving = false;
				Failed?.Invoke();
				return;
			}

			Completed?.Invoke();
		}

		private void OnMovementCanceled() {
			Stop();
			Lost?.Invoke();
		}
	}
}
