using AutoMapper;
using Finances.DTOs;
using Finances.Models;

namespace Finances;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserEntity, UserCreateDto>();
        CreateMap<UserCreateDto, UserEntity>();
        CreateMap<UserEntity, UserGetDto>();
        CreateMap<UserGetDto, UserEntity>();

        CreateMap<CategoryEntity, CategoryCreateDto>();
        CreateMap<CategoryCreateDto, CategoryEntity>();
        CreateMap<CategoryEntity, CategoryGetDto>();
        CreateMap<CategoryGetDto, CategoryEntity>();
        
        CreateMap<TransactionEntity, TransactionCreateDto>();
        CreateMap<TransactionCreateDto, TransactionEntity>();
        CreateMap<TransactionEntity, TransactionGetDto>();
        CreateMap<TransactionGetDto, TransactionEntity>();
        
        CreateMap<WalletEntity, WalletCreateDto>();
        CreateMap<WalletCreateDto, WalletEntity>();
        CreateMap<WalletEntity, WalletGetDto>();
        CreateMap<WalletGetDto, WalletEntity>();
    }
}