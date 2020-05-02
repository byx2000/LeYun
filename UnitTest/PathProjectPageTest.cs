using System;
using LeYun.Model;
using LeYun.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class PathProjectPageTest
    {
        class MockAddCarQueryOk : IAddCarQuery
        {
            public double WeightLimit { get; set; }
            public double DisLimit { get; set; }

            public MockAddCarQueryOk(double w, double d)
            {
                WeightLimit = w;
                DisLimit = d;
            }

            public bool Begin()
            {
                return true;
            }
        }

        class MockAddCarQueryCancel : IAddCarQuery
        {
            public double WeightLimit { get; set; }
            public double DisLimit { get; set; }

            public MockAddCarQueryCancel(double w, double d)
            {
                WeightLimit = w;
                DisLimit = d;
            }

            public bool Begin()
            {
                return false;
            }
        }

        [TestMethod]
        public void AddCarCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.AddCarQuery = new MockAddCarQueryOk(1.2, 37.5);
            vm.Segments.Add(new Segment());
            vm.AddCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 1);
            Assert.IsTrue(vm.Record.Cars[0].WeightLimit == 1.2);
            Assert.IsTrue(vm.Record.Cars[0].DisLimit == 37.5);
            Assert.IsTrue(vm.CurrentCar == vm.Record.Cars[0]);
            Assert.IsTrue(vm.Segments.Count == 0);

            vm.AddCarQuery = new MockAddCarQueryOk(5.3, 72.4);
            vm.AddCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 2);
            Assert.IsTrue(vm.Record.Cars[1].WeightLimit == 5.3);
            Assert.IsTrue(vm.Record.Cars[1].DisLimit == 72.4);
            Assert.IsTrue(vm.CurrentCar == vm.Record.Cars[1]);

            vm.AddCarQuery = new MockAddCarQueryCancel(1.1, 2.3);
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.AddCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 2);
            Assert.IsTrue(vm.Segments.Count == 2);

            vm.AddCarQuery = new MockAddCarQueryCancel(5.6, 7.8);
            vm.AddCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 2);
        }

        class MockEditCarQueryOk : IEditCarQuery
        {
            public double NewWeightLimit { get; set; }
            public double NewDisLimit { get; set; }

            public bool Begin(Car car)
            {
                NewWeightLimit = 3.76;
                NewDisLimit = 71.35;
                return true;
            }
        }

        class MockEditCarQueryCancel : IEditCarQuery
        {
            public double NewWeightLimit { get; set; }
            public double NewDisLimit { get; set; }

            public bool Begin(Car car)
            {
                NewWeightLimit = 5.74;
                NewDisLimit = 38.56;
                return false;
            }
        }

        [TestMethod]
        public void EditCarCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.Record.Cars.Add(new Car { WeightLimit = 3.7, DisLimit = 54.8 });
            vm.Record.Cars.Add(new Car { WeightLimit = 2.9, DisLimit = 37.65 });
            vm.Segments.Add(new Segment());

            vm.EditCarQuery = new MockEditCarQueryOk();
            vm.CurrentCar = vm.Record.Cars[0];
            vm.EditCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars[0].WeightLimit == 3.76);
            Assert.IsTrue(vm.Record.Cars[0].DisLimit == 71.35);
            Assert.IsTrue(vm.Record.Cars[1].WeightLimit == 2.9);
            Assert.IsTrue(vm.Record.Cars[1].DisLimit == 37.65);
            Assert.IsTrue(vm.Segments.Count == 0);

            vm.EditCarQuery = new MockEditCarQueryCancel();
            vm.CurrentCar = vm.Record.Cars[0];
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.EditCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars[0].WeightLimit == 3.76);
            Assert.IsTrue(vm.Record.Cars[0].DisLimit == 71.35);
            Assert.IsTrue(vm.Record.Cars[1].WeightLimit == 2.9);
            Assert.IsTrue(vm.Record.Cars[1].DisLimit == 37.65);
            Assert.IsTrue(vm.Segments.Count == 2);

            vm.EditCarQuery = new MockEditCarQueryCancel();
            vm.CurrentCar = vm.Record.Cars[1];
            vm.EditCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars[0].WeightLimit == 3.76);
            Assert.IsTrue(vm.Record.Cars[0].DisLimit == 71.35);
            Assert.IsTrue(vm.Record.Cars[1].WeightLimit == 2.9);
            Assert.IsTrue(vm.Record.Cars[1].DisLimit == 37.65);

            vm.EditCarQuery = new MockEditCarQueryOk();
            vm.CurrentCar = vm.Record.Cars[1];
            vm.EditCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars[0].WeightLimit == 3.76);
            Assert.IsTrue(vm.Record.Cars[0].DisLimit == 71.35);
            Assert.IsTrue(vm.Record.Cars[1].WeightLimit == 3.76);
            Assert.IsTrue(vm.Record.Cars[1].DisLimit == 71.35);
        }

        [TestMethod]
        public void RemoveCarCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.Record.Cars.Add(new Car { WeightLimit = 1, DisLimit = 2 });
            vm.Record.Cars.Add(new Car { WeightLimit = 3, DisLimit = 4 });
            vm.Record.Cars.Add(new Car { WeightLimit = 5, DisLimit = 6 });

            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());

            vm.CurrentCar = vm.Record.Cars[0];
            vm.RemoveCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 2);
            Assert.IsTrue(vm.Record.Cars[0].Equals(new Car { ID = 0, WeightLimit = 3, DisLimit = 4 }));
            Assert.IsTrue(vm.Record.Cars[1].Equals(new Car { ID = 1, WeightLimit = 5, DisLimit = 6 }));
            Assert.IsTrue(vm.Segments.Count == 0);

            vm.Record.Cars.Clear();
            vm.Record.Cars.Add(new Car { WeightLimit = 1, DisLimit = 2 });
            vm.Record.Cars.Add(new Car { WeightLimit = 3, DisLimit = 4 });
            vm.Record.Cars.Add(new Car { WeightLimit = 5, DisLimit = 6 });

            vm.CurrentCar = vm.Record.Cars[1];
            vm.RemoveCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 2);
            Assert.IsTrue(vm.Record.Cars[0].Equals(new Car { ID = 0, WeightLimit = 1, DisLimit = 2 }));
            Assert.IsTrue(vm.Record.Cars[1].Equals(new Car { ID = 1, WeightLimit = 5, DisLimit = 6 }));

            vm.Record.Cars.Clear();
            vm.Record.Cars.Add(new Car { WeightLimit = 1, DisLimit = 2 });
            vm.Record.Cars.Add(new Car { WeightLimit = 3, DisLimit = 4 });
            vm.Record.Cars.Add(new Car { WeightLimit = 5, DisLimit = 6 });

            vm.CurrentCar = vm.Record.Cars[2];
            vm.RemoveCarCommand.Execute(null);
            Assert.IsTrue(vm.Record.Cars.Count == 2);
            Assert.IsTrue(vm.Record.Cars[0].Equals(new Car { ID = 0, WeightLimit = 1, DisLimit = 2 }));
            Assert.IsTrue(vm.Record.Cars[1].Equals(new Car { ID = 1, WeightLimit = 3, DisLimit = 4 }));
        }

        class MockAddNodeQueryOk : IAddNodeQuery
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Demand { get; set; }

            public MockAddNodeQueryOk(double x, double y, double d)
            {
                X = x;
                Y = y;
                Demand = d;
            }

            public bool Begin()
            {
                return true;
            }
        }

        class MockAddNodeQueryCancel : IAddNodeQuery
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Demand { get; set; }

            public bool Begin()
            {
                X = 1.57;
                Y = 5.23;
                Demand = 3.7;
                return false;
            }
        }

        [TestMethod]
        public void AddNodeCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.AddNodeQuery = new MockAddNodeQueryOk(3.4, 5.52, 0.38);
            vm.Segments.Add(new Segment());
            vm.AddNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 1);
            Assert.IsTrue(vm.Record.Nodes[0].X == 3.4);
            Assert.IsTrue(vm.Record.Nodes[0].Y == 5.52);
            Assert.IsTrue(vm.Record.Nodes[0].Demand == 0.38);
            Assert.IsTrue(vm.CurrentNode == vm.Record.Nodes[0]);
            Assert.IsTrue(vm.Segments.Count == 0);

            vm.AddNodeQuery = new MockAddNodeQueryOk(0.7, 0.8, 1.01);
            vm.AddNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 2);
            Assert.IsTrue(vm.Record.Nodes[0].X == 3.4);
            Assert.IsTrue(vm.Record.Nodes[0].Y == 5.52);
            Assert.IsTrue(vm.Record.Nodes[0].Demand == 0.38);
            Assert.IsTrue(vm.Record.Nodes[1].X == 0.7);
            Assert.IsTrue(vm.Record.Nodes[1].Y == 0.8);
            Assert.IsTrue(vm.Record.Nodes[1].Demand == 1.01);
            Assert.IsTrue(vm.CurrentNode == vm.Record.Nodes[1]);

            vm.AddNodeQuery = new MockAddNodeQueryCancel();
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.AddNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 2);
            Assert.IsTrue(vm.Record.Nodes[0].X == 3.4);
            Assert.IsTrue(vm.Record.Nodes[0].Y == 5.52);
            Assert.IsTrue(vm.Record.Nodes[0].Demand == 0.38);
            Assert.IsTrue(vm.Record.Nodes[1].X == 0.7);
            Assert.IsTrue(vm.Record.Nodes[1].Y == 0.8);
            Assert.IsTrue(vm.Record.Nodes[1].Demand == 1.01);
            Assert.IsTrue(vm.CurrentNode == vm.Record.Nodes[1]);
            Assert.IsTrue(vm.Segments.Count == 2);
        }

        [TestMethod]
        public void MouseAddNodeTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.CanvasWidth = 100;
            vm.CanvasHeight = 50;
            vm.MouseX = 25;
            vm.MouseY = 20;
            GlobalData.MaxNodeX = 30;
            GlobalData.MaxNodeY = 20;
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.MouseAddNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 1);
            Assert.IsTrue(vm.Record.Nodes[0].X == 30.0 / 4);
            Assert.IsTrue(vm.Record.Nodes[0].Y == 20 * 0.4);
            Assert.IsTrue(vm.CurrentNode == vm.Record.Nodes[0]);
            Assert.IsTrue(vm.Segments.Count == 0);
        }

        class MockEditNodeQueryOk : IEditNodeQuery
        {
            public double NewX { get; set; }
            public double NewY { get; set; }
            public double NewDemand { get; set; }

            public bool Begin(Node currentNode)
            {
                NewX = 3.27;
                NewY = 4.89;
                NewDemand = 0.03;
                return true;
            }
        }

        class MockEditNodeQueryCancel : IEditNodeQuery
        {
            public double NewX { get; set; }
            public double NewY { get; set; }
            public double NewDemand { get; set; }

            public bool Begin(Node currentNode)
            {
                NewX = 0.56;
                NewY = 7.63;
                NewDemand = 8.12;
                return false;
            }
        }

        [TestMethod]
        public void EditNodeCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.EditNodeQuery = new MockEditNodeQueryOk();
            vm.Record.Nodes.Add(new Node { X = 0.2, Y = 0.3, Demand = 0.4 });
            vm.CurrentNode = vm.Record.Nodes[0];
            vm.Segments.Add(new Segment());
            vm.EditNodeCommand.Execute(null);
            Assert.IsTrue(vm.Segments.Count == 0);
            Assert.IsTrue(vm.Record.Nodes.Count == 1);
            Assert.IsTrue(vm.Record.Nodes[0].X == 3.27);
            Assert.IsTrue(vm.Record.Nodes[0].Y == 4.89);
            Assert.IsTrue(vm.Record.Nodes[0].Demand == 0.03);
            Assert.IsTrue(vm.CurrentNode == vm.Record.Nodes[0]);

            vm.EditNodeQuery = new MockEditNodeQueryCancel();
            vm.Record.Nodes.Add(new Node { X = 1.1, Y = 1.2, Demand = 1.3 });
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.CurrentNode = vm.Record.Nodes[1];
            vm.EditNodeCommand.Execute(null);
            Assert.IsTrue(vm.Segments.Count == 2);
            Assert.IsTrue(vm.Record.Nodes.Count == 2);
            Assert.IsTrue(vm.Record.Nodes[0].X == 3.27);
            Assert.IsTrue(vm.Record.Nodes[0].Y == 4.89);
            Assert.IsTrue(vm.Record.Nodes[0].Demand == 0.03);
            Assert.IsTrue(vm.CurrentNode == vm.Record.Nodes[1]);
            Assert.IsTrue(vm.Record.Nodes[1].X == 1.1);
            Assert.IsTrue(vm.Record.Nodes[1].Y == 1.2);
            Assert.IsTrue(vm.Record.Nodes[1].Demand == 1.3);
        }

        [TestMethod]
        public void RemoveNodeCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.Record.Nodes.Add(new Node { X = 1, Y = 2, Demand = 3 });
            vm.Record.Nodes.Add(new Node { X = 4, Y = 5, Demand = 6 });
            vm.Record.Nodes.Add(new Node { X = 7, Y = 8, Demand = 9 });

            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());

            vm.CurrentNode = vm.Record.Nodes[0];
            vm.RemoveNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 2);
            Assert.IsTrue(vm.Record.Nodes[0].Equals(new Node { ID = 0, X = 4, Y = 5, Demand = 6 }));
            Assert.IsTrue(vm.Record.Nodes[1].Equals(new Node { ID = 1, X = 7, Y = 8, Demand = 9 }));
            Assert.IsTrue(vm.Segments.Count == 0);

            vm.Record.Nodes.Clear();
            vm.Record.Nodes.Add(new Node { X = 1, Y = 2, Demand = 3 });
            vm.Record.Nodes.Add(new Node { X = 4, Y = 5, Demand = 6 });
            vm.Record.Nodes.Add(new Node { X = 7, Y = 8, Demand = 9 });

            vm.CurrentNode = vm.Record.Nodes[1];
            vm.RemoveNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 2);
            Assert.IsTrue(vm.Record.Nodes[0].Equals(new Node { ID = 0, X = 1, Y = 2, Demand = 3 }));
            Assert.IsTrue(vm.Record.Nodes[1].Equals(new Node { ID = 1, X = 7, Y = 8, Demand = 9 }));

            vm.Record.Nodes.Clear();
            vm.Record.Nodes.Add(new Node { X = 1, Y = 2, Demand = 3 });
            vm.Record.Nodes.Add(new Node { X = 4, Y = 5, Demand = 6 });
            vm.Record.Nodes.Add(new Node { X = 7, Y = 8, Demand = 9 });

            vm.CurrentNode = vm.Record.Nodes[2];
            vm.RemoveNodeCommand.Execute(null);
            Assert.IsTrue(vm.Record.Nodes.Count == 2);
            Assert.IsTrue(vm.Record.Nodes[0].Equals(new Node { ID = 0, X = 1, Y = 2, Demand = 3 }));
            Assert.IsTrue(vm.Record.Nodes[1].Equals(new Node { ID = 1, X = 4, Y = 5, Demand = 6 }));
        }

        [TestMethod]
        public void ClearCommandTest()
        {
            PathProjectPageViewModel vm = new PathProjectPageViewModel();

            vm.Record.Cars.Add(new Car { WeightLimit = 10.2, DisLimit = 72.0 });
            vm.Record.Cars.Add(new Car { WeightLimit = 10.2, DisLimit = 72.0 });
            vm.Record.Cars.Add(new Car { WeightLimit = 10.2, DisLimit = 72.0 });

            vm.Record.Nodes.Add(new Node { X = 1.1, Y = 1.2, Demand = 1.3 });
            vm.Record.Nodes.Add(new Node { X = 1.1, Y = 1.2, Demand = 1.3 });
            vm.Record.Nodes.Add(new Node { X = 1.1, Y = 1.2, Demand = 1.3 });
            vm.Record.Nodes.Add(new Node { X = 1.1, Y = 1.2, Demand = 1.3 });

            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());
            vm.Segments.Add(new Segment());

            vm.ClearCommand.Execute(null);

            Assert.IsTrue(vm.Record.Cars.Count == 0);
            Assert.IsTrue(vm.Record.Nodes.Count == 0);
            Assert.IsTrue(vm.Segments.Count == 0);
        }
    }
}
