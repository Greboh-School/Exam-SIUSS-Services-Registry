using Microsoft.AspNetCore.Identity;
using School.Shared.Core.Persistence.Models.Entities;

namespace Exam.SIUSS.Services.Registry.Models.Entities;

public class ApplicationPlayer : IdentityUser<Guid>, IEntityBase
{
    /// <summary>
    /// Id for external systems and communication
    /// </summary>
    /// <remarks>The Identity Platform (msn) provides an id but this should only be used internally</remarks>
    public Guid UserId { get; set; }
    
    #region IEntityBase
    
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    #endregion
}