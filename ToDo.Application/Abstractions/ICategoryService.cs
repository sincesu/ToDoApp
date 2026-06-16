using ToDo.Application.DTOs.Category;

namespace ToDo.Application.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);

        Task AddCategoryAsync(CategorySaveDto dto);

        Task UpdateCategoryAsync(Guid id, CategorySaveDto dto);

        Task DeleteCategoryAsync(Guid id);
    }
}
