using UnityEngine;

namespace Assets.Scripts.Utils {
	public static class Tools {
		public static Quaternion RotateToFaceTarget(Quaternion currentRotation, Vector3 currentPosition, Vector3 targetPosition, float speed) {
			Vector3 direction = targetPosition - currentPosition;
			if (direction.sqrMagnitude < 0.0001f)
				return currentRotation;

			Quaternion targetRotation = Quaternion.LookRotation(direction);
			Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, speed * GameTime.Current);

			return newRotation;
		}

		public static Quaternion GetLookAtRotation(Vector3 startPosition, Vector3 lookAtPosition, bool zeroY = false) {
			if (startPosition == lookAtPosition)
				return Quaternion.identity;

			Vector3 direction = (lookAtPosition - startPosition).normalized;

			if (zeroY)
				direction.y = 0;

			return Quaternion.LookRotation(direction);
		}
	}
}
