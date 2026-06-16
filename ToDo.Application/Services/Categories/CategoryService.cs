using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Domain.Entities.Categories;
using ToDo.Application.DTOs.Category;
using ToDo.Application.Extensions;
using ToDo.Application.Exceptions;

namespace ToDo.Application.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Category> _category;
        private readonly IToDoRepository _toDoRepository;

        public CategoryService(IMapper mapper,
            IUnitOfWork unitOfWork,
            IGenericRepository<Category> category,
            IToDoRepository toDoRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _category = category;
            _toDoRepository = toDoRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _category.GetAllAsync();

            var dto = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return dto;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var selected = await _category.GetOrThrowAsync(id);

            var dto = _mapper.Map<CategoryDto>(selected);

            return dto;
        }

        public async Task AddCategoryAsync(CategorySaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.name))
                throw new BadRequestException("The category name field cannot be left blank");

            if (await _category.GetQueryable()
                .AnyAsync(x => x.name == dto.name))
                throw new OverlapException("This data already exists");

            var entity = _mapper.Map<Category>(dto);

            entity.isDeleted = false;
            await _category.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateCategoryAsync(int id, CategorySaveDto dto)
        {
            var entity = await _category.GetOrThrowAsync(id);

            _mapper.Map(dto, entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var entity = await _category.GetOrThrowAsync(id);

            var relatedToDos = await _toDoRepository.GetQueryable()
                .Where(x => x.CategoryId == id).ToListAsync();

            foreach (var ToDo in relatedToDos)
                ToDo.isDeleted = true;

            entity.isDeleted = true;
            await _unitOfWork.CommitAsync();
        }
    }
}
