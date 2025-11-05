using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Domain.Entities;

public abstract class EntityBase
{
    protected EntityBase()
    {
        id = Guid.NewGuid();
    }
    protected EntityBase(Guid id)
    {
        this.id = id;
    }
    [Key]
    public Guid id { get; set; }
}
