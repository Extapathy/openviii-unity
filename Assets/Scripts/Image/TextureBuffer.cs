﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenVIII
{
    public class TextureBuffer : Texture_Base, ICloneable, ICollection, IStructuralComparable, IStructuralEquatable
    {

        #region Fields

        #endregion Fields

        #region Constructors

        public TextureBuffer(int width, int height, bool alert = true)
        {
            Height = height;
            Width = width;
            Colors = new Color[height * width];
            Alert = alert;
        }

        #endregion Constructors

        #region Properties

        public bool Alert { get; set; }
        public Color[] Colors { get; set; }
        public int Count => ((ICollection)Colors).Count;
        public override byte GetBytesPerPixel => 4;
        public override int GetClutCount => 0;
        public override int GetClutSize => 0;
        public override int GetColorsCountPerPalette => 0;
        public override int GetHeight => Height;
        public override int GetOrigX => 0;
        public override int GetOrigY => 0;
        public override int GetWidth => Width;
        public int Height { get; }
        public bool IsSynchronized => Colors.IsSynchronized;
        public int Length => Colors?.Length ?? 0;
        public object SyncRoot => Colors.SyncRoot;
        public int Width { get; }

        #endregion Properties

        #region Indexers

        public Color this[int i]
        {
            get => Colors[i]; set
            {
                if (Alert && Colors[i] != Color.clear)
                    throw new Exception("Color is set!");
                Colors[i] = value;
            }
        }

        public Color this[int x, int y]
        {
            get
            {
                var i = x + (y * Width);
                if (i < Count && i >= 0)
                    return Colors[i];
                Memory.Log.WriteLine($"{nameof(TextureBuffer)} :: this[int x, int y] => get :: {nameof(IndexOutOfRangeException)} :: {new System.Drawing.Point(x, y)} = {i}");
                return Color.clear; // fail silent...
            }
            set
            {
                var i = x + (y * Width);
                if (i < Count && i >= 0)
                    this[i] = value;
                else
                    Memory.Log.WriteLine($"{nameof(TextureBuffer)} :: this[int x, int y] => set :: {nameof(IndexOutOfRangeException)} :: {new System.Drawing.Point(x, y)} = {i} :: {nameof(value)} :: {value}");
            }
        }

        public Color[] this[Rect rectangle]
        {
            get => this[(int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height];
            set => this[(int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height] = value;
        }

        public Color[] this[int x, int y, int width, int height]
        {
            get
            {
                var pos = (x + y * Width);
                var r = new List<Color>(width * height);
                var row = 0;
                while (r.Count < width * height)
                {
                    r.AddRange(Colors.Skip(pos + row * Width).Take(width));
                    row++;
                }
                return r.ToArray();
            }
            set
            {
                for (var loopY = y; (loopY - y) < height; loopY++)
                    for (var loopX = x; (loopX - x) < width; loopX++)
                    {
                        var pos = (loopX + loopY * Width);
                        Colors[pos] = value[(loopX - x) + (loopY - y) * width];
                    }
            }
        }

        #endregion Indexers

        #region Methods

        public static explicit operator Texture2D(TextureBuffer @in)
        {
            if (Memory.Graphics?.GraphicsDevice != null && @in.Width > 0 && @in.Height > 0)
            {
                var tex = new Texture2D(@in.Width, @in.Height);
                @in.SetData(tex);
                return tex;
            }
            return null;
        }

        public static explicit operator Texture2DWrapper(TextureBuffer @in) => new Texture2DWrapper(@in.GetTexture());

        public static explicit operator TextureBuffer(Texture2D @in)
        {
            var texture = new TextureBuffer(@in.width, @in.height);

            texture.Colors = @in.GetPixels();
            return texture;
        }

        public static explicit operator TextureBuffer(Texture2DWrapper @in)
        {
            var texture = new TextureBuffer(@in.GetWidth, @in.GetHeight);
            var tex = @in.GetTexture();
            texture.Colors = tex.GetPixels();
            return texture;
        }

        public static implicit operator Color[](TextureBuffer @in) => @in.Colors;

        public object Clone() => Colors.Clone();

        public int CompareTo(object other, IComparer comparer) => ((IStructuralComparable)Colors).CompareTo(other, comparer);

        public void CopyTo(Array array, int index) => Colors.CopyTo(array, index);

        public bool Equals(object other, IEqualityComparer comparer) => ((IStructuralEquatable)Colors).Equals(other, comparer);

        public override void ForceSetClutColors(ushort newNumOfColors)
        {
        }

        public override void ForceSetClutCount(ushort newClut)
        {
        }

        public override Color[] GetClutColors(ushort clut) => null;

        public void GetData(Texture2D tex) 
        {
            Colors = tex.GetPixels();
        }

        public IEnumerator GetEnumerator() => Colors.GetEnumerator();

        public int GetHashCode(IEqualityComparer comparer) => ((IStructuralEquatable)Colors).GetHashCode(comparer);

        public override Texture2D GetTexture() => (Texture2D)this;

        public override Texture2D GetTexture(Color[] colors) => (Texture2D)this;

        public override Texture2D GetTexture(ushort clut) => (Texture2D)this;

        public override void Load(byte[] buffer, uint offset = 0) => throw new NotImplementedException();

        public override void Save(string path)
        {
            var tex = GetTexture();
            using (var fs = File.Create(path))
            {
                byte[] buffer = tex.EncodeToPNG();
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        public override void SaveCLUT(string path)
        {
        }

        public void SetData(Texture2D tex)
        {
            tex.SetPixels(Colors);
        }

        #endregion Methods
    }
}