﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Domain.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
