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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        /// <summary>
        /// Obtiene todos los pagos del usuario actual
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetMyPayments()
        {
            try
            {
                var userId = GetCurrentUserId();
                var payments = await _paymentService.GetPaymentsByUserAsync(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un pago por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!await _paymentService.UserCanAccessPaymentAsync(userId, id))
                    return Forbid();

                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                    return NotFound(new { message = "Pago no encontrado" });

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los pagos de una deuda específica
        /// </summary>
        [HttpGet("debt/{debtId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByDebt(Guid debtId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByDebtAsync(debtId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo pago
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var payment = await _paymentService.CreatePaymentAsync(createPaymentDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
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
        /// Elimina un pago
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!await _paymentService.UserCanAccessPaymentAsync(userId, id))
                    return Forbid();

                var success = await _paymentService.DeletePaymentAsync(id);
                if (!success)
                    return NotFound(new { message = "Pago no encontrado" });

                return Ok(new { message = "Pago eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el total pagado para una deuda
        /// </summary>
        [HttpGet("debt/{debtId}/total")]
        public async Task<ActionResult<decimal>> GetTotalPaidForDebt(Guid debtId)
        {
            try
            {
                var totalPaid = await _paymentService.GetTotalPaidForDebtAsync(debtId);
                return Ok(new { totalPaid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el resumen de pagos del usuario actual
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<PaymentSummaryDto>>> GetMySummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _paymentService.GetPaymentsSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Valida si un monto de pago es válido para una deuda
        /// </summary>
        [HttpPost("validate-amount")]
        public async Task<ActionResult<bool>> ValidatePaymentAmount([FromBody] ValidatePaymentDto validateDto)
        {
            try
            {
                var isValid = await _paymentService.ValidatePaymentAmountAsync(validateDto.DebtId, validateDto.Amount);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }

    public class ValidatePaymentDto
    {
        public Guid DebtId { get; set; }
        public decimal Amount { get; set; }
    }
}
