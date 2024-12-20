﻿using InterviewTest.App.Model;
using InterviewTest.App.Service;
using InterviewTest.App.ViewModel.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InterviewTest.App.ViewModel
{
    public class SharingFoodVm : INotifyPropertyChanged
    {
       
        private const double TOLERANCE = 0.0001;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<IProduct> Products { get; set; }
        public ObservableCollection<string> Types { get; set; }

        private readonly IProductStore _productStore;

        public ICommand AddProductCommand { get; }
        public ICommand CheckAvailabilityCommand { get; }

        private bool _isCheckingAvailability;
        public bool IsCheckingAvailability
        {
            get => _isCheckingAvailability;
            set
            {
                _isCheckingAvailability = value;
                OnPropertyChanged(nameof(IsCheckingAvailability));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
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
                if (Math.Abs(_unitPrice - value) > TOLERANCE)
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
        private ObservableCollection<string> GetProductTypes()
        {
            var productTypes = Assembly.GetExecutingAssembly()
                                       .GetTypes()
                                       .Where(t => t.IsClass && !t.IsAbstract && typeof(IProduct).IsAssignableFrom(t))
                                       .Select(t => t.Name)
                                       .ToList();

            return new ObservableCollection<string>(productTypes);
        }

        public SharingFoodVm()
        {
            _productStore = ServiceProvider.Instance.ProductStore;
            Products = new ObservableCollection<IProduct>(_productStore.GetProducts());
            _productStore.ProductAdded += _productStore_ProductAdded;
            _productStore.ProductRemoved += _productStore_ProductRemoved;

            Types = GetProductTypes();
            IsCheckingAvailability = true;

            AddProductCommand = new AsyncRelayCommand(AddProduct);
            CheckAvailabilityCommand = new AsyncRelayCommand(CheckAvailability);
        }

        private void _productStore_ProductAdded(IProduct product)
        {
            UpdateProductsCollection(() => Products.Add(product));
        }

        private void _productStore_ProductRemoved(Guid idProduct)
        {
            IProduct possibleProduct = Products.FirstOrDefault(p => p.Id == idProduct);
            if (possibleProduct != null)
            {
                UpdateProductsCollection(() => Products.Remove(possibleProduct));
            }
        }
        private void UpdateProductsCollection(Action updateAction)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                updateAction();
                RefreshProducts();
            });
        }
        private void CheckIfProductIsValid()
        {

            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("The product name cannot be empty.");
                return;
            }

            if (Quantity <= 0)
            {
                MessageBox.Show("Quantity must be greater than zero.");
                return;
            }

            if (UnitPrice <= 0)
            {
                MessageBox.Show("Unit price must be greater than zero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedType))
            {
                MessageBox.Show("Please select a product type.");
                return;
            }
        }
        private async Task AddProduct()
        {
            CheckIfProductIsValid();
            IProduct product;
            if (SelectedType == "Vegetable")
            {
                product = new Vegetable(Name, Quantity, UnitPrice);
            }
            else if (SelectedType == "Fruit")
            {
                product = new Fruit(Name, Quantity, UnitPrice);
            }
            else
            {
                MessageBox.Show("Invalid product type.");
                return;
            }
            await Task.Run(() => _productStore.AddProduct(product));

        }

        private void RefreshProducts()
        {
            var products = _productStore.GetProducts();
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private async Task CheckAvailability()
        {
            if (Products == null || !Products.Any())
            {

                MessageBox.Show("No products to check.");
                return;
            }

            IsCheckingAvailability = false;
            bool anyError = false;
            var sb = new StringBuilder();
            try
            {
                var checkers = Products.Select(p => new ProductAvailabilityChecker(p)).ToList();
                var tasks = checkers.Select(async c => await Task.Run(() => c.CheckIfAvailable())).ToList();
                await Task.WhenAll(tasks);

                foreach (var checker in checkers)
                {
                    if (!checker.Result)
                    {
                        anyError = true;
                        sb.AppendLine($"The product {checker.Product.Name} is not available");
                    }
                }
            }
            finally
            {
                IsCheckingAvailability = true;
            }

            if (!anyError)
            {
                MessageBox.Show("Everything is available.");
            }
            else
            {
                MessageBox.Show(sb.ToString());
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
