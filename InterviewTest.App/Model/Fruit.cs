using System;

namespace InterviewTest.App.Model
{
    public class Fruit : IProduct
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice
        {
            get { return UnitPrice * Count; }
        }
        public HealthIndex HealthIndex { get; }

        public Fruit(string name, int count, double unitPrice)
        {
            Id = Guid.NewGuid();
            HealthIndex = HealthIndex.Average;
            Name = name;
            Count = count;
            UnitPrice = unitPrice;
        }
    }
}
