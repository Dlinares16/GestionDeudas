using AutoMapper;
using GestionDeudas.DTO;
using GestionDeudas.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>()
                //.ForMember(dest => dest.UserId, opt => opt.Ignore())
                //.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false));
                //.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                //.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Debt mappings
            CreateMap<Debt, DebtDto>()
                .ForMember(dest => dest.TotalPaid, opt => opt.MapFrom(src =>
                    src.Payments != null ? src.Payments.Sum(p => p.Amount) : 0))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src =>
                    src.Amount - (src.Payments != null ? src.Payments.Sum(p => p.Amount) : 0)));

            CreateMap<CreateDebtDto, Debt>()
                .ForMember(dest => dest.DebtId, opt => opt.Ignore())
                .ForMember(dest => dest.CreditorId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateDebtDto, Debt>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Debt, DebtSummaryDto>()
                .ForMember(dest => dest.CreditorName, opt => opt.MapFrom(src =>
                    src.Creditor != null ? $"{src.Creditor.FirstName} {src.Creditor.LastName}" : ""))
                .ForMember(dest => dest.DebtorName, opt => opt.MapFrom(src =>
                    src.Debtor != null ? $"{src.Debtor.FirstName} {src.Debtor.LastName}" : ""))
                .ForMember(dest => dest.TotalPaid, opt => opt.MapFrom(src =>
                    src.Payments != null ? src.Payments.Sum(p => p.Amount) : 0))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src =>
                    src.Amount - (src.Payments != null ? src.Payments.Sum(p => p.Amount) : 0)))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src =>
                    src.DueDate.HasValue && src.DueDate < DateOnly.FromDateTime(DateTime.Now) && src.Status == "pending"));

            // Payment mappings
            CreateMap<Payment, PaymentDto>();
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Payment, PaymentSummaryDto>()
                .ForMember(dest => dest.DebtDescription, opt => opt.MapFrom(src =>
                    src.Debt != null ? src.Debt.Description : ""))
                .ForMember(dest => dest.CreditorName, opt => opt.MapFrom(src =>
                    src.Debt != null && src.Debt.Creditor != null ? $"{src.Debt.Creditor.FirstName} {src.Debt.Creditor.LastName}" : ""))
                .ForMember(dest => dest.DebtorName, opt => opt.MapFrom(src =>
                    src.Debt != null && src.Debt.Debtor != null ? $"{src.Debt.Debtor.FirstName} {src.Debt.Debtor.LastName}" : ""));

            // Friendship mappings
            CreateMap<Friendship, FriendshipDto>();
            CreateMap<CreateFriendshipDto, Friendship>()
                .ForMember(dest => dest.FriendshipId, opt => opt.Ignore())
                .ForMember(dest => dest.RequesterId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateFriendshipDto, Friendship>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Friendship, FriendshipRequestDto>()
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src =>
                    src.Requester != null ? $"{src.Requester.FirstName} {src.Requester.LastName}" : ""))
                .ForMember(dest => dest.RequesterEmail, opt => opt.MapFrom(src =>
                    src.Requester != null ? src.Requester.Email : ""));
        }
    }
}
