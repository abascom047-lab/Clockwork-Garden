using Assets.scripts.Clockwork.Parts;
using Assets.scripts.Items;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens {
	public class ClockworkCandyDispenserController : MonoBehaviour {
		public ClockworkPartHingeController TopHingeControl;
		public ClockworkPartItemContainerController ItemContainerControl;
		public Transform FireTransform;

		private Timer _fireTimer;

		void Awake() {
			_fireTimer = new Timer(0.5f, true);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;

			_fireTimer.Completed += OnFireTimerCompleted;
		}

		void OnDisable() {
			TopHingeControl.Opened -= OnTopHingeOpened;
			TopHingeControl.Closed -= OnTopHingeClosed;

			_fireTimer.Completed -= OnFireTimerCompleted;
		}

		void Update() {
			_fireTimer.Update();
		}

		private void OnTopHingeOpened() {
			if (ItemContainerControl.Pop(out BaseItemController poppedControl)) {
				if (poppedControl is ProjectileCandyController candyControl)
					candyControl.Fire(FireTransform.position, FireTransform.forward);
			}

			TopHingeControl.Close();
		}

		private void OnTopHingeClosed() {
			_fireTimer.Reset(ItemContainerControl.CanPop);
		}

		private void OnFireTimerCompleted() {
			if (!ItemContainerControl.CanPop)
				return;

			TopHingeControl.Open();
		}
	}
}
