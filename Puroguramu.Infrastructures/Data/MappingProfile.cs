using AutoMapper;
using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SchoolMember, IdentitySchoolMember>();
        CreateMap<IdentitySchoolMember, SchoolMember>();

        CreateMap<Student, IdentityStudent>()
            .ForMember(dest => dest.LabGroupId, opt => opt.MapFrom(src => src.LabGroup.IdGroup));

        CreateMap<IdentityStudent, Student>();
        CreateMap<Teacher, IdentityTeacher>();
        CreateMap<IdentityTeacher, Teacher>();
        CreateMap<GroupLab, EntityGroupLab>();
        CreateMap<EntityGroupLab, GroupLab>();
        CreateMap<Lesson, EntityLesson>();
        CreateMap<EntityLesson, Lesson>();

        CreateMap<Exercise, EntityExercise>()
            .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.Lesson.LessonId));

        CreateMap<EntityExercise, Exercise>();

        CreateMap<ExerciseAttempt, EntityExerciseAttempt>()
            .ForMember(dest => dest.ExerciseId, opt => opt.MapFrom(src => src.Exercise.ExerciseId));

        CreateMap<EntityExerciseAttempt, ExerciseAttempt>();
    }
}
