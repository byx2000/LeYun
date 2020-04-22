using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class CarCollection : ObservableCollection<Car>
    {
        public new void Add(Car car)
        {
            car.ID = Count;
            base.Add(car);
        }

        public void SaveToFile(string filename)
        {

        }
    }
}
