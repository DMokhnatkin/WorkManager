using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;
using WorkManager.Services.Timers;

namespace WorkManager.Services.Norms
{
    public interface INormService
    {
        Task<Norm> GetNormAsync(Project project);
        Task<NormProgress> GetProgressAsync(Project project, DateTime date);
    }
}
