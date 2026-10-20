using Assets.scripts.Items;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts {
	public class ClockworkPartItemContainerController : MonoBehaviour {
		public GameObject ItemPrefab;
		public int MaxItems = 1;
		public Vector3 StackDirection = Vector3.zero;
		public bool LastInFirstOut = false;

		public bool CanPop => LastInFirstOut ? _currentItemIndex > 0 : _currentItemIndex < MaxItems - 1;

		private BaseItemController[] _itemControls;
		private int _currentItemIndex = -1;

		void Awake() {
			if (MaxItems == 0)
				Debug.LogError($"Max Items can't be zero: {MaxItems}");

			if (LastInFirstOut)
				_currentItemIndex = MaxItems;

			_itemControls = new BaseItemController[MaxItems];
			for (int i = 0; i < MaxItems; i++) {
				GameObject newItemObject = GameObject.Instantiate(ItemPrefab);
				BaseItemController newItemControl = newItemObject.GetComponent<BaseItemController>();
				
				newItemControl.Setup(transform.position + (StackDirection * i), false);

				_itemControls[i] = newItemControl;
			}
		}

		private void Update() {
			for (int i = 0; i < MaxItems; i++) {
				if (!_itemControls[i].Active)
					_itemControls[i].transform.position = transform.position + (StackDirection * i);
			}
		}

		public bool Pop(out BaseItemController poppedControl) {
			poppedControl = null;
			int direction = LastInFirstOut ? -1 : 1;
			_currentItemIndex += direction;

			if (_currentItemIndex >= MaxItems || _currentItemIndex < 0)
				return false;

			poppedControl = _itemControls[_currentItemIndex];
			return true;
		}

		public void Reload() {
			_currentItemIndex = LastInFirstOut ? MaxItems : -1;

			for (int i = 0; i < MaxItems; i++)
				_itemControls[i].Setup(transform.position + (StackDirection * i), false);
		}
	}
}
