//Создайте коллекцию, в которую можно добавлять покупателей и категорию приобретенной ими
//продукции. Из коллекции можно получать категории товаров, которые купил покупатель или по
//категории определить покупателей. 
using System.Collections;


namespace SystemCollections
{
    interface ISetPropertiesForProduct
    {
        ISetPropertiesForProduct SetCheap();
        ISetPropertiesForProduct SetMedium();
        ISetPropertiesForProduct SetExpensive();
        ISetPropertiesForProduct SetPrice(decimal cost);
        ISetPropertiesForProduct SetAutomaticPrice(decimal cost);
    }
    class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Product[] Product { get; set; }
    }
    class Product : ISetPropertiesForProduct
    {
        private decimal _price;
        public string Name { get; set; }

        public decimal Price { get { return _price; } }
        protected string type = "Unknown";
        public string Type { get { return type; } }
        decimal _maxExpensivePrice = 5000;
        decimal _maxCheapPrice = 1000;
        public decimal MaxExpensivePrice
        {
            get
            {
                return _maxExpensivePrice;
            }
            set
            {
                if (value < _maxCheapPrice)
                {
                    throw new Exception("expensive cost cant be less than cheap one");
                }
                _maxExpensivePrice = value;
            }
        }
        public decimal MaxCheapPrice
        {
            get
            {
                return _maxCheapPrice;
            }
            set
            {
                if (value > _maxExpensivePrice)
                {
                    throw new Exception("cheap cost cant be more thn expensive one");
                }
                _maxCheapPrice = value;
            }
        }

        public ISetPropertiesForProduct SetCheap()
        {
            if (_price >= _maxCheapPrice)
            {
                _price = _maxCheapPrice;
            }
            type = "Cheap";
            return this;
        }

        public ISetPropertiesForProduct SetExpensive()
        {
            if (_price <= _maxExpensivePrice)
            {
                _price = _maxExpensivePrice;
            }
            type = "Expensive";
            return this;
        }

        public ISetPropertiesForProduct SetMedium()
        {
            if (_price > _maxExpensivePrice - 1)
            {
                _price = _maxExpensivePrice - 1;
            }
            else if (_price < _maxCheapPrice + 1)
            {
                _price = _maxCheapPrice  + 1;
            }
            type = "Medium";
            return this;
        }

        public ISetPropertiesForProduct SetPrice(decimal price)
        {
            if (type == "Unknown")
            {
                throw new Exception("Type was unkown, you are not allowed to put prices with unknown type");
            }
            else if (type == "Expensive" && price <= _maxExpensivePrice)
            {
                throw new Exception($"cost must be more than or equaled {_maxExpensivePrice} with expensive type");
            }
            else if (type == "Medium" && price < _maxCheapPrice + 1 && price > _maxExpensivePrice - 1)
            {
                throw new Exception($"cost must be between {_maxCheapPrice + 1} and {_maxExpensivePrice - 1} with medium type");
            }
            else if(type == "Cheap" && price >= _maxCheapPrice)
            {
                throw new Exception($"cost must be less than or equaled {_maxCheapPrice} with cheap type");
            }
            _price = Math.Abs(price);
            return this;
        }

        public ISetPropertiesForProduct SetAutomaticPrice(decimal price)
        {
            if (price <= _maxCheapPrice)
            {
                type = "Cheap";
            }
            else if (price > _maxCheapPrice + 1 && price <= _maxExpensivePrice - 1)
            {
                type = "Medium";
            }
            else
            {
                type = "Expensive";
            }
            _price = price;
            return this;
        }
    }
    class CustomerCollection : Product, IEnumerable
    {
        private Customer[] _customers = new Customer[0];
        public int CustomerCount { get { return _customers.Length; } }



        public IEnumerator GetEnumerator()
        {
            return _customers.GetEnumerator();
        }


        public ISetPropertiesForProduct AddCustomer(Product[] products , string customerName = "Unkown", string customerEmail = "Unknown")
        {

            Customer[] temp = new Customer[_customers.Length + 1];
            _customers.CopyTo(temp, 0);
            temp[^1] = new Customer() { Name = customerName, Email = customerEmail, Product = products };
            _customers = temp;
            return this;
        }
        public IEnumerable GetCustomers()
        {
            for (int i = 0; i < _customers.Length; i++)
            {
                yield return _customers[i];

            }
        }
        public Product[] GetProductArr(Index i)
        {
            return _customers[i].Product;
        }
        

    }
    public static class ExpandEnumerable
    {
        private static IEnumerable GeneralMethod(this IEnumerable obj, string type)
        {
            Product[] products = obj as Product[];
            for (int i = 0; i < products.Length; i++)
            {
                if (products[i].Type == type)
                {
                    yield return products[i];
                }
            }
        }
        public static IEnumerable GetExpensives(this IEnumerable obj)
        {
            return GeneralMethod(obj, "Expensive");                                
        }
        public static IEnumerable GetMedium(this IEnumerable obj)
        {
            return GeneralMethod(obj, "Medium");
        }
        public static IEnumerable GetCheap(this IEnumerable obj)
        {
            
            return GeneralMethod(obj, "Cheap");
        }
    }
    class Program
    {
        public static void Main()
        {
            Product[] products = new Product[12];
            Random random = new Random();
            Product[][] productsOfProducts = new Product[10][];

            for (int i = 0; i < productsOfProducts.Length; i++)
            {
                for (int j = 0, randNumber; j < products.Length; j++)
                {
                    randNumber = random.Next(0, 3);
                    products[j] = new Product();
                    if (randNumber == 0)
                    {
                        products[j].Name = "Pasta";
                        products[j].SetCheap().SetPrice(10);
                    }
                    else if (randNumber == 1)
                    {
                        products[j].Name = "Phone";
                        products[j].SetMedium().SetPrice(2599.3123m);
                    }
                    else if(randNumber == 2)
                    {
                        products[j].Name = "Car";
                        products[j].SetExpensive().SetPrice(1203003.1233m);
                    }
                }
                productsOfProducts[i] = products;
            }
            CustomerCollection customers = new CustomerCollection();
            for (int i = 0; i < 10; i++)
            {
                customers.AddCustomer(productsOfProducts[i], "Bebra", "ilestrn@gmail.com");
            }
            foreach(Customer customer in customers)
            {
                Console.WriteLine(customer.Name);
                foreach (Product product in customer.Product.GetExpensives())
                {
                    Console.WriteLine($"\t{product.Name}, {product.Price}");
                }
            }

        }
    }
}