using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class ClockworkPartPropulsionController : MonoBehaviour {
		public CharacterController CharControl;
		public float Force = 10f;

		public event Action Completed;

		private Vector3 _direction = Vector3.zero;
		private float _velocity = 0f;

		void Update() {
			_velocity = Mathf.Max(_velocity - 2.0f * GameTime.Current, 0f);
			if (_velocity < 0f)
				Deactivate();

			CharControl.Move(_direction * _velocity * GameTime.Current);
		}

		void OnControllerColliderHit(ControllerColliderHit hit) {
			Deactivate();
		}

		public void Activate(Vector3 direction) {
			_velocity = Force;
			_direction = direction;
			CharControl.Move(_direction * _velocity * GameTime.Current);
		}

		public void Deactivate(bool dispatchAction = true) {
			_velocity = 0;

			if (dispatchAction)
				Completed?.Invoke();
		}
	}
}
