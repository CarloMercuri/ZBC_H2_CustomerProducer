using System;
using System.Collections.Generic;
using System.Text;

namespace ConsumerProducer
{
    public class ProductBuffer
    {
        private Queue<Product> Products = new Queue<Product>();

        private bool itemsPresent;

        public bool ItemsPresent
        {
            get { return itemsPresent; }
            set { itemsPresent = value; }
        }


        public int CountProductsRemaining()
        {
            return Products.Count;
        }

        public bool TryInsertProduct()
        {
            if(Products.Count < 3)
            {
                Products.Enqueue(new Product());
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetProduct(out Product foundProduct)
        {
            foundProduct = null;

            if (Products.TryDequeue(out Product product))
            {
                itemsPresent = true;
                foundProduct = product;
                return true;
            }
            else
            {
                itemsPresent = false;
                return false;
            }
        }
    }
}
