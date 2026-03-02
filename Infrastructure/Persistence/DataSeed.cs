using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Shared.Constants;
using System.Text.Json;

namespace Persistence
{
    public class DataSeed(BloodDonationDbContext _dbContext, IPasswordHasher<BloodDonationUser> _passwordHasher) : IDataSeed
    {
        public async Task DataSeedAsync()
        {
            try
            {
                var PendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                if (PendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
                if (!_dbContext.Governorates.Any())
                {
                    var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataSeed", "Governorates.json");
                    using var GovernoratesData = File.OpenRead(FilePath);
                    var Governorates = await JsonSerializer.DeserializeAsync<List<Governorate>>(GovernoratesData);
                    if (Governorates is not null && Governorates.Any())
                    {
                        await _dbContext.Governorates.AddRangeAsync(Governorates);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                if (!_dbContext.Cities.Any())
                {
                    var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataSeed", "Cities.json");

                    using var CitiesData = File.OpenRead(FilePath);
                    var Cities = await JsonSerializer.DeserializeAsync<List<City>>(CitiesData);
                    if (Cities is not null && Cities.Any())
                    {
                        await _dbContext.Cities.AddRangeAsync(Cities);
                    }
                }
                if (!_dbContext.BloodTypes.Any())
                {
                    var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataSeed", "BloodTypes.json");
                    using var BloodTypesData = File.OpenRead(FilePath);
                    var BloodTypes = await JsonSerializer.DeserializeAsync<List<BloodTypes>>(BloodTypesData);
                    if (BloodTypes is not null && BloodTypes.Any())
                    {
                        await _dbContext.BloodTypes.AddRangeAsync(BloodTypes);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                if (!_dbContext.DonationCategories.Any())
                {
                    var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataSeed", "DonationCategories.json");
                    using var DonationCategoriesData = File.OpenRead(FilePath);
                    var DonationCategories = await JsonSerializer.DeserializeAsync<List<DonationCategories>>(DonationCategoriesData);
                    if (DonationCategories is not null && DonationCategories.Any())
                    {
                        await _dbContext.DonationCategories.AddRangeAsync(DonationCategories);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                if (!_dbContext.Compatibilities.Any())
                {
                    var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataSeed", "CompatibilityMatrix.json");
                    using var CompatibilitiesData = File.OpenRead(FilePath);
                    var CompatibilityMatrix = await JsonSerializer.DeserializeAsync<List<CompatibilityMatrix>>(CompatibilitiesData);
                    if (CompatibilityMatrix is not null && CompatibilityMatrix.Any())
                    {
                        await _dbContext.Compatibilities.AddRangeAsync(CompatibilityMatrix);
                    }
                }
                await _dbContext.SaveChangesAsync();
                if (!_dbContext.Users.Any())
                {
                    var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataSeed", "Admins.json");
                    using var AdminsData = File.OpenRead(FilePath);
                    var Admins = await JsonSerializer.DeserializeAsync<List<BloodDonationUser>>(AdminsData);
                    if (Admins is not null && Admins.Any())
                    {
                        var AdminRole = _dbContext.Roles.FirstOrDefault(R => R.Name == ElixirRoles.Admin);
                        var NewAdminsRoles = new List<IdentityUserRole<Guid>>();
                        //await _dbContext.Users.AddRangeAsync(Admins);
                        foreach (var Admin in Admins)
                        {
                            Admin.Id = Guid.NewGuid();
                            Admin.UserName = Admin.Email;
                            Admin.NormalizedUserName = Admin?.Email?.ToUpper();
                            Admin.NormalizedEmail = Admin.Email?.ToUpper();
                            Admin.SecurityStamp = Guid.NewGuid().ToString();
                            Admin.IsAvailable = true;
                            Admin.PasswordHash = _passwordHasher.HashPassword(Admin, "Elixir*Super*Admin2030$");
                            Admin.EmailConfirmed = true;
                            if (AdminRole != null)
                            {
                                NewAdminsRoles.Add(new IdentityUserRole<Guid>
                                {
                                    UserId = Admin.Id,
                                    RoleId = AdminRole.Id
                                });
                            }
                        }
                        await _dbContext.Users.AddRangeAsync(Admins);
                        if (NewAdminsRoles.Any())
                        {
                            await _dbContext.UserRoles.AddRangeAsync(NewAdminsRoles);
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
