using Assets.scripts.Clockwork.Parts;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserTowardsMovement : BaseDispenserMovement {
		private Vector3 _targetPosition;
		private float _distance;

		public DispenserTowardsMovement(ClockworkPartPropulsionController propulsionControl, Transform transform) : base(propulsionControl, transform) { }

		public bool Activate(Vector3 targetPosition, float distance) {
			_targetPosition = targetPosition;
			_distance = distance;

			if (!base.Activate())
				return false;

			return true;
		}

		protected override void Move() {
			if (!Active)
				return;

			float distance = Vector3.Distance(_transform.position, _targetPosition);

			if (_distance == 0 && distance <= 0.1f) {
				_transform.position = _targetPosition;
				Complete();
				return;
			}

			if (distance <= _distance) {
				Complete();
				return;
			}

			_propulsionControl.Activate(new Vector3(_transform.forward.x, 1.5f, _transform.forward.z));
		}
	}
}
