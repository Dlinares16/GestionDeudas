using AutoMapper;
using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DAL.Repositorios.Contrato;
using GestionDeudas.DTO;
using GestionDeudas.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios
{
    public class DebtService : IDebtService
    {
        private readonly IGenericRepository<Debt> _debtRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public DebtService(IGenericRepository<Debt> debtRepository, IGenericRepository<User> userRepository, IMapper mapper)
        {
            _debtRepository = debtRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DebtDto>> GetAllDebtsAsync()
        {
            var debts = await (await _debtRepository.Consultar())
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtDto>>(debts);
        }

        public async Task<DebtDto?> GetDebtByIdAsync(Guid debtId)
        {
            var debt = await (await _debtRepository.Consultar(d => d.DebtId == debtId))
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .FirstOrDefaultAsync();

            return debt != null ? _mapper.Map<DebtDto>(debt) : null;
        }

        public async Task<IEnumerable<DebtDto>> GetDebtsByCreditorAsync(Guid creditorId)
        {
            var debts = await (await _debtRepository.Consultar(d => d.CreditorId == creditorId))
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtDto>>(debts);
        }

        public async Task<IEnumerable<DebtDto>> GetDebtsByDebtorAsync(Guid debtorId)
        {
            var debts = await (await _debtRepository.Consultar(d => d.DebtorId == debtorId))
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtDto>>(debts);
        }

        public async Task<IEnumerable<DebtDto>> GetDebtsByUserAsync(Guid userId)
        {
            var debts = await (await _debtRepository.Consultar(d => d.CreditorId == userId || d.DebtorId == userId))
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtDto>>(debts);
        }

        public async Task<IEnumerable<DebtDto>> GetPendingDebtsAsync()
        {
            var debts = await (await _debtRepository.Consultar(d => d.Status == "pending"))
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtDto>>(debts);
        }

        public async Task<IEnumerable<DebtDto>> GetOverdueDebtsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var debts = await (await _debtRepository.Consultar(d => d.Status == "pending" && d.DueDate.HasValue && d.DueDate < today))
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtDto>>(debts);
        }

        public async Task<DebtDto> CreateDebtAsync(Guid creditorId, CreateDebtDto createDebtDto)
        {
            if (creditorId == createDebtDto.DebtorId)
                throw new ArgumentException("No puedes crear una deuda contigo mismo");

            var creditorExists = await _userRepository.Obtener(u => u.UserId == creditorId && u.IsActive == true) != null;
            var debtorExists = await _userRepository.Obtener(u => u.UserId == createDebtDto.DebtorId && u.IsActive == true) != null;

            if (!creditorExists || !debtorExists)
                throw new ArgumentException("Usuario no encontrado o inactivo");

            var debt = _mapper.Map<Debt>(createDebtDto);
            debt.DebtId = Guid.NewGuid();
            debt.CreditorId = creditorId;
            debt.CreatedAt = DateTime.Now;
            debt.UpdatedAt = DateTime.Now;

            var createdDebt = await _debtRepository.Crear(debt);
            return await GetDebtByIdAsync(createdDebt.DebtId) ?? _mapper.Map<DebtDto>(createdDebt);
        }

        public async Task<DebtDto?> UpdateDebtAsync(Guid debtId, UpdateDebtDto updateDebtDto)
        {
            var existingDebt = await _debtRepository.Obtener(d => d.DebtId == debtId);
            if (existingDebt == null) return null;

            // Validar que la deuda no esté pagada
            if (existingDebt.Status == "Paid" || existingDebt.Status == "Pagada")
            {
                throw new InvalidOperationException("No se puede editar una deuda que ya está pagada");
            }

            _mapper.Map(updateDebtDto, existingDebt);
            existingDebt.UpdatedAt = DateTime.Now;

            await _debtRepository.Editar(existingDebt);
            return await GetDebtByIdAsync(debtId);
        }

        public async Task<bool> DeleteDebtAsync(Guid debtId)
        {
            var debt = await _debtRepository.Obtener(d => d.DebtId == debtId);
            if (debt == null) return false;

            return await _debtRepository.Eliminar(debt);
        }

        public async Task<bool> MarkDebtAsPaidAsync(Guid debtId)
        {
            var debt = await _debtRepository.Obtener(d => d.DebtId == debtId);
            if (debt == null) return false;

            debt.Status = "paid";
            debt.UpdatedAt = DateTime.Now;

            return await _debtRepository.Editar(debt);
        }

        public async Task<bool> CancelDebtAsync(Guid debtId)
        {
            var debt = await _debtRepository.Obtener(d => d.DebtId == debtId);
            if (debt == null) return false;

            debt.Status = "cancelled";
            debt.UpdatedAt = DateTime.Now;

            return await _debtRepository.Editar(debt);
        }

        public async Task<IEnumerable<DebtSummaryDto>> GetDebtsSummaryAsync(Guid? userId = null)
        {
            var query = await _debtRepository.Consultar();

            if (userId.HasValue)
            {
                query = query.Where(d => d.CreditorId == userId || d.DebtorId == userId);
            }

            var debts = await query
                .Include(d => d.Creditor)
                .Include(d => d.Debtor)
                .Include(d => d.Payments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DebtSummaryDto>>(debts);
        }

        public async Task<decimal> GetTotalAmountOwedToUserAsync(Guid userId)
        {
            var debts = await (await _debtRepository.Consultar(d => d.CreditorId == userId && d.Status == "pending"))
                .Include(d => d.Payments)
                .ToListAsync();

            return debts.Sum(d => d.Amount - (d.Payments?.Sum(p => p.Amount) ?? 0));
        }

        public async Task<decimal> GetTotalAmountUserOwesAsync(Guid userId)
        {
            var debts = await (await _debtRepository.Consultar(d => d.DebtorId == userId && d.Status == "pending"))
                .Include(d => d.Payments)
                .ToListAsync();

            return debts.Sum(d => d.Amount - (d.Payments?.Sum(p => p.Amount) ?? 0));
        }

        public async Task<bool> UserCanAccessDebtAsync(Guid userId, Guid debtId)
        {
            var debt = await _debtRepository.Obtener(d => d.DebtId == debtId);
            return debt != null && (debt.CreditorId == userId || debt.DebtorId == userId);
        }
    }
}
