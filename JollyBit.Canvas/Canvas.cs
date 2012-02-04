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
			CurveSmoothness = 0.5f;
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

		#region Paths
		public void BeginPath()
		{
			_subPaths.Clear();
		}
		public void ClosePath() // Untested
		{
			if(lastSubpath != null && lastSubpath is ComplexSubpath)
				(lastSubpath as ComplexSubpath).ClosePath();
		}
		public void MoveTo(float x, float y)
		{
			ComplexSubpath subpath = new ComplexSubpath(new Vector2(x, y).ApplyTransform(ref _tranMatrix));
			_subPaths.Add(subpath);
		}
		
		public void LineTo(float x, float y)
		{
			ComplexSubpath subpath = ensureComplexSubpath(x, y);
			if (subpath == null) return;
			subpath.LineTo(new Vector2(x, y).ApplyTransform(ref _tranMatrix));		
		}
		public void QuadraticCurveTo(float cpx, float cpy, float x, float y)
		{
			ComplexSubpath subpath = ensureComplexSubpath(x, y);
			if (subpath == null) return;
			subpath.QuadraticCurveTo(new Vector2(cpx, cpy).ApplyTransform(ref _tranMatrix), new Vector2(x, y).ApplyTransform(ref _tranMatrix), CurveSmoothness);
		}
		public void BezierCurveTo(float cp1x, float cp1y, float cp2x, float cp2y, float x, float y)
		{
			throw new System.NotImplementedException();
		}
		public void ArcTo(float x1, float y1, float x2, float y2, float radius)
		{
			throw new System.NotImplementedException();
		}
		public void Rect(float x, float y, float w, float h)
		{
			_subPaths.Add(new RectSubpath(x, y, w, h));
		}
		public void Arc(float x, float y, float radius, float startAngle, float endAngle, bool anticlockwise)
		{
			throw new System.NotImplementedException();
		}
		public void Arc(float x, float y, float radius, float startAngle, float endAngle)
		{
			Arc(x,y,radius,startAngle,endAngle,false);
		}
		/// <summary>
		/// Sets how accurately curves should be drawn. The default value is 0.5 which is smoother than the display device can display.
		/// </summary>
		public float CurveSmoothness { get; set; }
		
		#region Internal
		/// <summary>
		/// Ensures their is a subpath and returns the subpath if found. If the subpath is not found a new subpath is added to _subpaths and null is returned.
		/// Consult http://www.whatwg.org/specs/web-apps/current-work/multipage/the-canvas-element.html#ensure-there-is-a-subpath for more info.
		/// </summary>
		protected ComplexSubpath ensureComplexSubpath(float x, float y)
		{
			if (lastSubpath == null || lastSubpath.IsClosed)
			{
				MoveTo(x, y);
				return null;
			}
			if (!(lastSubpath is ComplexSubpath))
			{
				lastSubpath = new ComplexSubpath(lastSubpath);
			}
			return lastSubpath as ComplexSubpath;
		}
		
		protected ISubpath lastSubpath
		{
			get
			{
				if (_subPaths.Count == 0) return null;
				return _subPaths[_subPaths.Count - 1];
			}
			set
			{
				if (value == null) throw new System.InvalidOperationException("The lastSubpath can not be set to null");
				_subPaths[_subPaths.Count - 1] = value;
			}
		}
		#endregion
		#endregion
		
		#region Clip
		public void clip()
		{
			throw new System.NotImplementedException();	
		}
		#endregion
		
		#region Misc
		/// <summary>
		/// Determines whether the specified point is in the current path.
		/// </summary>
		bool IsPointInPath(double x, double y)
		{
			throw new System.NotImplementedException();	
		}
		#endregion
		
		#region Text
		public String Font { get; set; }
        public TextAlign textAlign { get; set; }
		public TextBaseline TextBaseline { get; set; } // "top", "hanging", "middle", "alphabetic", "ideographic", "bottom" (default: "alphabetic")
		public void FillText(string text, double x, double y, double maxWidth)
		{
			throw new System.NotImplementedException();
		}
		public void FillText(string text, double x, double y)
		{
			throw new System.NotImplementedException();
		}
		public void StrokeText(string text, double x, double y, double maxWidth)
		{
			throw new System.NotImplementedException();
		}
		public void StrokeText(string text, double x, double y)
		{	
			throw new System.NotImplementedException();
		}
		public ITextMetrics MeasureText(string text)
		{
			throw new System.NotImplementedException();
		}
		#endregion

		#region Stroking
		protected virtual void strokeLineSegment(LineSegment segment1, LineSegment segment2, LineJoinStyle joinStyle, bool joinOnLeft)
		{
			//Draw Linsegment
			if (segment1 == null) return;
			fillConvexPolygon(segment1);
			//Draw join
			if (segment2 == null) return;
			Vector2 joinEndPoint = joinOnLeft ? segment1.LeftEndPoint : segment1.RightEndPoint;
			Vector2 joinStartPoint = joinOnLeft ? segment2.LeftStartPoint : segment2.RightStartPoint;
			switch (joinStyle)
			{
				case LineJoinStyle.Miter:
					fillConvexPolygon(joinEndPoint, segment1.EndPoint, joinStartPoint);
					break;
				case LineJoinStyle.Bevel:
				case LineJoinStyle.Round:
				default:
					throw new System.NotImplementedException();
			}
		}

		public virtual void Stroke()
		{
			_subPaths.Apply(
				subpath =>
				{
					subpath
					.Select(i => (Vector2?)i)
					.Neighbors()
					.Where(i => i.Item1 != null && i.Item2 != null)
					.Select(i => new LineSegment(i.Item1.Value, i.Item2.Value, LineWidth))
					.Repeat((rect, itemIndex, repeatCount) => repeatCount < 1 || (subpath.IsClosed && itemIndex < 1)) //If closed we need to go one past end
					.Neighbors()
					.Apply(
						segmentPair =>
						{
							bool pointOnLeft = true;
							if (segmentPair.Item1 != null && segmentPair.Item2 != null)
								pointOnLeft = BezierCurvesHelper.CalcWhatSideOfLinePointIsOn(segmentPair.Item1.StartPoint, segmentPair.Item1.EndPoint, segmentPair.Item2.EndPoint) > 0;
							strokeLineSegment(segmentPair.Item1, segmentPair.Item2, LineJoinStyle.Miter,pointOnLeft);
						});
				});
		}
		#endregion

		#region Filling
		public virtual void Fill()
		{
			throw new System.NotImplementedException();
		}
		protected abstract void fillConvexPolygon(IEnumerable<Vector2> points);
		protected void fillConvexPolygon(params Vector2[] points)
		{
			fillConvexPolygon((IEnumerable<Vector2>)points);
		}
		protected IList<ISubpath> _subPaths = new List<ISubpath>();
		#endregion

		#region Drawing Images
		public void DrawImage(IImageData image, double dx, double dy) 
		{ 
			throw new System.NotImplementedException(); 
		}
		public void DrawImage(IImageData image, double dx, double dy, double dw, double dh) 
		{ 
			throw new System.NotImplementedException(); 
		}
		public void DrawImage(IImageData image, double sx, double sy, double sw, double sh, double dx, double dy, double dw, double dh) 
		{ 
			throw new System.NotImplementedException(); 
		}
		#endregion

		#region Pixel Manipulation
		public IImageData CreateImageData(float sw, float sh)
		{
			throw new System.NotImplementedException();
		}
		public IImageData CreateImageData(IImageData imagedata)
		{
			throw new System.NotImplementedException();
		}
		public IImageData GetImageData(float sx, float sy, float sw, float sh)
		{
			throw new System.NotImplementedException();
		}
		public void PutImageData(IImageData imagedata, float dx, float dy)
		{
			throw new System.NotImplementedException();
		}
		public void PutImageData(IImageData imagedata, float dx, float dy, float dirtyX, float dirtyY, float dirtyWidth, float dirtyHeight)
		{
			throw new System.NotImplementedException();
		}
		#endregion
	}

	public interface ITextMetrics 
	{
		float Width { get; }
	};

	public interface IImageData 
	{
		ulong Width { get; }
		ulong Height { get; }
		byte[] Data { get; }
	};

	public enum TextBaseline
	{
		Top,
		Middle,
		Alphabetic,
		Ideographic,
		Bottom
	}

	public enum TextAlign
	{
		Start,
		End,
		Left,
		Right,
		Center
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
		Miter,
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
