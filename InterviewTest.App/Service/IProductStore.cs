using InterviewTest.App.Model;
using System;
using System.Collections.Generic;

namespace InterviewTest.App.Service
{
    public interface IProductStore
    {
        IEnumerable<IProduct> GetProducts();
        void AddProduct(IProduct product);
        void RemoveProduct(Guid productId);

        //Let's assume we cannot update a product
        event Action<IProduct> ProductAdded;
        event Action<Guid> ProductRemoved;
    }
}
