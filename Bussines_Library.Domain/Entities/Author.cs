using Bussines_Library.Domain.Common;
using Bussines_Library.Domain.Constants;
using Bussines_Library.Domain.Exceptions;

namespace Bussines_Library.Domain.Entities
{
    public sealed class Author : AuditableEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string FathersSurname { get; private set; }
        public string MothersSurname { get; private set; }
        public string? Nationality { get; private set; }
        public bool IsActive { get; private set; }

        // Navegación con los libros
        private readonly List<Book> _books = new();
        public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

        #region Constructors

        // Existe por el entity framework, para poder mapear la
        // entidad a la base de datos
        private Author()
        {
            Name = string.Empty;
            FathersSurname = string.Empty;
            MothersSurname = string.Empty;
            IsActive = true;
        }

        // El constructor con parámetros es privado para que no se pueda instanciar
        // directamente desde fuera de la clase.
        private Author(Guid id, string name, string fathersSurname, string mothersSurname,
            string? nationality, bool isActive)
        {
            Id = id;
            Name = name;
            FathersSurname = fathersSurname;
            MothersSurname = mothersSurname;
            Nationality = nationality;
            IsActive = isActive;
        }

        #endregion

        #region Methods

        public static Author Create(string name, string fathersSurname, string mothersSurname,
            string? nationality)
        {
            return new Author(Guid.NewGuid(), name, fathersSurname, mothersSurname, nationality, true);
        }

        public void Update(string name, string fathersSurname, string mothersSurname, string? nationality)
        {
            SetName(name);
            SetFathersSurname(fathersSurname);
            SetMothersSurname(mothersSurname);
            SetNationality(nationality);
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        #endregion

        #region Private Methods

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException(AuthorConstants.NAME_NOT_EMPTY, AuthorConstants.NAME_REQUIRED);
            }

            var trimmedName = name.Trim();
            if (trimmedName.Length > AuthorConstants.NAME_MAX_LENGTH)
            {
                throw new DomainException(AuthorConstants.NAME_TOO_LONG, AuthorConstants.NAME_MAX_LENGTH_MESSAGE);
            }

            Name = trimmedName;
        }

        private void SetFathersSurname(string fathersSurname)
        {
            if (string.IsNullOrWhiteSpace(fathersSurname))
            {
                throw new DomainException(AuthorConstants.FATHERS_SURNAME_NOT_EMPTY, AuthorConstants.FATHERS_SURNAME_REQUIRED);
            }

            var trimmedFathersSurname = fathersSurname.Trim();

            if (trimmedFathersSurname.Length > AuthorConstants.FATHERS_SURNAME_MAX_LENGTH)
            {
                throw new DomainException(AuthorConstants.FATHERS_SURNAME_TOO_LONG, AuthorConstants.FATHERS_SURNAME_MAX_LENGTH_MESSAGE);
            }

            FathersSurname = trimmedFathersSurname;
        }

        private void SetMothersSurname(string mothersSurname)
        {
            if (string.IsNullOrWhiteSpace(mothersSurname))
            {
                throw new DomainException(AuthorConstants.MOTHERS_SURNAME_NOT_EMPTY, AuthorConstants.MOTHERS_SURNAME_REQUIRED);
            }

            var trimmedMothersSurname = mothersSurname.Trim();
            
            if (trimmedMothersSurname.Length > AuthorConstants.MOTHERS_SURNAME_MAX_LENGTH)
            {
                throw new DomainException(AuthorConstants.MOTHERS_SURNAME_TOO_LONG, AuthorConstants.MOTHERS_SURNAME_MAX_LENGTH_MESSAGE);
            }

            MothersSurname = trimmedMothersSurname;
        }

        private void SetNationality(string? nationality)
        {
            if (!string.IsNullOrWhiteSpace(nationality))
            {
                var trimmedNationality = nationality.Trim();

                if (trimmedNationality.Length > AuthorConstants.NATIONALITY_MAX_LENGTH)
                {
                    throw new DomainException(AuthorConstants.NATIONALITY_TOO_LONG, AuthorConstants.NATIONALITY_MAX_LENGTH_MESSAGE);
                }

                Nationality = trimmedNationality;
            }
        }

        #endregion
    }
}
