using System;
using System.Collections.Generic;
using SystemWeb.Models;

namespace SystemWeb.Repository.Interface
{
    public interface iUsersImageRepository
    {
        /// <summary>
        /// GET ALL UsersImages
        /// </summary>
        /// <returns>It return all UsersImages placed in the Database</returns>
        IEnumerable<UsersImage> GetUsersImages();

        /// <summary>
        /// GET UsersImages BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the UsersImages by given Id</returns>
        UsersImage GetUsersImagesById(Guid? Id);

        /// <summary>
        /// Insert specified UsersImages
        /// </summary>
        /// <param name="insertUsersImages"></param>
        void InsertUsersImages(UsersImage insertUsersImages);

        /// <summary>
        /// Update specified UsersImages
        /// </summary>
        /// <param name="updateUsersImages"></param>
        void UpdateUsersImages(UsersImage updateUsersImages);

        /// <summary>
        /// Delete specified pv
        /// </summary>
        /// <param name="Id"></param>
        void DeleteUsersImages(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
