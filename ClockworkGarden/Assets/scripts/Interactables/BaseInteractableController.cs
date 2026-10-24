using UnityEngine;

namespace Assets.scripts.Interactables {
	public class BaseInteractableController : MonoBehaviour {
		void OnTriggerEnter(Collider other) {
			Triggered(other);
		}

		protected virtual void Triggered(Collider other) { }
	}
}
