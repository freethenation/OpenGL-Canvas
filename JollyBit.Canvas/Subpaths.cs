using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas
{
	public interface ISubpath : IEnumerable<Vector2>
	{
		bool IsClosed { get; }
	}

	public static class SubpathExtensions
	{
		public static IEnumerable<Vector2> CloseSubpath(this ISubpath subpath)
		{
			foreach (var point in subpath) yield return point;
			Vector2 finalVector = subpath.FirstOrDefault();
			if (finalVector != null) yield return finalVector;
		}
	}

	public class Subpath : ISubpath
	{
		private readonly IEnumerable<Vector2> _vectors;
		private readonly bool _isClosed;
		public Subpath(IEnumerable<Vector2> vectors, bool isClose)
		{
			_vectors = vectors;
			_isClosed = isClose;
		}

		public bool IsClosed
		{
			get { return _isClosed; }
		}

		public IEnumerator<Vector2> GetEnumerator()
		{
			return _vectors.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
