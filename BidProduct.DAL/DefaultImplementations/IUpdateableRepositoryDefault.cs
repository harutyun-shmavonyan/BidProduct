using Microsoft.EntityFrameworkCore;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Abstract.Repositories;
using BidProduct.DAL.Extensions;
using BidProduct.DAL.Models;

namespace BidProduct.DAL.DefaultImplementations
{
    public interface IUpdateableRepositoryDefault<TEntity> : IUpdateableRepository<TEntity>,
        IEfRepositoryDefault<TEntity> where TEntity : Entity
    {
        TEntity IUpdateableRepository<TEntity>.Update(IChangeTracker<TEntity> changeTracker)
        {
            var entity = changeTracker.Entity;
            var isAttached = Context.TryAttach(entity);

            if (changeTracker.Entity is IHasModified hasModified)
            {
                hasModified.Modified = DateTime.UtcNow;
                changeTracker.Register(hm => ((IHasModified)hm).Modified);
            }

            if (!isAttached)
            {
                entity = Context.Set<TEntity>().Local.Single(e => e.Id == entity.Id || e == entity);

                if (changeTracker.TrackedMode == TrackMode.Include)
                {
                    foreach (var property in changeTracker.Properties)
                    {
                        Context.Entry(entity).Property(property).CurrentValue = property.Compile()(changeTracker.Entity);
                    }
                }
                else
                {
                    //TODO find a solution
                    throw new NotImplementedException("Can not use tracking mode Exclude for already tracked entity");
                }
            }

            var isTrackModeExclude = changeTracker.TrackedMode == TrackMode.Exclude;

            if (isTrackModeExclude)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
            foreach (var property in changeTracker.Properties)
            {
                if (Context.Entry(entity).Property(property).IsModified == false)
                {
                    Context.Entry(entity).Property(property).IsModified = !isTrackModeExclude;
                }
            }

            return entity;
        }

        TEntity IUpdateableRepository<TEntity>.Update(TEntity newEntity)
        {
            var changeTracker = new ChangeTracker<TEntity>(newEntity, TrackMode.Exclude);
            return Update(changeTracker);
        }
    }
}