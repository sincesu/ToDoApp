using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Filter;
using ToDo.Application.DTOs.History;
using ToDo.Application.DTOs.ToDo;
using ToDo.Application.Exceptions;
using ToDo.Application.Extensions;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Histories;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Entities.Users;
using ToDo.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace ToDo.Application.Services.ToDo
{
    public class ToDoService : IToDoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToDoRepository _toDoRepository;
        private readonly IGenericRepository<Category> _category;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<AppUser> _appUserRepository;
        private readonly IGenericRepository<TaskHistory> _taskHistoryRepository;

        private readonly IGenericRepository<Comment> _commentRepository; 

        public ToDoService(
            IMapper mapper
            , IUnitOfWork unitOfWork
            , IToDoRepository toDoRepository
            , IGenericRepository<Category> category
            , IHttpContextAccessor httpContextAccessor
            , IGenericRepository<Comment> commentRepository
            , IGenericRepository<AppUser> appUserRepository
            , IGenericRepository<TaskHistory> taskHistoryRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _toDoRepository = toDoRepository;
            _category = category;
            _httpContextAccessor = httpContextAccessor;
            _commentRepository = commentRepository;
            _appUserRepository = appUserRepository;
            _taskHistoryRepository = taskHistoryRepository;
        }

        public async Task<IEnumerable<ToDoItemsDto>> GetItemsAsync(ToDoFilterDto? filter)
        {
            bool isAdmin = _httpContextAccessor.HttpContext!.User.IsInRole("Admin") == true;
            Guid currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

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

        public async Task<ToDoItemsDto?> GetByIdAsync(Guid id)
        {
            bool isAdmin = _httpContextAccessor.HttpContext!.User.IsInRole("Admin") == true;
            Guid currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var query = _toDoRepository.GetQueryable(true)
                .Include(x => x.Attachments)
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

        public async Task AssignTask(AssignTaskDto dto)
        {
            var currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();

            var task = await _toDoRepository.GetQueryable()
            .Include(x => x.AppUser)
            .FirstOrDefaultAsync(x => x.id == dto.ToDoItemsId);

            if (task == null)
                throw new NotFoundException("Task not found task list");

            //kullanıcıya aynı task üst üste tanımlanamasın.
            if (task.AppUserId == dto.AssignedToUserId)
                throw new BadRequestException("The task is already assigned to this user");

            // zaten user yoksa appuserservice içinde exception fırlatacak.
            await _appUserRepository.GetOrThrowAsync(dto.AssignedToUserId);

            var oldState = task.State;

            _mapper.Map(dto, task);
           
            task.State = TaskState.InProgress;
            task.AppUserId = dto.AssignedToUserId;

            var history = CreateHistoryLog(task.id, oldState, task.State, currentUserId);
            await _taskHistoryRepository.AddAsync(history);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<TaskHistoryDto>> GetTaskHistoriesAsync(Guid id)
        {
            var historyList = await _taskHistoryRepository.GetQueryable()
                .Where(x => x.ToDoItemId == id)
                .ToListAsync();

            var dtoList = _mapper.Map<IEnumerable<TaskHistoryDto>>(historyList);

            return dtoList;
        }


        public async Task AddAsync(ToDoItemsSaveDto item)
        {
            Guid currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();

            await _category.GetOrThrowAsync(item.CategoryId);

            var toDoEntity = _mapper.Map<ToDoItems>(item);

            toDoEntity.AppUserId = currentUserId;
            
            toDoEntity.createdDate = DateTime.Now;
            toDoEntity.State = TaskState.Created;

            await _toDoRepository.AddAsync(toDoEntity);
            var history = CreateHistoryLog(toDoEntity.id, TaskState.Created, toDoEntity.State, currentUserId);
            await _taskHistoryRepository.AddAsync(history);
            await _unitOfWork.CommitAsync();
        }
        
        public async Task<IEnumerable<ToDoItemsDto>> GetCompletedItemsAsync(ToDoFilterDto? filter)
        {
            bool isAdmin = _httpContextAccessor.HttpContext!.User.IsInRole("Admin") == true;
            Guid currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

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

        public async Task UpdateState(Guid id, ChangeTaskStateDto item)
        {
            Guid currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var task = await _toDoRepository.GetOrThrowAsync(id);

            if (task.State == item.State)
                throw new BadRequestException("The task is already in this state!");

            if (!isAdmin && task.AppUserId != currentUserId)
                throw new UnAuthorizedAccessException();

            if (!isAdmin && (task.State == TaskState.Cancelled || task.State == TaskState.Inactive || task.State == TaskState.Completed))
                throw new BadRequestException("The status of a canceled, inactive, or completed task cannot be changed again!");

            if (!isAdmin && item.State != TaskState.ReadyForTest)
                throw new UnAuthorizedAccessException("Users are only allowed to change task status to Ready_For_Test");
            
            var oldState = task.State;
            task.State = item.State;

            var history = CreateHistoryLog(id, oldState, task.State, currentUserId);
            await _taskHistoryRepository.AddAsync(history);
            await _unitOfWork.CommitAsync();
        }

        public async Task ToMarkForCompletedAsync(Guid id)
        {
            Guid currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();

            var item = await _toDoRepository.GetOrThrowAsync(id);

            var oldState = item.State;

            item.completedDate = item.isCompleted ? DateTime.Now : null;

            item.State = TaskState.Completed;

            var history = CreateHistoryLog(id, oldState, item.State, currentUserId);
            await _taskHistoryRepository.AddAsync(history);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Guid id, ToDoUpdateDto dto) //buna bilerek state'i eklemiyorum çünkü state için ayrı metot var.
        {
            Guid currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin") == true;

            var item = await _toDoRepository.GetOrThrowAsync(id);

            if (!isAdmin && item.AppUserId != currentUserId)
                throw new UnAuthorizedAccessException($"You are not authorized to update task with id {id}");

            if (dto.CategoryId.HasValue)
                await _category.GetOrThrowAsync(dto.CategoryId.Value);

            _mapper.Map(dto, item);

            await _unitOfWork.CommitAsync();
        }
        
        public async Task DeleteAllCommentsOfTaskAsync(Guid id, bool savechanges = true)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var task = await _toDoRepository.GetQueryable()
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.id == id)
                ?? throw new NotFoundException("Task not found task list");

            foreach (var comment in task.Comments)
                comment.isDeleted = true;

            var oldState = task.State;

            task.State = TaskState.Inactive;
            task.isDeleted = true;

            var history = CreateHistoryLog(id, oldState, task.State, currentUserId);
            await _taskHistoryRepository.AddAsync(history);
            
            if (savechanges)
                await _unitOfWork.CommitAsync();
        }

        private TaskHistory CreateHistoryLog(Guid taskId, TaskState oldState, TaskState newState, Guid userId)
        {
            return new TaskHistory
            {
                id = Guid.NewGuid(),
                ToDoItemId = taskId,
                OldState = oldState,
                NewState = newState,
                ChangedAt = DateTime.Now,
                ChangeByUserId = userId
            };
        }

    }
}
