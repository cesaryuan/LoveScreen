using LoveScreen.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoveScreen.Controls
{
    /// <summary>
    /// ImageEditTool.xaml 的交互逻辑
    /// </summary>
    public partial class InkCanvasWithImageEditTool : UserControl
    {
        public InkCanvasWithImageEditTool()
        {
            InitializeComponent();
            this.InkCanvas = inkCanvas;
            CommandManager.RegisterClassCommandBinding(typeof(InkCanvasWithImageEditTool), new CommandBinding(ApplicationCommands.Redo, RedoCommand_Executed));
            CommandManager.RegisterClassCommandBinding(typeof(InkCanvasWithImageEditTool), new CommandBinding(ApplicationCommands.Undo, UndoCommand_Executed, UndoCommand_CanExecute));
            this.AddHandler(InkCanvasWithImageEditTool.SelectedColorChagnedEvent, new RoutedEventHandler(PenColorChanged));
            this.AddHandler(InkCanvasWithImageEditTool.SelectedSizeChagnedEvent, new RoutedEventHandler(PenSizeChanged));
            this.AddHandler(InkCanvasWithImageEditTool.DrawModeStyleChangedEvent, new RoutedEventHandler(DrawModeStyleChanged));
            
            // 初始绘图属性
            inkCanvas.UseCustomCursor = true;
            inkCanvas.Cursor = Cursors.Pen;
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
        }

        #region 撤回、重做相关
        /// <summary>
        ///     Undo集合
        /// </summary>
        List<Stroke> m_undoList = new List<Stroke>();

        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = inkCanvas.Strokes.Count > 0;
        }
        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Stroke stroke = inkCanvas.Strokes.LastOrDefault();
            m_undoList.Add(stroke);
            inkCanvas.Strokes.Remove(stroke);
        }


        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (m_undoList.Count > 0)
            {
                Stroke stroke = m_undoList.LastOrDefault();
                inkCanvas.Strokes.Add(stroke);
                m_undoList.Remove(stroke);
            }
        }

        #endregion

        #region Event
        /// <summary>
        ///     绘制模式改变事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DrawModeStyleChanged(object sender, RoutedEventArgs e)
        {
            InkCanvasWithImageEditTool tool = (InkCanvasWithImageEditTool)sender;
            inkCanvas.SetInkTool(InkCanvasWithImageEditTool.DrawTools.ElementAt((int)e.OriginalSource));
            //LongScreenWindow longScreen = new LongScreenWindow();
            //longScreen.Left = HightLightRect.Left;
            //longScreen.Top = HightLightRect.Top;
            //longScreen.Width = HightLightRect.Width;
            //longScreen.Height = HightLightRect.Height;
            //longScreen.Show();
            //this.Close();
        }
        public void PenColorChanged(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = (Color)e.OriginalSource;
        }
        public void PenSizeChanged(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = inkCanvas.DefaultDrawingAttributes.Width = (int)e.OriginalSource;
        }
      
        #endregion


        public Rect HightLightRect
        {
            get { return (Rect)GetValue(HightLightRectProperty); }
            set { SetValue(HightLightRectProperty, value); }
        }
        // Using a DependencyProperty as the backing store for HightLightRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HightLightRectProperty =
            DependencyProperty.Register("HightLightRect", typeof(Rect), typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent SelectedColorChagnedEvent = ColorPicker.SelectedColorChagnedEvent.AddOwner(typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent SelectedSizeChagnedEvent = SizePicker.SelectedSizeChagnedEvent.AddOwner(typeof(InkCanvasWithImageEditTool));

        public static readonly DependencyProperty SelectedColorProperty = ColorPicker.SelectedColorProperty.AddOwner(typeof(InkCanvasWithImageEditTool));

        public static readonly DependencyProperty SelectedSizeProperty = SizePicker.SelectedSizeProperty.AddOwner(typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent DrawModeStyleChangedEvent = EventManager.RegisterRoutedEvent("DrawModeStyleChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent OkBtnClickEvent = EventManager.RegisterRoutedEvent("OkBtnClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent CancelBtnClickEvent = EventManager.RegisterRoutedEvent("CancelBtnClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent TopBtnClickEvent = EventManager.RegisterRoutedEvent("TopBtnClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent LongScreenBtnClickEvent = EventManager.RegisterRoutedEvent("LongScreenBtnClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InkCanvasWithImageEditTool));

        public static readonly RoutedEvent RecordBtnClickEvent = EventManager.RegisterRoutedEvent("RecordBtnClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(InkCanvasWithImageEditTool));

        //public event RoutedEventHandler DrawModeStyleChanged
        //{
        //    add
        //    {
        //        base.AddHandler(InkCanvasWithImageEditTool.DrawModeStyleChangedEvent, value);
        //    }

        //    remove
        //    {
        //        base.RemoveHandler(InkCanvasWithImageEditTool.DrawModeStyleChangedEvent, value);
        //    }
        //}

        public event RoutedEventHandler OkBtnClick
        {
            add
            {
                base.AddHandler(InkCanvasWithImageEditTool.OkBtnClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(InkCanvasWithImageEditTool.OkBtnClickEvent, value);
            }
        }
        public event RoutedEventHandler CancelBtnClick
        {
            add
            {
                base.AddHandler(InkCanvasWithImageEditTool.CancelBtnClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(InkCanvasWithImageEditTool.CancelBtnClickEvent, value);
            }
        }
        public event RoutedEventHandler TopBtnClick
        {
            add
            {
                base.AddHandler(InkCanvasWithImageEditTool.TopBtnClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(InkCanvasWithImageEditTool.TopBtnClickEvent, value);
            }
        }
        public event RoutedEventHandler LongScreenBtnClick
        {
            add
            {
                base.AddHandler(InkCanvasWithImageEditTool.LongScreenBtnClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(InkCanvasWithImageEditTool.LongScreenBtnClickEvent, value);
            }
        }
        public event RoutedEventHandler RecordBtnClick
        {
            add
            {
                base.AddHandler(InkCanvasWithImageEditTool.RecordBtnClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(InkCanvasWithImageEditTool.RecordBtnClickEvent, value);
            }
        }

        public static IEnumerable<DynamicRenderer> DrawTools { get; } = new DynamicRenderer[] {
            //  矩形
            new RectangleDynamicRenderer(),
            new EllipseDynamicRenderer(),
            new DynamicRenderer()
        };


        public DrawMode DrawModeStyle
        {
            get { return (DrawMode)GetValue(DrawModeStyleProperty); }
            set { SetValue(DrawModeStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawModeStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawModeStyleProperty =
            DependencyProperty.Register("DrawModeStyle", typeof(DrawMode), typeof(InkCanvasWithImageEditTool), new PropertyMetadata(DrawMode.Pen, DrawModeStyleChangedCallback));

        public static void DrawModeStyleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InkCanvasWithImageEditTool tool = (InkCanvasWithImageEditTool)d;
            RoutedEventArgs args = new RoutedEventArgs(InkCanvasWithImageEditTool.DrawModeStyleChangedEvent, e.NewValue);
            tool.RaiseEvent(args);
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public int SelectedSize
        {
            get { return (int)GetValue(SelectedSizeProperty); }
            set { SetValue(SelectedSizeProperty, value); }
        }

        public ExtendedInkCanvas InkCanvas = null;

        
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = (Rectangle)sender;
            DrawModeStyle = (DrawMode)Convert.ToInt32(rectangle.Tag);
        }

        private void TopButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(InkCanvasWithImageEditTool.TopBtnClickEvent, e.OriginalSource);
            RaiseEvent(args);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(InkCanvasWithImageEditTool.CancelBtnClickEvent, e.OriginalSource);
            RaiseEvent(args);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(InkCanvasWithImageEditTool.OkBtnClickEvent, e.OriginalSource);
            RaiseEvent(args);
        }
        private void LongScreenButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(InkCanvasWithImageEditTool.LongScreenBtnClickEvent, e.OriginalSource);
            RaiseEvent(args);
        }
        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(InkCanvasWithImageEditTool.RecordBtnClickEvent, e.OriginalSource);
            RaiseEvent(args);
        }
    }

    public enum DrawMode
    {
        Rect = 0,
        Line = 1,
        Pen = 2
    }
}
