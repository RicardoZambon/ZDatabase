﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using ZDatabase.EntityFrameworkCore.Common.Services;

namespace ZDatabase.EntityFrameworkCore.Common.ValueGenerators
{
    /// <summary>
    /// Generates the current user for properties when an entity is added to a context.
    /// </summary>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    /// <seealso cref="Microsoft.EntityFrameworkCore.ValueGeneration.ValueGenerator{TUserKey}" />
    public class CurrentUserGenerator<TUserKey>
        : ValueGenerator<TUserKey>
        where TUserKey : struct
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override TUserKey Next(EntityEntry entry)
        {
            return entry.Context.GetService<ICurrentUserProvider<TUserKey>>()?.CurrentUserID ?? default;
        }
    }
}