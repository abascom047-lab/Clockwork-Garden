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
	}
}
