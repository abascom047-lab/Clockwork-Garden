using Assets.scripts.Clockwork.Parts;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserRotationMovement : BaseDispenserMovement {
		private Quaternion _targetRotation;

		private const float ROTATION_SPEED = 50f;

		public DispenserRotationMovement(ClockworkPartPropulsionController propulsionControl, Transform transform) : base(propulsionControl, transform) { }

		public override bool Update(bool grounded) {
			if (!base.Update(grounded))
				return false;

			if (_grounded)
				return false;

			_transform.rotation = Quaternion.RotateTowards(_transform.rotation, _targetRotation, ROTATION_SPEED * GameTime.Current);
			return true;
		}

		public bool Activate(Quaternion targetRotation) {
			_targetRotation = targetRotation;

			if (!base.Activate())
				return false;

			return true;
		}

		protected override void Move() {
			if (!Active)
				return;

			if (Quaternion.Angle(_transform.rotation, _targetRotation) < 0.5f) {
				_transform.rotation = _targetRotation;
				Complete();

				return;
			}

			_propulsionControl.Activate(Vector3.up * 1.5f);
		}
	}
}
