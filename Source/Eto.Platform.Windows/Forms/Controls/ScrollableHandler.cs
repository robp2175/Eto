using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ScrollableHandler : WindowsContainer<ScrollableHandler.CustomScrollable, Scrollable>, IScrollable
	{
		SWF.Panel content;
		

		public class CustomScrollable : System.Windows.Forms.Panel
		{
			public ScrollableHandler Handler { get; set; }
			
			protected override bool ProcessDialogKey (SWF.Keys keyData)
			{
				SWF.KeyEventArgs e = new SWF.KeyEventArgs (keyData);
				base.OnKeyDown (e);
				return e.Handled;
			}
			
			protected override void OnCreateControl ()
			{
				base.OnCreateControl ();
				AutoSize = false;
			}
			
			protected override SD.Point ScrollToControl (SWF.Control activeControl)
			{
				/*if (autoScrollToControl) return base.ScrollToControl(activeControl);
				else return this.AutoScrollPosition;*/
				return this.AutoScrollPosition;
			}
		}

		public override Size DesiredSize
		{
			get
			{
				return base.DesiredSize;
			}
		}

		protected override void CalculateMinimumSize ()
		{
			base.CalculateMinimumSize ();
		}

		public override void SetScale (bool xscale, bool yscale)
		{
			var layout = WindowsLayout;

			if (layout != null)
				layout.SetScale (false, false);

			base.SetScale (xscale, yscale);
		}

		public override SWF.Control ContentContainer
		{
			get { return content; }
		}
		
		public BorderType Border {
			get {
				switch (Control.BorderStyle) {
				case SWF.BorderStyle.FixedSingle:
					return BorderType.Line;
				case SWF.BorderStyle.None:
					return BorderType.None;
				case SWF.BorderStyle.Fixed3D:
					return BorderType.Bezel;
				default:
					throw new NotSupportedException ();
				}
			}
			set {
				switch (value) {
				case BorderType.Bezel:
					Control.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
					break;
				case BorderType.Line:
					Control.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
					break;
				case BorderType.None:
					Control.BorderStyle = System.Windows.Forms.BorderStyle.None;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}

		public ScrollableHandler ()
		{
			ExpandContentHeight = ExpandContentWidth = true;
			SkipLayoutScale = true;
			Control = new CustomScrollable{ Handler = this };
			this.Control.Size = SD.Size.Empty;
			this.Control.MinimumSize = SD.Size.Empty;
			
			Control.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			Control.AutoScroll = true;
			
			Control.AutoSize = true;
			//Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			Control.VerticalScroll.SmallChange = 5;
			Control.VerticalScroll.LargeChange = 10;
			Control.HorizontalScroll.SmallChange = 5;
			Control.HorizontalScroll.LargeChange = 10;

			//control.AutoScrollPosition = new SD.Point(0,0);
			//control.AutoScrollMinSize = new System.Drawing.Size(500,500);
			//control.DisplayRectangle = new System.Drawing.Rectangle(0,0,500,1000);
			//control.BackColor = System.Drawing.Color.Black;
			content = new SWF.Panel ();
			content.AutoSize = true;
			Control.Controls.Add (content);

			Control.SizeChanged += delegate
			{
				if (ExpandContentWidth || ExpandContentHeight)
				{
					if (Widget.Layout != null && Widget.Layout.InnerLayout != null)
					{
						var layout = Widget.Layout.InnerLayout.Handler as IWindowsLayout;
						if (layout != null && layout.LayoutObject != null)
						{
							var c = layout.LayoutObject as SWF.Control;
							var minSize = Control.ClientSize;
							if (!ExpandContentWidth) minSize.Width = 0;
							if (!ExpandContentHeight) minSize.Height = 0;
							c.MinimumSize = minSize;
						}
					}
				}
			};
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Scrollable.ScrollEvent:
				Control.Scroll += delegate(object sender, System.Windows.Forms.ScrollEventArgs e) {
					this.Widget.OnScroll (new ScrollEventArgs (this.ScrollPosition));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public void UpdateScrollSizes ()
		{
			Control.PerformLayout ();
		}

		public Point ScrollPosition {
			get { return new Point (-Control.AutoScrollPosition.X, -Control.AutoScrollPosition.Y); }
			set { 
				Control.AutoScrollPosition = value.ToSD ();
			}
		}

		public Size ScrollSize {
			get { return this.Control.DisplayRectangle.Size.ToEto (); }
			set { Control.AutoScrollMinSize = value.ToSD (); }
		}

		public Rectangle VisibleRect {
			get { return new Rectangle (ScrollPosition, Size.Min (ScrollSize, ClientSize)); }
		}


		public bool ExpandContentWidth
		{
			get; set;
		}

		public bool ExpandContentHeight
		{
			get; set;
		}
	}
}
