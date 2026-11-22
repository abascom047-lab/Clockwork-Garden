using Assets.scripts.Clockwork.Parts.Interfaces;
using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Controllers {
	public enum ClockworkHingeState {
		Open,
		Close,
		Opening,
		Closing
	}

	public enum ClockworkHingeError {
		None,
		CantOpen,
		CantClose
	}

	public class ClockworkPartHingeController : MonoBehaviour, IPartControllableSpeed {
		public Vector3 MinRotation = Vector3.zero;
		public Vector3 MaxRotation = Vector3.zero;
		public float Speed = 100.0f;
		public bool Reverse = false;

		public event Action Opened;
		public event Action Closed;
		public event Action<ClockworkHingeError> Failure;

		public bool IsOpen => _state == ClockworkHingeState.Open;
		public bool IsClosed => _state == ClockworkHingeState.Close;

		private ClockworkHingeState _state = ClockworkHingeState.Close;
		private ClockworkHingeError _currentError = ClockworkHingeError.None;
		private Vector3 _currentRotation = Vector3.zero;
		private float _openSpeed;
		private float _closeSpeed;

		private const float MIN_ROTATION_ALLOWED = -360f;
		private const float MAX_ROTATION_ALLOWED = 360f;
		private const float ROTATION_TOLERANCE = 0.1f;

		void Awake() {
			_currentRotation = transform.localEulerAngles;
			_openSpeed = Speed;
			_closeSpeed = Speed;

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


		#region // IPartControllableSpeed Methods
		public float GetBaseSpeed() {
			return Speed;
		}

		public void SetSpeed(float newSpeed) {
			_openSpeed = Mathf.Clamp(newSpeed, 0f, Speed);
			_closeSpeed = Mathf.Clamp(newSpeed, 0f, Speed);
		}

		public void ResetSpeed() {
			SetSpeed(Speed);
		}

		public float GetSpeed() {
			return _openSpeed;
		}

		public float GetBaseSpeedByIndex(int index) {
			if (index > 1 || index < 0)
				return 0f;

			return Speed;
		}

		public void SetSpeedByIndex(int index, float newSpeed) {
			if (index > 1 || index < 0)
				return;

			if (index == 0) {
				_openSpeed = Mathf.Clamp(newSpeed, 0f, Speed);
				return;
			}

			_closeSpeed = Mathf.Clamp(newSpeed, 0f, Speed);
		}

		public void ResetSpeedByIndex(int index) {
			SetSpeedByIndex(index, Speed);
		}

		public float GetSpeedByIndex(int index) {
			if (index > 1 || index < 0)
				return 0f;

			if (index == 0)
				return _openSpeed;

			return _closeSpeed;
		}
		#endregion


		private void UpdateOpening() {
			if (_openSpeed == 0f) {
				SendFailure(ClockworkHingeError.CantOpen);
				return;
			} else {
				ClearFailure(ClockworkHingeError.CantOpen);
			}

				Vector3 targetRotation = Reverse ? MinRotation : MaxRotation;

			UpdateRotation(targetRotation, _openSpeed);
			IfAtTargetRotation(targetRotation, ClockworkHingeState.Open, Opened);
		}

		private void UpdateClosing() {
			if (_closeSpeed == 0f) {
				SendFailure(ClockworkHingeError.CantClose);
				return;
			} else {
				ClearFailure(ClockworkHingeError.CantClose);
			}

				Vector3 targetRotation = Reverse ? MaxRotation : MinRotation;

			UpdateRotation(targetRotation, _closeSpeed);
			IfAtTargetRotation(targetRotation, ClockworkHingeState.Close, Closed);
		}

		private void UpdateRotation(Vector3 target, float speed) {
			float delta = speed * GameTime.Current;
			float x = Mathf.MoveTowardsAngle(_currentRotation.x, target.x, delta);
			float y = Mathf.MoveTowardsAngle(_currentRotation.y, target.y, delta);
			float z = Mathf.MoveTowardsAngle(_currentRotation.z, target.z, delta);

			_currentRotation = new Vector3(x, y, z);
			transform.localRotation = Quaternion.Euler(_currentRotation);
		}

		private void IfAtTargetRotation(Vector3 target, ClockworkHingeState state, Action trigger) {
			if (HasReachedRotation(target)) {
				_currentRotation = target;
				transform.localRotation = Quaternion.Euler(target);

				_state = state;
				trigger?.Invoke();
			}
		}

		private bool HasReachedRotation(Vector3 target) {
			Quaternion current = Quaternion.Euler(_currentRotation);
			Quaternion destination = Quaternion.Euler(target);

			return Quaternion.Angle(current, destination) <= ROTATION_TOLERANCE;
		}

		private void SendFailure(ClockworkHingeError error) {
			if (_currentError == ClockworkHingeError.None) {
				_currentError = error;
				Failure?.Invoke(error);
			}
		}

		private void ClearFailure(ClockworkHingeError error) {
			if (_currentError == error)
				_currentError = ClockworkHingeError.None;
		}
	}
}
