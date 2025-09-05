using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionDeudas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DebtsController : ControllerBase
    {
        private readonly IDebtService _debtService;

        public DebtsController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        /// <summary>
        /// Obtiene todas las deudas del usuario actual
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DebtDto>>> GetMyDebts()
        {
            try
            {
                var userId = GetCurrentUserId();
                var debts = await _debtService.GetDebtsByUserAsync(userId);
                return Ok(debts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una deuda por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DebtDto>> GetDebtById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!await _debtService.UserCanAccessDebtAsync(userId, id))
                    return Forbid();

                var debt = await _debtService.GetDebtByIdAsync(id);
                if (debt == null)
                    return NotFound(new { message = "Deuda no encontrada" });

                return Ok(debt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las deudas donde el usuario actual es acreedor
        /// </summary>
        [HttpGet("as-creditor")]
        public async Task<ActionResult<IEnumerable<DebtDto>>> GetDebtsAsCreditor()
        {
            try
            {
                var userId = GetCurrentUserId();
                var debts = await _debtService.GetDebtsByCreditorAsync(userId);
                return Ok(debts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las deudas donde el usuario actual es deudor
        /// </summary>
        [HttpGet("as-debtor")]
        public async Task<ActionResult<IEnumerable<DebtDto>>> GetDebtsAsDebtor()
        {
            try
            {
                var userId = GetCurrentUserId();
                var debts = await _debtService.GetDebtsByDebtorAsync(userId);
                return Ok(debts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las deudas pendientes del usuario actual
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<DebtDto>>> GetMyPendingDebts()
        {
            try
            {
                var userId = GetCurrentUserId();
                var allDebts = await _debtService.GetDebtsByUserAsync(userId);
                var pendingDebts = allDebts.Where(d => d.Status == "pending");
                return Ok(pendingDebts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las deudas vencidas del usuario actual
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<DebtDto>>> GetMyOverdueDebts()
        {
            try
            {
                var userId = GetCurrentUserId();
                var allDebts = await _debtService.GetDebtsByUserAsync(userId);
                var today = DateOnly.FromDateTime(DateTime.Now);
                var overdueDebts = allDebts.Where(d => d.Status == "pending" && d.DueDate.HasValue && d.DueDate < today);
                return Ok(overdueDebts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva deuda
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DebtDto>> CreateDebt([FromBody] CreateDebtDto createDebtDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var creditorId = GetCurrentUserId();
                var debt = await _debtService.CreateDebtAsync(creditorId, createDebtDto);

                return CreatedAtAction(nameof(GetDebtById), new { id = debt.DebtId }, debt);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una deuda
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DebtDto>> UpdateDebt(Guid id, [FromBody] UpdateDebtDto updateDebtDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();

                if (!await _debtService.UserCanAccessDebtAsync(userId, id))
                    return Forbid();

                var updatedDebt = await _debtService.UpdateDebtAsync(id, updateDebtDto);
                if (updatedDebt == null)
                    return NotFound(new { message = "Deuda no encontrada" });

                return Ok(updatedDebt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una deuda
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDebt(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!await _debtService.UserCanAccessDebtAsync(userId, id))
                    return Forbid();

                var success = await _debtService.DeleteDebtAsync(id);
                if (!success)
                    return NotFound(new { message = "Deuda no encontrada" });

                return Ok(new { message = "Deuda eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Marca una deuda como pagada
        /// </summary>
        [HttpPatch("{id}/mark-paid")]
        public async Task<IActionResult> MarkDebtAsPaid(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!await _debtService.UserCanAccessDebtAsync(userId, id))
                    return Forbid();

                var success = await _debtService.MarkDebtAsPaidAsync(id);
                if (!success)
                    return NotFound(new { message = "Deuda no encontrada" });

                return Ok(new { message = "Deuda marcada como pagada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una deuda
        /// </summary>
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelDebt(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!await _debtService.UserCanAccessDebtAsync(userId, id))
                    return Forbid();

                var success = await _debtService.CancelDebtAsync(id);
                if (!success)
                    return NotFound(new { message = "Deuda no encontrada" });

                return Ok(new { message = "Deuda cancelada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el resumen de deudas del usuario actual
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<DebtSummaryDto>>> GetMySummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _debtService.GetDebtsSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el total adeudado al usuario actual
        /// </summary>
        [HttpGet("total-owed-to-me")]
        public async Task<ActionResult<decimal>> GetTotalOwedToMe()
        {
            try
            {
                var userId = GetCurrentUserId();
                var total = await _debtService.GetTotalAmountOwedToUserAsync(userId);
                return Ok(new { totalOwedToMe = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el total que debe el usuario actual
        /// </summary>
        [HttpGet("total-i-owe")]
        public async Task<ActionResult<decimal>> GetTotalIOwe()
        {
            try
            {
                var userId = GetCurrentUserId();
                var total = await _debtService.GetTotalAmountUserOwesAsync(userId);
                return Ok(new { totalIOwe = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}
