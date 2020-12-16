using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Discord.Interactive
{
    public interface ICriteria<T>
    {
        Task<bool> ValidateAsync(T obj);
    }

}