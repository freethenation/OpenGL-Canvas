using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.Canvas
{
	public static class Func
	{
		public static Func<T> Memoize<T>(this Func<T> func)
		{
			T ret = default(T);
			bool called = false;
			return () =>
				{
					if (!called) ret = func();
					return ret;
				};
		}
	}
}