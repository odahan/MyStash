using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyStash.Helpers
{
    /// <summary>
    /// The VisualTreeHelper class contains static methods that are useful for performing
    /// common tasks with visual tree nodes. 
    /// When possible methods are typed to BindableObject and may accept or
    /// return either type of visual tree node (e.g. GetParent).
    ///
    /// </summary>
    public static class VisualTreeHelper
    {
        /// <summary>
        /// Get the number of direct children of the specified Visual.
        /// </summary>
        public static int GetChildrenCount(BindableObject reference)
        {
            return GetChildren(reference).Count;
        }

        /// <summary>
        /// returns list of children of the bindable object (direct children)
        /// </summary>
        public static List<BindableObject> GetChildren(BindableObject reference)
        {
            var result = new List<BindableObject>();
            if ((reference as ContentView)?.Content != null) result.Add(((ContentView) reference).Content);
            if ((reference as Layout<View>)?.Children != null && ((Layout<View>)reference).Children.Count>0) result.AddRange(((Layout<View>)reference).Children);
            if ((reference as ContentPage)!= null) result.Add(((ContentPage)reference).Content);
            if ((reference as MasterDetailPage) != null)
            {
                result.AddRange(GetChildren(((MasterDetailPage)reference).Master));
                result.AddRange(GetChildren(((MasterDetailPage)reference).Detail));
            }
            if ((reference as NavigationPage) != null) result.AddRange(GetChildren(((NavigationPage)reference).CurrentPage));
            if ((reference as TabbedPage) != null)
            {
                foreach(var t in ((TabbedPage)reference).ItemsSource) result.AddRange(GetChildren(t as BindableObject));
            }
            return result;
        }

        /// <summary>
        /// Returns the child of Visual visual at the specified index.
        /// </summary>
        public static BindableObject GetChild(BindableObject reference, int childIndex)
        {
            try
            {
                return GetChildren(reference)[childIndex];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Visual parent of this Visual.
        /// </summary>
        public static BindableObject GetParent(BindableObject reference)
        {
            return (reference as VisualElement)?.Parent;
        }


        public static BindableObject GetRoot(BindableObject reference)
        {
            if (reference == null) return null;
            var r = GetParent(reference);
            return r == null ? reference : GetRoot(r);
        }


        public static BindableObject FindObject(BindableObject root, Predicate<BindableObject> predicate)
        {
            if (root == null || predicate==null ) return null;
            var children = GetChildren(root);
            foreach (var child in children)
            {
                if (predicate(child)) return child;
            }
            foreach (var child in children)
            {
                var r = FindObject(child, predicate);
                if (r != null) return r;
            }
            return null;
        }


        public static List<BindableObject> FindObjects(BindableObject root, Predicate<BindableObject> predicate)
        {
            var res  = new List<BindableObject>();
            if (root == null || predicate == null) return res;
            var children = GetChildren(root);
            foreach (var child in children)
            {
                if (predicate(child)) res.Add(child);
            }
            foreach (var child in children)
            {
                res.AddRange(FindObjects(child, predicate));
            }
            return res;
        }
        
    }
}