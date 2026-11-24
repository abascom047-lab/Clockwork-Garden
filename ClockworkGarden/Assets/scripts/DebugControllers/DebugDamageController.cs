using Assets.scripts.Clockwork.Parts.Interfaces;
using Assets.scripts.Clockwork.Parts.Status;
using UnityEngine;

namespace Assets.scripts.DebugControllers {
	public class DebugDamageController : MonoBehaviour {
		public StatusType Type = StatusType.None;
		public float Value = 0f;
		public bool Ready = false;

		private IPartDamageable damageableInterface;

		void Awake() {
			damageableInterface = GetComponent<IPartDamageable>();
		}

		void Update() {
			if (Ready) {
				damageableInterface.AddDamage(Type, Value);
				Ready = false;
			}
		}
	}
}
