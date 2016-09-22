using System.Linq;
using Xamarin.Forms;

namespace MyStash.Controls
{
    public class PasswordCheckerBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            if (entry == null) return;
            entry.TextChanged += entryTextChanged;
            base.OnAttachedTo(entry);
        }
        private void entryTextChanged(object sender, TextChangedEventArgs e)
        {
            const string allowed = "0123456789ABCDEF";
            var isValid = !string.IsNullOrWhiteSpace(e.NewTextValue)
                && e.NewTextValue.ToUpper().Cast<char>().All(c => allowed.Contains(c.ToString()));
            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            if (entry == null) return;
            entry.TextChanged -= entryTextChanged;
            entry.TextColor = Color.Default;
            base.OnDetachingFrom(entry);
        }
    }
}
