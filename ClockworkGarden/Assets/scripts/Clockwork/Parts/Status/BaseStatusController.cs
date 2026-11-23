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
		public float CurrentValue = 0f;
		public float StartValue = 0f;
		public float DecaySpeed = 0f;

		protected StatusType _type = StatusType.None;

		void Start() {
			OnStart();
		}

		void Update() {
			if (DecaySpeed > 0)
				CurrentValue = Mathf.MoveTowards(CurrentValue, StartValue, DecaySpeed * GameTime.Current);

			OnUpdate();
		}

		public void Add(StatusType addType, int addValue) {
			if (addType == _type)
				CurrentValue += addValue;
		}

		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
	}
}
