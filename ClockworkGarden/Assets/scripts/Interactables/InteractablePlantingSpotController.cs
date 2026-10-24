using Assets.scripts.Items;
using UnityEngine;

namespace Assets.scripts.Interactables {
	public class InteractablePlantingSpotController : BaseInteractableController {
		public bool Planted { get; private set; } = false;

		protected override void Triggered(Collider other) {
			if (other.TryGetComponent<ProjectileCandyController>(out ProjectileCandyController candyControl)) {
				Planted = true;
			}
		}
	}
}
