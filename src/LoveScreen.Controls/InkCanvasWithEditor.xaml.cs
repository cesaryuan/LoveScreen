using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoveScreen.Controls
{

    /// <summary>
    /// InkCanvasWithEditor.xaml 的交互逻辑
    /// </summary>
    public partial class InkCanvasWithEditor : UserControl
    {
        public Rect HightLightRect
        {
            get { return (Rect)GetValue(HightLightRectProperty); }
            set { SetValue(HightLightRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HightLightRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HightLightRectProperty =
            DependencyProperty.Register("HightLightRect", typeof(Rect), typeof(InkCanvasWithEditor));
        public InkCanvasWithEditor()
        {
            InitializeComponent();
        }
    }
}
