#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BidProduct.API/BidProduct.API.csproj", "BidProduct.API/"]
COPY ["BidProduct.SL/BidProduct.SL.csproj", "BidProduct.SL/"]
COPY ["BidProduct.SL.Models/BidProduct.SL.Models.csproj", "BidProduct.SL.Models/"]
COPY ["BidPorduct.SL.Abstract/BidProduct.SL.Abstract.csproj", "BidPorduct.SL.Abstract/"]
COPY ["BidProduct.DAL.Models/BidProduct.DAL.Models.csproj", "BidProduct.DAL.Models/"]
COPY ["BidProduct.DAL.Abstract/BidProduct.DAL.Abstract.csproj", "BidProduct.DAL.Abstract/"]
COPY ["BidProduct.Common/BidProduct.Common.csproj", "BidProduct.Common/"]
COPY ["BidProduct.DAL/BidProduct.DAL.csproj", "BidProduct.DAL/"]
RUN dotnet restore "BidProduct.API/BidProduct.API.csproj"
COPY . .
WORKDIR "/src/BidProduct.API"
RUN dotnet build "BidProduct.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BidProduct.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BidProduct.API.dll"]