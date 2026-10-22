using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Interactables;
using Assets.scripts.Miscellaneous;
using Assets.Scripts.Utils;
using System.Linq;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens {
	public enum DispenserState {
		Idle,
		Searching,
		Approaching,
		Planting
	}

	public enum DispenserApproachingState {
		None,
		FaceTowards,
		MoveTowards,
	}

	public class ClockworkCandyDispenserController : MonoBehaviour {
		public ClockworkPartHingeController TopHingeControl;
		public ClockworkPartItemContainerController ItemContainerControl;
		public ClockworkPartLauncherController LauncherControl;
		public ClockworkPartPropulsionController PropulsionControl;
		public ClockworkPartDetectorController DetectorControl;
		public GravityController GravityControl;

		private Timer _fireTimer;
		private Timer _idleTimer;
		private Timer _jumpTimer;
		private DispenserState _state = DispenserState.Idle;
		private DispenserApproachingState _approachingState = DispenserApproachingState.None;
		private BaseInteractableController _targetPlantingControl;
		private bool _grounded = true;

		private const float ROTATION_SPEED = 50f;

		void Awake() {
			_fireTimer = new Timer(0.5f);
			_idleTimer = new Timer(0.25f, true);
			_jumpTimer = new Timer(0.4f);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;
			PropulsionControl.Completed += OnPropulsionControlCompleted;
			GravityControl.Grounded += OnGravityControlGrounded;

			_fireTimer.Completed += OnFireTimerCompleted;
			_idleTimer.Completed += OnIdleTimerCompleted;
			_jumpTimer.Completed += OnJumpTimerCompleted;
		}

		void OnDisable() {
			TopHingeControl.Opened -= OnTopHingeOpened;
			TopHingeControl.Closed -= OnTopHingeClosed;
			PropulsionControl.Completed -= OnPropulsionControlCompleted;
			GravityControl.Grounded -= OnGravityControlGrounded;

			_fireTimer.Completed -= OnFireTimerCompleted;
			_idleTimer.Completed -= OnIdleTimerCompleted;
			_jumpTimer.Completed -= OnJumpTimerCompleted;
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
			_approachingState = DispenserApproachingState.FaceTowards;

			OnJumpTimerCompleted();
		}
		
		private void UpdateApproaching() {
			_jumpTimer.Update();

			if (_grounded || _approachingState != DispenserApproachingState.FaceTowards)
				return;

			Quaternion targetRotation = GetTargetRotation();
			if (transform.rotation != targetRotation) {
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ROTATION_SPEED * GameTime.Current);
			}
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

		private void OnPropulsionControlCompleted() {
			_jumpTimer.Reset(true);
		}

		private void OnGravityControlGrounded() {
			_grounded = true;
			PropulsionControl.Deactivate();
		}

		private void OnFireTimerCompleted() {
			if (!ItemContainerControl.CanPop)
				return;

			TopHingeControl.Open();
		}

		private void OnIdleTimerCompleted() {
			_state = DispenserState.Searching;
		}

		private void OnJumpTimerCompleted() {
			switch (_approachingState) {
				case DispenserApproachingState.FaceTowards: {
						OnJumpTimerCompletedFaceTowards();
						break;
					}
				case DispenserApproachingState.MoveTowards: {
						OnJumpTimerCompletedMoveTowards();
						break;
					}
			}
		}

		private void OnJumpTimerCompletedFaceTowards() {
			if (Quaternion.Angle(transform.rotation, GetTargetRotation()) <= 0.5f) {
				_approachingState = DispenserApproachingState.MoveTowards;
				OnPropulsionControlCompleted();
				return;
			}

			PropulsionControl.Activate(Vector3.up * 1.5f);
			_grounded = false;
		}

		private void OnJumpTimerCompletedMoveTowards() {
			if (Vector3.Distance(transform.position, _targetPlantingControl.transform.position) <= 5f) {
				_state = DispenserState.Planting;
				_approachingState = DispenserApproachingState.None;
				return;
			}

			PropulsionControl.Activate(new Vector3(transform.forward.x, 1.5f, transform.forward.z));
			_grounded = false;
		}
	}
}
