using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LoveScreen.Windows.Models
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
		internal int width
		{
			get
			{
				return this.Right - this.Left;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00006231 File Offset: 0x00004431
		internal int height
		{
			get
			{
				return this.Bottom - this.Top;
			}
		}
		public RECT(int Dimension)
        {
            Left = Top = Right = Bottom = Dimension;
        }
    }
}
