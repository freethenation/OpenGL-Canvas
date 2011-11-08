using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using JollyBit.Canvas;

namespace JollyBit.Canvas.OpenGL
{
	public class CanvasOpenGL : Canvas
	{
		private Matrix4 _projMatrix;
		public CanvasOpenGL()
		{
			_projMatrix = Matrix4.CreateOrthographicOffCenter(0, 400, 400, 0, -1, 1);
		}

		public override void BeginBatch()
		{
			GL.PushAttrib(AttribMask.AllAttribBits);
			GL.Disable(EnableCap.DepthTest);

			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.LoadMatrix(ref _projMatrix);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.LoadMatrix(ref Matrix4.Identity);

			base.BeginBatch();
		}

		public override void EndBatch()
		{
			GL.PopAttrib();

			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();

			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();

			base.EndBatch();
		}

		private void callInBatch(Action func)
		{
			bool inBatch = InBatch;
			if (!inBatch) BeginBatch();
			func();
			if (!inBatch) EndBatch();
		}

		private void applyFillStyle()
		{
			GL.Color3(1, 1, 1);
		}

		public override void Stroke()
		{
			base.Stroke();
			foreach (ISubpath subpath in _subPaths)
			{
				callInBatch(() =>
				{
					GL.Color3(.5, .5f, .5f);
					GL.Begin(BeginMode.LineStrip);
					foreach (Vector2 point in subpath.CloseSubpath())
					{
						GL.Vertex2(point);
					}
					GL.End();
				});
			}
		}

		protected override void fillConvexPolygon(IEnumerable<Vector2> points)
		{
			callInBatch(() =>
			{
				applyFillStyle();
				GL.Begin(BeginMode.Polygon);
				foreach (Vector2 point in points)
				{
					GL.Vertex2(point);
				}
				GL.End();
			});
		}
	}
}
