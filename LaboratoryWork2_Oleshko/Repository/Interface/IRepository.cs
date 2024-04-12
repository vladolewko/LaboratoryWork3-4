using LaboratoryWork2_Oleshko.Models;
using System;
using System.Collections.Generic;

namespace LaboratoryWork2_Oleshko.Repository.Interface
{
    public interface IRepository<T> where T : EnvironmentObject
    {
        List<T> GetAll(Func<T, bool> filter = null);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        T GetByFilter(Func<T, bool> filter);
    }
}
