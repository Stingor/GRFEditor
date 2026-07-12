using GRF.Image;
using System;
using System.Windows;
using TokeiLibrary.WPF.Styles;

namespace GRFEditor.Tools.SpriteEditor {
	public partial class ResizeDialog : TkWindow {
		private bool _isUpdating;

		public float ScaleX { get; private set; }
		public float ScaleY { get; private set; }
		public GrfScalingMode ScalingMode { get; private set; }

		public ResizeDialog() : base("Resize", "scale.png", SizeToContent.Height) {
			InitializeComponent();

			_widthBox.Loaded += (s, e) => {
				_widthBox.SelectAll();
				_widthBox.Focus();
			};
		}

		private void _widthBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			if (_isUpdating || _lockAspect == null || _heightBox == null) return;
			if (_lockAspect.IsChecked == true && float.TryParse(_widthBox.Text, out float v)) {
				_isUpdating = true;
				_heightBox.Text = _widthBox.Text;
				_isUpdating = false;
			}
		}

		private void _heightBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			if (_isUpdating || _lockAspect == null || _widthBox == null) return;
			if (_lockAspect.IsChecked == true && float.TryParse(_heightBox.Text, out float v)) {
				_isUpdating = true;
				_widthBox.Text = _heightBox.Text;
				_isUpdating = false;
			}
		}

		private void _buttonOk_Click(object sender, RoutedEventArgs e) {
			if (!float.TryParse(_widthBox.Text, out float scaleXPct) || scaleXPct <= 0) {
				MessageBox.Show("Width must be a positive number.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (!float.TryParse(_heightBox.Text, out float scaleYPct) || scaleYPct <= 0) {
				MessageBox.Show("Height must be a positive number.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			ScaleX = scaleXPct / 100f;
			ScaleY = scaleYPct / 100f;
			ScalingMode = _scalingMode.SelectedIndex == 1 ? GrfScalingMode.LinearScaling : GrfScalingMode.NearestNeighbor;

			DialogResult = true;
			Close();
		}

		private void _buttonCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}
	}
}
