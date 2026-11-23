using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Helpers {
	public static class SpeedModifierHelper {
		public static void ModifySpeed(IPartModifiableSpeed modSpeed) {
			float newSpeed = modSpeed.BaseSpeed;
			foreach (float modifier in modSpeed.SpeedModifiers.Values)
				newSpeed *= modifier;

			modSpeed.SetSpeed(newSpeed);
		}
	}
}
