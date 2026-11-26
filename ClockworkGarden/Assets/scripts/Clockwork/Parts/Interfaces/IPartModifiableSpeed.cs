using Assets.scripts.Clockwork.Parts.Status;
using System.Collections.Generic;

namespace Assets.scripts.Clockwork.Parts.Interfaces {
	public interface IPartModifiableSpeed {
		float BasePrimarySpeed { get; }
		float BaseSecondarySpeed { get; }
		float CurrentPrimarySpeed { get; }
		float CurrentSecondarySpeed { get; }
		Dictionary<StatusType, float> PrimarySpeedModifiers { get; }
		Dictionary<StatusType, float> SecondarySpeedModifiers { get; }

		void SetSpeed(float newSpeed);
		void ResetSpeed();
		void SetPrimarySpeed(float newSpeed);
		void ResetPrimarySpeed();
		void SetSecondarySpeed(float newSpeed);
		void ResetSecondarySpeed();
		void UpsertSpeedModifier(StatusType statusType, float multiplier);
		void DeleteSpeedModifier(StatusType statusType);
		void UpsertPrimarySpeedModifier(StatusType statusType, float multiplier);
		void DeletePrimarySpeedModifier(StatusType statusType);
		void UpsertSecondarySpeedModifier(StatusType statusType, float multiplier);
		void DeleteSecondarySpeedModifier(StatusType statusType);
	}
}
