using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public enum StatusType {
		None,
		Physical,
		Durability,
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
			OnUpdate();
		}

		public virtual void Add(StatusType addType, float addValue) {
			if (addType == _type)
				CurrentValue += addValue;
		}

		public virtual void Set(StatusType setType, float newValue) {
			if (setType == _type)
				CurrentValue = newValue;
		}

		protected virtual void OnStart() { }
		1
		protected virtual void OnUpdate() {
			if (DecaySpeed > 0)
				CurrentValue = Mathf.MoveTowards(CurrentValue, StartValue, DecaySpeed * GameTime.Current);
		}
	}
}
