
using DataAcessRepository;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestSimetricaConsulting.Controllers.V1;
using TestSimetricaConsulting.Model;

namespace UnitTestSimetricaConsulting
{
    public class AssignmentRepoControllerTests
    {
        private readonly Mock<IRepository<Assignment>> _repositoryMock;
        private readonly AssignmentRepoController _controller;

        public AssignmentRepoControllerTests()
        {
            _repositoryMock = new Mock<IRepository<Assignment>>();
            _controller = new AssignmentRepoController(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAssignments_ReturnsOk()
        {
            // Arrange
            var assignments = new List<Assignment> { new Assignment(), new Assignment() };
            _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(assignments);

            // Act
            var result = await _controller.GetAssignments();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(assignments, okResult.Value);
        }

        [Fact]
        public async Task GetAssignment_WithValidId_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var assignment = new Assignment { Id = id };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(assignment);

            // Act
            var result = await _controller.GetAssignment(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsType<Assignment>(okResult.Value);
            Assert.Equal(assignment, model);
        }

        [Fact]
        public async Task GetAssignment_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            _repositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((Assignment)null);

            // Act
            var result = await _controller.GetAssignment(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAssignment_ValidModel_ReturnsCreated()
        {
            // Arrange
            var assignment = new CreateAssignmentRequest { 
                Title = "Test Assignment", 
                CreationDate = DateTime.UtcNow,
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Status = "Pending" 
            };

            // Act
            var result = await _controller.CreateAssignment(assignment);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(_controller.GetAssignment), createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task CreateAssignment_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var assignment = new CreateAssignmentRequest(); // Modelo inválido

            // Act
            var result = await _controller.CreateAssignment(assignment);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task UpdateAssignment_WithValidModel_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var updateAssignment = new UpdateAssignmentRequest
            {
                Title = "Updated Assignment",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = "In Progress"
            };

            var assignment = new Assignment { Id = id };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(assignment);

            // Act
            var result = await _controller.UpdateAssignment(id, updateAssignment);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAssignment_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            var assignment = new UpdateAssignmentRequest { 
                Title = "Updated Assignment",
                Description = "Updated Description", 
                DueDate = DateTime.UtcNow.AddDays(14), 
                Status = "In Progress" 
            };

            // Act
            var result = await _controller.UpdateAssignment(id, assignment);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAssignment_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            _repositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(new Assignment());

            // Act
            var result = await _controller.DeleteAssignment(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAssignment_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            _repositoryMock.Setup(repo => repo.DeleteAsync(id)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteAssignment(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
