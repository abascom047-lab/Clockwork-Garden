using Assets.scripts.Clockwork.Parts;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserTowardsMovement : BaseDispenserMovement {
		private Transform _targetTransform;
		private float _distance;

		public DispenserTowardsMovement(ClockworkPartPropulsionController propulsionControl, Transform transform) : base(propulsionControl, transform) { }

		public bool Activate(Transform targetTransform, float distance) {
			_targetTransform = targetTransform;
			_distance = distance;

			if (!base.Activate())
				return false;

			return true;
		}

		protected override void Move() {
			if (!Active)
				return;

			if (Vector3.Distance(_transform.position, _targetTransform.position) <= 5f) {
				Complete();
				return;
			}

			_propulsionControl.Activate(new Vector3(_transform.forward.x, 1.5f, _transform.forward.z));
		}
	}
}
