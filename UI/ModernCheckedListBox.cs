using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 自绘的勾选列表。CheckedListBox 不支持自定义行高，
    /// 因此这里用轻量级面板实现更大的行距和圆角复选框。
    /// </summary>
    public class ModernCheckedListBox : Panel
    {
        private sealed class Item
        {
            public string Text { get; }
            public bool Checked { get; set; }

            public Item(string text, bool isChecked)
            {
                Text = text;
                Checked = isChecked;
            }
        }

        public sealed class ItemCollection : IEnumerable<string>
        {
            private readonly ModernCheckedListBox _owner;

            internal ItemCollection(ModernCheckedListBox owner)
            {
                _owner = owner;
            }

            public int Count => _owner._items.Count;

            public string this[int index] => _owner._items[index].Text;

            public void Add(string text)
            {
                _owner.AddItem(text, false);
            }

            public void AddRange(IEnumerable<object> items)
            {
                foreach (var item in items)
                {
                    if (item is not null)
                        Add(item.ToString() ?? "");
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                foreach (var item in _owner._items)
                    yield return item.Text;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private const int RowHeight = 40;
        private const int CheckBoxSize = 18;

        private readonly List<Item> _items = new();
        private int _hoverIndex = -1;
        private int _focusedIndex = -1;

        public ModernCheckedListBox()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = Theme.Surface;
            Font = Theme.BodyFont;
            ForeColor = Theme.TextPrimary;
            AutoScroll = true;
            Padding = new Padding(0, 4, 0, 4);
            TabStop = true;

            Items = new ItemCollection(this);
        }

        public event ItemCheckEventHandler? ItemCheck;

        public event EventHandler? CheckedChanged;

        public ItemCollection Items { get; }

        public IEnumerable<string> CheckedItems
        {
            get
            {
                foreach (var item in _items)
                {
                    if (item.Checked)
                        yield return item.Text;
                }
            }
        }

        public List<int> CheckedIndices
        {
            get
            {
                var indices = new List<int>();
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Checked)
                        indices.Add(i);
                }
                return indices;
            }
        }

        public bool GetItemChecked(int index)
        {
            return index >= 0 && index < _items.Count && _items[index].Checked;
        }

        public void SetItemChecked(int index, bool isChecked)
        {
            if (index < 0 || index >= _items.Count)
                return;

            var item = _items[index];
            if (item.Checked == isChecked)
                return;

            var oldState = item.Checked ? CheckState.Checked : CheckState.Unchecked;
            item.Checked = isChecked;
            Invalidate();

            ItemCheck?.Invoke(
                this,
                new ItemCheckEventArgs(
                    index,
                    isChecked ? CheckState.Checked : CheckState.Unchecked,
                    oldState));

            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int scrollOffset = AutoScrollPosition.Y;
            int availableWidth = ClientSize.Width - 8;
            if (VerticalScroll.Visible)
                availableWidth -= SystemInformation.VerticalScrollBarWidth;

            int top = Padding.Top + scrollOffset;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var rowRect = new Rectangle(2, top + i * RowHeight + 3, Math.Max(10, availableWidth - 4), RowHeight - 6);

                bool hovered = i == _hoverIndex;
                bool focused = Focused && i == _focusedIndex;

                if (hovered || focused)
                {
                    using var rowPath = Theme.RoundedRect(rowRect, 9);
                    using var rowBrush = new SolidBrush(focused ? Theme.Selection : Theme.Hover);
                    graphics.FillPath(rowBrush, rowPath);
                }

                var boxRect = new Rectangle(
                    rowRect.Left + 10,
                    rowRect.Top + (rowRect.Height - CheckBoxSize) / 2,
                    CheckBoxSize,
                    CheckBoxSize);

                using (var boxPath = Theme.RoundedRect(boxRect, 5))
                {
                    if (item.Checked)
                    {
                        using var fill = new SolidBrush(Theme.Primary);
                        graphics.FillPath(fill, boxPath);
                    }
                    else
                    {
                        using var fill = new SolidBrush(Theme.Surface);
                        graphics.FillPath(fill, boxPath);
                        using var border = new Pen(Theme.BorderStrong, 1.4f);
                        graphics.DrawPath(border, boxPath);
                    }
                }

                if (item.Checked)
                {
                    using var checkPen = new Pen(Color.White, 2f)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round,
                        LineJoin = LineJoin.Round
                    };

                    graphics.DrawLines(checkPen, new[]
                    {
                        new Point(boxRect.Left + 4, boxRect.Top + 9),
                        new Point(boxRect.Left + 8, boxRect.Top + 13),
                        new Point(boxRect.Left + 14, boxRect.Top + 5)
                    });
                }

                var textRect = new Rectangle(
                    boxRect.Right + 10,
                    rowRect.Top,
                    Math.Max(10, rowRect.Right - boxRect.Right - 14),
                    rowRect.Height);

                TextRenderer.DrawText(
                    graphics,
                    item.Text,
                    Font,
                    textRect,
                    focused ? Theme.SelectionText : Theme.TextPrimary,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.NoPrefix);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int index = IndexAt(e.Location);
            if (index != _hoverIndex)
            {
                _hoverIndex = index;
                Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_hoverIndex != -1)
            {
                _hoverIndex = -1;
                Cursor = Cursors.Default;
                Invalidate();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            Focus();

            int index = IndexAt(e.Location);
            if (index < 0)
                return;

            _focusedIndex = index;
            SetItemChecked(index, !GetItemChecked(index));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_items.Count == 0)
                return;

            if (e.KeyCode is Keys.Up or Keys.Down or Keys.Home or Keys.End)
            {
                if (_focusedIndex < 0)
                    _focusedIndex = 0;
                else
                {
                    _focusedIndex = e.KeyCode switch
                    {
                        Keys.Up => Math.Max(0, _focusedIndex - 1),
                        Keys.Down => Math.Min(_items.Count - 1, _focusedIndex + 1),
                        Keys.Home => 0,
                        _ => _items.Count - 1
                    };
                }

                ScrollControlIntoViewForIndex(_focusedIndex);
                Invalidate();
                e.Handled = true;            }
            else if (e.KeyCode == Keys.Space)
            {
                if (_focusedIndex < 0)
                    _focusedIndex = 0;

                SetItemChecked(_focusedIndex, !GetItemChecked(_focusedIndex));
                e.Handled = true;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (_focusedIndex < 0 && _items.Count > 0)
                _focusedIndex = 0;
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        private void AddItem(string text, bool isChecked)
        {
            _items.Add(new Item(text, isChecked));
            UpdateScrollSize();
            Invalidate();
        }

        private void UpdateScrollSize()
        {
            AutoScrollMinSize = new Size(0, Padding.Vertical + _items.Count * RowHeight);
        }

        private int IndexAt(Point location)
        {
            int y = location.Y - Padding.Top - AutoScrollPosition.Y;
            if (y < 0)
                return -1;

            int index = y / RowHeight;
            return index >= 0 && index < _items.Count ? index : -1;
        }

        private void ScrollControlIntoViewForIndex(int index)
        {
            int itemTop = Padding.Top + index * RowHeight;
            int itemBottom = itemTop + RowHeight;
            int viewTop = -AutoScrollPosition.Y;
            int viewBottom = viewTop + ClientSize.Height;

            if (itemTop < viewTop)
                AutoScrollPosition = new Point(0, itemTop);
            else if (itemBottom > viewBottom)
                AutoScrollPosition = new Point(0, itemBottom - ClientSize.Height + Padding.Bottom);
        }
    }
}
