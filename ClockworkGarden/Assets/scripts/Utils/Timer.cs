using System;
using System.Diagnostics;

namespace Assets.Scripts.Utils {
    public class Timer {
        public event Action Completed;

		public bool Active { get; private set; } = false;
        public bool Loop { get; private set; } = false;
		public float CurrentTime { get; private set; } = 0.0f;

		private readonly float maxTime;


		public Timer(float duration, bool start = false, bool loop = false) {
			if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f)
				throw new ArgumentOutOfRangeException(
					nameof(duration),
					"Timer duration must be a positive, finite number."
				);

			Active = start;
            Loop = loop;
			maxTime = duration;
		}

        public void Update() {
			if (!Active)
				return;

			CurrentTime += GameTime.Current;

			if (CurrentTime < maxTime)
				return;

            if (Loop) {
                CurrentTime %= maxTime;
            } else {
                Stop();
            }

			Completed?.Invoke();
		}

        public void Start() {
            Active = true;
        }

        public void Pause() {
            Active = false;
        }

        public void Stop(bool invokeAction = false) {
			CurrentTime = 0.0f;
			Active = false;

            if (invokeAction)
                Completed?.Invoke();
        }

        public void Reset(bool start = false) {
            CurrentTime = 0.0f;
            Active = start;
        }
    }
}
