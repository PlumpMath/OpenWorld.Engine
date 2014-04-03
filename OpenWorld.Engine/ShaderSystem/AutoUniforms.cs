﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpenWorld.Engine.ShaderSystem
{
	/// <summary>
	/// Provides autogenerated uniforms for arbitrary objects.
	/// </summary>
	public sealed class AutoUniforms
	{
		private delegate void SetUniformDelegate(CompiledShader shader, object target);

		private class AutomaticShaderUniform
		{
			// Using a closure here to get a maximum of performance
			public SetUniformDelegate Apply;

			public AutomaticShaderUniform(string name)
			{
				this.Name = name;
			}

			public override string ToString()
			{
				return this.Name ?? base.ToString();
			}

			public string Name { get; private set; }
		}

		/// <summary>
		/// Defines a proxy object that can actually set uniform values.
		/// </summary>
		public class Proxy
		{
			private AutoUniforms uniforms;
			private object target;

			internal Proxy(AutoUniforms uniforms, object target)
			{
				this.uniforms = uniforms;
				this.target = target;
			}

			/// <summary>
			/// Sets the uniforms in a compiled shader.
			/// </summary>
			/// <param name="shader">Target shader</param>
			public void SetUniforms(CompiledShader shader)
			{
				foreach (var uniform in this.uniforms.automaticUniforms)
					uniform.Apply(shader, target);
			}
		}

		private static Dictionary<Type, AutoUniforms> cache = new Dictionary<Type, AutoUniforms>();

		/// <summary>
		/// Gets an IShaderUniforms for a given object. The uniforms are generated from all properties with a Uniform-Attribute.
		/// </summary>
		/// <param name="source">Object that contains uniforms.</param>
		/// <returns>IShaderUniforms for that object.</returns>
		public static Proxy Get(object source)
		{
			if (source == null) return null;

			AutoUniforms uniforms;
			lock (cache)
			{
				var type = source.GetType();
				if (!cache.ContainsKey(type))
					cache.Add(type, new AutoUniforms(type));
				uniforms = cache[type];
			}

			return new Proxy(uniforms, source);
		}

		private List<AutomaticShaderUniform> automaticUniforms;

		private AutoUniforms(Type type)
		{
			this.GenerateAssignments(type);
		}

		private void GenerateAssignments(Type type)
		{
			this.automaticUniforms = new List<AutomaticShaderUniform>();

			string prefix = "";

			UniformPrefixAttribute[] prefAttribs = (UniformPrefixAttribute[])type.GetCustomAttributes(typeof(UniformPrefixAttribute), true);
			if (prefAttribs.Length == 1)
			{
				prefix = prefAttribs[0].Prefix;
			}

			foreach (var property in type.GetProperties())
			{
				if (!property.CanRead)
					continue;
				UniformAttribute[] attribs = (UniformAttribute[])property.GetCustomAttributes(typeof(UniformAttribute), true);
				if (attribs.Length != 1)
					continue;

				var name = prefix + attribs[0].UniformName;

				// Create a closure for each uniform.
				// The closure later can simply and fast assign the uniform values.
				AutomaticShaderUniform uniform = new AutomaticShaderUniform(name);

				Type propertyType = property.PropertyType;
				if (propertyType == typeof(float))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((float)value);
						
					};
				}
				else if (propertyType == typeof(Vector2))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((Vector2)value);
						
					};
				}
				else if (propertyType == typeof(Vector3))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((Vector3)value);
						
					};
				}
				else if (propertyType == typeof(Vector4))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((Vector4)value);
						
					};
				}
				else if (propertyType == typeof(Color))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((Color)value);
						
					};
				}
				else if (propertyType == typeof(Matrix4))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((Matrix4)value, attribs[0].Transpose);
						
					};
				}
				else if (propertyType == typeof(bool))
				{
					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((bool)value);
						
					};
				}
				else if (typeof(Texture).IsAssignableFrom(propertyType))
				{
					ThreadLocal<Texture2D> defaultTexture = new ThreadLocal<Texture2D>(() =>
					{
						Color defaultColor;
						if (!Color.TryParse(attribs[0].DefaultColor, out defaultColor))
							return null;
						System.Drawing.Color color = System.Drawing.Color.FromArgb(
							MathHelper.Clamp((int)(255.0f * defaultColor.A), 0, 255),
							MathHelper.Clamp((int)(255.0f * defaultColor.R), 0, 255),
							MathHelper.Clamp((int)(255.0f * defaultColor.G), 0, 255),
							MathHelper.Clamp((int)(255.0f * defaultColor.B), 0, 255));
						System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(16, 16);
						for (int x = 0; x < bmp.Width; x++)
						{
							for (int y = 0; y < bmp.Height; y++)
							{
								bmp.SetPixel(x, y, color);
							}
						}
						Texture2D tex;
						if (attribs[0].SRGB != SRGBType.Default)
							tex = new Texture2D(attribs[0].SRGB == SRGBType.Yes);
						else
							tex = new Texture2D();
						tex.Load(bmp);
						return tex;
					});

					uniform.Apply = (shader, target) =>
					{
						var value = property.GetValue(target, new object[0]);
						if (shader[name] == null) return;
						shader[name].SetValue((Texture)value ?? defaultTexture.Value);
					};
				}
				else
				{
					Console.WriteLine("{0} is not supported for automatic uniform assignment.");
					continue;
				}
				this.automaticUniforms.Add(uniform);
			}
		}
	}
}