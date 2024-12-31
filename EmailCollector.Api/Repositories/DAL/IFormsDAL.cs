using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories.DAL;

public interface IFormsDAL
{
    /// <summary>
    /// Saves a signup form in a single transaction with its different settings and configs.
    /// </summary>
    /// <param name="form">form to save</param>
    /// <returns></returns>
    Task SaveFormWithTransaction(SignupForm form);
}