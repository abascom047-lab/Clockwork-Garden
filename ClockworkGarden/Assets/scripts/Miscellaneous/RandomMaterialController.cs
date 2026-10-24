using UnityEngine;

namespace Assets.scripts.Miscellaneous {
	public class RandomMaterialController : MonoBehaviour {
		public Material[] PossibleMaterials;
		public Renderer MaterialRenderer;

		void Awake() {
			ResetMaterial();
		}

		public void ResetMaterial() {
			int materialIndex = Random.Range(0, PossibleMaterials.Length);
			MaterialRenderer.material = PossibleMaterials[materialIndex];
		}
	}
}
