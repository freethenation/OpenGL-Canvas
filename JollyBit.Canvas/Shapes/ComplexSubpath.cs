﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas.Shapes
{
	public class ComplexSubpath : ISubpath
	{
		private List<Vector2> _vectors = new List<Vector2>();
		public ComplexSubpath(Vector2 startPoint)
		{
			_vectors.Add(startPoint);
		}

		public ComplexSubpath(IEnumerable<Vector2> vectors)
		{
			vectors.Apply(i => _vectors.Add(i));
		}

		public void LineTo(Vector2 point)
		{
			_vectors.Add(point);
		}

		public void QuadraticCurveTo(Vector2 controlPoint, Vector2 anchorPoint2, float flatness)
		{
			_vectors.AddRange(BezierCurvesHelper.CreateQuadraticBezierCurve(lastVector, anchorPoint2, controlPoint, flatness));
		}

		public void ClosePath()
		{
			if (!IsClosed) LineTo(_vectors[0]);
			if (!IsClosed) throw new System.Exception("Path could not be closed! It is invalid.");
		}

		public bool IsClosed
		{
			get { return _vectors.Count > 1 && _vectors[0] == lastVector; }
		}

		private Vector2 lastVector
		{
			get { return _vectors[_vectors.Count - 1]; }
		}

		public IEnumerator<OpenTK.Vector2> GetEnumerator()
		{
			return _vectors.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
