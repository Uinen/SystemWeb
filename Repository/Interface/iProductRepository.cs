using System;
using System.Collections.Generic;
using SystemWeb.Models;

namespace SystemWeb.Repository.Interface
{
    public interface iProductRepository : IDisposable
    {
        /// <summary>
        /// GET ALL FUEL PRODUCTS 
        /// </summary>
        /// <returns>It return all products placed in the Database</returns>
        IEnumerable<Product> GetProducts();

        /// <summary>
        /// GET PRODUCTS BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the product by given Id</returns>
        Product GetProductsById(Guid? Id);

        /// <summary>
        /// Insert specified product
        /// </summary>
        /// <param name="insertProduct"></param>
        void InsertProduct(Product insertProduct);

        /// <summary>
        /// Update specified product
        /// </summary>
        /// <param name="updateProduct"></param>
        void UpdateProduct(Product updateProduct);

        /// <summary>
        /// Delete specified product
        /// </summary>
        /// <param name="Id"></param>
        void DeleteProduct(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
