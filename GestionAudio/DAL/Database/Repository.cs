/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using DTO;
using DTO.Entity;

namespace DAL.Database
{
    public class Repository<T> where T : BaseEntity
    {
        #region Public Fields

        public static DbApplicationContext Context;

        #endregion Public Fields

        #region Private Fields

        private readonly string _errorMessage = string.Empty;
        private IDbSet<T> _entities;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// On the first time, create a context
        /// </summary>
        public Repository()
        {
            if (Context == null)
                Context = new DbApplicationContext();
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual IQueryable<T> Table => Entities;

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Get entities from context
        /// </summary>
        private IDbSet<T> Entities => _entities ?? (_entities = Context.Set<T>());

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Remove(entity);
                Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(_errorMessage, dbEx);
            }
        }

        /// <summary>
        /// Get an entity by Id
        /// </summary>
        /// <param name="entity"></param>
        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        /// <summary>
        /// Get all entity from database
        /// </summary>
        /// <returns></returns>
        public List<T> GetList()
        {
            return Entities.ToList();
        }

    
        /// <summary>
        /// Update or insert an entity
        /// </summary>
        /// <param name="entity"></param>
        public void AddOrUpdate(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));
                Entities.AddOrUpdate(entity);
                Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(_errorMessage, dbEx);
            }
        }

        #endregion Public Methods
    }
}