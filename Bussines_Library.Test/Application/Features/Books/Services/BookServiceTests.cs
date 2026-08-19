using Bussines_Library.Application.Abstractions.Data;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Books.Commads.CreateBook;
using Bussines_Library.Application.Features.Books.Commads.DeleteBook;
using Bussines_Library.Application.Features.Books.Commads.UpdateBook;
using Bussines_Library.Application.Features.Books.Parameters;
using Bussines_Library.Application.Features.Books.Queries.GetBookById;
using Bussines_Library.Application.Features.Books.Queries.GetBooks;
using Bussines_Library.Application.Features.Books.Repositories;
using Bussines_Library.Application.Features.Books.Services;
using Bussines_Library.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Bussines_Library.Test.Application.Features.Books.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly BookService _bookService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IValidator<CreateBookCommand>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateBookCommand>> _updateValidatorMock;
        private readonly Mock<IValidator<GetBooksQuery>> _getBooksValidatorMock;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _createValidatorMock = new Mock<IValidator<CreateBookCommand>>();
            _updateValidatorMock = new Mock<IValidator<UpdateBookCommand>>();
            _getBooksValidatorMock = new Mock<IValidator<GetBooksQuery>>();

            _bookService = new BookService(
                _bookRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _getBooksValidatorMock.Object);
        }

        #region CreateBookAsyncTests
        
        [Fact]
        public async Task CreateBookAsync_ShouldCreateBook_WhenCommandIsValid()
        {
            // Arrange ->
            var command = new CreateBookCommand("Libro de Prueba", "Descripcion de prueba", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));

            // Configurar el comportamiento del validador simulado para que devuelva un resultado de
            // validación exitoso
            _createValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Configurar el comportamiento del repositorio simulado para que devuelva false cuando se verifique
            // la existencia del libro por nombre
            _bookRepositoryMock.Setup(r => r.ExistByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act -> Metodo que quermos validar
            var result = await _bookService.CreateBookAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be(command.Name);
            result.Value.Description.Should().Be(command.Description);

            // Verificar que se haya llamado al método AddAsync del repositorio con un objeto Book y cuantas veces se
            // haya llamado al método SaveChangesAsync del unit of work
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);

            // Verificar que se haya llamado al método SaveChangesAsync del unit of work una vez
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookAsync_ShouldReturnValidationFailure_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateBookCommand("", "Descripción de prueba", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure(nameof(CreateBookCommand.Name), "El nombre del libro es requerido."),
                new ValidationFailure(nameof(CreateBookCommand.Description), "La descripción del libro no puede estar vacía.")
            };

            var ValidationResult = new ValidationResult(validationFailures);

            _createValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ValidationResult);

            // Act
            var result = await _bookService.CreateBookAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);

            result.ValidationErrors.Should().NotBeNull();
            result.ValidationErrors.Should().ContainKey(nameof(CreateBookCommand.Name));
            result.ValidationErrors.Should().ContainKey(nameof(CreateBookCommand.Description));

            _bookRepositoryMock.Verify(
                r => r.ExistByNameAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);

            _bookRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_ShouldReturnFailure_WhenBookAlreadyExists()
        {
            // Arrange
            var command =  new CreateBookCommand("Harry Potter y la Piedra Filosofal", "Descripción de prueba", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));

            _createValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.ExistByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _bookService.CreateBookAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();

            result.Error.Type.Should().Be(ErrorType.Conflict);
            result.Error.Code.Should().Be("Book.AlreadyExists");
            result.Error.Message.Should().Be("Book with the same name already exists");

            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookAsync_ShouldTrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var command = new CreateBookCommand("Libro de Prueba", "Descripción de prueba", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));

            _createValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.ExistByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _bookService.CreateBookAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region GetAllBooksAsyncTests
        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnPagedResponse_WhenQueryIsValid()
        {
            // Arrange
            var query = new GetBooksQuery
            {
                Search = "Harry Potter",
                PageNumber = 1,
                PageSize = 10,
                Sort = "title:asc"
            };

            var pagination = new PaginationRequest(query.PageNumber, query.PageSize);
            var bookQueryParameters = new BookQueryParameters(pagination, query.Search, query.Sort);

            _getBooksValidatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var pagedResponse = new PagedResponse<Book>(new List<Book>(), 1, 10, 0, 0, false, false);
            _bookRepositoryMock.Setup(r => r.GetAllAsync(bookQueryParameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _bookService.GetBooksAsync(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(10);
            result.Value.PageNumber.Should().Be(0);
            result.Value.PageSize.Should().Be(1);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnValidationFailure_WhenQueryIsInvalid()
        {
            // Arrange
            var query = new GetBooksQuery
            {
                Search = "Harry Potter",
                PageNumber = -1,
                PageSize = 10,
                Sort = "title:asc"
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure(nameof(GetBooksQuery.PageNumber), "Page number must be greater than 0.")
            };

            var validationResult = new ValidationResult(validationFailures);

            _getBooksValidatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _bookService.GetBooksAsync(query);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
            result.ValidationErrors.Should().NotBeNull();
            result.ValidationErrors.Should().ContainKey(nameof(GetBooksQuery.PageNumber));
            _bookRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<BookQueryParameters>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        #endregion

        #region GetBookByIdAsyncTests

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = new Guid("84331D9C-8CF3-45E1-890B-CEBD30A2D64B");
            var authorId = new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789");
            var query = new GetBookByIdQuery(bookId);

            var book = Book.Create("Harry Potter", "La historia de un joven brujo.", authorId);

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            // Act
            var result = await _bookService.GetBooksAsync(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be(book.Title);
            result.Value.Description.Should().Be(book.Description);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnFailure_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var query = new GetBookByIdQuery(bookId);

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _bookService.GetBooksAsync(query);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.NotFound);
            result.Error.Code.Should().Be("Book.NotFound");
            result.Error.Message.Should().Be("Book not found");
        }

        #endregion

        #region UpdateBookAsyncTests

        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateBook_WhenCommandIsValid()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(bookId, "Nuevo Título", "Nueva Descripción");
            var existingBook = Book.Create("Título Antiguo", "Descripción Antigua", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);

            _updateValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.ExistByNameAsync(command.Name, bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _bookService.UpdateBookAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be(command.Name);
            result.Value.Description.Should().Be(command.Description);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldReturnFailure_WhenCommandIsInvalid()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(bookId, "", "Nueva Descripción");

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure(nameof(UpdateBookCommand.Name), "El nombre del libro es requerido.")
            };

            var validationResult = new ValidationResult(validationFailures);
            _updateValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _bookService.UpdateBookAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
            result.ValidationErrors.Should().NotBeNull();
            result.ValidationErrors.Should().ContainKey(nameof(UpdateBookCommand.Name));

            _bookRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldTrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(bookId, "Nuevo Título", "Nueva Descripción");
            var existingBook = Book.Create("", "", Guid.Empty);
            
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);

            _updateValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.ExistByNameAsync(command.Name, bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));
            
            // Act
            Func<Task> act = async () => await _bookService.UpdateBookAsync(command);
            
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
        }

        #endregion

        #region DeleteBookAsyncTests

        [Fact]
        public async Task DeleteBookAsync_ShouldDeleteBook_WhenCommandIsValid()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);
            var existingBook = Book.Create("Título", "Descripción", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);
            
            // Act
            var result = await _bookService.DeleteBookAsync(command);
            
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be(existingBook.Title);
            result.Value.Description.Should().Be(existingBook.Description);

            _bookRepositoryMock.Verify(r => r.Delete(existingBook), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldReturnFailure_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _bookService.DeleteBookAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.NotFound);
            result.Error.Code.Should().Be("Book.NotFound");
            result.Error.Message.Should().Be("Book not found");

            _bookRepositoryMock.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldTrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);
            var existingBook = Book.Create("Título", "Descripción", new Guid("D9A1B2C3-4E5F-6789-ABCD-EF0123456789"));

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.Delete(existingBook))
                .Throws(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _bookService.DeleteBookAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion
    }
}
