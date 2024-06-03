using DataAcessRepository;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestSimetricaConsulting.Filter;
using TestSimetricaConsulting.Model;

namespace TestSimetricaConsulting.Controllers.V1
{
    /// <summary>
    /// Endpoints para el CRUD de actividades con implementación de llamadas a paquete de Oracle.
    /// </summary>
    [ApiController]
    [Authorize]
    [JwtAuthorizationFilter]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AssignmentRepoController : ControllerBase
    {
        private readonly IRepository<Assignment> _repository;

        public AssignmentRepoController(IRepository<Assignment> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtiene todas las actividades.
        /// </summary>
        /// <returns>Una lista de actividades.</returns>
        /// <response code="200">Lista de actividades devuelta</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
            return Ok(await _repository.GetAllAsync());
        }

        /// <summary>
        /// Obtiene una actividad por su ID.
        /// </summary>
        /// <param name="id">ID de la actividad.</param>
        /// <returns>La actividad con el ID especificado.</returns>
        /// <response code="200">Actividad devuelta</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="404">Actividad no encontrada</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignment(int id)
        {
            var assignment = await _repository.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return Ok(assignment);
        }

        /// <summary>
        /// Crea una nueva actividad.
        /// </summary>
        /// <param name="actividad">Datos de la nueva actividad.</param>
        /// <returns>La actividad creada.</returns>
        /// <response code="201">Actividad creada</response>
        /// <response code="400">Datos de la actividad inválidos</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult> CreateAssignment([FromBody] CreateAssignmentRequest assignmentRequest)
        {
            // Validar el modelo
            if (!ModelState.IsValid || string.IsNullOrEmpty(assignmentRequest.Title))
            {
                return BadRequest(ModelState);
            }

            var assignment = new Assignment()
            {
                Title = assignmentRequest.Title,
                CreationDate = assignmentRequest.CreationDate ?? DateTime.UtcNow,
                Description = assignmentRequest.Description,
                DueDate = assignmentRequest.DueDate,
                Status = assignmentRequest.Status,
            };

            await _repository.AddAsync(assignment);
            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
        }

        /// <summary>
        /// Actualiza una actividad existente.
        /// </summary>
        /// <param name="id">ID de la actividad a actualizar. **string**</param>
        /// <param name="actividad">Datos actualizados de la actividad.</param>
        /// <returns>Respuesta sin contenido.</returns>
        /// <response code="204">Actividad actualizada</response>
        /// <response code="400">Datos de la actividad inválidos</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAssignment(int id, UpdateAssignmentRequest assignmentRequest)
        {
            var assignment = await _repository.GetByIdAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(assignmentRequest.Title))
                assignment.Title = assignmentRequest.Title;

            if (!string.IsNullOrEmpty(assignmentRequest.Description))
                assignment.Description = assignmentRequest.Description;

            if (assignmentRequest is not null)
                assignment.DueDate = assignmentRequest.DueDate;

            if (!string.IsNullOrEmpty(assignmentRequest.Status))
                assignment.Status = assignmentRequest.Status;

            if (!string.IsNullOrEmpty(assignmentRequest.Title))
                assignment.Title = assignmentRequest.Title;

            await _repository.UpdateAsync(assignment);
            return NoContent();
        }

        /// <summary>
        /// Elimina una actividad existente.
        /// </summary>
        /// <param name="id">ID de la actividad a eliminar.</param>
        /// <returns>Respuesta sin contenido.</returns>
        /// <response code="204">Actividad eliminada</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="404">Actividad no encontrada</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAssignment(int id)
        {
            var assignment = await _repository.GetByIdAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
