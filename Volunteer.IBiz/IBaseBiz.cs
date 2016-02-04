using System;
using System.Collections.Generic;

namespace WGX.Lean.IBiz
{
    public interface IBaseBiz
    {
        bool HasError
        {
            get;
        }

        Dictionary<string, string> Errors
        {
            get;
            set;
        }

        string ErrorMessage { get; }

        void ParseErrors(Exception e);
    }


    public interface IBaseBiz<T> : IBaseBiz where T: class
    {
        T GetById(long id);

        T Add(T entity);

        T Edit(T entity);

        bool Delete(long id);
    }
}
