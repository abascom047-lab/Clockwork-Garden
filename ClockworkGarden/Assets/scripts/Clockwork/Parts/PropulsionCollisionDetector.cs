using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class PropulsionCollisionDetector : MonoBehaviour {
		public ClockworkPartPropulsionController PropulsionControl;

		private Vector3 _lastDirection;

		private void OnEnable() {
			PropulsionControl.Activated += OnPropulsionControllerActivated;
		}

		private void OnDisable() {
			PropulsionControl.Activated -= OnPropulsionControllerActivated;
		}

		private void OnControllerColliderHit(ControllerColliderHit hit) {
			// assume a down move direction is gravity's doing.
			if (hit.moveDirection.y < 0)
				return;

			if (!PropulsionControl.Active || hit.moveDirection.x != _lastDirection.x || hit.moveDirection.z != _lastDirection.z)
				return;


			PropulsionControl.Deactivate(PropulsionEndReason.Collision);
		}

		private void OnPropulsionControllerActivated(Vector3 direction) {
			_lastDirection = direction;
		}
	}
}
