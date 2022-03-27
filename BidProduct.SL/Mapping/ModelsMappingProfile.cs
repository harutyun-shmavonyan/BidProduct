using System.Reflection;
using AutoMapper;
using BidProduct.Common;
using BidProduct.DAL.Models;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.SL.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            var assemblies = new[]
            {
                Assembly.GetAssembly(typeof(Entity<>)),
                Assembly.GetAssembly(typeof(GetProductQueryResponse))
            };

            var cloneableTypes = assemblies.Where(a => a != null)
                .Select(a => a!.GetTypes().Where(t => t.GetCustomAttribute(typeof(CloneableAttribute)) != null))
                .SelectMany(t => t)
                .ToList();

            cloneableTypes.ForEach(r => CreateMap(r, r));

            CreateMap<Product, GetProductQueryResponse>().ReverseMap();
            CreateMap<Product, CreateProductCommand>().ReverseMap();
            CreateMap<Product, CreateProductCommandResponse>().ReverseMap();
        }
    }
}
