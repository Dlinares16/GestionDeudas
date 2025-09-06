using AutoMapper;
using GestionDeudas.BLL.AWS;
using GestionDeudas.BLL.AWS.Contrato;
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
        private readonly ICacheAWS _cacheAWS;

        // Configuración de tiempo de expiración del caché
        private readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _balanceCacheExpiration = TimeSpan.FromMinutes(15); // Los balances cambian más frecuentemente

        public UserService(IGenericRepository<User> userRepository, IMapper mapper, GestionDeudasContext context, ICacheAWS cacheAWS)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
            _cacheAWS = cacheAWS;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            const string cacheKey = "all_active_users";

            // Intentar obtener del caché
            var cachedUsers = await _cacheAWS.GetCacheAsync<List<UserDto>>(cacheKey);
            if (cachedUsers != null)
            {
                return cachedUsers;
            }

            // Si no está en caché, obtener de la base de datos
            var users = await (await _userRepository.Consultar(u => u.IsActive == true)).ToListAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            // Guardar en caché
            await _cacheAWS.SetCacheAsync(cacheKey, userDtos, _defaultCacheExpiration);

            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var cacheKey = $"user_by_id_{userId}";

            // Intentar obtener del caché
            var cachedUser = await _cacheAWS.GetCacheAsync<UserDto>(cacheKey);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            // Si no está en caché, obtener de la base de datos
            var user = await _userRepository.Obtener(u => u.UserId == userId && u.IsActive == true);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);

            // Guardar en caché
            await _cacheAWS.SetCacheAsync(cacheKey, userDto, _defaultCacheExpiration);

            return userDto;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var cacheKey = CacheKeys.UserByEmail(email);

            // Intentar obtener del caché
            var cachedUser = await _cacheAWS.GetCacheAsync<UserDto>(cacheKey);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            // Si no está en caché, obtener de la base de datos
            var user = await _userRepository.Obtener(u => u.Email == email && u.IsActive == true);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);

            // Guardar en caché
            await _cacheAWS.SetCacheAsync(cacheKey, userDto, _defaultCacheExpiration);

            return userDto;
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
            var userDto = _mapper.Map<UserDto>(createdUser);

            // Invalidar caché relacionado
            await InvalidateUserCacheAsync(user.UserId, user.Email);
            await _cacheAWS.DeleteCacheAsync("all_active_users"); // Invalidar lista de usuarios

            // Guardar el nuevo usuario en caché
            await _cacheAWS.SetCacheAsync($"user_by_id_{user.UserId}", userDto, _defaultCacheExpiration);
            await _cacheAWS.SetCacheAsync(CacheKeys.UserByEmail(user.Email), userDto, _defaultCacheExpiration);

            return userDto;
        }

        public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.Obtener(u => u.UserId == userId && u.IsActive == true);
            if (existingUser == null) return null;

            var oldEmail = existingUser.Email;

            _mapper.Map(updateUserDto, existingUser);
            existingUser.UpdatedAt = DateTime.Now;

            await _userRepository.Editar(existingUser);
            var userDto = _mapper.Map<UserDto>(existingUser);

            // Invalidar caché del usuario (tanto por ID como por email anterior)
            await InvalidateUserCacheAsync(userId, oldEmail);

            // Si el email cambió, también invalidar el cache con el email anterior
            if (oldEmail != existingUser.Email)
            {
                await _cacheAWS.DeleteCacheAsync(CacheKeys.UserByEmail(oldEmail));
            }

            await _cacheAWS.DeleteCacheAsync("all_active_users"); // Invalidar lista de usuarios

            // Actualizar caché con los nuevos datos
            await _cacheAWS.SetCacheAsync($"user_by_id_{userId}", userDto, _defaultCacheExpiration);
            await _cacheAWS.SetCacheAsync(CacheKeys.UserByEmail(existingUser.Email), userDto, _defaultCacheExpiration);

            return userDto;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            var result = await _userRepository.Eliminar(user);

            if (result)
            {
                // Invalidar caché relacionado
                await InvalidateUserCacheAsync(userId, user.Email);
                await _cacheAWS.DeleteCacheAsync("all_active_users");
            }

            return result;
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            var result = await _userRepository.Editar(user);

            if (result)
            {
                // Invalidar caché relacionado
                await InvalidateUserCacheAsync(userId, user.Email);
                await _cacheAWS.DeleteCacheAsync("all_active_users");
            }

            return result;
        }

        public async Task<bool> ActivateUserAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;

            var result = await _userRepository.Editar(user);

            if (result)
            {
                // Invalidar caché relacionado
                await InvalidateUserCacheAsync(userId, user.Email);
                await _cacheAWS.DeleteCacheAsync("all_active_users");

                // Actualizar caché con el usuario activado
                var userDto = _mapper.Map<UserDto>(user);
                await _cacheAWS.SetCacheAsync($"user_by_id_{userId}", userDto, _defaultCacheExpiration);
                await _cacheAWS.SetCacheAsync(CacheKeys.UserByEmail(user.Email), userDto, _defaultCacheExpiration);
            }

            return result;
        }

        public async Task<UserBalanceDto?> GetUserBalanceAsync(Guid userId)
        {
            var cacheKey = $"user_balance_{userId}";

            // Intentar obtener del caché
            var cachedBalance = await _cacheAWS.GetCacheAsync<UserBalanceDto>(cacheKey);
            if (cachedBalance != null)
            {
                return cachedBalance;
            }

            // Si no está en caché, obtener de la base de datos
            var balances = await GetAllUserBalancesAsync();
            var userBalance = balances.FirstOrDefault(b => b.UserId == userId);

            if (userBalance != null)
            {
                // Guardar en caché solo el balance específico
                await _cacheAWS.SetCacheAsync(cacheKey, userBalance, _balanceCacheExpiration);
            }

            return userBalance;
        }

        public async Task<IEnumerable<UserBalanceDto>> GetAllUserBalancesAsync()
        {
            const string cacheKey = "all_user_balances";

            // Intentar obtener del caché
            var cachedBalances = await _cacheAWS.GetCacheAsync<List<UserBalanceDto>>(cacheKey);
            if (cachedBalances != null)
            {
                return cachedBalances;
            }

            // Si no está en caché, obtener de la base de datos
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
            var balancesList = balances.ToList();

            // Guardar en caché
            await _cacheAWS.SetCacheAsync(cacheKey, balancesList, _balanceCacheExpiration);

            return balancesList;
        }

        public async Task<bool> VerifyEmailAsync(Guid userId)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user == null) return false;

            user.EmailVerified = true;
            user.UpdatedAt = DateTime.Now;

            var result = await _userRepository.Editar(user);

            if (result)
            {
                // Invalidar caché relacionado para que se actualice con el nuevo estado
                await InvalidateUserCacheAsync(userId, user.Email);
            }

            return result;
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            // Para verificación de existencia, primero intentamos el caché
            var cachedUser = await _cacheAWS.GetCacheAsync<UserDto>($"user_by_id_{userId}");
            if (cachedUser != null)
            {
                return true;
            }

            // Si no está en caché, verificar en base de datos
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            return user != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            // Para verificación de existencia, primero intentamos el caché
            var cachedUser = await _cacheAWS.GetCacheAsync<UserDto>(CacheKeys.UserByEmail(email));
            if (cachedUser != null)
            {
                return true;
            }

            // Si no está en caché, verificar en base de datos
            var user = await _userRepository.Obtener(u => u.Email == email);
            return user != null;
        }

        /// <summary>
        /// Invalida todas las entradas de caché relacionadas con un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="email">Email del usuario</param>
        private async Task InvalidateUserCacheAsync(Guid userId, string email)
        {
            // Ejecutar invalidaciones en paralelo para mejor performance
            var tasks = new[]
            {
                _cacheAWS.DeleteCacheAsync($"user_by_id_{userId}"),
                _cacheAWS.DeleteCacheAsync($"user_by_email_{email.ToLower()}"),
                _cacheAWS.DeleteCacheAsync($"user_balance_{userId}"),
                _cacheAWS.DeleteCacheAsync("all_user_balances")
            };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Invalida el caché de balances de usuarios (útil cuando se crean/actualizan deudas)
        /// </summary>
        public async Task InvalidateUserBalanceCacheAsync()
        {
            await _cacheAWS.DeleteCacheAsync("all_user_balances");
        }

        /// <summary>
        /// Invalida el caché de un usuario específico (útil para llamadas desde otros servicios)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task InvalidateSpecificUserCacheAsync(Guid userId)
        {
            // Necesitamos obtener el email del usuario para invalidar completamente
            var user = await _userRepository.Obtener(u => u.UserId == userId);
            if (user != null)
            {
                await InvalidateUserCacheAsync(userId, user.Email);
            }
        }
    }
}