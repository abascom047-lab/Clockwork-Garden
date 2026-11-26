using Assets.scripts.Clockwork.Parts.Helpers;
using Assets.scripts.Clockwork.Parts.Interfaces;
using Assets.scripts.Clockwork.Parts.Status;
using Assets.scripts.Clockwork.Parts.Status.Conditions;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
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

	public class ClockworkPartHingeController : MonoBehaviour, IPartDamageable, IPartModifiableSpeed {
		public BaseStatusController[] StatusControls;
		public Vector3 MinRotation = Vector3.zero;
		public Vector3 MaxRotation = Vector3.zero;
		public float Speed = 100f;
		public bool Reverse = false;

		public event Action Opened;
		public event Action Closed;
		public event Action<ClockworkHingeError> Failure;

		public bool IsOpen => _state == ClockworkHingeState.Open;
		public bool IsClosed => _state == ClockworkHingeState.Close;


		#region // IPartDamageable properties
		public HashSet<IStatusCondition> StatusConditions { get; private set; }
		#endregion

		#region // IPartModifiableSpeed properties
		public float BaseSpeed { get; set; }
		public float CurrentSpeed { get; private set; }
		public Dictionary<StatusType, float> SpeedModifiers { get; private set; }
		#endregion


		private ClockworkHingeState _state = ClockworkHingeState.Close;
		private ClockworkHingeError _currentError = ClockworkHingeError.None;

		private Vector3 _currentRotation = Vector3.zero;

		private const float MIN_ROTATION_ALLOWED = -360f;
		private const float MAX_ROTATION_ALLOWED = 360f;
		private const float ROTATION_TOLERANCE = 0.1f;

		void Awake() {
			StatusConditions = new HashSet<IStatusCondition>();

			BaseSpeed = Speed;
			CurrentSpeed = BaseSpeed;
			SpeedModifiers = new Dictionary<StatusType, float>();

			_currentRotation = transform.localEulerAngles;

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


		#region // IPartDamageable Methods
		public void AddDamage(StatusType statusType, float damage) {
			DamageableHelpers.AddDamage(StatusControls, statusType, damage);
		}

		public void AddStatusCondition(IStatusCondition newCondition) {
			StatusConditions.Add(newCondition);
		}

		public void RemoveStatusCondition(IStatusCondition oldCondition) {
			StatusConditions.Remove(oldCondition);
		}
		#endregion


		#region // IPartControllableSpeed Methods
		public void SetSpeed(float newSpeed) {
			CurrentSpeed = Mathf.Max(0, newSpeed);
		}

		public void ResetSpeed() {
			SetSpeed(BaseSpeed);
		}

		public void UpsertModifier(StatusType statusType, float multiplier) {
			SpeedModifiers[statusType] = multiplier;
			SpeedModifierHelper.ModifySpeed(this);
		}

		public void DeleteModifier(StatusType statusType) {
			SpeedModifiers.Remove(statusType);
			SpeedModifierHelper.ModifySpeed(this);
		}
		#endregion


		private void UpdateOpening() {
			float openSpeed = GetSpeed();
			if (openSpeed == 0f) {
				SendFailure(ClockworkHingeError.CantOpen);
				return;
			} else {
				ClearFailure(ClockworkHingeError.CantOpen);
			}

				Vector3 targetRotation = Reverse ? MinRotation : MaxRotation;

			UpdateRotation(targetRotation, openSpeed);
			IfAtTargetRotation(targetRotation, ClockworkHingeState.Open, Opened);
		}

		private void UpdateClosing() {
			float closeSpeed = GetSpeed();
			if (closeSpeed == 0f) {
				SendFailure(ClockworkHingeError.CantClose);
				return;
			} else {
				ClearFailure(ClockworkHingeError.CantClose);
			}

				Vector3 targetRotation = Reverse ? MaxRotation : MinRotation;

			UpdateRotation(targetRotation, closeSpeed);
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

		private float GetSpeed() {
			if (DamageableHelpers.TryGetHighestPriorityCondition(StatusConditions, out IStatusCondition priorityCondition)) {
				if (priorityCondition.TryGetSpeedModifier(out float modifier))
					return Speed * modifier;
			}

			return CurrentSpeed;
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
