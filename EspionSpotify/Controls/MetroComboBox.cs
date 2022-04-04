using System;
using System.Windows.Forms;

namespace EspionSpotify.Controls
{
    public class MetroComboBox : MetroFramework.Controls.MetroComboBox
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ((HandledMouseEventArgs) e).Handled = true;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            base.OnLostFocus(e);
        }
    }
}