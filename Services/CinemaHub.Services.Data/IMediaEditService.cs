namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Web.ViewModels.Media;

    public interface IMediaEditService
    {
        Task ApplyEditForApproval(MediaDetailsInputModel edit, string userId, string rootPath);

        Task<IEnumerable<T>> GetEditsForApproval<T>(int page, int count);

        Task<T> GetEditForApproval<T>(string editId);

        Task<int> GetEditsForApprovalCount();

        Task DeleteEdit(string editId);

    }
}
