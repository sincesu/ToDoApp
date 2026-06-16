using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Application.DTOs.User;
using Microsoft.AspNetCore.Http;
using ToDo.Domain.Entities.Users;
using ToDo.Application.Extensions;

using BCryptTool = BCrypt.Net.BCrypt;

namespace ToDo.Application.Services.Users
{
    public class AppUserService : IAppUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AppUser> _appUserRepository;
        private readonly IToDoRepository _toDoRepository;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppUserService(IUnitOfWork unitOfWork
            , IMapper mapper
            , IGenericRepository<AppUser> appUserRepository
            , IToDoRepository toDoRepository
            , ITokenService tokenService
            , IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appUserRepository = appUserRepository;
            _toDoRepository = toDoRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AppUserDto>> GetAllUsersAsync()
        {
            var entity = await _appUserRepository.GetAllAsync();

            var allUsers = _mapper.Map<IEnumerable<AppUserDto>>(entity);

            return allUsers;
        }

        public async Task AddUserAsync(AppUserSaveDto dto)
        {
            if (await _appUserRepository.GetQueryable()
                .AnyAsync(x => x.name == dto.name))
                throw new OverlapException("That username is already in use, choose a different username");

            string hashedPassword = BCryptTool.HashPassword(dto.password);
            var entity = _mapper.Map<AppUser>(dto);

            entity.isDeleted = false;
            entity.role = "User";
            entity.password = hashedPassword;

            await _appUserRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _appUserRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.name == dto.name);
            //firstordefaultasync metotu büyük/küçük harf dikkat etmez
            if (user == null)
                throw new NotFoundException("Username or password incorrect");

            if (user.name != dto.name) // büyük/küçük harf hassasiyeti için
                throw new NotFoundException("Username or password incorrect");

            bool isPasswordValid = BCryptTool.Verify(dto.password, user.password);

            if (isPasswordValid == false)
                throw new NotFoundException("Username or password incorrect");

            var tokenString = _tokenService.CreateToken(user);

            return tokenString;
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
                throw new UnauthorizedAccessException();

            if (await _appUserRepository.GetQueryable().AnyAsync(x => x.name == dto.name))
                throw new OverlapException("This user already exists");

            _mapper.Map(dto, item);

            if (!string.IsNullOrWhiteSpace(dto.password))
                item.password = BCryptTool.HashPassword(dto.password);

            await _appUserRepository.UpdateAsync(item);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            Guid currentUserId = _httpContextAccessor.HttpContext!.User.GetUserId();
            bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var entity = await _appUserRepository.GetOrThrowAsync(id);

            if (isAdmin == false && currentUserId != entity.id)
                throw new UnauthorizedAccessException();

            var relatedToDos = await _toDoRepository.GetQueryable()
                .Where(x => x.AppUserId == id)
                .ToListAsync();

            foreach (var ToDo in relatedToDos)
                ToDo.isDeleted = true;

            entity.isDeleted = true;
            await _unitOfWork.CommitAsync();
        }
    }
}
