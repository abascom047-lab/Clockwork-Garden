using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Interactables;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser.State {
	public class DispenserIdleState : IDispenserState {
		public event Action<InteractablePlantingSpotController> TargetFound;
		public event Action Lost;

		private readonly ClockworkPartDetectorController _detectorControl;
		private readonly Transform _transform;
		private readonly Vector3 _center;

		private DispenserRotationMovement _rotationMovement;
		private DispenserTowardsMovement _towardsMovement;
		private DispenserForwardMovement _forwardMovement;

		private Timer _searchTimer;

		private const float SEARCH_TIME = 0.5f;

		public DispenserIdleState(
			ClockworkPartDetectorController detectorControl,
			ClockworkPartPropulsionController propulsionControl,
			Transform transform,
			Vector3 center
		) {

			_detectorControl = detectorControl;
			_transform = transform;
			_center = center;

			_rotationMovement = new DispenserRotationMovement(propulsionControl, _transform);
			_towardsMovement = new DispenserTowardsMovement(propulsionControl, _transform);
			_forwardMovement = new DispenserForwardMovement(propulsionControl, _transform);

			_searchTimer = new Timer(SEARCH_TIME);
		}

		public void OnEnable() {
			_rotationMovement.Enable();
			_towardsMovement.Enable();
			_forwardMovement.Enable();

			_rotationMovement.Completed += OnRotationMovementCompleted;
			_towardsMovement.Completed += OnTowardsMovementCompleted;
			_towardsMovement.Canceled += OnMovementCanceled;
			_forwardMovement.Completed += OnForwardMovementCompleted;
			_forwardMovement.Canceled += OnMovementCanceled;

			_searchTimer.Completed += OnSearchTimerCompleted;
		}

		public void OnDisable() {
			_rotationMovement.Disable();
			_towardsMovement.Disable();
			_forwardMovement.Disable();

			_rotationMovement.Completed -= OnRotationMovementCompleted;
			_towardsMovement.Completed -= OnTowardsMovementCompleted;
			_towardsMovement.Canceled -= OnMovementCanceled;
			_forwardMovement.Completed -= OnForwardMovementCompleted;
			_forwardMovement.Canceled -= OnMovementCanceled;

			_searchTimer.Completed -= OnSearchTimerCompleted;
		}

		public void Update(bool grounded) {
			_rotationMovement.Update(grounded);
			_towardsMovement.Update(grounded);
			_forwardMovement.Update(grounded);

			_searchTimer.Update();
		}

		public void Start() {
			_rotationMovement.Activate(Tools.GetLookAtRotation(_transform.position, _center, true));
			_searchTimer.Reset(true);
		}

		public void Stop() {
			_rotationMovement.Deactivate();
			_towardsMovement.Deactivate();
			_forwardMovement.Deactivate();

			_searchTimer.Stop();
		}

		private void OnRotationMovementCompleted() {
			_towardsMovement.Activate(_center, 0f);
		}

		private void OnTowardsMovementCompleted() {
			_forwardMovement.Activate(5);
		}

		private void OnForwardMovementCompleted() {
			_rotationMovement.Activate(Tools.GetLookAtRotation(_transform.position, _center, true));
		}

		private void OnMovementCanceled() {
			Stop();
			Lost?.Invoke();
		}

		private void OnSearchTimerCompleted() {
			List<InteractablePlantingSpotController> possibleTargetsList = _detectorControl.GetAll<InteractablePlantingSpotController>();
			if (possibleTargetsList.Count == 0) {
				_searchTimer.Start();
				return;
			}

			foreach (InteractablePlantingSpotController target in possibleTargetsList) {
				if (!target.Planted) {
					_searchTimer.Stop();
					TargetFound?.Invoke(target);

					return;
				}
			}

			_searchTimer.Start();
		}
	}
}
