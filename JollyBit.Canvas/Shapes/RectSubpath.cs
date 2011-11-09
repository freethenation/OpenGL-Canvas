using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas.Shapes
{
	public class RectSubpath : ISubpath
	{
		public readonly float X;
		public readonly float Y;
		public readonly float W;
		public readonly float H;
		public RectSubpath(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			W = width;
			H = height;
		}

		public IEnumerator<Vector2> GetEnumerator()
		{
			return CreateVectorsForRect(X, Y, W, H).GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool IsClosed
		{
			get { return true; }
		}

		public static IEnumerable<Vector2> CreateVectorsForRect(float x, float y, float w, float h)
		{
			yield return new Vector2(x, y);
			yield return new Vector2(x, y + h);
			yield return new Vector2(x + w, y + h);
			yield return new Vector2(x + w, y);
			yield return new Vector2(x, y);
		}

	}
}
