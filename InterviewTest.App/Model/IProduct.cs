using System;

namespace InterviewTest.App.Model
{
    public interface IProduct
    {
        Guid Id { get; }
        string Name { get; set; }
        int Count { get; set; }
        double UnitPrice { get; set; }
        double TotalPrice { get; }
        HealthIndex HealthIndex { get; }
    }
}
