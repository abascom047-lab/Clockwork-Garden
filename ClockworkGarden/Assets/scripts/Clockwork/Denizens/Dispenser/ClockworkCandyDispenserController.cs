using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Interactables;
using Assets.scripts.Miscellaneous;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class ClockworkCandyDispenserController : MonoBehaviour {
		public ClockworkPartHingeController HingeControl;
		public ClockworkPartItemContainerController ItemContainerControl;
		public ClockworkPartLauncherController LauncherControl;
		public ClockworkPartPropulsionController PropulsionControl;
		public ClockworkPartDetectorController DetectorControl;
		public GravityController GravityControl;

		private Vector3 _center = Vector3.zero;
		private InteractablePlantingSpotController _targetPlantingControl;
		private IDispenserState _previousState = null;
		private IDispenserState _nextState = null;
		private bool _grounded = true;

		private DispenserIdleState _idleState;
		private DispenserApproachingState _approachingState;
		private DispenserLostState _lostState;
		private DispenserPlantingState _plantingState;

		void Awake() {
			_center = transform.position;

			_idleState = new DispenserIdleState(DetectorControl, PropulsionControl, transform, _center);
			_approachingState = new DispenserApproachingState(PropulsionControl, transform);
			_lostState = new DispenserLostState(PropulsionControl, transform);
			_plantingState = new DispenserPlantingState(ItemContainerControl, HingeControl, LauncherControl);
		}

		void Start() {
			_nextState = _idleState;
		}

		void OnEnable() {
			PropulsionControl.Activated += OnPropulsionControlActivated;
			GravityControl.Grounded += OnGravityControlGrounded;

			_idleState.Enable();
			_approachingState.Enable();
			_lostState.Enable();
			_plantingState.Enable();

			_idleState.TargetFound += OnIdleStateTargetFound;
			_idleState.Lost += OnStateLost;
			_approachingState.Completed += OnApproachingStateCompleted;
			_approachingState.Failed += OnApproachingStateFailed;
			_approachingState.Lost += OnStateLost;
			_lostState.Completed += OnLostStateCompleted;
			_plantingState.Completed += OnPlantingStateCompleted;
			_plantingState.Failed += OnPlantingStateFailed;
		}

		void OnDisable() {
			PropulsionControl.Activated -= OnPropulsionControlActivated;
			GravityControl.Grounded -= OnGravityControlGrounded;

			_idleState.Disable();
			_approachingState.Disable();
			_lostState.Disable();
			_plantingState.Disable();

			_idleState.TargetFound -= OnIdleStateTargetFound;
			_idleState.Lost -= OnStateLost;
			_approachingState.Completed -= OnApproachingStateCompleted;
			_approachingState.Failed -= OnApproachingStateFailed;
			_approachingState.Lost -= OnStateLost;
			_lostState.Completed -= OnLostStateCompleted;
			_plantingState.Completed -= OnPlantingStateCompleted;
			_plantingState.Failed -= OnPlantingStateFailed;
		}

		void Update() {
			if (_nextState != null) {
				IDispenserState nextState = _nextState;
				_nextState = null;
				nextState.Start();

				return;
			}

			_idleState.Update(_grounded);
			_approachingState.Update(_grounded);
			_lostState.Update(_grounded);
			_plantingState.Update(_grounded);
		}

		private void OnPropulsionControlActivated(Vector3 direction) {
			if (direction.y > 0)
				_grounded = false;
		}

		private void OnGravityControlGrounded() {
			_grounded = true;
			PropulsionControl.Deactivate(PropulsionEndReason.Cancelled);
		}

		private void OnIdleStateTargetFound(InteractablePlantingSpotController target) {
			_targetPlantingControl = target;
			_idleState.Stop();
			_approachingState.SetTargetTransform(_targetPlantingControl.transform);
			_nextState = _approachingState;
		}

		private void OnApproachingStateCompleted() {
			_approachingState.Stop();
			_nextState = _plantingState;
		}

		private void OnApproachingStateFailed() {
			_approachingState.Stop();
			_nextState = _idleState;
		}

		private void OnStateLost(IDispenserState lostState) {
			_previousState = lostState;
			_nextState = _lostState;
		}

		private void OnLostStateCompleted() {
			_lostState.Stop();
			_nextState = _previousState;
		}

		private void OnPlantingStateCompleted() {
			_plantingState.Stop();
			_targetPlantingControl = null;
			_nextState = _idleState;
		}

		private void OnPlantingStateFailed() {
			_plantingState.Stop();
			_targetPlantingControl = null;
			_nextState = _idleState;
		}
	}
}
