using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Items {
	public class ProjectileCandyController : BaseItemController {
		public float Speed = 15f;
		public float Gravity = 5f;

		private Vector3 _direction;
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
			if (!Active)
				return;

			_lifeTimer.Update();
			
			
			Vector3 directionVector = _direction * Speed;
			if (Gravity > 0)
				directionVector += Vector3.down * Gravity;

			transform.position += directionVector * GameTime.Current;
		}

		public override void Setup(Vector3 position, bool active) {
			_lifeTimer.Reset(active);

			base.Setup(position, active);
		}

		public void Fire(Vector3 startPosition, Vector3 direction) {
			if (Active)
				return;

			_direction = direction;
			_lifeTimer.Start();

			base.Setup(startPosition, true);
		}

		private void OnLifeTimerCompleted() {
			base.Setup(Vector3.zero, false);
		}
	}
}
