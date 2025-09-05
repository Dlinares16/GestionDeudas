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
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Debt> _debtRepository;
        private readonly IMapper _mapper;

        public PaymentService(IGenericRepository<Payment> paymentRepository, IGenericRepository<Debt> debtRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _debtRepository = debtRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
        {
            var payments = await (await _paymentRepository.Consultar())
                .Include(p => p.Debt)
                .ThenInclude(d => d.Creditor)
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await (await _paymentRepository.Consultar(p => p.PaymentId == paymentId))
                .Include(p => p.Debt)
                .ThenInclude(d => d.Creditor)
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .FirstOrDefaultAsync();

            return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByDebtAsync(Guid debtId)
        {
            var payments = await (await _paymentRepository.Consultar(p => p.DebtId == debtId))
                .Include(p => p.Debt)
                .ThenInclude(d => d.Creditor)
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByUserAsync(Guid userId)
        {
            var payments = await (await _paymentRepository.Consultar())
                .Include(p => p.Debt)
                .ThenInclude(d => d.Creditor)
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .Where(p => p.Debt.CreditorId == userId || p.Debt.DebtorId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            var debt = await (await _debtRepository.Consultar(d => d.DebtId == createPaymentDto.DebtId))
                .Include(d => d.Payments)
                .FirstOrDefaultAsync();

            if (debt == null)
                throw new ArgumentException("La deuda no existe");

            if (debt.Status != "pending")
                throw new ArgumentException("No se pueden agregar pagos a una deuda que no está pendiente");

            var totalPaid = debt.Payments?.Sum(p => p.Amount) ?? 0;
            var remainingAmount = debt.Amount - totalPaid;

            if (createPaymentDto.Amount > remainingAmount)
                throw new ArgumentException($"El monto del pago ({createPaymentDto.Amount:C}) excede el monto restante ({remainingAmount:C})");

            var payment = _mapper.Map<Payment>(createPaymentDto);
            payment.PaymentId = Guid.NewGuid();
            payment.CreatedAt = DateTime.UtcNow;

            var createdPayment = await _paymentRepository.Crear(payment);

            // Verificar si la deuda está completamente pagada
            var newTotalPaid = totalPaid + createPaymentDto.Amount;
            if (newTotalPaid >= debt.Amount)
            {
                debt.Status = "paid";
                debt.UpdatedAt = DateTime.UtcNow;
                await _debtRepository.Editar(debt);
            }

            return await GetPaymentByIdAsync(createdPayment.PaymentId) ?? _mapper.Map<PaymentDto>(createdPayment);
        }

        public async Task<bool> DeletePaymentAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.Obtener(p => p.PaymentId == paymentId);
            if (payment == null) return false;

            var debt = await (await _debtRepository.Consultar(d => d.DebtId == payment.DebtId))
                .Include(d => d.Payments)
                .FirstOrDefaultAsync();

            if (debt != null && debt.Status == "paid")
            {
                // Si se elimina un pago y la deuda estaba marcada como pagada, cambiar a pendiente
                var remainingPayments = debt.Payments?.Where(p => p.PaymentId != paymentId).Sum(p => p.Amount) ?? 0;
                if (remainingPayments < debt.Amount)
                {
                    debt.Status = "pending";
                    debt.UpdatedAt = DateTime.UtcNow;
                    await _debtRepository.Editar(debt);
                }
            }

            return await _paymentRepository.Eliminar(payment);
        }

        public async Task<decimal> GetTotalPaidForDebtAsync(Guid debtId)
        {
            var payments = await (await _paymentRepository.Consultar(p => p.DebtId == debtId)).ToListAsync();
            return payments.Sum(p => p.Amount);
        }

        public async Task<IEnumerable<PaymentSummaryDto>> GetPaymentsSummaryAsync(Guid? userId = null)
        {
            var query = await _paymentRepository.Consultar();

            if (userId.HasValue)
            {
                query = query.Where(p => p.Debt.CreditorId == userId || p.Debt.DebtorId == userId);
            }

            var payments = await query
                .Include(p => p.Debt)
                .ThenInclude(d => d.Creditor)
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentSummaryDto>>(payments);
        }

        public async Task<bool> UserCanAccessPaymentAsync(Guid userId, Guid paymentId)
        {
            var payment = await (await _paymentRepository.Consultar(p => p.PaymentId == paymentId))
                .Include(p => p.Debt)
                .FirstOrDefaultAsync();

            return payment != null && (payment.Debt.CreditorId == userId || payment.Debt.DebtorId == userId);
        }

        public async Task<bool> ValidatePaymentAmountAsync(Guid debtId, decimal paymentAmount)
        {
            var debt = await (await _debtRepository.Consultar(d => d.DebtId == debtId))
                .Include(d => d.Payments)
                .FirstOrDefaultAsync();

            if (debt == null || debt.Status != "pending") return false;

            var totalPaid = debt.Payments?.Sum(p => p.Amount) ?? 0;
            var remainingAmount = debt.Amount - totalPaid;

            return paymentAmount > 0 && paymentAmount <= remainingAmount;
        }
    }
}
