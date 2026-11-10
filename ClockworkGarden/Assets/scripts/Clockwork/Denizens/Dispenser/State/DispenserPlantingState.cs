using Assets.scripts.Clockwork.Parts;
using System;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserPlantingState : IDispenserState {
		public event Action Completed;
		public event Action Failed;

		private DispenserShooting _shooting;
		private bool _isActive = false;

		public DispenserPlantingState(
			ClockworkPartItemContainerController itemContainerControl,
			ClockworkPartHingeController hingeControl,
			ClockworkPartLauncherController launcherControl
		) {

			_shooting = new DispenserShooting(itemContainerControl, hingeControl, launcherControl);
		}

		public void OnEnable() {
			_shooting.Enable();
			_shooting.Completed += OnShootingCompleted;
		}

		public void OnDisable() {
			_shooting.Disable();
			_shooting.Completed -= OnShootingCompleted;
		}

		public void Update(bool grounded) { }

		public void Start() {
			if (_isActive)
				return;

			if (!_shooting.Activate()) {
				Failed?.Invoke();
				return;
			}

			_isActive = true;
		}

		public void Stop() {
			_isActive = false;
		}

		private void OnShootingCompleted() {
			_isActive = false;
			Completed?.Invoke();
		}
	}
}
