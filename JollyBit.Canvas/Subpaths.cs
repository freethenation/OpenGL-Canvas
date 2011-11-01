using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas
{
	public interface ISubpath : IEnumerable<Vector2> { }

	public class RectSubpath : ISubpath
	{
		public readonly Vector2 TopLeft;
		public readonly Vector2 BottomLeft;
		public readonly Vector2 BorromRight;
		public readonly Vector2 TopRight;
		public RectSubpath(float x, float y, float width, float height, ref Matrix4 transform)
		{
			Vector3 tmp = new Vector3(x, y, 0); ;
			Vector3.Transform(ref tmp, ref transform, out tmp);
			TopLeft = tmp.Xy;

			tmp = new Vector3(x, y + height, 0); ;
			Vector3.Transform(ref tmp, ref transform, out tmp);
			BottomLeft = tmp.Xy;

			tmp = new Vector3(x + width, y + height, 0); ;
			Vector3.Transform(ref tmp, ref transform, out tmp);
			BorromRight = tmp.Xy;

			tmp = new Vector3(x + width, y, 0); ;
			Vector3.Transform(ref tmp, ref transform, out tmp);
			TopRight = tmp.Xy;
		}
		public IEnumerator<Vector2> GetEnumerator()
		{
			yield return TopLeft;
			yield return BottomLeft;
			yield return BorromRight;
			yield return TopRight;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
