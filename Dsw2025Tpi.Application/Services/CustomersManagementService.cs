using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Models;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Dsw2025Tpi.Application.Services;
public class CustomersManagementService
{
    private readonly IRepository _repository;
    public CustomersManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task addCustomer(RegisterModel data)
    {
        if(data.Email.IsNullOrEmpty() || data.Username.IsNullOrEmpty() || data.Username.IsNullOrEmpty() || data.Password.IsNullOrEmpty() || data.Role.IsNullOrEmpty()) 
        {
            throw new ArgumentException("Uno/s de los campos ingresados está vacio.");
        }

        var customers = await _repository.GetAll<Customer>();
        if (customers != null)
        {
            foreach (var c in customers)
            {
                if (data.Username == c.name || data.Email == c.eMail) { throw new DuplicateEntityException($"Usuario con el nombre o Email ingresado ya existente."); }
            }
        }

        Customer customer = new Customer(data.Email, data.Username, "3815554444");
        await _repository.Add(customer);
    }

    public async Task deleteCustomer(string name)
    {
        var customers = await _repository.First<Customer>((c) => c.name == name);
        if (customers != null)
        {
             await _repository.Delete(customers);
        }
        else
        {
            throw new NoFoundEntityException($"Ningun cliente cargado/disponible.");
        }
    }

    public async Task<Customer?> getCustomerByName(string customerName)
    {
        var customerByName = await _repository.First<Customer>((c) => c.name == customerName);
        if (customerByName != null)
        {
            return customerByName;
        }
        else
        {
            throw new NoFoundEntityException($"Ninguna orden con el nombre {customerName} cargada/disponible.");
        }
    }
}