using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Specifications;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using System.Security.Claims;


namespace Service
{
    public class AccountService(
        UserManager<BloodDonationUser> _userManager,
        IMapper _mapper,
        ILogger<AccountService> _logger,
        IGeoLocationService _geoLocationService,
        IUnitOfWork _unitOfWork
        ) : IAccountService
    {
        public async Task<AccountDTo> GetAccountProfileAsync(Guid UserId)
        {
            var User = await _userManager.Users.Include(U => U.City).Include(U => U.BloodType).FirstOrDefaultAsync(U => U.Id == UserId) ?? throw new UserNotFoundException(UserId);
            return _mapper.Map<AccountDTo>(User);
        }
        public async Task UpdateAccountProfileAsync(UpdateAccountDTo updateAccountDTo, string UserId)
        {
            var User = await _userManager.FindByIdAsync(UserId) ?? throw new UserNotFoundException(UserId);
            var OldPhoneNumber = User.PhoneNumber;
            User.PhoneNumber = updateAccountDTo.PhoneNumber;
            User.CityId = updateAccountDTo.CityId;
            var Result = await _userManager.UpdateAsync(User);
            if (Result.Succeeded)
            {
                _logger.LogInformation("User {UserId} updated their phone number", UserId);
            }
            else
            {
                _logger.LogWarning("Failed to update user {UserId} profile. Errors: {Errors}", UserId, string.Join(", ", Result.Errors.Select(E => E.Description)));
                throw new BadRequestException(Result.Errors.Select(E => E.Description).ToList());
            }
        }
        public async Task RefreshDeviceTokenAndLocationAsync(UpdateDeviceTokenAndLoccationDTo updateDeviceTokenAndLoccationDTo, string UserId)
        {
            _logger.LogInformation("User {UserId} is refreshing their device token and location.", UserId);
            var User = await _userManager.FindByIdAsync(UserId) ?? throw new UserNotFoundException(UserId);
            User.Longitude = updateDeviceTokenAndLoccationDTo.Longitude;
            User.Latitude = updateDeviceTokenAndLoccationDTo.Latitude;
            User.DeviceToken = updateDeviceTokenAndLoccationDTo.DeviceToken;
            await _userManager.UpdateAsync(User);
            await _geoLocationService.UpdateDonorLocationAndDeviceTokenAsync(UserId,
                updateDeviceTokenAndLoccationDTo.DeviceToken,
                updateDeviceTokenAndLoccationDTo.Longitude,
                updateDeviceTokenAndLoccationDTo.Latitude,
                User.BloodTypeId.ToString());
        }
        public async Task<PaginatedResult<DonationHistoryDTo>> GetDonationHistoryAsync(DonationHistoryQueryParams Params, string UserId)
        {
            var USer = await _userManager.FindByIdAsync(UserId) ?? throw new UserNotFoundException(UserId);
            var Repo = _unitOfWork.GetRepository<DonationHistory, long>();
            var Specificaiton = new DonationHistoryToUser(Params, Guid.Parse(UserId));
            var Count = await Repo.CountAsync(Specificaiton);
            var Histories = await Repo.GetAllAsync(Specificaiton);
            var HistoriesDTo = _mapper.Map<IEnumerable<DonationHistoryDTo>>(Histories);
            return new PaginatedResult<DonationHistoryDTo>(Histories.Count(), Params.PageNumber, Count, HistoriesDTo);
        }

    }
}
