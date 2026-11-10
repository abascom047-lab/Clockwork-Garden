using System;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public interface IDispenserState {
		void Enable();
		void Disable();
		void Update(bool grounded);
		void Start();
		void Stop();
	}
}
