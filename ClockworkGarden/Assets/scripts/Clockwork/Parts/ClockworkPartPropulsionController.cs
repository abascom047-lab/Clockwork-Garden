using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public enum PropulsionEndReason {
		Completed,
		Collision,
		Cancelled
	}

	public class ClockworkPartPropulsionController : MonoBehaviour {
		public CharacterController CharControl;
		public float Force = 10f;

		public bool Active { get; private set; } = false;
		public bool IsMoving { get; private set; } = false;

		public event Action<PropulsionEndReason> Completed;
		public event Action<Vector3> Activated;

		private Vector3 _direction = Vector3.zero;
		private float _velocity = 0f;

		void Update() {
			if (!Active)
				return;

			IsMoving = true;
			try {
				CharControl.Move(_direction * _velocity * GameTime.Current);
			} finally {
				IsMoving = false;
			}

			_velocity = Mathf.Max(_velocity - 2.0f * GameTime.Current, 0f);
			if (_velocity <= 0f)
				Deactivate(PropulsionEndReason.Completed);
		}

		public void Activate(Vector3 direction) {
			if (Active)
				return;

			Active = true;
			_velocity = Force;
			_direction = direction;

			Activated?.Invoke(direction);
		}

		public void Deactivate(PropulsionEndReason reason) {
			if (!Active)
				return;

			Active = false;
			_velocity = 0;

			Completed?.Invoke(reason);
		}
	}
}
