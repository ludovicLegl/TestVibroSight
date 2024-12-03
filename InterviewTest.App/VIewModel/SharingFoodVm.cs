using InterviewTest.App.Model;
using InterviewTest.App.Service;
using InterviewTest.App.ViewModel.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InterviewTest.App.ViewModel
{
    public class SharingFoodVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly List<IProduct> _products = [];
        private readonly IProductStore _productStore;
        public ObservableCollection<string> Types { get; set; }
        public ICommand AddProductCommand { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        private double _unitPrice;
        public double UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged(nameof(UnitPrice));
                }
            }
        }
        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
            }
        }





        public SharingFoodVm()
        {
            _productStore = ServiceProvider.Instance.ProductStore;
            //TODO
            _products.AddRange(_productStore.GetProducts());
            _productStore.ProductAdded += _productStore_ProductAdded;
            _productStore.ProductRemoved += _productStore_ProductRemoved;
            Types = new ObservableCollection<string> { "Fruit", "Vegetable" };


            AddProductCommand = new AsyncRelayCommand(AddProduct);
        }

        private void _productStore_ProductAdded(IProduct obj)
        {
            _products.Add(obj);
            RefreshProducts();
        }
        private void _productStore_ProductRemoved(Guid obj)
        {
            IProduct possibleProduct = _products.FirstOrDefault(p => p.Id == obj);
            if (possibleProduct != null)
            {
                _products.Remove(possibleProduct);
                RefreshProducts();
            }
        }

        private async Task AddProduct()
        {

            IProduct p = SelectedType == "Vegetable" ? new Vegetable(Name, Quantity, UnitPrice) : new Fruit(Name, Quantity, UnitPrice);
            await Task.Run(() => _productStore.AddProduct(p));

        }
        private void RefreshProducts()
        {
            /*   _productList.Items.Clear();
               foreach (IProduct product in _products)
               {
                   _productList.Items.Add(product);
               }*/
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<ProductAvailabilityChecker> checkers = new List<ProductAvailabilityChecker>();
            List<Thread> t = new List<Thread>();
            foreach (IProduct p in _products)
            {
                ProductAvailabilityChecker productAvailabilityChecker = new ProductAvailabilityChecker(p);
                checkers.Add(productAvailabilityChecker);
                Thread thread = new Thread(productAvailabilityChecker.CheckIfAvailable);
                t.Add(thread);
                thread.Start();
            }
            foreach (Thread thread in t)
            {
                thread.Join();
            }

            StringBuilder sb = new StringBuilder();
            bool anyError = false;
            foreach (ProductAvailabilityChecker checker in checkers)
            {
                if (!checker.Result)
                {
                    anyError = true;
                    sb.AppendLine("The product " + checker.Product.Name + " is not available");
                }
            }
            if (!anyError)
            {
                //     MessageBox.Show(this, "Everything is available.");
            }
            else
            {
                //     MessageBox.Show(this, sb.ToString());
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
