using UnityEngine;

namespace Assets.scripts.Items {
	public class BaseItemController : MonoBehaviour {
		public bool Active { get; protected set; } = false;

		public virtual void Setup(Vector3 position, bool active) {
			Active = active;
			transform.position = position;

			gameObject.SetActive(active);
		}
	}
}
