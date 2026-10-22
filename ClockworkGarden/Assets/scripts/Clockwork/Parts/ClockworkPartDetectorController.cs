using Assets.scripts.Interactables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class ClockworkPartDetectorController : MonoBehaviour {
		public int Total => _detectedInteractableControls.Count;
		public List<BaseInteractableController> All=> _detectedInteractableControls.ToList();

		private HashSet<BaseInteractableController> _detectedInteractableControls;

		void OnTriggerEnter(Collider other) {
			if (_detectedInteractableControls == null)
				_detectedInteractableControls = new HashSet<BaseInteractableController>();

			if (other.TryGetComponent<BaseInteractableController>(out BaseInteractableController otherControl))
				_detectedInteractableControls.Add(otherControl);
		}

		void OnTriggerExit(Collider other) {
			if (_detectedInteractableControls == null)
				_detectedInteractableControls = new HashSet<BaseInteractableController>();

			if (_detectedInteractableControls.Count == 0)
				return;

			if (other.TryGetComponent<BaseInteractableController>(out BaseInteractableController otherControl))
				_detectedInteractableControls.Remove(otherControl);
		}

		public List<T> GetAll<T>() {
			List<T> result = new List<T>();

			if (_detectedInteractableControls == null || _detectedInteractableControls.Count == 0)
				return result;

			foreach (BaseInteractableController interactableControl in _detectedInteractableControls) {
				if (interactableControl is T TControl)
					result.Add(TControl);
			}

			return result;
		}
	}
}
