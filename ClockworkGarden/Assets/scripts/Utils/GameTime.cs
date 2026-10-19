using UnityEngine;

namespace Assets.Scripts.Utils {
	public static class GameTime {
		public static float Multiplier = 1;
		public static float UiMultiplier = 1;

		public static bool IsStopped => Multiplier == 0;

		public static bool IsUiStopped => Multiplier == 1;
		
		public static float Current => Time.deltaTime * Multiplier;

		public static float Ui => Time.deltaTime * UiMultiplier;

		public static void Stop() {
			Multiplier = 0;
		}

		public static void Start() {
			Multiplier = 1;
		}

		public static void StopUi() {
			UiMultiplier = 0;
		}

		public static void StartUi() {
			UiMultiplier = 1;
		}
	}
}
