using Assets.scripts.Clockwork.Parts.Helpers;
using Assets.scripts.Clockwork.Parts.Status;
using System.Collections.Generic;

namespace Assets.scripts.Clockwork.Parts.Interfaces {
	public interface IPartModifiableSpeed {
		float CurrentSpeed { get; }
		float BaseSpeed { get; }
		Dictionary<StatusType, float> SpeedModifiers { get; }

		void SetSpeed(float newSpeed);
		void ResetSpeed();
		void UpsertModifier(StatusType statusType, float multiplier);
		void DeleteModifier(StatusType statusType);
	}
}
