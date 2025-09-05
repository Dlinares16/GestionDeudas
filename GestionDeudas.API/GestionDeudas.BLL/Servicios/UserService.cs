using AutoMapper;
using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DAL.DBContext;
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
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly GestionDeudasContext _context;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper, GestionDeudasContext context)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await (await _userRepository.Consultar(u => u.IsActive == true)).ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId && u.IsActive == true);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.Obtener(u => u.Email == email && u.IsActive == true);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await EmailExistsAsync(createUserDto.Email))
                throw new ArgumentException("El email ya está registrado");

            var user = _mapper.Map<User>(createUserDto);
            user.UserId = Guid.NewGuid();
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            var createdUser = await _userRepository.Crear(user);
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.Obtener(u => u.UserId == userId && u.IsActive == true);
            if (existingUser == null) return null;

            _mapper.Map(updateUserDto, existingUser);
            existingUser.UpdatedAt = DateTime.Now;

            await _userRepository.Editar(existingUser);
            return _mapper.Map<UserDto>(existingUser);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            return await _userRepository.Eliminar(user);
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            return await _userRepository.Editar(user);
        }

        public async Task<bool> ActivateUserAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;

            return await _userRepository.Editar(user);
        }

        public async Task<UserBalanceDto?> GetUserBalanceAsync(Guid userId)
        {
            var balances = await GetAllUserBalancesAsync();
            return balances.FirstOrDefault(b => b.UserId == userId);
        }

        public async Task<IEnumerable<UserBalanceDto>> GetAllUserBalancesAsync()
        {
            var sql = @"
                SELECT 
                    user_id as UserId,
                    email as Email,
                    first_name as FirstName,
                    last_name as LastName,
                    total_owed_to_me as TotalOwedToMe,
                    total_i_owe as TotalIOwe,
                    net_balance as NetBalance
                FROM user_balance
                ORDER BY net_balance DESC";

            var balances = await _context.Database.SqlQueryRaw<UserBalanceDto>(sql).ToListAsync();
            return balances;
        }

        public async Task<bool> VerifyEmailAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            user.EmailVerified = true;
            user.UpdatedAt = DateTime.Now;

            return await _userRepository.Editar(user);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            return user != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userRepository.Obtener(u => u.Email == email);
            return user != null;
        }
    }
}
