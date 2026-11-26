namespace Assets.scripts.Clockwork.Parts.Status {
	public class SingleThresholdStatusController : BaseStatusController {
		protected int _threshold;
		protected bool _above = false;
		protected bool _below = false;

		protected override void OnUpdate() {
			if (CurrentValue == _threshold && (_above || _below))
				AtThreshold();

			if (CurrentValue > _threshold && !_above)
				AboveThreshold();

			if (CurrentValue < _threshold && !_below)
				BelowThreshold();

			base.OnUpdate();
		}

		protected virtual void AtThreshold() {
			_above = false;
			_below = false;
		}

		protected virtual void AboveThreshold() {
			_above = true;
			_below = false;
		}

		protected virtual void BelowThreshold() {
			_above = false;
			_below = true;
		}
	}
}
