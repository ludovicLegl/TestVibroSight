using System;

namespace InterviewTest.App.Model
{
    public class Vegetable : IProduct
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

        public Vegetable(string name, int count, double unitPrice)
        {
            Id = Guid.NewGuid();
            HealthIndex = HealthIndex.Good;
            Name = name;
            Count = count;
            UnitPrice = unitPrice;
        }
    }
}
