using AutoMapper;
using TaskList.Domain.DTO;
using TaskList.Domain.Entity;

namespace TaskList.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateTaskListDto, TaskList.Domain.Entity.TaskList>();
            CreateMap<UpdateTaskListDto, TaskList.Domain.Entity.TaskList>();
            CreateMap<TaskList.Domain.Entity.TaskList, TaskListDto>()
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count(t => !t.IsDeleted)));


            CreateMap<CreateTaskItemDto, TaskItem>();
            CreateMap<UpdateTaskItemDto, TaskItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TaskItem, TaskItemDto>();
        }
    }
}
