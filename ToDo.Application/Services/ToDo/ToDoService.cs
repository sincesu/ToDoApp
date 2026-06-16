using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Entities.Categories;
using Microsoft.AspNetCore.Http;
using ToDo.Application.DTOs.ToDo;
using ToDo.Application.DTOs.Filter;
using ToDo.Application.Extensions;
using ToDo.Application.Exceptions;

namespace ToDo.Application.Services.ToDo
{
    public class ToDoService : IToDoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToDoRepository _toDoRepository;
        private readonly IGenericRepository<Category> _category;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ToDoService(
            IMapper mapper
            , IUnitOfWork unitOfWork
            , IToDoRepository toDoRepository
            , IGenericRepository<Category> category
            , IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _toDoRepository = toDoRepository;
            _category = category;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ToDoItemsDto>> GetItemsAsync(ToDoFilterDto? filter) //isadmin ve id kontrolü eklenecek hepsine.
        {
            bool isAdmin = _httpContextAccessor.HttpContext!.User.IsInRole("Admin") == true;
            int currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var query = _toDoRepository.GetToDosWithCategory(filter)
                .Where(x => x.isCompleted == false);

            if (isAdmin)
                query = query.Include(x => x.AppUser);
            else
                query = query.Where(x => x.AppUserId == currentUserId);

            var activeItems = await query.ToListAsync();

            var dtoItems = _mapper.Map<IEnumerable<ToDoItemsDto>>(activeItems);

            return dtoItems;
        }

        public async Task AddAsync(ToDoItemsSaveDto item)
        {
            int currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();

            await _category.GetOrThrowAsync(item.CategoryId);

            var toDoEntity = _mapper.Map<ToDoItems>(item);

            toDoEntity.AppUserId = currentUserId;
            
            toDoEntity.createdDate = DateTime.Now;
            toDoEntity.isCompleted = false;

            await _toDoRepository.AddAsync(toDoEntity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ToDoItemsDto?> GetByIdAsync(int id)
        {
            bool isAdmin = _httpContextAccessor.HttpContext!.User.IsInRole("Admin") == true;
            int currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var query = _toDoRepository.GetQueryable()
                .Where(x => x.id == id);

            if (isAdmin)
                query = query.Include(x => x.AppUser);
            else
                query = query.Where(x => x.AppUserId == currentUserId);

            var item = await query.FirstOrDefaultAsync()
                ?? throw new NotFoundException($"{id} has not found");

            var dtoItems = _mapper.Map<ToDoItemsDto>(item);

            return dtoItems;
        }
        
        public async Task<IEnumerable<ToDoItemsDto>> GetCompletedItemsAsync(ToDoFilterDto? filter)
        {
            bool isAdmin = _httpContextAccessor.HttpContext!.User.IsInRole("Admin") == true;
            int currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var query = _toDoRepository.GetToDosWithCategory(filter)
                .Where(x => x.isCompleted == true);

            if (isAdmin)
                query = query.Include(x => x.AppUser);
            else
                query = query.Where(x => x.AppUserId == currentUserId);

            var completedItems = await query.ToListAsync();

            var dtoItems = _mapper.Map<IEnumerable<ToDoItemsDto>>(completedItems);

            return dtoItems;
        }

        public async Task ToMarkAsync(int id)
        {
            int currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin") == true;

            var item = await _toDoRepository.GetOrThrowAsync(id);

            if (!isAdmin && currentUserId != item.AppUserId)
                throw new UnauthorizedAccessException();

            item.isCompleted = !item.isCompleted;

            item.completedDate = item.isCompleted ? DateTime.Now : null;

            await _toDoRepository.UpdateAsync(item);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(int id, ToDoUpdateDto dto)
        {
            int currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin") == true;

            var item = await _toDoRepository.GetOrThrowAsync(id);

            if (!isAdmin && item.AppUserId != currentUserId)
                throw new UnauthorizedAccessException();

            if (dto.CategoryId.HasValue)
                await _category.GetOrThrowAsync(dto.CategoryId.Value);

            _mapper.Map(dto, item);

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            int currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin") == true;

            var item = await _toDoRepository.GetOrThrowAsync(id);

            if (!isAdmin && currentUserId != item.AppUserId)
                throw new UnauthorizedAccessException();

            item.isDeleted = true;
            await _unitOfWork.CommitAsync();
        }
    }
}
