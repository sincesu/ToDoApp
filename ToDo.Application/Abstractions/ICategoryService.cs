using ToDo.Application.DTOs.Category;

namespace ToDo.Application.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        Task<CategoryDto?> GetCategoryByIdAsync(int id);

        Task AddCategoryAsync(CategorySaveDto dto);

        Task UpdateCategoryAsync(int id, CategorySaveDto dto);

        Task DeleteCategoryAsync(int id);
    }
}
