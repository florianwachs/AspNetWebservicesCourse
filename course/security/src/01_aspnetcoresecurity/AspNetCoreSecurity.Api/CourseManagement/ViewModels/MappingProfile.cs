using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Domain.Domain;
using AutoMapper;

namespace AspNetCoreSecurity.Api.CourseManagement.ViewModels
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, AssignedCourseVm>()
                .ForMember(c => c.CourseId, map => map.MapFrom(s => s.Id))
                .ForMember(c => c.CourseName, map => map.MapFrom(s => s.Name))
                .ForMember(c => c.ProfessorName, map =>
                {
                    map.PreCondition(c => c.Professor != null);
                    map.MapFrom(c => $"{c.Professor.FirstName} {c.Professor.LastName}");
                });
        }
    }
}
