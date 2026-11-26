namespace Assets.scripts.Clockwork.Parts.Status {
	public enum SingleThresholdState {
		None,
		Above,
		At,
		Below
	}

	public class SingleThresholdStatusController : BaseStatusController {
		protected int _threshold;
		protected SingleThresholdState _state;

		protected override void OnUpdate() {
			if (CurrentValue == _threshold && _state != SingleThresholdState.At)
				AtThreshold();

			if (CurrentValue > _threshold && _state != SingleThresholdState.Above)
				AboveThreshold();

			if (CurrentValue < _threshold && _state != SingleThresholdState.Below)
				BelowThreshold();

			base.OnUpdate();
		}

		protected virtual void AtThreshold() {
			_state = SingleThresholdState.At;
		}

		protected virtual void AboveThreshold() {
			_state = SingleThresholdState.Above;
		}

		protected virtual void BelowThreshold() {
			_state = SingleThresholdState.Below;
		}
	}
}
