using Bussines_Library.Domain.Common;
using Bussines_Library.Domain.Exceptions;

namespace Bussines_Library.Domain.Entities
{
    public sealed class Book : AuditableEntity
    {
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;
        
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;


        private Book()
        {
            Title = string.Empty;
        }

        private Book(Guid id, string title, string? description)
        {
            Id = id;
            Title = title;
            Description = description;
        }

        public static Book Create(string title, string? description) => new(Guid.NewGuid(), title, description);
        public void Rename(string title) => SetTitle(title);
        public void changeDescription(string? description) => SetDescription(description);

        public void Update(string title, string? description)
        {
            SetTitle(title);
            SetDescription(description);
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title cannot be null or empty.", "Title is required");

            var trimmedTitle = title.Trim();
            if (trimmedTitle.Length > NameMaxLength)
                throw new DomainException($"Title cannot exceed {NameMaxLength} characters.", "Title is too long");

            Title = trimmedTitle;
        }

        private void SetDescription(string? description)
        {
            var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            if (trimmedDescription is { Length: > DescriptionMaxLength })
                throw new DomainException($"Description cannot exceed {DescriptionMaxLength} characters.", "Description is too long");
            Description = trimmedDescription;
        }

    }
}
