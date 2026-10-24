using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class PropulsionCollisionDetector : MonoBehaviour {
		public ClockworkPartPropulsionController PropulsionControl;

		private void OnControllerColliderHit(ControllerColliderHit hit) {
			// assume a down move direction is gravity's doing.
			if (hit.moveDirection.y < 0)
				return;

			if (!PropulsionControl.Active || !PropulsionControl.IsMoving)
				return;


			PropulsionControl.Deactivate(PropulsionEndReason.Collision);
		}
	}
}
