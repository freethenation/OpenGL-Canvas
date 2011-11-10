using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using JollyBit.Canvas.OpenGL;
using JollyBit.Canvas;
using System.Drawing;

namespace JollyBit.Canvas.OpenGL.Demo
{
	/// <summary>
	/// Demonstrates the GameWindow class.
	/// </summary>
	public class TDSWindow : GameWindow
	{

		public TDSWindow()
			: base(400, 400)
		{
			Keyboard.KeyDown += Keyboard_KeyDown;
		}

		#region Keyboard_KeyDown

		/// <summary>
		/// Occurs when a key is pressed.
		/// </summary>
		/// <param name="sender">The KeyboardDevice which generated this event.</param>
		/// <param name="e">The key that was pressed.</param>
		void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				this.Exit();

			if (e.Key == Key.F11)
				if (this.WindowState == WindowState.Fullscreen)
					this.WindowState = WindowState.Normal;
				else
					this.WindowState = WindowState.Fullscreen;
		}

		#endregion

		#region OnLoad

		public Canvas canvas = new CanvasOpenGL();

		/// <summary>
		/// Setup OpenGL and load resources here.
		/// </summary>
		/// <param name="e">Not used.</param>
		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(Color.LightBlue);
		}

		static short[] range(short max)
		{
			var y = new short[max];
			for (short x = 0; x < max; x++)
				y[x] = x;
			return y;
		}

		#endregion

		#region OnResize

		/// <summary>
		/// Respond to resize events here.
		/// </summary>
		/// <param name="e">Contains information on the new GameWindow size.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
		}

		#endregion

		#region OnUpdateFrame

		/// <summary>
		/// Add your game logic here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			// Nothing to do!
		}

		#endregion

		#region OnRenderFrame

		/// <summary>
		/// Add your game rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			canvas.LineWidth = 10;
			canvas.BeginBatch();
			canvas.BeginPath();
			//Test 1			
			canvas.Rect(200, 200, 50, 50);
			//Test 2
			canvas.MoveTo(200, 200);
			canvas.LineTo(250, 250);
			canvas.QuadraticCurveTo(300, 50, 100, 200);

			canvas.MoveTo(250, 250);
			canvas.LineTo(100, 200);
			canvas.Stroke();
			canvas.EndBatch();

			this.SwapBuffers();
		}

		#endregion

		#region public static void Main()

		/// <summary>
		/// Entry point of this example.
		/// </summary>
		[STAThread]
		public static void Main()
		{
			using (TDSWindow example = new TDSWindow())
			{
				example.Run(30.0, 0.0);
			}
		}

		#endregion
	}
}