using GRF.FileFormats.ActFormat;
using GRF.FileFormats.ActFormat.Commands;
using GRF.Image;
using System;
using System.Collections.Generic;

namespace GRF.FileFormats.SprFormat.Commands {
	public class Resize : IActCommand {
		private readonly int _absoluteIndex;
		private readonly float _scaleX;
		private readonly float _scaleY;
		private readonly GrfScalingMode _scalingMode;
		private GrfImage _originalImage;
		private List<(Anchor anchor, int origX, int origY)> _originalAnchors;

		public Resize(int absoluteIndex, float scaleX, float scaleY, GrfScalingMode scalingMode) {
			_absoluteIndex = absoluteIndex;
			_scaleX = scaleX;
			_scaleY = scaleY;
			_scalingMode = scalingMode;
		}

		#region IActCommand Members

		public void Execute(Act act) {
			var image = act.Sprite.Images[_absoluteIndex];
			_originalImage = image.Clone();
			// NearestNeighbor for Indexed8 to avoid unintended type conversion
			var mode = image.GrfImageType == GrfImageType.Indexed8 ? GrfScalingMode.NearestNeighbor : _scalingMode;
			image.Scale(_scaleX, _scaleY, mode);
			_updateAnchors(act);
		}

		public void Undo(Act act) {
			act.Sprite.Images[_absoluteIndex] = _originalImage;

			if (_originalAnchors != null) {
				foreach (var (anchor, origX, origY) in _originalAnchors) {
					anchor.OffsetX = origX;
					anchor.OffsetY = origY;
				}
			}
		}

		private void _updateAnchors(Act act) {
			_originalAnchors = new List<(Anchor, int, int)>();

			foreach (var action in act) {
				foreach (var frame in action.Frames) {
					Layer refLayer = null;

					foreach (var layer in frame.Layers) {
						if (layer.GetAbsoluteSpriteId(act.Sprite) == _absoluteIndex) {
							refLayer = layer;
							break;
						}
					}

					if (refLayer == null || frame.Anchors.Count == 0)
						continue;

					foreach (var anchor in frame.Anchors) {
						_originalAnchors.Add((anchor, anchor.OffsetX, anchor.OffsetY));
						anchor.OffsetX = (int)Math.Round(refLayer.OffsetX + _scaleX * (anchor.OffsetX - refLayer.OffsetX));
						anchor.OffsetY = (int)Math.Round(refLayer.OffsetY + _scaleY * (anchor.OffsetY - refLayer.OffsetY));
					}
				}
			}
		}

		public string CommandDescription {
			get { return "Resize sprite image " + _absoluteIndex; }
		}

		#endregion
	}
}
