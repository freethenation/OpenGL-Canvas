using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using JollyBit.Canvas.Shapes;

namespace JollyBit.Canvas
{
	public abstract class Canvas
	{
		public Canvas()
		{
			//Misc
			InBatch = false;
			//Lines
			LineWidth = 1.0f;
			LineCap = LineCapStyle.Butt;
			LineJoin = LineJoinStyle.Miter;
			MiterLimit = 10.0f;
		}

		#region  Transformations
		private Matrix4 _tranMatrix = Matrix4.Identity;
		public void Scale(float x, float y)
		{
			Matrix4 m = Matrix4.Scale(x, y, 1);
			Matrix4.Mult(ref m, ref _tranMatrix, out _tranMatrix);
		}
		public void Rotate(float angle)
		{
			Matrix4 m = Matrix4.CreateRotationZ(angle);
			Matrix4.Mult(ref m, ref _tranMatrix, out _tranMatrix);
		}
		public void Translate(float x, float y)
		{
			Matrix4 m = Matrix4.CreateTranslation(new Vector3(x, y, 0));
			Matrix4.Mult(ref m, ref _tranMatrix, out _tranMatrix);
		}
		public void Transform(float m00, float m10, float m01, float m11, float m02, float m12)
		{
			Matrix4 m = new Matrix4(m00, m01, m02, 0, m10, m11, m12, 0, 0, 0, 1, 0, 0, 0, 0, 1);
			Matrix4.Mult(ref m, ref _tranMatrix, out _tranMatrix);
		}
		public void SetTransform(float m00, float m10, float m01, float m11, float m02, float m12)
		{
			_tranMatrix = Matrix4.Identity;
			Transform(m00, m10, m01, m11, m02, m12);
		}
		protected Vector3 applyCurrentTransform(Vector3 point)
		{
			Vector3.Transform(ref point, ref _tranMatrix, out point);
			return point;
		}
		protected IEnumerable<Vector3> applyCurrentTransform(IEnumerable<Vector3> points)
		{
			return points.Select(
				i =>
				{
					Vector3.Transform(ref i, ref _tranMatrix, out i);
					return i;
				});
		}
		#endregion

		#region Batching
		public bool InBatch { get; private set; }
		public virtual void BeginBatch() { InBatch = true; }
		public virtual void EndBatch() { InBatch = false; }
		#endregion

		#region Lines
		private float _lineWidth;
		public float LineWidth
		{
			get { return _lineWidth; }
			set
			{
				if (value < 0f || float.IsInfinity(value) || float.IsNaN(value)) return;
				_lineWidth = value;
			}
		}
		public LineCapStyle LineCap { get; set; }
		public LineJoinStyle LineJoin { get; set; }
		public float MiterLimit { get; set; }
		#endregion

		public void Rect(float x, float y, float w, float h)
		{
			_subPaths.Add(new Rect(x, y, w, h, _tranMatrix));
		}

		protected virtual void strokeLineSegment(LineSegment segment1, LineSegment segment2, LineJoinStyle joinStyle)
		{
			//Draw Linsegment
			fillConvexPolygon(segment1);
			//Draw join
			fillConvexPolygon(segment1.RightEndPoint, segment1.EndPoint, segment2.RightStartPoint);
		}

		public virtual void Stroke()
		{
			foreach (ISubpath subpath in _subPaths)
			{
				var segments = (subpath.IsClosed ? subpath.Repeat((vect, itemIndex, repeatCount) => repeatCount < 1 || itemIndex < 1) : subpath)
					.Neighbors()
					.Select(i=> new LineSegment(i.Item1, i.Item2, LineWidth));
				foreach (var segmentPair in (subpath.IsClosed ? segments.Repeat((rect, itemIndex, repeatCount) => repeatCount < 1 || itemIndex < 1) : segments).Neighbors())
				{
					strokeLineSegment(segmentPair.Item1, segmentPair.Item2, LineJoinStyle.Miter);
				}
			}
		}

		public virtual void Fill()
		{
		}

		protected abstract void fillConvexPolygon(IEnumerable<Vector2> points);
		protected void fillConvexPolygon(params Vector2[] points)
		{
			fillConvexPolygon((IEnumerable<Vector2>)points);
		}
		protected IList<ISubpath> _subPaths = new List<ISubpath>();
	}

	public enum LineCapStyle
	{
		/// <summary>
		///  The butt value means that the end of each line has a flat edge perpendicular to the direction of the line (and that no additional line cap is added).
		/// </summary>
		Butt,
		/// <summary>
		/// The round value means that a semi-circle with the diameter equal to the width of the line must then be added on to the end of the line.
		/// </summary>
		Round,
		/// <summary>
		///  The square value means that a rectangle with the length of the line width and the width of half the line width, placed flat against the edge perpendicular to the direction of the line, must be added at the end of each line.
		/// </summary>
		Square
	}

	public enum LineJoinStyle
	{
		Bevel,
		Round,
		Miter
	}

	public enum CompositeOperations
	{
		/// <summary>
		/// A atop B. Display the source image wherever both images are opaque. Display the destination image wherever the destination image is opaque but the source image is transparent. Display transparency elsewhere.
		/// </summary>
		SourceAtop,
		/// <summary>
		/// A in B. Display the source image wherever both the source image and destination image are opaque. Display transparency elsewhere.
		/// </summary>
		SourceIn,
		/// <summary>
		/// A out B. Display the source image wherever the source image is opaque and the destination image is transparent. Display transparency elsewhere.
		/// </summary>
		SourceOut,
		/// <summary>
		/// A over B. Display the source image wherever the source image is opaque. Display the destination image elsewhere.
		/// </summary>
		SourceOver,
		/// <summary>
		/// B atop A. Same as source-atop but using the destination image instead of the source image and vice versa.
		/// </summary>
		DestinationAtop,
		/// <summary>
		/// B in A. Same as source-in but using the destination image instead of the source image and vice versa.
		/// </summary>
		DestinationIn,
		/// <summary>
		/// B out A. Same as source-out but using the destination image instead of the source image and vice versa.
		/// </summary>
		DestinationOut,
		/// <summary>
		/// B over A. Same as source-over but using the destination image instead of the source image and vice versa.
		/// </summary>
		DestinationOver,
		/// <summary>
		/// A plus B. Display the sum of the source image and destination image, with color values approaching 255 (100%) as a limit.
		/// </summary>
		Lighter,
		/// <summary>
		/// A (B is ignored). Display the source image instead of the destination image.
		/// </summary>
		Copy,
		/// <summary>
		/// A xor B. Exclusive OR of the source image and destination image.
		/// </summary>
		Xor
	}
}
