﻿using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl;
using MatterHackers.VectorMath;
using System;
using System.Collections.Specialized;

namespace MatterHackers.Agg.UI
{
	public class DropDownMenu : Menu
	{
		public event EventHandler SelectionChanged;

		private TextWidget mainControlText;

		public DropDownMenu(GuiWidget topMenuWidget, Direction direction = Direction.Down)
			: base(direction)
		{
		}

		public DropDownMenu(string topMenuText, Direction direction = Direction.Down, double pointSize = 12)
			: base(direction)
		{
			SetStates(topMenuText, pointSize);
		}

		public DropDownMenu(string topMenuText, GuiWidget buttonView, Direction direction = Direction.Down, double pointSize = 12)
			: base(buttonView, direction)
		{
			SetStates(topMenuText, pointSize);
		}

		bool menuAsWideAsItems = true;
		public bool MenuAsWideAsItems 
		{
			get
			{
				return menuAsWideAsItems;
			}

			set
			{
				menuAsWideAsItems = value;
			}
		}

		private int borderWidth = 1;

		bool drawDirectionalArrow = true;
		public bool DrawDirectionalArrow 
		{
			get
			{
				return drawDirectionalArrow;
			}

			set
			{
				drawDirectionalArrow = value;
			}
		}

		public int BorderWidth
		{
			get { return borderWidth; }
			set { borderWidth = value; }
		}

		public RGBA_Bytes BorderColor { get; set; }

		public RGBA_Bytes NormalArrowColor { get; set; }

		public RGBA_Bytes HoverArrowColor { get; set; }

		public RGBA_Bytes NormalColor { get; set; }

		public RGBA_Bytes HoverColor { get; set; }

		private RGBA_Bytes textColor = RGBA_Bytes.Black;

		public RGBA_Bytes TextColor
		{
			get { return textColor; }
			set
			{
				textColor = value;
				//mainControlText.TextColor = TextColor;
			}
		}

