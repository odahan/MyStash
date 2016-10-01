using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyStash.Cells
{
    public partial class ProCell : BaseCell
    {
        public ProCell()
        {
            InitializeComponent();
        }

        protected override void OnSettingsChanged()
        {
            DateLabel.IsVisible = DisplayDate;
            LineSeparator.IsVisible = DrawLines;
        }
    }
}
