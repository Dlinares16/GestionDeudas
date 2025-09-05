using GestionDeudas.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios.Contrato
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
        Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByDebtAsync(Guid debtId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByUserAsync(Guid userId);
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
        Task<bool> DeletePaymentAsync(Guid paymentId);
        Task<decimal> GetTotalPaidForDebtAsync(Guid debtId);
        Task<IEnumerable<PaymentSummaryDto>> GetPaymentsSummaryAsync(Guid? userId = null);
        Task<bool> UserCanAccessPaymentAsync(Guid userId, Guid paymentId);
        Task<bool> ValidatePaymentAmountAsync(Guid debtId, decimal paymentAmount);
    }
}