		private int selectedIndex = -1;

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;
				OnSelectionChanged(null);
				Invalidate();
			}
		}

		public String SelectedValue
		{
			get { return GetValue(SelectedIndex); }
		}

		public string GetValue(int itemIndex)
		{
			return MenuItems[SelectedIndex].Value;
		}

		private void SetStates(string topMenuText, double pointSize)
		{
			SetDisplayAttributes();

			MenuItems.CollectionChanged += new NotifyCollectionChangedEventHandler(MenuItems_CollectionChanged);
			mainControlText = new TextWidget(topMenuText, pointSize: pointSize);
			mainControlText.TextColor = this.TextColor;
			mainControlText.AutoExpandBoundsToText = true;
			mainControlText.VAnchor = UI.VAnchor.ParentCenter;
			mainControlText.HAnchor = UI.HAnchor.ParentLeft;
			AddChild(mainControlText);
			HAnchor = HAnchor.FitToChildren;
			VAnchor = VAnchor.FitToChildren;
			this.Name = topMenuText + " Menu";

			MouseEnter += new EventHandler(DropDownList_MouseEnter);
			MouseLeave += new EventHandler(DropDownList_MouseLeave);

			//IE Don't show arrow unless color is set explicitly
			NormalArrowColor = new RGBA_Bytes(255, 255, 255, 0);
			HoverArrowColor = TextColor;
		}

		protected override void DropListItems_Closed(object sender, EventArgs e)
		{
			BackgroundColor = NormalColor;
			base.DropListItems_Closed(sender, e);
		}

		private void DropDownList_MouseLeave(object sender, EventArgs e)
		{
			if (!this.IsOpen)
			{
				BackgroundColor = NormalColor;
			}
		}

		private void DropDownList_MouseEnter(object sender, EventArgs e)
		{
			BackgroundColor = HoverColor;
		}

		private void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(this, e);
			}
		}

		private void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Vector2 minSize = new Vector2(LocalBounds.Width, LocalBounds.Height);
			foreach (MenuItem item in MenuItems)
			{
				minSize.x = Math.Max(minSize.x, item.Width);
			}

			string startText = mainControlText.Text;
			foreach (MenuItem item in MenuItems)
			{
				mainControlText.Text = item.Text;

				minSize.x = Math.Max(minSize.x, LocalBounds.Width);
				minSize.y = Math.Max(minSize.y, LocalBounds.Height);
			}
			mainControlText.Text = startText;

			if (MenuAsWideAsItems)
			{
				this.MinimumSize = minSize;
			}

			foreach (MenuItem item in e.NewItems)
			{
				item.MinimumSize = minSize;
				// remove it if it is there so we don't have two. It is ok to remove a delagate that is not present.
				item.Selected -= new EventHandler(item_Selected);
				item.Selected += new EventHandler(item_Selected);
			}
		}

		private void item_Selected(object sender, EventArgs e)
		{
			int newSelectedIndex = 0;
			foreach (MenuItem item in MenuItems)
			{
				if (item == sender)
				{
					break;
				}
				newSelectedIndex++;
			}

			SelectedIndex = newSelectedIndex;
			BackgroundColor = NormalColor;
		}

		private void SetDisplayAttributes()
		{
			this.TextColor = ActiveTheme.Instance.PrimaryTextColor;
			this.NormalColor = ActiveTheme.Instance.PrimaryBackgroundColor;
			this.HoverColor = RGBA_Bytes.Gray;
			this.MenuItemsBorderWidth = 1;
			this.MenuItemsBackgroundColor = RGBA_Bytes.White;
			this.MenuItemsBorderColor = RGBA_Bytes.Gray;
			this.MenuItemsPadding = new BorderDouble(10, 10, 10, 10);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			base.OnDraw(graphics2D);
			this.DrawBorder(graphics2D);
			if (DrawDirectionalArrow)
			{
				this.DoDrawDirectionalArrow(graphics2D);
			}
		}

		protected virtual void DoDrawDirectionalArrow(Graphics2D graphics2D)
		{
			PathStorage littleArrow = new PathStorage();
			if (this.MenuDirection == Direction.Down)
			{
				littleArrow.MoveTo(-4, 0);
				littleArrow.LineTo(4, 0);
				littleArrow.LineTo(0, -5);
			}
			else if (this.MenuDirection == Direction.Up)
			{
				littleArrow.MoveTo(-4, -5);
				littleArrow.LineTo(4, -5);
				littleArrow.LineTo(0, 0);
			}
			else
			{
				throw new NotImplementedException("Pulldown direction has not been implemented");
			}
			if (UnderMouseState != UI.UnderMouseState.NotUnderMouse)
			{
				graphics2D.Render(littleArrow, LocalBounds.Right - 10, LocalBounds.Bottom + Height - 4, NormalArrowColor);
			}
			else
			{
				graphics2D.Render(littleArrow, LocalBounds.Right - 10, LocalBounds.Bottom + Height - 4, HoverArrowColor);
			}
		}

		private void DrawBorder(Graphics2D graphics2D)
		{
			RectangleDouble Bounds = LocalBounds;
			if (borderWidth > 0)
			{
				if (borderWidth == 1)
				{
					graphics2D.Rectangle(Bounds, BorderColor);
				}
				else
				{
					RoundedRect borderRect = new RoundedRect(this.LocalBounds, 0);
					Stroke strokeRect = new Stroke(borderRect, borderWidth);
					graphics2D.Render(strokeRect, BorderColor);
				}
			}
		}

		public MenuItem AddItem(string name, string value = null, double pointSize = 12)
		{
			if (value == null)
			{
				value = name;
			}
			if (mainControlText.Text != "")
			{
				mainControlText.Margin = MenuItemsPadding;
			}

			GuiWidget normalTextWithMargin = new GuiWidget();
			normalTextWithMargin.HAnchor = HAnchor.FitToChildren;
			normalTextWithMargin.BackgroundColor = MenuItemsBackgroundColor;
			TextWidget normal = new TextWidget(name, pointSize: pointSize);
			normal.Margin = MenuItemsPadding;
			normal.TextColor = MenuItemsTextColor;
			normalTextWithMargin.AddChild(normal);
			normalTextWithMargin.VAnchor = VAnchor.FitToChildren;

			GuiWidget hoverTextWithMargin = new GuiWidget();
			hoverTextWithMargin.HAnchor = HAnchor.FitToChildren;
			hoverTextWithMargin.BackgroundColor = MenuItemsBackgroundHoverColor;
			TextWidget hover = new TextWidget(name, pointSize: pointSize);
			hover.Margin = MenuItemsPadding;
			hover.TextColor = mainControlText.TextColor;
			hoverTextWithMargin.AddChild(hover);
			hoverTextWithMargin.VAnchor = VAnchor.FitToChildren;

			MenuItem menuItem = new MenuItem(new MenuItemStatesView(normalTextWithMargin, hoverTextWithMargin), value);
			menuItem.Text = name;
			menuItem.Name = name + " Menu Item";
			MenuItems.Add(menuItem);

			return menuItem;
		}
	}
}