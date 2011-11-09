using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.Canvas
{
	public static class Util
	{
		public static IEnumerable<Vector2> ApplyTransform(this IEnumerable<Vector2> vectors, Matrix4 transform)
		{
			foreach (Vector2 vector in vectors)
			{
				Vector3 vect = new Vector3(vector.X, vector.Y, 0f);
				Vector3.Transform(ref vect, ref transform, out vect);
				yield return vect.Xy;
			}
		}

		public static Vector2 ApplyTransform(this Vector2 vector, ref Matrix4 transform)
		{
			Vector3 vect = new Vector3(vector.X, vector.Y, 0f);
			Vector3.Transform(ref vect, ref transform, out vect);
			return vect.Xy;
		}

		[System.Diagnostics.DebuggerStepThrough]
		public static void Apply<T>(this IEnumerable<T> source, Action<T> func)
		{
			foreach (var item in source)
			{
				func(item);
			}
		}

		public static IEnumerable<KeyValuePair<int, T>> Enumerate<T>(this IEnumerable<T> source)
		{
			int i = 0;
			foreach (T item in source)
			{
				yield return new KeyValuePair<int, T>(i, item);
				i++;
			}
		}

		[System.Diagnostics.DebuggerStepThrough]
		public static IEnumerable<Tuple<T, T>> Neighbors<T>(this IEnumerable<T> source)
		{
			T last = default(T);
			foreach (var item in source)
			{
				 yield return new Tuple<T, T>(last, item);
				last = item;
			}
			yield return new Tuple<T, T>(last, default(T));
		}

		public static IEnumerable<T> Join<T>(this IEnumerable<T> source1, IEnumerable<T> source2)
		{
			foreach (var item in source1) yield return item;
			foreach (var item in source2) yield return item;
		}

		[System.Diagnostics.DebuggerStepThrough]
		public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, Func<T, bool> funcContinue)
		{
			return Repeat(source, (t, itemIndex, repeatCount) => funcContinue(t));
		}

		[System.Diagnostics.DebuggerStepThrough]
		public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, Func<T, int, int, bool> funcContinue)
		{
			IEnumerator<T> enumerator = source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				enumerator = source.GetEnumerator();
				int repeatCount = 0;
				while (true)
				{
					int itemIndex = 0;
					while (enumerator.MoveNext() && funcContinue(enumerator.Current, itemIndex, repeatCount))
					{
						yield return enumerator.Current;
						itemIndex++;
					}
					if (!funcContinue(enumerator.Current, itemIndex, repeatCount)) break;
					repeatCount++;
					enumerator = source.GetEnumerator();
				}
			}
		}
	}
}
