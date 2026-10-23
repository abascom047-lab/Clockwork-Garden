using Assets.scripts.Clockwork.Parts;
using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class BaseDispenserMovement {
		public event Action Completed;
		public event Action Canceled;

		public bool Active { get; protected set; } = false;

		protected ClockworkPartPropulsionController _propulsionControl;
		protected Transform _transform;
		protected Timer _movementTimer;

		protected bool _grounded = true;

		public BaseDispenserMovement(ClockworkPartPropulsionController propulsionControl, Transform transform) {
			_propulsionControl = propulsionControl;
			_transform = transform;
			_movementTimer = new Timer(0.4f);
		}

		public virtual void Enable() {
			_propulsionControl.Completed += OnPropulsionControlCompleted;
			_movementTimer.Completed += OnMovementTimerCompleted;
		}

		public virtual void Disable() {
			_propulsionControl.Completed -= OnPropulsionControlCompleted;
			_movementTimer.Completed -= OnMovementTimerCompleted;
		}

		public virtual bool Update(bool grounded) {
			if (!Active)
				return false;

			_movementTimer.Update();
			_grounded = grounded;

			return true;
		}

		public virtual bool Activate() {
			if (Active)
				return false;

			Active = true;
			Move();

			return true;
		}

		public virtual bool Deactivate() {
			if (!Active)
				return false;

			Active = false;
			return true;
		}

		protected void Complete() {
			Active = false;
			Completed?.Invoke();
		}

		protected virtual void Move() { }

		protected void OnPropulsionControlCompleted(PropulsionEndReason reason) {
			if (!Active)
				return;

			if (reason == PropulsionEndReason.Collision) {
				Active = false;
				_movementTimer.Stop();
				Canceled?.Invoke();

				return;
			}

			_movementTimer.Reset(true);
		}

		protected void OnMovementTimerCompleted() {
			Move();
		}
	}
}
