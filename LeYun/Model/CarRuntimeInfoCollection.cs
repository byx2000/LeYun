using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class CarRuntimeInfoCollection : ObservableCollection<CarRuntimeInfo>
    {
        public void SetFinishedState(int id, bool isFinished)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (this[i].ID == id)
                {
                    this[i].IsFinished = isFinished;
                }
            }
        }
    }
}
