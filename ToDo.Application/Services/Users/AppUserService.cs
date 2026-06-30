using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.User;
using ToDo.Application.Exceptions;
using ToDo.Application.Extensions;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Users;
using BCryptTool = BCrypt.Net.BCrypt;

namespace ToDo.Application.Services.Users
{
    public class AppUserService : IAppUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AppUser> _appUserRepository;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IToDoService _toDoService;

        public AppUserService(IUnitOfWork unitOfWork
            , IMapper mapper
            , IGenericRepository<AppUser> appUserRepository
            , ITokenService tokenService
            , IHttpContextAccessor httpContextAccessor
            , IToDoService toDoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appUserRepository = appUserRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _toDoService = toDoService;
        }
            
        public async Task<IEnumerable<AppUserDto>> GetAllUsersAsync()
        {
            var entity = await _appUserRepository.GetAllAsync();

            var allUsers = _mapper.Map<IEnumerable<AppUserDto>>(entity);

            return allUsers;
        }

        public async Task<IEnumerable<UserTasksDto>> GetAllTasksAsync() //admin tüm taskları görmek için todoservice'e gitsin
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var entityList = await _appUserRepository.GetQueryable()
                .Include(x => x.ToDoItems)
                .Where(x => x.id == currentUserId)
                .ToListAsync();

            var dtoList = _mapper.Map<ICollection<UserTasksDto>>(entityList);

            foreach (var user in dtoList)
            {
                foreach (var task in user.Items)
                    task.AppUser = null;
            }

            return dtoList;
        }

        public async Task<AppUserDto> MyProfile()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var entity = await _appUserRepository.GetQueryable()
                .Include(x => x.ToDoItems)
                .FirstOrDefaultAsync(x => x.id == currentUserId);

            if (entity == null)
                throw new NotFoundException("User not found");

            var dto = _mapper.Map<AppUserDto>(entity);
            
            return dto;
        }
        public async Task<AppUserDto> GetByUserIdAsync(Guid id)
        {
            var entity = await _appUserRepository.GetOrThrowAsync(id);

            var dto = _mapper.Map<AppUserDto>(entity);

            return dto;
        }

        public async Task UpdateAsync(Guid id, AppUserUpdateDto dto)
        {
            Guid currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var item = await _appUserRepository.GetOrThrowAsync(id);

            if (isAdmin == false && currentUserId != item.id)
                throw new UnAuthorizedAccessException();

            //İsmi eşleşen ama ID'si GÜNCELLENEN KİŞİ OLMAYAN başka biri var mı
            if (await _appUserRepository.GetQueryable(true)
                .AnyAsync(x => x.name == dto.name && x.id != id))
                throw new OverlapException("This user already exists");

            _mapper.Map(dto, item);

            if (!string.IsNullOrWhiteSpace(dto.password))
                item.password = BCryptTool.HashPassword(dto.password);

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var user = await _appUserRepository.GetQueryable()
            .Include(x => x.ToDoItems)
            .FirstOrDefaultAsync(x => x.id == id);
        
            if (user == null)
                throw new NotFoundException("User not found");

            foreach (var todo in user.ToDoItems)
                await _toDoService.DeleteAllCommentsOfTaskAsync(todo.id, false);

            user.isDeleted = true;
            
            await _unitOfWork.CommitAsync();
        }
    }
}
