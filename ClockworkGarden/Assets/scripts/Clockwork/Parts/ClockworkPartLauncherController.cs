using Assets.scripts.Items;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class ClockworkPartLauncherController : MonoBehaviour {
		public ClockworkPartItemContainerController ItemContainerControl;

		public void Fire() {
			if (ItemContainerControl == null)
				return;

			if (ItemContainerControl.Pop(out BaseItemController poppedControl))
				Fire(poppedControl);
		}

		public void Fire(BaseItemController itemControl) { 
			if (itemControl is ProjectileCandyController projectileControl)
				Fire(projectileControl);
		}

		public void Fire(ProjectileCandyController projectileControl) {
			projectileControl.Fire(transform.position, transform.forward);
		}
	}
}
