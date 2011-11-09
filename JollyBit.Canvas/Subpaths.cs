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
}
