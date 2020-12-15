using System;
using Discord.Commands;

namespace Discord.Interactive
{
    public interface ICriteria<T>
    {
        bool Validate(T obj);
    }

}