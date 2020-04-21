using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class NodeCollection : ObservableCollection<Node>
    {
        public new void Add(Node node)
        {
            node.ID = Count;
            base.Add(node);
        }
    }
}
