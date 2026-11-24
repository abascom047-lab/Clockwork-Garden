using Assets.scripts.Clockwork.Parts.Status;

namespace Assets.scripts.Clockwork.Parts.Interfaces {
	public interface IPartDamageable {
		void AddDamage(StatusType statusType, float damage);
	}
}
