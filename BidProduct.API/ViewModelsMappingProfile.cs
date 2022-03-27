using AutoMapper;
using BidProduct.API.ViewModels.Product;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.API
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile()
        {
            CreateMap<GetProductQueryResponse, ProductReadViewModel>();
            CreateMap<CreateProductCommandResponse, ProductReadViewModel>();
            CreateMap<ProductCreateViewModel, CreateProductCommand>();
        }
    }
}
