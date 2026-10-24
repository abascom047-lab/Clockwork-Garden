using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Items {
	public class ProjectileCandyController : BaseItemController {
		public float Speed = 15f;
		public float Gravity = 5f;

		private Vector3 _velocity;
		private Timer _lifeTimer;

		void Awake() {
			_lifeTimer = new Timer(1.0f);
		}

		void OnEnable() {
			_lifeTimer.Completed += OnLifeTimerCompleted;
		}

		void OnDisable() {
			_lifeTimer.Completed -= OnLifeTimerCompleted;
		}

		void Update() {
			_lifeTimer.Update();

			if (!Active)
				return;

			float delta = GameTime.Current;
			transform.position += _velocity * delta;
			_velocity += Vector3.down * Gravity * delta;
		}

		public override void Setup(Vector3 position, bool active) {
			_lifeTimer.Reset(active);

			base.Setup(position, active);
		}

		public void Fire(Vector3 startPosition, Vector3 direction) {
			if (Active)
				return;

			_velocity = direction.normalized * Speed;
			_lifeTimer.Start();

			base.Setup(startPosition, true);
		}

		private void OnLifeTimerCompleted() {
			base.Setup(Vector3.zero, false);
		}
	}
}
