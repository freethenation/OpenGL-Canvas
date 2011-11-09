using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas.Shapes
{

	/*               X+
	 | -----------------
	 | 
	 |       Left
	 |   ------------>
	 |       Right
  Y+ |
	 */
	public class LineSegment : IEnumerable<Vector2>
	{
		public readonly Vector2 StartPoint;
		public readonly Vector2 EndPoint;
		public readonly float Rotation;
		public readonly float LineWidth;

		public readonly Vector2 LeftStartPoint;
		public readonly Vector2 RightStartPoint;
		public readonly Vector2 RightEndPoint;
		public readonly Vector2 LeftEndPoint;

		public LineSegment(Vector2 startPoint, Vector2 endPoint, float lineWidth)
		{
			LineWidth = lineWidth;
			StartPoint = startPoint;
			EndPoint = endPoint;
			Vector2 EPMinusSP = endPoint - startPoint;
			Rotation = (float)Math.Atan2(EPMinusSP.Y, EPMinusSP.X);
			//Set rect points
			Matrix4 resultMatrix;
			Matrix4 scaleMatrix = Matrix4.Scale(EPMinusSP.Length, LineWidth * 0.5f, 1f);
			Matrix4 rotMatrix = Matrix4.CreateRotationZ(Rotation);
			Matrix4 tranMatrix = Matrix4.CreateTranslation(startPoint.X, startPoint.Y, 0);
			Matrix4.Mult(ref scaleMatrix, ref rotMatrix, out resultMatrix);
			Matrix4.Mult(ref resultMatrix, ref tranMatrix, out resultMatrix);

			LeftStartPoint = new Vector2(0, -1f).ApplyTransform(ref resultMatrix);
			RightStartPoint = new Vector2(0, 1f).ApplyTransform(ref resultMatrix);
			RightEndPoint = new Vector2(1f, 1f).ApplyTransform(ref resultMatrix);
			LeftEndPoint = new Vector2(1f, -1f).ApplyTransform(ref resultMatrix);
		}

		public IEnumerator<Vector2> GetEnumerator()
		{
			yield return LeftStartPoint;
			yield return RightStartPoint;
			yield return RightEndPoint;
			yield return LeftEndPoint;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
