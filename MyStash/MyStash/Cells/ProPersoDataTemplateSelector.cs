using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStash.Models;
using Xamarin.Forms;

namespace MyStash.Cells
{
    public class ProPersoDataTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate proCell;
        private readonly DataTemplate persoCell;
        private readonly DataTemplate errorCell;

        public ProPersoDataTemplateSelector()
        {
            proCell = new DataTemplate(typeof(ProCell));
            persoCell = new DataTemplate(typeof(PersoCell));
            errorCell = new DataTemplate(() => new TextCell {Text = "NULL ITEM"});
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var sheet = item as InfoSheet;
            if (sheet == null) return errorCell;
            return sheet.IsPro ? proCell : persoCell;

        }
    }
}
