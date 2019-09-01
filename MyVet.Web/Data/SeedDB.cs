using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;

namespace MyVet.Web.Data
{
    public class SeedDB
    {
        private readonly DataContext _datacontext;
        private readonly IUserHelper _userHelper;

        public SeedDB(DataContext context, IUserHelper userHelper)
        {
            _datacontext = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _datacontext.Database.EnsureCreatedAsync();
            await CheckRoles();
            var manager = await CheckUserAsync("1010", "Andres Mauricio", "Sanchez Gonzalez", "asanchez2912@gmail.com", "3002852726", "Calle Luna Calle Sol", "Admin");
            var customer = await CheckUserAsync("2020", "Matias", "Sanchez Arenas", "asanchez@tsgroup.com.co", "3012300663", "Calle Luna Calle Sol", "Customer");
            await CheckPetTypesAsync();
            await CheckServiceTypesAsync();
            await CheckOwnerAsync(customer);
            await CheckManagerAsync(manager);
            await CheckPetsAsync();
            await CheckAgendasAsync();
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, 
                                                string email, string phone, string address, string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUser2RoleAsync(user, role);
            }

            return user;
        }

        private async Task CheckPetsAsync()
        {
            if (!_datacontext.Pets.Any())
            {
                var owner = _datacontext.Owners.FirstOrDefault();
                var petType = _datacontext.PetTypes.FirstOrDefault();

                AddPet("Leonardo", owner, petType, "green turtle");
                AddPet("Casimiro", owner, petType, "Catalan donkey");
                AddPet("Samsom", owner, petType, "American Pitbull");

                await _datacontext.SaveChangesAsync();
            }
        }

        private async Task CheckServiceTypesAsync()
        {
            if (!_datacontext.ServiceTypes.Any())
            {
                _datacontext.Add(new ServiceType { Name = "Medical consultation" });
                _datacontext.Add(new ServiceType { Name = "Emergency" });
                _datacontext.Add(new ServiceType { Name = "vaccination" });
                _datacontext.Add(new ServiceType { Name = "Cutting and brushing" });

                await _datacontext.SaveChangesAsync();
            }
        }

        private async Task CheckPetTypesAsync()
        {
            if (!_datacontext.PetTypes.Any())
            {
                _datacontext.PetTypes.Add(new PetType { Name = "Cat" });
                _datacontext.PetTypes.Add(new PetType { Name = "Dog" });
                _datacontext.PetTypes.Add(new PetType { Name = "Turtle" });
                _datacontext.PetTypes.Add(new PetType { Name = "Snake" });
                _datacontext.PetTypes.Add(new PetType { Name = "Horse" });
                _datacontext.PetTypes.Add(new PetType { Name = "Donkey" });
                _datacontext.PetTypes.Add(new PetType { Name = "Hamster" });

                await _datacontext.SaveChangesAsync();
            }
        }

        private async Task CheckOwnerAsync(User user)
        {
            if (!_datacontext.Owners.Any())
            {
                _datacontext.Owners.Add(new Owner { User = user });

                await _datacontext.SaveChangesAsync();
            }
        }

        private async Task CheckManagerAsync(User user)
        {
            if (!_datacontext.Managers.Any())
            {
                _datacontext.Managers.Add(new Manager { User = user });

                await _datacontext.SaveChangesAsync();
            }
        }

        private void AddPet(string name, Owner owner, PetType petType, string race)
        {
            _datacontext.Pets.Add(new Pet
            {
                Born = DateTime.Now.AddYears(-2),
                Name = name,
                Owner = owner,
                PetType = petType,
                Race = race
            });
        }

        private async Task CheckAgendasAsync()
        {
            if (!_datacontext.Agendas.Any())
            {
                var initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                var finalDate = initialDate.AddMonths(1);
                while (initialDate < finalDate)
                {
                    if (initialDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        var finalDate2 = initialDate.AddHours(10);
                        while (initialDate < finalDate2)
                        {
                            _datacontext.Agendas.Add(new Agenda
                            {
                                Date = initialDate,
                                IsAvailable = true
                            });

                            initialDate = initialDate.AddMinutes(30);
                        }

                        initialDate = initialDate.AddHours(14);
                    }
                    else
                    {
                        initialDate = initialDate.AddDays(1);
                    }
                }
            }

            await _datacontext.SaveChangesAsync();
        }
    }

}
