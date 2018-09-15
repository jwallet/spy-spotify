using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EspionSpotify.Controls
{
    public class MetroTile : MetroFramework.Controls.MetroTile
    {
        private bool _isFocused = false;
        private bool _isHovered = false;

        protected override void OnPaintForeground(PaintEventArgs e)
        {
            base.OnPaintForeground(e);

            Color color = Color.FromArgb(175, 240, 200);

            if (_isHovered || _isFocused)
            {
                color = Color.FromArgb(30, 215, 96);
            }

            using (Pen p = new Pen(color))
            {
                p.Width = 3;
                Rectangle borderRect = new Rectangle(1, 1, Width - 3, Height - 3);
                e.Graphics.DrawRectangle(p, borderRect);
            }
        }
        protected override void OnGotFocus(EventArgs e)
        {
            _isFocused = true;
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _isFocused = false;
            _isHovered = false;

            Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(150);
                base.OnLeave(e);
            });
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);
        }
    }
}
