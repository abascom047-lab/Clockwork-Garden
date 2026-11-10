using Assets.scripts.Clockwork.Parts;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserForwardMovement : BaseDispenserMovement {
		private int _maxHops = 0;
		private int _hops = 0;

		public DispenserForwardMovement(ClockworkPartPropulsionController propulsionControl, Transform transform) : base(propulsionControl, transform) { }

		public bool Activate(int totalHops) {
			_maxHops = totalHops;
			_hops = 0;

			if (!base.Activate())
				return false;

			return true;
		}

		protected override void Move() {
			if (!Active)
				return;

			_hops++;
			if (_hops > _maxHops) {
				Complete();
				return;
			}

			_propulsionControl.Activate(new Vector3(_transform.forward.x, 1.5f, _transform.forward.z));
		}
	}
}
