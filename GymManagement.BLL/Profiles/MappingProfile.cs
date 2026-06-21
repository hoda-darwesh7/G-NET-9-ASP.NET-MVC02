using AutoMapper;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MemperProfiles();
            SessionProfiles();
        }

        private void MemperProfiles()
        {
            CreateMap<Member, MemberViewModel>()
                 .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));

            CreateMap<HelthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
                });

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
                {
                    BuildingNumber = src.BuildingNumber,
                    City = src.City,
                    Street = src.Street,
                }))
                .ForMember(dest => dest.HelthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));
        }

        private void SessionProfiles()
        {
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Trainer, TrainerSelectViewModel>();
        }
    }
}
