using System;

namespace Assets.scripts.Clockwork.Denizens.Dispenser {
	public interface IDispenserState {
		void OnEnable();
		void OnDisable();
		void Update(bool grounded);
		void Start();
		void Stop();
	}
}
