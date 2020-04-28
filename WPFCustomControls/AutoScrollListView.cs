using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCustomControls
{
    public class AutoScrollListView : ListView
    {
        static AutoScrollListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoScrollListView), new FrameworkPropertyMetadata(typeof(AutoScrollListView)));
        }

        // 列表项改变时，自动滚动到该项
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            var newItemCount = e.NewItems.Count;

            if (newItemCount > 0)
                this.ScrollIntoView(e.NewItems[newItemCount - 1]);

            base.OnItemsChanged(e);
        }

        // 列表项被选中时，自动滚动到该项
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            this.ScrollIntoView(e.AddedItems[0]);

            base.OnSelectionChanged(e);
        }
    }
}
