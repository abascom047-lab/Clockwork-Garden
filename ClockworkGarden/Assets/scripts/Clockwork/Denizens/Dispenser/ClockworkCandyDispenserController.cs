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

		private DispenserRotationMovement _idleRotationMovement;
		private DispenserTowardsMovement _idleTowardsMovement;
		private DispenserRotationMovement _approachingRotationMovement;
		private DispenserTowardsMovement _approachingTowardsMovement; 

		private Timer _fireTimer;
		private Timer _idleTimer;
		private DispenserState _state = DispenserState.Idle;
		private Vector3 _center = Vector3.zero;
		private BaseInteractableController _targetPlantingControl;
		private bool _grounded = true;

		void Awake() {
			_idleRotationMovement = new DispenserRotationMovement(PropulsionControl, transform);
			_idleTowardsMovement = new DispenserTowardsMovement(PropulsionControl, transform);
			_approachingRotationMovement = new DispenserRotationMovement(PropulsionControl, transform);
			_approachingTowardsMovement = new DispenserTowardsMovement(PropulsionControl, transform);

			_fireTimer = new Timer(0.5f);
			_idleTimer = new Timer(0.25f, true);

			_center = transform.position;
			_idleTowardsMovement.Activate(_center, 0f);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;
			PropulsionControl.Activated += OnPropulsionControlActivated;
			GravityControl.Grounded += OnGravityControlGrounded;

			_idleRotationMovement.Enable();
			_idleTowardsMovement.Enable();
			_approachingRotationMovement.Enable();
			_approachingTowardsMovement.Enable();

			_idleRotationMovement.Completed += OnIdleRotationMovementCompleted;
			_approachingRotationMovement.Completed += OnApproachingRotationMovementCompleted;
			_approachingTowardsMovement.Completed += OnApproachingTowardsMovementCompleted;

			_fireTimer.Completed += OnFireTimerCompleted;
			_idleTimer.Completed += OnIdleTimerCompleted;
		}

		void OnDisable() {
			TopHingeControl.Opened -= OnTopHingeOpened;
			TopHingeControl.Closed -= OnTopHingeClosed;
			PropulsionControl.Activated -= OnPropulsionControlActivated;
			GravityControl.Grounded -= OnGravityControlGrounded;

			_idleRotationMovement.Disable();
			_idleTowardsMovement.Disable();
			_approachingRotationMovement.Disable();
			_approachingTowardsMovement.Disable();

			_idleRotationMovement.Completed -= OnIdleRotationMovementCompleted;
			_approachingRotationMovement.Completed -= OnApproachingRotationMovementCompleted;
			_approachingTowardsMovement.Completed -= OnApproachingTowardsMovementCompleted;

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
			_idleRotationMovement.Update(_grounded);
			_idleTowardsMovement.Update(_grounded);

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

			_approachingRotationMovement.Activate(GetTargetRotation());
		}
		
		private void UpdateApproaching() {
			_approachingRotationMovement.Update(_grounded);
			_approachingTowardsMovement.Update(_grounded);
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

		private void OnIdleRotationMovementCompleted() {
			_idleTowardsMovement.Activate(_center, 0f);
		}

		private void OnApproachingRotationMovementCompleted() {
			_approachingTowardsMovement.Activate(_targetPlantingControl.transform.position, 5f);
		}

		private void OnApproachingTowardsMovementCompleted() {
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
