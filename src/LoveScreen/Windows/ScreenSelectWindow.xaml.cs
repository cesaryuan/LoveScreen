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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Ink;
using LoveScreen.Controls;
using LoveScreen.Controls.Models;
using LoveScreen.Windows.Extensions;

namespace LoveScreen.Windows
{
    /// <summary>
    /// ScreenSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenSelectWindow : Window
    {
        #region DependencyProperty
        /// <summary>
        ///     选择框的范围
        /// </summary>
        public Rect HightLightRect
        {
            get { return (Rect)GetValue(HightLightRectProperty); }
            set { SetValue(HightLightRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HightLightRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HightLightRectProperty =
            DependencyProperty.Register("HightLightRect", typeof(Rect), typeof(ScreenSelectWindow));


        /// <summary>
        ///     选择框上的鼠标样式
        /// </summary>
        public Cursor InnerRectCursor
        {
            get { return (Cursor)GetValue(InnerRectCursorProperty); }
            set { SetValue(InnerRectCursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerRectCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerRectCursorProperty =
            DependencyProperty.Register("InnerRectCursor", typeof(Cursor), typeof(ScreenSelectWindow));


        /// <summary>
        ///     放大镜图片相对于原图位置
        /// </summary>
        public Rect HelpImageRect
        {
            get { return (Rect)GetValue(HelpImageRectProperty); }
            set { SetValue(HelpImageRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HelpImageRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HelpImageRectProperty =
            DependencyProperty.Register("HelpImageRect", typeof(Rect), typeof(ScreenSelectWindow));


        /// <summary>
        ///     放大镜位置
        /// </summary>
        public Thickness HelpRectMargin
        {
            get { return (Thickness)GetValue(HelpRectMarginProperty); }
            set { SetValue(HelpRectMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HelpRectMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HelpRectMarginProperty =
            DependencyProperty.Register("HelpRectMargin", typeof(Thickness), typeof(ScreenSelectWindow));


        /// <summary>
        ///     取色：十六进制颜色
        /// </summary>
        public string PixInfoStr
        {
            get { return (string)GetValue(PixInfoStrProperty); }
            set { SetValue(PixInfoStrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PixInfoStr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PixInfoStrProperty =
            DependencyProperty.Register("PixInfoStr", typeof(string), typeof(ScreenSelectWindow), new PropertyMetadata(""));


        /// <summary>
        ///     选中框模式，1：自动 3：编辑 4：查看
        /// </summary>
        public InnerFrameModeEnum InnerFrameMode
        {
            get { return (InnerFrameModeEnum)GetValue(InnerFrameModeProperty); }
            set { SetValue(InnerFrameModeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for InnerFrameMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerFrameModeProperty =
            DependencyProperty.Register("InnerFrameMode", typeof(InnerFrameModeEnum), typeof(ScreenSelectWindow), new PropertyMetadata(InnerFrameModeEnum.WaitingForSelect));
        #endregion



        double dpi = 0;

        public ScreenSelectWindow()
        {
            InitializeComponent();
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Left = this.Top = 0;
            this.dpi = this.Dpi();

            BackgroundImg.Source = ConvertHelper.ToBitmapImage(ScreenHelper.CaptureFullScreen());

            BitmapSource img = (BitmapSource)BackgroundImg.Source;
            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            m_pixels = new byte[size];
            img.CopyPixels(m_pixels, stride, 0);

            SetDrawTool();

            // 注册撤销与重做命令
            CommandManager.RegisterClassCommandBinding(typeof(ScreenSelectWindow), new CommandBinding(ApplicationCommands.Redo, RedoCommand_Executed));
            CommandManager.RegisterClassCommandBinding(typeof(ScreenSelectWindow), new CommandBinding(ApplicationCommands.Undo, UndoCommand_Executed, UndoCommand_CanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(ScreenSelectWindow), new CommandBinding(ApplicationCommands.Close, CloseCommand_Executed));
            CommandManager.RegisterClassInputBinding(typeof(ScreenSelectWindow), new KeyBinding(ApplicationCommands.Close, new KeyGesture(Key.Escape)));


            //  绑定事件
            imageEditTool.AddHandler(ImageEditTool.SelectedColorChagnedEvent, new RoutedEventHandler(PenColorChanged));
            imageEditTool.AddHandler(ImageEditTool.SelectedSizeChagnedEvent, new RoutedEventHandler(PenSizeChanged));
            imageEditTool.AddHandler(ImageEditTool.DrawModeStyleChangedEvent, new RoutedEventHandler(DrawModeStyleChanged));
            imageEditTool.AddHandler(ImageEditTool.OkBtnClickEvent, new RoutedEventHandler(ToolOkBtnClick));
            imageEditTool.AddHandler(ImageEditTool.CancelBtnClickEvent, new RoutedEventHandler(ToolCancelBtnClick));
            imageEditTool.AddHandler(ImageEditTool.TopBtnClickEvent, new RoutedEventHandler(ToolTopBtnClick));
            imageEditTool.AddHandler(ImageEditTool.LongScreenBtnClickEvent, new RoutedEventHandler(ToolLongScreenBtnClick));
            imageEditTool.AddHandler(ImageEditTool.RecordBtnClickEvent, new RoutedEventHandler(ToolRecordBtnClick));
        }

        /// <summary>
        ///     绘制模式改变事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DrawModeStyleChanged(object sender, RoutedEventArgs e)
        {
            ImageEditTool tool = (ImageEditTool)sender;
            inkCanvas.SetInkTool(ImageEditTool.DrawTools.ElementAt((int)e.OriginalSource));
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
        public void ToolOkBtnClick(object sender, RoutedEventArgs e)
        {
            SystemHelper.SetClipboard(GetSelectImage());
            this.Close();
        }
        public void ToolCancelBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void ToolTopBtnClick(object sender, RoutedEventArgs e)
        {
            TopImage topImage = new TopImage(SystemHelper.SetClipboard(GetSelectImage()), HightLightRect.Left, HightLightRect.Top);
            topImage.Show();
            this.Close();
        }
        public void ToolLongScreenBtnClick(object sender, RoutedEventArgs e)
        {
            LongScreenWindow topImage = new LongScreenWindow(HightLightRect.Left, HightLightRect.Top, HightLightRect.Width, HightLightRect.Height);
            topImage.Show();
            this.Close();
        }
        public void ToolRecordBtnClick(object sender, RoutedEventArgs e)
        {
            RecordGifWidnow topImage = new RecordGifWidnow(HightLightRect.Left, HightLightRect.Top, HightLightRect.Width, HightLightRect.Height);
            topImage.Show();
            this.Close();
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

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
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

        #region 放大镜相关

        /// <summary>
        ///     缓存的图片像素集合，用于取色
        /// </summary>
        byte[] m_pixels;
        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            Rect newRect = HelpImageRect;
            int helpSize = 10;
            newRect.X = (point.X - helpSize) / this.Width;
            newRect.Y = (point.Y - helpSize) / this.Height;
            newRect.Width = helpSize * 2 / this.Width;
            newRect.Height = helpSize * 2 / this.Height;
            HelpImageRect = newRect;
            int helpMarginOffset = 10;

            //  设置放大镜位置及颜色信息
            HelpRectMargin = new Thickness((point.X
                                            + helpMarginOffset
                                            + HelpCanvas.ActualWidth) > this.Width ? (point.X - helpMarginOffset - HelpCanvas.ActualWidth) : (point.X + helpMarginOffset), (point.Y + helpMarginOffset + HelpCanvas.ActualHeight) > this.Height ? (point.Y - helpMarginOffset - HelpCanvas.ActualHeight) : (point.Y + helpMarginOffset), 0, 0);
            if (m_pixels != null && m_pixels.Length > 0)
            {
                BitmapSource img = ((BitmapSource)BackgroundImg.Source);
                int stride = img.PixelWidth * 4;
                int index = (int)(point.Y * stride + point.X * 4);
                if (m_pixels.Length > index + 3)
                {
                    byte red = m_pixels[index];
                    byte green = m_pixels[index + 1];
                    byte blue = m_pixels[index + 2];
                    byte alpha = m_pixels[index + 3];
                    PixInfoStr = $"#{red:X2}{green:X2}{blue:X2}";
                }
                else
                    PixInfoStr = "";
            }


        }
        #endregion



        int m_editMode = 0x0000;
        bool m_isMouseDown = false;
        Point m_lastPoint = new Point(0, 0);

        private void innerFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right) return;
            m_isMouseDown = true;
            Point location = e.GetPosition(innerFrame);
            if (Math.Abs(location.X - HightLightRect.X) < 13)
            {
                location.X = HightLightRect.X;
                m_editMode |= 0x1000;
            }
            else if (Math.Abs(location.X - HightLightRect.X - HightLightRect.Width) < 13)
            {
                location.X = HightLightRect.X + HightLightRect.Width;
                m_editMode |= 0x0010;
            }
            if (Math.Abs(location.Y - HightLightRect.Y) < 13)
            {
                location.Y = HightLightRect.Y;
                m_editMode |= 0x0100;
            }
            else if (Math.Abs(location.Y - HightLightRect.Y - HightLightRect.Height) < 13)
            {
                location.Y = HightLightRect.Y + HightLightRect.Height;
                m_editMode |= 0x0001;
            }
            ScreenHelper.SetMousePos((int)(location.X * dpi), (int)(location.Y * dpi));
            ((UIElement)sender).CaptureMouse();
            InnerFrameMode = InnerFrameModeEnum.Resizing;
            m_lastPoint = location;
            //e.Handled = true;
        }
        private void innerFrame_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isMouseDown)
            {
                if (InnerFrameMode == InnerFrameModeEnum.Resizing)
                {
                    //  计算选择框高亮区域
                    CalcHightLightRect(e);
                }
            }
            SetMouseCursor(e);
        }
        private void innerFrame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_isMouseDown = false;
            //  设置查看模式
            InnerFrameMode = InnerFrameModeEnum.Drawing;
            //  清空编辑模式
            m_editMode = 0;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void SetMouseCursor(MouseEventArgs e)
        {
            Point location = e.GetPosition(innerFrame);
            //Rect HightLightRect = this.HightLightRect.RectWithDpi(dpi);
            if (Math.Abs(location.X - HightLightRect.X) < 13)
            {
                if (Math.Abs(location.Y - HightLightRect.Y) < 13)
                    InnerRectCursor = Cursors.SizeNWSE;
                else if (Math.Abs(location.Y - HightLightRect.Y - HightLightRect.Height) < 13)
                    InnerRectCursor = Cursors.SizeNESW;
                else
                    InnerRectCursor = Cursors.SizeWE;
            }
            else if (Math.Abs(location.X - HightLightRect.X - HightLightRect.Width) < 13)
            {
                if (Math.Abs(location.Y - HightLightRect.Y) < 13)
                    InnerRectCursor = Cursors.SizeNESW;
                else if (Math.Abs(location.Y - HightLightRect.Y - HightLightRect.Height) < 13)
                    InnerRectCursor = Cursors.SizeNWSE;
                else
                    InnerRectCursor = Cursors.SizeWE;
            }
            else if (Math.Abs(location.Y - HightLightRect.Y) < 13)
                InnerRectCursor = Cursors.SizeNS;
            else if (Math.Abs(location.Y - HightLightRect.Y - HightLightRect.Height) < 13)
                InnerRectCursor = Cursors.SizeNS;
        }

        /// <summary>
        ///     计算选择框高亮区域
        /// </summary>
        /// <param name="location"></param>
        private void CalcHightLightRect(MouseEventArgs e)
        {
            Point location = e.GetPosition(innerFrame);
            Rect curRect = HightLightRect;
            double offsetX = location.X - m_lastPoint.X;
            double offsetY = location.Y - m_lastPoint.Y;
            if ((m_editMode & 0x1000) > 0)
            {
                if (curRect.Width - offsetX < 0)
                {
                    double width = offsetX - curRect.Width;
                    curRect.X = curRect.Right;
                    curRect.Width = width;
                    m_editMode &= 0x0111;
                    m_editMode |= 0x0010;
                }
                else
                {
                    curRect.X += offsetX;
                    curRect.Width -= offsetX;
                }
            }
            else if ((m_editMode & 0x0010) > 0)
            {
                if (curRect.Width + offsetX < 0)
                {
                    double width = Math.Abs(offsetX + curRect.Width);
                    curRect.X = curRect.X - width;
                    curRect.Width = width;
                    m_editMode &= 0x1101;
                    m_editMode |= 0x1000;
                }
                else
                {
                    curRect.Width += offsetX;
                }
            }
            if ((m_editMode & 0x0100) > 0)
            {
                if (curRect.Height - offsetY < 0)
                {
                    double height = offsetY - curRect.Height;
                    curRect.Y = curRect.Bottom;
                    curRect.Height = height;
                    m_editMode &= 0x1011;
                    m_editMode |= 0x0001;
                }
                else
                {
                    curRect.Y += offsetY;
                    curRect.Height -= offsetY;
                }
            }
            else if ((m_editMode & 0x0001) > 0)
            {
                if (curRect.Height + offsetY < 0)
                {
                    double height = Math.Abs(offsetY + curRect.Height);
                    curRect.Y = curRect.Y - height;
                    curRect.Height = height;
                    m_editMode &= 0x1110;
                    m_editMode |= 0x0100;
                }
                else
                {
                    curRect.Height += offsetY;
                }
            }
            HightLightRect = curRect;
            m_lastPoint = location;
        }

       
        private void outterFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right) return;
            if (InnerFrameMode == InnerFrameModeEnum.WaitingForSelect)
            {
                //  创建选择框
                m_isMouseDown = true;
                Point location = e.GetPosition(innerFrame);
                Rect newRect = HightLightRect;
                newRect.Location = location;
                newRect.Width = 0;
                newRect.Height = 0;
                HightLightRect = newRect;
                //  使用选择框编辑模式进行绘制
                InnerFrameMode = InnerFrameModeEnum.Resizing;
                m_editMode = 0x0011;
                m_lastPoint = location;
                e.Handled = true;
                innerFrame.CaptureMouse();
            }
        }

        private void window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (InnerFrameMode == InnerFrameModeEnum.WaitingForSelect)
                {
                    this.Close();
                }
                else
                {
                    inkCanvas.Strokes.Clear();
                    m_editMode = 0;
                    InnerFrameMode = InnerFrameModeEnum.WaitingForSelect;
                    HightLightRect = new Rect(0, 0, 0, 0);
                }
            }
        }

        private void SetDrawTool()
        {
            inkCanvas.UseCustomCursor = true;
            inkCanvas.Cursor = Cursors.Pen;
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
        }

        private BitmapSource GetSelectImage()
        {
            double dpi = this.Dpi();
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();
            Rect rect = HightLightRect.RectWithDpi(dpi);
            CroppedBitmap croppedBitmap01 = new CroppedBitmap((BitmapSource)BackgroundImg.Source,
                                                              new Int32Rect(
                                                                  (int)rect.X,
                                                                  (int)rect.Y,
                                                                  (int)rect.Width,
                                                                  (int)rect.Height));
            context.DrawImage(croppedBitmap01, new Rect(0, 0, rect.Width, rect.Height));
            inkCanvas.Strokes.Draw(context);
            context.Close();
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)(inkCanvas.ActualWidth * dpi),
                (int)(inkCanvas.ActualHeight * dpi),
                96,
                96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }

        DateTime m_lastClickTime;

        private void inkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_undoList.Clear();
            if (e.ChangedButton == MouseButton.Left)
            {
                DateTime curTime = DateTime.Now;
                Point curPoint = e.GetPosition(this);
                // 双击截图
                if ((curTime - m_lastClickTime).Milliseconds < 200 && Math.Abs(m_lastPoint.X - curPoint.X) < 5 && Math.Abs(m_lastPoint.Y - curPoint.Y) < 5)
                {
                    inkCanvas.Strokes.Remove(inkCanvas.Strokes.LastOrDefault());
                    SystemHelper.SetClipboard(GetSelectImage());
                    this.Close();
                }
                else
                {
                    m_lastPoint = curPoint;
                    m_lastClickTime = curTime;
                }
            }
        }

        private void outterFrame_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }

    public enum InnerFrameModeEnum
    {
        WaitingForSelect = 1,
        Resizing = 3,
        Drawing = 4,
        LongScreen = 5,

    }
}
