using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public enum ClockworkHingeState {
		Open,
		Close,
		Opening,
		Closing
	}

	public class ClockworkHingeController : MonoBehaviour {
		public Vector3 MinRotation = Vector3.zero;
		public Vector3 MaxRotation = Vector3.zero;
		public float Speed = 100.0f;

		public event Action Opened;
		public event Action Closed;

		public bool IsOpen => _state == ClockworkHingeState.Open;
		public bool IsClosed => _state == ClockworkHingeState.Close;

		private ClockworkHingeState _state = ClockworkHingeState.Close;
		private Vector3 _currentRotation = Vector3.zero;

		private const float MIN_ROTATION_ALLOWED = -360f;
		private const float MAX_ROTATION_ALLOWED = 360f;

		void Awake() {
			_currentRotation = transform.eulerAngles;

			if (MinRotation.x < MIN_ROTATION_ALLOWED || MinRotation.y < MIN_ROTATION_ALLOWED || MinRotation.z < MIN_ROTATION_ALLOWED)
				Debug.LogError($"MinRotation too low: {MinRotation} Minimum Rotation: {MIN_ROTATION_ALLOWED}");

			if (MaxRotation.x > MAX_ROTATION_ALLOWED || MaxRotation.y > MAX_ROTATION_ALLOWED || MaxRotation.z > MAX_ROTATION_ALLOWED)
				Debug.LogError($"MaxRotation too High: {MaxRotation} Maximum Rotation: {MAX_ROTATION_ALLOWED}");
		}

		void Update() {
			switch (_state) {
				case ClockworkHingeState.Opening: {
						UpdateOpening();
						break;
					}
				case ClockworkHingeState.Closing: {
						UpdateClosing();
						break;
					}
			}
		}

		public void Open() {
			if (!IsClosed)
				return;

			_state = ClockworkHingeState.Opening;
		}

		public void Close() {
			if (!IsOpen)
				return;

			_state = ClockworkHingeState.Closing;
		}

		private void UpdateOpening() {
			if (_currentRotation == MaxRotation) {
				_state = ClockworkHingeState.Open;
				Opened?.Invoke();

				return;
			}

			UpdateRotation(MaxRotation);
		}

		private void UpdateClosing() {
			if (_currentRotation == MinRotation) {
				_state = ClockworkHingeState.Close;
				Closed?.Invoke();

				return;
			}

			UpdateRotation(MinRotation);
		}

		private void UpdateRotation(Vector3 target) {
			float delta = Speed * GameTime.Current;
			float x = Mathf.MoveTowardsAngle(_currentRotation.x, target.x, delta);
			float y = Mathf.MoveTowardsAngle(_currentRotation.y, target.y, delta);
			float z = Mathf.MoveTowardsAngle(_currentRotation.z, target.z, delta);

			_currentRotation = new Vector3(x, y, z);
			transform.rotation = Quaternion.Euler(_currentRotation);
		}
	}
}
