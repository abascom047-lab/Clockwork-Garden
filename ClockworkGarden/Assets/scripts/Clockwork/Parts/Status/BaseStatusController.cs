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
		public float CurrentValue = 0f;
		public float StartValue = 0f;
		public float DecaySpeed = 0f;

		void Start() {
			OnStart();
		}

		void Update() {
			if (DecaySpeed > 0)
				CurrentValue = Mathf.MoveTowards(CurrentValue, StartValue, DecaySpeed * GameTime.Current);

			OnUpdate();
		}

		public void Add(StatusType addType, int addValue) {
			if (addType == Type)
				CurrentValue += addValue;
		}

		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
	}
}
