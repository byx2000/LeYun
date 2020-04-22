using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeYun.Model
{
    class CarCollection : ObservableCollection<Car>
    {
        private const string CarDataFileFlag = "CarData";

        public new void Add(Car car)
        {
            car.ID = Count;
            base.Add(car);
        }

        public void SaveToFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine(CarDataFileFlag);
                    sw.WriteLine(Count.ToString());
                    for (int i = 0; i < Count; ++i)
                    {
                        sw.WriteLine(this[i].WeightLimit);
                        sw.WriteLine(this[i].DisLimit);
                    }
                }
            }
        }

        public void ReadFromFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    if (sr.ReadLine() != CarDataFileFlag)
                    {
                        throw new Exception("文件格式错误！");
                    }

                    Clear();
                    int cnt = int.Parse(sr.ReadLine());
                    for (int i = 0; i < cnt; ++i)
                    {
                        double weightLimit = double.Parse(sr.ReadLine());
                        double disLimit = double.Parse(sr.ReadLine());
                        Add(new Car { WeightLimit = weightLimit, DisLimit = disLimit });
                    }
                }
            }
        }
    }
}
