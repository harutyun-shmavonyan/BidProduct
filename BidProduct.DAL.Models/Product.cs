﻿using BidProduct.DAL.Abstract;

namespace BidProduct.DAL.Models;

public record Product : Entity<long>, IHasCreated, IHasModified
{
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}