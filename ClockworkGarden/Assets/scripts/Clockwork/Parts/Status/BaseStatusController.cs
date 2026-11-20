using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public enum StatusType {
		None,
		Magnetism,
		Clocking,
		Temperature,
		Sticky,
		Sanity
	}

	public class BaseStatusController : MonoBehaviour {
		public StatusType Type = StatusType.None; 
		public float CurrentValue = 0;
		public float StartValue = 0;
		public float DecaySpeed = 4f;

		private Timer _decayTimer;

		private const float INCREMENT = 10f;

		void Awake() {
			CurrentValue = StartValue;
			_decayTimer = new Timer(DecaySpeed, DecaySpeed > 0, DecaySpeed > 0);
		}

		void OnEnable() {
			_decayTimer.Completed += OnDecayTimerCompleted;
		}

		void OnDisable() {
			_decayTimer.Completed -= OnDecayTimerCompleted;
		}

		void Update() {
			_decayTimer.Update();
			OnUpdate();
		}

		public void Add(StatusType addType, int addValue) {
			if (addType == Type)
				CurrentValue += addValue;

			_decayTimer.Reset(true);
		}

		protected virtual void OnUpdate() { }

		private void OnDecayTimerCompleted() {
			CurrentValue = Mathf.MoveTowards(CurrentValue, StartValue, INCREMENT);
		}
	}
}
