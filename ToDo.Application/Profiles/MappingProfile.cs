using AutoMapper;
using Todo.Domain.Entities;
using ToDo.Application.DTOs.Attachment;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Comment;
using ToDo.Application.DTOs.History;
using ToDo.Application.DTOs.ToDo;
using ToDo.Application.DTOs.User;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Histories;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Entities.Users;

namespace ToDo.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDoItems, ToDoItemsDto>();
            CreateMap<ToDoItemsSaveDto, ToDoItems>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategorySaveDto, Category>();
            CreateMap<AppUser, AppUserDto>();
            CreateMap<AppUserSaveDto, AppUser>();
            CreateMap<CommentSaveDto, Comment>();
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentUpdateDto, Comment>();
            CreateMap<AssignTaskDto, ToDoItems>();
            CreateMap<FileAttachment, AttachmentDto>();
            CreateMap<TaskHistory, TaskHistoryDto>();
            CreateMap<AppUser, UserTasksDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ToDoItems));
            CreateMap<ToDoUpdateDto, ToDoItems>()
            .ForMember(dest => dest.CategoryId, opt => opt.Condition(src => src.CategoryId.HasValue))
            .ForMember(dest => dest.priority, opt => opt.Condition(src => src.priority.HasValue))
            .ForMember(dest => dest.content, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.content)));
            CreateMap<AppUserUpdateDto, AppUser>()
            .ForMember(dest => dest.name, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.name)))
            .ForMember(dest => dest.password, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.password)))
            .ForMember(dest => dest.email, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.email)));
        }
    }
}
