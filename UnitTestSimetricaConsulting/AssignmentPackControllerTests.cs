using DataAccessPackage;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestSimetricaConsulting.Controllers.V1;
using TestSimetricaConsulting.Model;

namespace UnitTestSimetricaConsulting
{
    public class AssignmentPackControllerTests
    {
        private readonly Mock<IAssignmentService> _assignmentServiceMock;
        private readonly AssignmentPackController _controller;

        public AssignmentPackControllerTests()
        {
            _assignmentServiceMock = new Mock<IAssignmentService>();
            _controller = new AssignmentPackController(_assignmentServiceMock.Object);
        }

        [Fact]
        public void GetAllAssignments_ReturnsOk()
        {
            // Arrange
            var assignments = new List<Assignment> { new Assignment(), new Assignment() };
            _assignmentServiceMock.Setup(service => service.ReadAllAssignments()).Returns(assignments);

            // Act
            var result = _controller.GetAllAssignments();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Assignment>>(okResult.Value);
            Assert.Equal(assignments, model);
        }

        [Fact]
        public void GetAssignment_WithValidId_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var assignment = new Assignment { Id = id };
            _assignmentServiceMock.Setup(service => service.ReadAssignment(id)).Returns(assignment);

            // Act
            var result = _controller.GetAssignment(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Assignment>(okResult.Value);
            Assert.Equal(assignment, model);
        }

        [Fact]
        public void GetAssignment_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            _assignmentServiceMock.Setup(service => service.ReadAssignment(id)).Returns((Assignment)null);

            // Act
            var result = _controller.GetAssignment(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateAssignment_ValidInput_ReturnsOk()
        {
            // Arrange
            var request = new CreateAssignmentRequest
            {
                Title = "Test Assignment",
                CreationDate = DateTime.Now,
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(7),
                Status = "Pending"
            };

            // Act
            var result = _controller.CreateAssignment(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public void UpdateAssignment_ValidInput_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var request = new UpdateAssignmentRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(14),
                Status = "InProgress"
            };
            var assignment = new Assignment { Id = id };
            _assignmentServiceMock.Setup(service => service.ReadAssignment(id)).Returns(assignment);

            // Act
            var result = _controller.UpdateAssignment(id, request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteAssignment_ExistingId_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var assignment = new Assignment { Id = id };
            _assignmentServiceMock.Setup(service => service.ReadAssignment(id)).Returns(assignment);
            // Act
            var result = _controller.DeleteAssignment(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteAssignment_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = 999;
            _assignmentServiceMock.Setup(repo => repo.DeleteAssignment(id))
                      .Throws(new KeyNotFoundException());

            // Act
            var result = _controller.DeleteAssignment(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
