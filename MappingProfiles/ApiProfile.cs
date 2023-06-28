using AutoMapper;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.DTOs;

namespace Document_Extractor.MappingProfiles;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        /* Read only dataset  */
        CreateMap<AppUser, AppUserDTO>().ReverseMap();
        CreateMap<AppRole, AppRoleDTO>().ReverseMap();
        CreateMap<Patient, PatientDTO>().ReverseMap();
        CreateMap<Patient, PatientTemp>().ReverseMap();
        CreateMap<PatientTemp, PatientTempDTO>().ReverseMap();
        CreateMap<PatientTemp, ExtractDTO>().ReverseMap();
        CreateMap<Team, TeamDTO>().ReverseMap();
        CreateMap<AppConstant, AppConstantDTO>().ReverseMap();
        CreateMap<UserType, UserTypeDTO>().ReverseMap();

        // CreateMap<Employee, EditEmployeeModel>()
        //         .ForMember(dest => dest.ConfirmEmail,
        //                    opt => opt.MapFrom(src => src.Email));
        // CreateMap<EditEmployeeModel, Employee>();


    }
}