using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.User;
using ToDo.Application.Exceptions;
using ToDo.Domain.Entities.Users;
using ToDo.Application.DTOs.Auth;
using BCryptTool = BCrypt.Net.BCrypt;

namespace ToDo.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IGenericRepository<AppUser> _appUserRepository;

        public AuthService( IMapper mapper
            , IUnitOfWork unitOfWork
            , ITokenService tokenService
            , IGenericRepository<AppUser> appUserRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _appUserRepository = appUserRepository;
        }

        public async Task RegisterAsync(AppUserSaveDto dto)
        {
            if (await _appUserRepository.GetQueryable(true)
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

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _appUserRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.name == dto.name)
            ?? throw new NotFoundException("Username or password incorrect");

            if (user.name != dto.name) //büyük_küçük harf kontrolü
                throw new NotFoundException("Username or password incorrect");

            bool isPasswordValid = BCryptTool.Verify(dto.password, user.password);

            if (isPasswordValid == false)
                throw new NotFoundException("Username or password incorrect");

            var JwtToken = _tokenService.CreateToken(user);
            var RefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _unitOfWork.CommitAsync();

            return new TokenResponseDto
            {
                AccessToken = JwtToken,
                RefreshToken = RefreshToken,
            };
        }

        public async Task<TokenResponseDto> RefreshTokenLoginAsync(RefreshTokenDto dto)
        {
            var user = await _appUserRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken)
            ?? throw new NotFoundException("User not found");

            if (user.RefreshTokenExpiryTime < DateTime.Now)
                throw new BadRequestException("Please log in again");

            var JwtToken = _tokenService.CreateToken(user);
            var RefreshToken = _tokenService.CreateRefreshToken();
            
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            user.RefreshToken = RefreshToken;

            await _unitOfWork.CommitAsync();
            return new TokenResponseDto
            {
                AccessToken = JwtToken,
                RefreshToken = RefreshToken,
            };
        }
    }
}
