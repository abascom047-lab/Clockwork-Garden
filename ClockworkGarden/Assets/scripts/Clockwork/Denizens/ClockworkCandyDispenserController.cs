using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Miscellaneous;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens {
	public class ClockworkCandyDispenserController : MonoBehaviour {
		public ClockworkPartHingeController TopHingeControl;
		public ClockworkPartItemContainerController ItemContainerControl;
		public ClockworkPartLauncherController LauncherControl;
		public ClockworkPartPropulsionController PropulsionControl;
		public GravityController GravityControl;

		private Timer _fireTimer;
		private bool _once = false;

		void Awake() {
			_fireTimer = new Timer(0.5f);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;
			PropulsionControl.Completed += OnPropulsionControlCompleted;
			GravityControl.Grounded += OnGravityControlGrounded;

			_fireTimer.Completed += OnFireTimerCompleted;
		}

		void OnDisable() {
			TopHingeControl.Opened -= OnTopHingeOpened;
			TopHingeControl.Closed -= OnTopHingeClosed;
			PropulsionControl.Completed -= OnPropulsionControlCompleted;
			GravityControl.Grounded -= OnGravityControlGrounded;

			_fireTimer.Completed -= OnFireTimerCompleted;
		}

		void Update() {
			_fireTimer.Update();

			if (!_once) {
				PropulsionControl.Activate(new Vector3(transform.forward.x, 1.5f, transform.forward.z));
				_once = true;
			}
		}

		private void OnTopHingeOpened() {
			LauncherControl.Fire();
			TopHingeControl.Close();
		}

		private void OnTopHingeClosed() {
			_fireTimer.Reset(ItemContainerControl.CanPop);
		}

		private void OnPropulsionControlCompleted() {
			PropulsionControl.Activate(new Vector3(transform.forward.x, 2f, transform.forward.z));
		}

		private void OnGravityControlGrounded() {
			PropulsionControl.Deactivate();
		}

		private void OnFireTimerCompleted() {
			if (!ItemContainerControl.CanPop)
				return;

			TopHingeControl.Open();
		}
	}
}
