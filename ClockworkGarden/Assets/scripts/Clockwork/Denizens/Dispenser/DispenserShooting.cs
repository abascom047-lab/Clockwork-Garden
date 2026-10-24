using Assets.scripts.Clockwork.Parts;
using System;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public class DispenserShooting {
		public bool Active { get; private set; } = false;

		public event Action Completed;

		private ClockworkPartItemContainerController _itemContainerControl;
		private ClockworkPartHingeController _hingeControl;
		private ClockworkPartLauncherController _launcherControl;

		public DispenserShooting(
			ClockworkPartItemContainerController itemContainerControl,
			ClockworkPartHingeController hingeControl,
			ClockworkPartLauncherController launcherControl)
		{
			_itemContainerControl = itemContainerControl;
			_hingeControl = hingeControl;
			_launcherControl = launcherControl;
		}

		public void Enable() {
			_hingeControl.Opened += OnHingeControlOpened;
			_hingeControl.Closed += OnHingeControlClosed;
		}

		public void Disable() {
			_hingeControl.Opened -= OnHingeControlOpened;
			_hingeControl.Closed -= OnHingeControlClosed;
		}

		public bool Activate() {
			if (Active || !_itemContainerControl.CanPop)
				return false;

			Active = true;
			_hingeControl.Open();

			return true;
		}

		private void OnHingeControlOpened() {
			_launcherControl.Fire();
			_hingeControl.Close();
		}

		private void OnHingeControlClosed() {
			Active = false;
			Completed?.Invoke();
		}
	}
}
