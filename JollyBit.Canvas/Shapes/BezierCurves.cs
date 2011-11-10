using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas.Shapes
{
	/// <summary>
	/// A static class with methods for createing bezier curves. Consult http://www.caffeineowl.com/graphics/2d/vectorial/bezierintro.html for a good bezier curves reference.
	/// </summary>
	public static class BezierCurvesHelper
	{
		#region Utility Functions
		/// <summary>
		/// Calculates the minimum distance between a line and a point.
		/// </summary>
		/// <param name="lineStart">The point at which the line starts</param>
		/// <param name="lineEnd">The point at which the line ends</param>
		/// <param name="point">The point</param>
		/// <param name="whatSideOfLine">A negative number if point is on right side of line else returns a positive number. Return zero if the point is on the line.
		/// The same value as CalcWhatSideOfLinePointIsOn would return.</param>
		/// <returns></returns>
		public static float CalcSquaredDistanceBetweenLineAndPoint(ref Vector2 lineStart, ref Vector2 lineEnd, ref Vector2 point, out float whatSideOfLine)
		{
			//http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm
			whatSideOfLine = CalcWhatSideOfLinePointIsOn(lineStart, lineEnd, point);
			Vector2 lineDiff;
			Vector2.Subtract(ref lineEnd, ref lineStart, out lineDiff);
			return (whatSideOfLine * whatSideOfLine) / lineDiff.LengthSquared;
		}

		/// <summary>
		/// Calculates the minimum distance between a line and a point.
		/// </summary>
		/// <param name="lineStart">The point at which the line starts</param>
		/// <param name="lineEnd">The point at which the line ends</param>
		/// <param name="point">The point</param>
		public static float CalcSquaredDistanceBetweenLineAndPoint(ref Vector2 lineStart, ref Vector2 lineEnd, ref Vector2 point)
		{
			float whatSideOfLine;
			return CalcSquaredDistanceBetweenLineAndPoint(ref lineStart, ref lineEnd, ref point, out whatSideOfLine);
		}

		/// <summary>
		/// Returns a negative number if point is on right side of line else returns a positive number. Return zero if the point is on the line.
		/// </summary>
		public static float CalcWhatSideOfLinePointIsOn(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			return (lineEnd.X - lineStart.X) * (point.Y - lineStart.Y) - (lineEnd.Y - lineStart.Y) * (point.X - lineStart.X);
		}

		#endregion

		#region QuadraticBezierCurve
		public static IEnumerable<Vector2> CreateQuadraticBezierCurve(Vector2 anchorPoint1, Vector2 anchorPoint2, Vector2 controlPoint, float flatness)
		{
			flatness = flatness * flatness;
			yield return anchorPoint1;
			foreach (var item in _createQuadraticBezierCurve(anchorPoint1, anchorPoint2, controlPoint, flatness)) yield return item;
			yield return anchorPoint2;
		}
		private static IEnumerable<Vector2> _createQuadraticBezierCurve(Vector2 anchorPoint1, Vector2 anchorPoint2, Vector2 controlPoint, float flatnessSquared)
		{
			//1) Check if flat enough
			if (CalcSquaredDistanceBetweenLineAndPoint(ref anchorPoint1, ref anchorPoint2, ref controlPoint) > flatnessSquared)
			{
				//2) Compute nessary values
				//segControl1 = (a1+c)/2
				Vector2 segControl1;
				Vector2.Add(ref anchorPoint1, ref controlPoint, out segControl1);
				Vector2.Divide(ref segControl1, 2f, out segControl1);
				//segControl2 = (a1+c)/2
				Vector2 segControl2;
				Vector2.Add(ref anchorPoint2, ref controlPoint, out segControl2);
				Vector2.Divide(ref segControl2, 2f, out segControl2);
				//segAnchor = (segControl1 + segControl2)/2 = (2c+a1+a2)/4
				Vector2 segAnchor;
				Vector2.Add(ref segControl1, ref segControl2, out segAnchor);
				Vector2.Divide(ref segAnchor, 2f, out segAnchor);

				//3) Recurse segments
				foreach (var item in _createQuadraticBezierCurve(anchorPoint1, segAnchor, segControl1, flatnessSquared)) yield return item;
				yield return segAnchor;
				foreach (var item in _createQuadraticBezierCurve(segAnchor, anchorPoint2, segControl2, flatnessSquared)) yield return item;
			}
		}
		#endregion

		#region CubicBezierCurve
		//public static IEnumerable<Vector2> CreateCubicBezierCurve(Vector2 anchorPoint1, Vector2 anchorPoint2, Vector2 controlPoint1, Vector2 controlPoint2, float flatness)
		//{
		//    flatness = flatness * flatness;
		//    yield return anchorPoint1;
		//    foreach (var item in _createCubicBezierCurve(anchorPoint1, anchorPoint2, controlPoint1, controlPoint2, flatness)) yield return item;
		//    yield return anchorPoint2;
		//}
		//private static IEnumerable<Vector2> _createCubicBezierCurve(Vector2 anchorPoint1, Vector2 anchorPoint2, Vector2 controlPoint1, Vector2 controlPoint2, float flatnessSquared)
		//{
		//    //1) Check if flat enough
		//    //if (CalcSquaredDistanceBetweenLineAndPoint(ref anchorPoint1, ref anchorPoint2, ref controlPoint) > flatnessSquared)
		//    //{
		//        //2) Compute nessary values
		//        //segControl1 = (a1+c)/2
		//        Vector2 segControl1;
		//        Vector2.Add(ref anchorPoint1, ref controlPoint, out segControl1);
		//        Vector2.Divide(ref segControl1, 2f, out segControl1);
		//        //segControl2 = (a1+c)/2
		//        Vector2 segControl2;
		//        Vector2.Add(ref anchorPoint2, ref controlPoint, out segControl2);
		//        Vector2.Divide(ref segControl2, 2f, out segControl2);
		//        //segAnchor = (a1 + 3•c1 + 3•c2 + a2)/8
		//        Vector2 segAnchor;
		//        Vector2.Multiply(ref controlPoint, 2f, out segAnchor);
		//        Vector2.Add(ref segAnchor, ref anchorPoint1, out segAnchor);
		//        Vector2.Add(ref segAnchor, ref anchorPoint2, out segAnchor);
		//        Vector2.Divide(ref segAnchor, 4f, out segAnchor);

		//        //3) Recurse segments
		//        foreach (var item in _createQuadraticBezierCurve(anchorPoint1, segAnchor, segControl1, flatnessSquared)) yield return item;
		//        yield return segAnchor;
		//        foreach (var item in _createQuadraticBezierCurve(segAnchor, anchorPoint2, segControl2, flatnessSquared)) yield return item;
		//    //}
		//}
		#endregion
	}
}
