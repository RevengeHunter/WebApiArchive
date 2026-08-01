using System;
using System.Collections.Generic;
using System.Text;

namespace Bussines_Library.Application.Abstractions.Data
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
