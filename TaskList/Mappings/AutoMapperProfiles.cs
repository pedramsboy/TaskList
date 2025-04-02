using AutoMapper;
using TaskList.Models.Domain;
using TaskList.Models.DTO;

namespace TaskList.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateTaskListDto, TaskList.Models.Domain.TaskList>();
            CreateMap<UpdateTaskListDto, TaskList.Models.Domain.TaskList>();
            CreateMap<TaskList.Models.Domain.TaskList, TaskListDto>()
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count(t => !t.IsDeleted)));


            CreateMap<CreateTaskItemDto, TaskItem>();
            CreateMap<UpdateTaskItemDto, TaskItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TaskItem, TaskItemDto>();
        }
    }
}
