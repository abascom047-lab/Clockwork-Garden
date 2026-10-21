using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Miscellaneous {
	public class GravityController : MonoBehaviour {
		public CharacterController CharControl;
		public float Speed = 40f;

		public event Action Grounded;

		private float _velocity = 0f;

		void Update() {
			_velocity += Speed * GameTime.Current;
			CharControl.Move(Vector3.down * _velocity * GameTime.Current);

			if (CharControl.isGrounded) {
				_velocity = 0f;
				Grounded?.Invoke();
			}
		}
	}
}
