using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class ClockworkPartPropulsionController : MonoBehaviour {
		public CharacterController CharControl;
		public float Force = 10f;

		public bool Active { get; private set; } = false;

		public event Action Completed;
		public event Action<Vector3> Activated;

		private Vector3 _direction = Vector3.zero;
		private float _velocity = 0f;

		void Update() {
			if (!Active)
				return;

			CharControl.Move(_direction * _velocity * GameTime.Current);

			_velocity = Mathf.Max(_velocity - 2.0f * GameTime.Current, 0f);
			if (_velocity <= 0f)
				Deactivate();
		}

		void OnControllerColliderHit(ControllerColliderHit hit) {
			if (!Active)
				return;

			Deactivate();
		}

		public void Activate(Vector3 direction) {
			if (Active)
				return;

			Active = true;
			_velocity = Force;
			_direction = direction;

			Activated?.Invoke(direction);
		}

		public void Deactivate(bool dispatchAction = true) {
			if (!Active)
				return;

			Active = false;
			_velocity = 0;

			if (dispatchAction)
				Completed?.Invoke();
		}
	}
}
