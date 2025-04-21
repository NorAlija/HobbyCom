using HobbyCom.Domain.src.Entities;

namespace HobbyCom.Application.src.DTOs
{

    public abstract class BaseDto
    {
        public Guid Id { get; set; } // Used for Read DTOs
    }

    public abstract class BaseCreateDto<T> where T : BaseEntity
    {
        public abstract T ToEntity(); // Transforms DTO into a new entity
    }

    public abstract class BaseUpdateDto<T> where T : BaseEntity
    {
        public abstract void UpdateEntity(T entity); // Updates an existing entity
    }

    public abstract class BaseReadDto<T> : BaseEntity where T : BaseEntity
    {
        public abstract BaseReadDto<T> FromEntity(T entity); // Transforms an entity into a DTO
    }

}