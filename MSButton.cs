using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace milano88.UI.Controls
{
    public class MSButton : Control
    {
        public MSButton()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Size = new Size(150, 35);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.Transparent;
            UpdateGraphicsBuffer();
        }

        private void UpdateGraphicsBuffer()
        {
            if (Width > 0 && Height > 0)
            {
                BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                context.MaximumBuffer = new Size(Width + 1, Height + 1);
                _bufGraphics = context.Allocate(CreateGraphics(), ClientRectangle);
            }
        }

        private BufferedGraphics _bufGraphics;
        private bool IsMouseDown, IsMouseOver;

        private Color borderColor = Color.Black;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        private int borderSize;
        [Category("Custom Properties")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value < 0 ? 0 : value;
                Invalidate();
            }
        }

        private int borderRadius;
        [Category("Custom Properties")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                roundedCorners = false;
                borderRadius = value < 0 ? 0 : value > Height / 2 ? Height / 2 : value;
                Invalidate();
            }
        }

        private bool roundedCorners;
        [Category("Custom Properties")]
        [DefaultValue(false)]
        public bool UseRoundedCorners
        {
            get { return roundedCorners; }
            set
            {
                if (value)
                {
                    roundedCorners = value;
                    borderRadius = Height / 2;
                }
                else
                {
                    roundedCorners = value;
                    borderRadius = 0;
                }
                Invalidate();
            }
        }

        private Color buttonColor = Color.DodgerBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color ButtonColor
        {
            get => buttonColor;
            set { buttonColor = value; Invalidate(); }
        }

        private Color buttonColorHover = Color.FromArgb(26, 129, 229);
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "26, 129, 229")]
        public Color ButtonColorHover
        {
            get => buttonColorHover;
            set { buttonColorHover = value; Invalidate(); }
        }

        private Color buttonColorDown = Color.DeepSkyBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DeepSkyBlue")]
        public Color ButtonColorDown
        {
            get => buttonColorDown;
            set { buttonColorDown = value; Invalidate(); }
        }

        private Color buttonColorDisabled = SystemColors.ControlLight;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "ControlLight")]
        public Color ButtonColorDisabled
        {
            get => buttonColorDisabled;
            set { buttonColorDisabled = value; Invalidate(); }
        }

        public enum TextAlign { Center, Left, Right }
        private TextAlign textAligment;
        [Category("Custom Properties")]
        [DefaultValue(TextAlign.Center)]
        public TextAlign TextAligment
        {
            get => textAligment;
            set { textAligment = value; Invalidate(); }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        private Color disabledForeColor = Color.Silver;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color DisabledForeColor
        {
            get => disabledForeColor;
            set { disabledForeColor = value; Invalidate(); }
        }

        [Category("Custom Properties")]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private Point textOffset = Point.Empty;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Point), "0,0")]
        public Point TextOffset
        {
            get => textOffset;
            set { textOffset = value; Invalidate(); }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(Font), "Segoe UI, 9pt")]
        public override Font Font
        {
            get => base.Font;
            set { base.Font = value; Invalidate(); }
        }

        private Size imageSize;
        [Category("Custom Properties")]
        public Size ImageSize
        {
            get => imageSize;
            set { imageSize = value; Invalidate(); }
        }

        private Point imageOffset = Point.Empty;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Point), "0,0")]
        public Point ImageOffset
        {
            get => imageOffset;
            set { imageOffset = value; Invalidate(); }
        }

        public enum ImageAlign { Left, Center, Right }
        ImageAlign imageAligment;
        [Category("Custom Properties")]
        [DefaultValue(ImageAlign.Left)]
        public ImageAlign ImageAligment
        {
            get => imageAligment;
            set { imageAligment = value; Invalidate(); }
        }

        private Image buttonImage;
        [Category("Custom Properties")]
        [DefaultValue(null)]
        public Image ButtonImage
        {
            get => buttonImage;
            set
            {
                buttonImage = value;
                if (value != null)
                    imageSize = value.Size;
                else imageSize = Size.Empty;
                Invalidate();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Width < Height) Width = Height;
            if (roundedCorners) borderRadius = Height / 2;
            else
            {
                if (borderRadius > Height / 2)
                    borderRadius = Height / 2;
            }
            UpdateGraphicsBuffer();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (IsMouseDown) return;
            IsMouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOver = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                IsMouseDown = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
                IsMouseDown = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (borderRadius == 0)
            {
                using (SolidBrush backColor = new SolidBrush(buttonColor))
                using (SolidBrush hoverColor = new SolidBrush(buttonColorHover))
                using (SolidBrush downColor = new SolidBrush(buttonColorDown))
                {
                    _bufGraphics.Graphics.SmoothingMode = SmoothingMode.None;
                    Region = new Region(ClientRectangle);

                    if (!IsMouseOver && !IsMouseDown)
                        _bufGraphics.Graphics.FillRectangle(backColor, ClientRectangle);

                    if (IsMouseOver && !IsMouseDown)
                        _bufGraphics.Graphics.FillRectangle(hoverColor, ClientRectangle);

                    if (IsMouseDown)
                        _bufGraphics.Graphics.FillRectangle(downColor, ClientRectangle);

                    if (borderSize > 0)
                    {
                        using (Pen penBorder = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset })
                            _bufGraphics.Graphics.DrawRectangle(penBorder, 0, 0, Width - 0.5F, Height - 0.5F);
                    }

                    if (!Enabled)
                    {
                        using (SolidBrush disabledColor = new SolidBrush(buttonColorDisabled))
                            _bufGraphics.Graphics.FillRectangle(disabledColor, ClientRectangle);

                        DrawText(_bufGraphics.Graphics, disabledForeColor);
                        DrawImage(_bufGraphics.Graphics, buttonImage, true);
                    }
                    else
                    {
                        DrawText(_bufGraphics.Graphics, ForeColor);
                        DrawImage(_bufGraphics.Graphics, buttonImage, false);
                    }
                }
            }
            else
            {
                Rectangle rectSurface = ClientRectangle;
                Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
                int smoothSize = 2;
                if (borderSize > 0) smoothSize = borderSize;

                using (GraphicsPath pathSurface = RoundedRectCreate(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = RoundedRectCreate(rectBorder, borderRadius - borderSize))
                using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
                {
                    _bufGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    Region = new Region(pathSurface);

                    if (!IsMouseOver && !IsMouseDown)
                    {
                        using (SolidBrush brushNormal = new SolidBrush(buttonColor))
                        using (Pen pen = new Pen(Color.FromArgb(180, buttonColor)))
                        {
                            _bufGraphics.Graphics.DrawPath(pen, pathSurface);
                            _bufGraphics.Graphics.FillPath(brushNormal, pathSurface);
                        }
                    }

                    if (IsMouseOver && !IsMouseDown)
                    {
                        using (SolidBrush brushHover = new SolidBrush(buttonColorHover))
                        using (Pen pen = new Pen(Color.FromArgb(180, buttonColorHover)))
                        {
                            _bufGraphics.Graphics.DrawPath(pen, pathSurface);
                            _bufGraphics.Graphics.FillPath(brushHover, pathSurface);
                        }
                    }

                    if (IsMouseDown)
                    {
                        using (SolidBrush brushDown = new SolidBrush(buttonColorDown))
                        using (Pen pen = new Pen(Color.FromArgb(180, buttonColorDown)))
                        {
                            _bufGraphics.Graphics.DrawPath(pen, pathSurface);
                            _bufGraphics.Graphics.FillPath(brushDown, pathSurface);
                        }
                    }

                    if (borderSize >= 1)
                    {
                        using (Pen penBorder = new Pen(borderColor, borderSize))
                            _bufGraphics.Graphics.DrawPath(penBorder, pathBorder);
                    }

                    if (!Enabled)
                    {
                        using (SolidBrush brushDisabled = new SolidBrush(buttonColorDisabled))
                        using (Pen pen = new Pen(Color.FromArgb(180, buttonColorDisabled)))
                        {
                            _bufGraphics.Graphics.DrawPath(pen, pathSurface);
                            _bufGraphics.Graphics.FillPath(brushDisabled, pathBorder);
                        }
                        DrawText(_bufGraphics.Graphics, disabledForeColor);
                        DrawImage(_bufGraphics.Graphics, buttonImage, true);
                    }
                    else
                    {
                        DrawText(_bufGraphics.Graphics, ForeColor);
                        DrawImage(_bufGraphics.Graphics, buttonImage, false);
                    }

                    _bufGraphics.Graphics.DrawPath(penSurface, pathSurface);
                }
            }

            _bufGraphics.Render(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null && BackColor == Color.Transparent)
            {
                Rectangle rect = new Rectangle(Left, Top, Width, Height);
                _bufGraphics.Graphics.TranslateTransform(-rect.X, -rect.Y);
                try
                {
                    using (PaintEventArgs pea = new PaintEventArgs(_bufGraphics.Graphics, rect))
                    {
                        pea.Graphics.SetClip(rect);
                        InvokePaintBackground(Parent, pea);
                        InvokePaint(Parent, pea);
                    }
                }
                finally
                {
                    _bufGraphics.Graphics.TranslateTransform(rect.X, rect.Y);
                }
            }
            else
            {
                using (SolidBrush backColor = new SolidBrush(BackColor))
                    _bufGraphics.Graphics.FillRectangle(backColor, ClientRectangle);
            }
            _bufGraphics.Render(pevent.Graphics);
        }

        private void DrawText(Graphics graphics, Color color)
        {
            if (Text != null)
            {
                int textWidth = TextRenderer.MeasureText(Text, Font).Width;
                int textHeight = TextRenderer.MeasureText(Text, Font).Height;
                int textX = (Width - textWidth) / 2;
                int textY = (Height - textHeight) / 2;
                if (textAligment == TextAlign.Left)
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(textOffset.X, textY + textOffset.Y, textWidth, textHeight), color);
                else if (textAligment == TextAlign.Center)
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(textX, textY, textWidth, textHeight), color);
                else if (textAligment == TextAlign.Right)
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(Width - textWidth - textOffset.X, textY + textOffset.Y, textWidth, textHeight), color);
            }
        }

        private void DrawImage(Graphics graphics, Image image, bool isDisabled)
        {
            if (image != null && !isDisabled)
            {
                int imageX = (Width - imageSize.Width) / 2;
                int imageY = (Height - imageSize.Height) / 2;
                if (imageAligment == ImageAlign.Left)
                {
                    if (roundedCorners)
                        graphics.DrawImage(buttonImage, (borderRadius / 2) + imageOffset.X, imageY + imageOffset.Y, imageSize.Width, imageSize.Height);
                    else graphics.DrawImage(buttonImage, 5 + imageOffset.X, imageY + imageOffset.Y, imageSize.Width, imageSize.Height);
                }
                else if (imageAligment == ImageAlign.Center)
                    graphics.DrawImage(buttonImage, imageX, imageY, imageSize.Width, imageSize.Height);
                else if (imageAligment == ImageAlign.Right)
                {
                    if (roundedCorners)
                        graphics.DrawImage(buttonImage, Width - buttonImage.Width - imageOffset.X - (borderRadius / 2), imageY + imageOffset.Y, imageSize.Width, imageSize.Height);
                    else graphics.DrawImage(buttonImage, Width - buttonImage.Width - imageOffset.X - 5, imageY + imageOffset.Y, imageSize.Width, imageSize.Height);
                }
            }
            else if (image != null && isDisabled)
            {
                int imageX = (Width - imageSize.Width) / 2;
                int imageY = (Height - imageSize.Height) / 2;
                if (imageAligment == ImageAlign.Left)
                    graphics.DrawImage(ToolStripRenderer.CreateDisabledImage(buttonImage), imageOffset.X, imageY + imageOffset.Y, imageSize.Width, imageSize.Height);
                else if (imageAligment == ImageAlign.Center)
                    graphics.DrawImage(ToolStripRenderer.CreateDisabledImage(buttonImage), imageX, imageY, imageSize.Width, imageSize.Height);
                else if (imageAligment == ImageAlign.Right)
                    graphics.DrawImage(ToolStripRenderer.CreateDisabledImage(buttonImage), Width - buttonImage.Width - imageOffset.X, imageY + imageOffset.Y, imageSize.Width, imageSize.Height);
            }
        }

        private GraphicsPath RoundedRectCreate(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
