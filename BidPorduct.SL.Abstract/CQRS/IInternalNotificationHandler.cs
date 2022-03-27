using MediatR;

namespace BidProduct.SL.Abstract.CQRS
{
    public interface IInternalNotificationHandler<in TNotification> : INotificationHandler<TNotification>
        where TNotification : IInternalNotification
    {
    }
}