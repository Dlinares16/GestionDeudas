using GestionDeudas.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios.Contrato
{
    public interface IDebtService
    {
        Task<IEnumerable<DebtDto>> GetAllDebtsAsync();
        Task<DebtDto?> GetDebtByIdAsync(Guid debtId);
        Task<IEnumerable<DebtDto>> GetDebtsByCreditorAsync(Guid creditorId);
        Task<IEnumerable<DebtDto>> GetDebtsByDebtorAsync(Guid debtorId);
        Task<IEnumerable<DebtDto>> GetDebtsByUserAsync(Guid userId);
        Task<IEnumerable<DebtDto>> GetPendingDebtsAsync();
        Task<IEnumerable<DebtDto>> GetOverdueDebtsAsync();
        Task<DebtDto> CreateDebtAsync(Guid creditorId, CreateDebtDto createDebtDto);
        Task<DebtDto?> UpdateDebtAsync(Guid debtId, UpdateDebtDto updateDebtDto);
        Task<bool> DeleteDebtAsync(Guid debtId);
        Task<bool> MarkDebtAsPaidAsync(Guid debtId);
        Task<bool> CancelDebtAsync(Guid debtId);
        Task<IEnumerable<DebtSummaryDto>> GetDebtsSummaryAsync(Guid? userId = null);
        Task<decimal> GetTotalAmountOwedToUserAsync(Guid userId);
        Task<decimal> GetTotalAmountUserOwesAsync(Guid userId);
        Task<bool> UserCanAccessDebtAsync(Guid userId, Guid debtId);
    }
}
