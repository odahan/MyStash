using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace MyStash.Controls
{
    public class BindablePicker : Picker
    {
        public BindablePicker()
        {
            SelectedIndexChanged += onSelectedIndexChanged;
        }

        [LocalizationRequired(false)]
        public static readonly BindableProperty SelectedItemProperty =
             BindableProperty.Create("SelectedItem", typeof(object),
                                        typeof(BindablePicker), null, BindingMode.TwoWay,
                                        null, onSelectedItemChanged);
        [LocalizationRequired(false)]
        public static readonly BindableProperty ItemsSourceProperty =
             BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker),
                                        null, BindingMode.OneWay, null, onItemsSourceChanged);
        [LocalizationRequired(false)]
        public static readonly BindableProperty DisplayPropertyProperty =
             BindableProperty.Create("DisplayProperty", typeof(string), typeof(BindablePicker),
                                        null, BindingMode.OneWay, null, onDisplayPropertyChanged);

        [LocalizationRequired(false)]
        public static readonly BindableProperty EnableNullOptionProperty =
            BindableProperty.Create("AddNullOption", typeof(bool), typeof(BindablePicker), false);

        [LocalizationRequired(false)]
        public static readonly BindableProperty NullOptionTitleProperty =
            BindableProperty.Create("NullOptionTitle", typeof(string),
                                    typeof(BindablePicker), default(string));

        // Ajoute un élément pour le choix "rien"
        public bool EnableNullOption
        {
            get { return (bool)GetValue(EnableNullOptionProperty); }
            set { SetValue(EnableNullOptionProperty, value); }
        }

        // titre du choix "rien"
        public string NullOptionTitle
        {
            get { return (string)GetValue(NullOptionTitleProperty); }
            set { SetValue(NullOptionTitleProperty, value); }
        }

        // Liste des items
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // instance de l'item sélectionné
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // nom de la propriété à afficher dans le picker
        public string DisplayProperty
        {
            get { return (string)GetValue(DisplayPropertyProperty); }
            set { SetValue(DisplayPropertyProperty, value); }
        }

        private void onSelectedIndexChanged(object sender, EventArgs e)
        {
            if (settingIndex) return;
            if (SelectedIndex == -1)
                SelectedItem = null;
            else if (EnableNullOption && SelectedIndex == 0)
                SelectedItem = null;
            else
                SelectedItem = ItemsSource[SelectedIndex];
        }

        private bool settingIndex;

        private static void onSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (BindablePicker)bindable;
            picker.SelectedItem = newValue;
            if (picker.ItemsSource == null || picker.SelectedItem == null) return;
            var count = 0;
            foreach (var obj in picker.ItemsSource)
            {
                if (obj == picker.SelectedItem)
                {
                    picker.settingIndex = true;
                    picker.SelectedIndex = count;
                    picker.settingIndex = false;
                    break;
                }
                count++;
            }
        }

        private static void onDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (BindablePicker)bindable;
            picker.DisplayProperty = (string)newValue;
            loadItemsAndSetSelected(bindable);
        }

        private static void onItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (BindablePicker)bindable;
            if (picker.EnableNullOption && oldValue is IList && newValue is IList
                && (((IList)oldValue).Count + 1) == (((IList)newValue).Count))
            {
                //l'item null a déjà été ajouté
            }
            else if (picker.EnableNullOption)
            {
                var objectList = new List<object> { picker.NullOptionTitle };
                objectList.AddRange(((IList)newValue).Cast<object>());
                picker.ItemsSource = objectList;
            }
            else
            {
                picker.ItemsSource = (IList)newValue;
            }

            loadItemsAndSetSelected(bindable);
        }

        private static void loadItemsAndSetSelected(BindableObject bindable)
        {
            var picker = (BindablePicker)bindable;
            if (picker.ItemsSource == null) return;
            picker.SelectedIndex = -1;
            picker.Items.Clear();
            var count = 0;
            foreach (var obj in picker.ItemsSource)
            {
                string value;
                if (picker.DisplayProperty != null)
                {
                    var prop = obj.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, picker.DisplayProperty, StringComparison.OrdinalIgnoreCase));
                    value = prop != null ? prop.GetValue(obj).ToString() : obj.ToString();
                }
                else
                {
                    value = obj.ToString();
                }
                picker.Items.Add(value);
                if (picker.SelectedItem != null)
                {
                    if (picker.SelectedItem == obj)
                    {
                        picker.SelectedIndex = count;
                    }
                }
                count++;
            }
        }
    }
}


