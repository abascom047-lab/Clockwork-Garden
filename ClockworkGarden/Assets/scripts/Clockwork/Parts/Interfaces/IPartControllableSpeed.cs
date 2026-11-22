namespace Assets.scripts.Clockwork.Parts.Interfaces {
	public interface IPartControllableSpeed {
		public float GetBaseSpeed();
		public void SetSpeed(float newSpeed);
		public void ResetSpeed();
		public float GetSpeed();
		public float GetBaseSpeedByIndex(int index);
		public void SetSpeedByIndex(int index, float newSpeed);
		public void ResetSpeedByIndex(int index);
		public float GetSpeedByIndex(int index);
	}
}
