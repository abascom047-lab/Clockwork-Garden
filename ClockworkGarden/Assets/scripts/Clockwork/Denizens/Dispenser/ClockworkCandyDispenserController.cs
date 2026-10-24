using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Interactables;
using Assets.scripts.Miscellaneous;
using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public enum DispenserState {
		Idle,
		Searching,
		Approaching,
		Lost,
		Planting
	}

	public class ClockworkCandyDispenserController : MonoBehaviour {
		public ClockworkPartHingeController HingeControl;
		public ClockworkPartItemContainerController ItemContainerControl;
		public ClockworkPartLauncherController LauncherControl;
		public ClockworkPartPropulsionController PropulsionControl;
		public ClockworkPartDetectorController DetectorControl;
		public GravityController GravityControl;

		private DispenserRotationMovement _rotationMovement;
		private DispenserTowardsMovement _towardsMovement;
		private DispenserForwardMovement _forwardMovement;
		private DispenserShooting _shooting;

		private Timer _idleTimer;
		private DispenserState _state = DispenserState.Idle;
		private DispenserState _lastState = DispenserState.Idle;
		private Vector3 _center = Vector3.zero;
		private InteractablePlantingSpotController _targetPlantingControl;
		private bool _grounded = true;

		void Awake() {
			_rotationMovement = new DispenserRotationMovement(PropulsionControl, transform);
			_towardsMovement = new DispenserTowardsMovement(PropulsionControl, transform);
			_forwardMovement = new DispenserForwardMovement(PropulsionControl, transform);
			_shooting = new DispenserShooting(ItemContainerControl, HingeControl, LauncherControl);

			_idleTimer = new Timer(5.0f, true);

			_center = transform.position;
		}

		void OnEnable() {
			PropulsionControl.Activated += OnPropulsionControlActivated;
			GravityControl.Grounded += OnGravityControlGrounded;

			_rotationMovement.Enable();
			_towardsMovement.Enable();
			_forwardMovement.Enable();
			_shooting.Enable();

			_rotationMovement.Completed += OnRotationMovementCompleted;
			_towardsMovement.Completed += OnTowardsMovementCompleted;
			_towardsMovement.Canceled += OnMovementCanceled;
			_forwardMovement.Completed += OnForwardMovementCompleted;
			_forwardMovement.Canceled += OnMovementCanceled;
			_shooting.Completed += OnShootingCompleted;

			_idleTimer.Completed += OnIdleTimerCompleted;
		}

		void OnDisable() {
			PropulsionControl.Activated -= OnPropulsionControlActivated;
			GravityControl.Grounded -= OnGravityControlGrounded;

			_rotationMovement.Disable();
			_towardsMovement.Disable();
			_forwardMovement.Disable();
			_shooting.Disable();

			_rotationMovement.Completed -= OnRotationMovementCompleted;
			_towardsMovement.Completed -= OnTowardsMovementCompleted;
			_towardsMovement.Canceled -= OnMovementCanceled;
			_forwardMovement.Completed -= OnForwardMovementCompleted;
			_forwardMovement.Canceled -= OnMovementCanceled;
			_shooting.Completed -= OnShootingCompleted;

			_idleTimer.Completed -= OnIdleTimerCompleted;
		}

		void Update() {
			UpdateMovers();

			switch (_state) {
				case DispenserState.Idle: {
						_idleTimer.Update();
						break;
					}
				case DispenserState.Searching: {
						UpdateSearching();
						break;
					}
			}
		}

		private void UpdateMovers() {
			_rotationMovement.Update(_grounded);
			_towardsMovement.Update(_grounded);
			_forwardMovement.Update(_grounded);
		}

		private void UpdateSearching() {
			List<InteractablePlantingSpotController> possibleTargetsList = DetectorControl.GetAll<InteractablePlantingSpotController>();
			if (possibleTargetsList.Count == 0) {
				StartIdle();
				return;
			}

			foreach (InteractablePlantingSpotController target in possibleTargetsList) {
				if (!target.Planted) {
					_targetPlantingControl = target;
					StartApproaching();
					return;
				}
			}

			StartIdle();
		}

		private void SetState(DispenserState newState) {
			_lastState = _state;
			_state = newState;
		}

		private void RevertState() {
			DeactivateMovers();

			switch (_lastState) {
				case DispenserState.Idle: {
						StartIdle();
						break;
					}
				case DispenserState.Approaching: {
						StartApproaching();
						break;
					}
			}
		}

		private void StartIdle() {
			SetState(DispenserState.Idle);

			_idleTimer.Reset(true);
			_rotationMovement.Activate(Tools.GetLookAtRotation(transform.position, _center, true));
		}

		private void StartApproaching() {
			if (_targetPlantingControl == null) {
				StartIdle();
				return;
			}

			SetState(DispenserState.Approaching);
			_rotationMovement.Activate(Tools.GetLookAtRotation(transform.position, _targetPlantingControl.transform.position, true));
		}

		private void StartPlanting() {
			DeactivateMovers();

			if (!_shooting.Activate()) {
				StartIdle();
				return;
			}

			SetState(DispenserState.Planting);
		}

		private void DeactivateMovers() {
			_rotationMovement.Deactivate();
			_towardsMovement.Deactivate();
			_forwardMovement.Deactivate();
		}

		private void OnPropulsionControlActivated(Vector3 direction) {
			if (direction.y > 0)
				_grounded = false;
		}

		private void OnGravityControlGrounded() {
			_grounded = true;
			PropulsionControl.Deactivate(PropulsionEndReason.Cancelled);
		}

		private void OnRotationMovementCompleted() {
			switch (_state) {
				case DispenserState.Idle: {
						_towardsMovement.Activate(_center, 0f);
						break;
					}
				case DispenserState.Approaching: {
						_towardsMovement.Activate(_targetPlantingControl.transform.position, 7.5f);
						break;
					}
				case DispenserState.Lost: {
						_forwardMovement.Activate(3);
						break;
					}
			}
		}

		private void OnTowardsMovementCompleted() {
			switch (_state) {
				case DispenserState.Idle: {
						_forwardMovement.Activate(5);
						break;
					}
				case DispenserState.Approaching: {
						StartPlanting();
						break;
					}
			}
		}

		private void OnForwardMovementCompleted() {
			switch (_state) {
				case DispenserState.Idle: {
						_rotationMovement.Activate(Tools.GetLookAtRotation(transform.position, _center, true));
						break;
					}
				case DispenserState.Lost: {
						RevertState();
						break;
					}
			}
		}

		private void OnMovementCanceled() {
			if (_state != DispenserState.Lost)
				SetState(DispenserState.Lost);

			_idleTimer.Stop();
			_rotationMovement.Activate(Quaternion.LookRotation(transform.right));
		}

		private void OnShootingCompleted() {
			StartIdle();
		}

		private void OnIdleTimerCompleted() {
			DeactivateMovers();
			SetState(DispenserState.Searching);
		}
	}
}
