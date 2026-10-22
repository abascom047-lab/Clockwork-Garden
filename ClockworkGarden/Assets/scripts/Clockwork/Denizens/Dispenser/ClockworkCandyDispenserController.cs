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

		private Timer _fireTimer;
		private Timer _idleTimer;
		private DispenserState _state = DispenserState.Idle;
		private BaseInteractableController _targetPlantingControl;
		private bool _grounded = true;

		void Awake() {
			_rotationMovement = new DispenserRotationMovement(PropulsionControl, transform);
			_towardsMovement = new DispenserTowardsMovement(PropulsionControl, transform);

			_fireTimer = new Timer(0.5f);
			_idleTimer = new Timer(0.25f, true);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;
			PropulsionControl.Activated += OnPropulsionControlActivated;
			GravityControl.Grounded += OnGravityControlGrounded;

			_rotationMovement.Enable();
			_towardsMovement.Enable();

			_rotationMovement.Completed += OnRotationMovementCompleted;
			_towardsMovement.Completed += OnTowardsMovementCompleted;

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

			_rotationMovement.Completed -= OnRotationMovementCompleted;
			_towardsMovement.Completed -= OnTowardsMovementCompleted;

			_fireTimer.Completed -= OnFireTimerCompleted;
			_idleTimer.Completed -= OnIdleTimerCompleted;
		}

		void Update() {
			switch (_state) {
				case DispenserState.Idle: {
						UpdateIdle();
						break;
					}
				case DispenserState.Searching: {
						UpdateSearching();
						break;
					}
				case DispenserState.Approaching: {
						UpdateApproaching();
						break;
					}
			}
		}

		private void UpdateIdle() {
			_idleTimer.Update();
		}

		private void UpdateSearching() {
			if (DetectorControl.Total == 0) {
				_state = DispenserState.Idle;
				_idleTimer.Start();

				return;
			}

			_targetPlantingControl = DetectorControl.All.First();
			_state = DispenserState.Approaching;

			_rotationMovement.Activate(GetTargetRotation());
		}
		
		private void UpdateApproaching() {
			_rotationMovement.Update(_grounded);
			_towardsMovement.Update(_grounded);
		}

		private Quaternion GetTargetRotation() {
			Vector3 forward = _targetPlantingControl.transform.position - transform.position;
			forward = forward.normalized;
			forward.y = 0;

			return Quaternion.LookRotation(forward);
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
			PropulsionControl.Deactivate();
		}

		private void OnRotationMovementCompleted() {
			_towardsMovement.Activate(_targetPlantingControl.transform, 5f);
		}

		private void OnTowardsMovementCompleted() {
			_state = DispenserState.Planting;
		}

		private void OnFireTimerCompleted() {
			if (!ItemContainerControl.CanPop)
				return;

			TopHingeControl.Open();
		}

		private void OnIdleTimerCompleted() {
			_state = DispenserState.Searching;
		}
	}
}
