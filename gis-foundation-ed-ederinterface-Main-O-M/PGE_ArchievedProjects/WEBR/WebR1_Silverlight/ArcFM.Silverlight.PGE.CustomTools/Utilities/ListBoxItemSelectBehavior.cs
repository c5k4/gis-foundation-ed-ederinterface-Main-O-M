using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public class ListBoxItemSelectBehavior: Behavior<ListBox>
    {
        public ListBoxItemSelectBehavior(): base()
        {}

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            
            foreach (StoredViewDataStructure data in listBox.Items)
            {
                if (data.Selected) data.Selected = false;
            }
            (listBox.SelectedItem as StoredViewDataStructure).Selected = true;

        }
        
    }
}
