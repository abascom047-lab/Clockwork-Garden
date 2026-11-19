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
		public int CurrentValue = 0;

		public void Add(StatusType addType, int addValue) {
			if (addType == StatusType.None)
				CurrentValue += addValue;
		}
	}
}
