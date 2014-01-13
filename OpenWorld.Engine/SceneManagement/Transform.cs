﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenWorld.Engine.SceneManagement
{
	/// <summary>
	/// Defines a transformation.
	/// </summary>
	public class Transform
	{
		private Matrix4 matrix;

		/// <summary>
		/// Creates a new transform.
		/// </summary>
		public Transform()
		{
			this.matrix = Matrix4.Identity;
		}

		/// <summary>
		/// Translates the transform.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void Translate(float x, float y, float z)
		{
			this.Translate(new Vector3(x, y, z));
		}

		/// <summary>
		/// Translates the transform.
		/// </summary>
		/// <param name="translation"></param>
		public void Translate(Vector3 translation)
		{
			this.matrix = Matrix4.CreateTranslation(translation) * this.matrix;
		}

		/// <summary>
		/// Returns the global transformation matrix.
		/// </summary>
		/// <returns></returns>
		public Matrix4 GetGlobalMatrix()
		{
			return this.matrix;
		}

		/// <summary>
		/// Gets or sets the local position of the transform.
		/// </summary>
		public Vector3 Position
		{
			get { return this.matrix.Row3.Xyz; }
			set { this.matrix.Row3.Xyz = value; }
		}
	}
}