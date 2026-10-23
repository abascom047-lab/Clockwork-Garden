using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Interactables;
using Assets.scripts.Miscellaneous;
using Assets.Scripts.Utils;
using System.Linq;
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
		public ClockworkPartHingeController TopHingeControl;
		public ClockworkPartItemContainerController ItemContainerControl;
		public ClockworkPartLauncherController LauncherControl;
		public ClockworkPartPropulsionController PropulsionControl;
		public ClockworkPartDetectorController DetectorControl;
		public GravityController GravityControl;

		private DispenserRotationMovement _rotationMovement;
		private DispenserTowardsMovement _towardsMovement;
		private DispenserForwardMovement _forwardMovement;

		private Timer _fireTimer;
		private Timer _idleTimer;
		private DispenserState _state = DispenserState.Idle;
		private DispenserState _lastState = DispenserState.Idle;
		private Vector3 _center = Vector3.zero;
		private BaseInteractableController _targetPlantingControl;
		private bool _grounded = true;

		void Awake() {
			_rotationMovement = new DispenserRotationMovement(PropulsionControl, transform);
			_towardsMovement = new DispenserTowardsMovement(PropulsionControl, transform);
			_forwardMovement = new DispenserForwardMovement(PropulsionControl, transform);

			_fireTimer = new Timer(0.5f);
			_idleTimer = new Timer(0.25f, true);

			_center = transform.position;
			_towardsMovement.Activate(_center, 0f);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;
			PropulsionControl.Activated += OnPropulsionControlActivated;
			GravityControl.Grounded += OnGravityControlGrounded;

			_rotationMovement.Enable();
			_towardsMovement.Enable();
			_forwardMovement.Enable();

			_rotationMovement.Completed += OnRotationMovementCompleted;
			_towardsMovement.Completed += OnTowardsMovementCompleted;
			_towardsMovement.Canceled += OnMovementCanceled;
			_forwardMovement.Completed += OnForwardMovementCompleted;
			_forwardMovement.Canceled += OnMovementCanceled;

			_fireTimer.Completed += OnFireTimerCompleted;
			_idleTimer.Completed += OnIdleTimerCompleted;
		}

		void OnDisable() {
			TopHingeControl.Opened -= OnTopHingeOpened;
			TopHingeControl.Closed -= OnTopHingeClosed;
			PropulsionControl.Activated -= OnPropulsionControlActivated;
			GravityControl.Grounded -= OnGravityControlGrounded;

			_rotationMovement.Disable();
			_towardsMovement.Disable();
			_forwardMovement.Disable();

			_rotationMovement.Completed -= OnRotationMovementCompleted;
			_towardsMovement.Completed -= OnTowardsMovementCompleted;
			_towardsMovement.Canceled -= OnMovementCanceled;
			_forwardMovement.Completed -= OnForwardMovementCompleted;
			_forwardMovement.Canceled -= OnMovementCanceled;

			_fireTimer.Completed -= OnFireTimerCompleted;
			_idleTimer.Completed -= OnIdleTimerCompleted;
		}

		void Update() {
			UpdateTimers();
			UpdateMovers();

			switch (_state) {
				case DispenserState.Searching: {
						UpdateSearching();
						break;
					}
			}
		}

		private void UpdateTimers() {
			_idleTimer.Update();
		}

		private void UpdateMovers() {
			_rotationMovement.Update(_grounded);
			_towardsMovement.Update(_grounded);
			_forwardMovement.Update(_grounded);
		}

		private void UpdateSearching() {
			if (DetectorControl.Total == 0) {
				StartIdle();
				return;
			}

			_targetPlantingControl = DetectorControl.All.First();
			StartApproaching();
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
			_idleTimer.Start();
			_rotationMovement.Activate(Tools.GetLookAtRotation(transform.position, _center, true));

			SetState(DispenserState.Idle);
		}

		private void StartApproaching() {
			if (_targetPlantingControl == null)
				StartIdle();

			_rotationMovement.Activate(Tools.GetLookAtRotation(transform.position, _targetPlantingControl.transform.position, true));
			SetState(DispenserState.Approaching);
		}

		private void DeactivateMovers() {
			_rotationMovement.Deactivate();
			_towardsMovement.Deactivate();
			_forwardMovement.Deactivate();
		}

		private void OnTopHingeOpened() {
			LauncherControl.Fire();
			TopHingeControl.Close();
		}

		private void OnTopHingeClosed() {
			_fireTimer.Reset(ItemContainerControl.CanPop);
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
						_towardsMovement.Activate(_targetPlantingControl.transform.position, 5f);
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
						DeactivateMovers();
						SetState(DispenserState.Planting);
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

			_rotationMovement.Activate(Quaternion.LookRotation(transform.right));
		}

		private void OnFireTimerCompleted() {
			if (!ItemContainerControl.CanPop)
				return;

			TopHingeControl.Open();
		}

		private void OnIdleTimerCompleted() {
			DeactivateMovers();
			SetState(DispenserState.Searching);
		}
	}
}
