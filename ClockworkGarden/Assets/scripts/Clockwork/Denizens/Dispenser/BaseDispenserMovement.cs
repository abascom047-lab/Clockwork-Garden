using Assets.scripts.Clockwork.Parts;
using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class BaseDispenserMovement {
		public event Action Completed;

		public bool Active { get; protected set; }

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
			_propulsionControl.Completed += OnPropulsionControlCompleted;
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

		protected void Complete() {
			Completed?.Invoke();
		}

		protected virtual void Move() { }

		protected void OnPropulsionControlCompleted() {
			if (!Active)
				return;

			_movementTimer.Reset(true);
		}

		protected void OnMovementTimerCompleted() {
			Move();
		}
	}
}
