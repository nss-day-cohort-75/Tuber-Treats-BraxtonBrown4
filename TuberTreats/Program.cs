using TuberTreats.Models;
using TuberTreats.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);


List<Topping> toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Cheese" },
    new Topping { Id = 2, Name = "Bacon" },
    new Topping { Id = 3, Name = "Sour Cream" },
    new Topping { Id = 4, Name = "Chives" },
    new Topping { Id = 5, Name = "Butter" }
};

List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Alice", Address = "123 Spud St" },
    new Customer { Id = 2, Name = "Bob", Address = "456 Tater Ave" },
    new Customer { Id = 3, Name = "Carol", Address = "789 Potato Blvd" },
    new Customer { Id = 4, Name = "Dan", Address = "321 Mash Rd" },
    new Customer { Id = 5, Name = "Eve", Address = "654 Hash Ln" }
};

List<TuberDriver> tuberDrivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "Driver One" },
    new TuberDriver { Id = 2, Name = "Driver Two" },
    new TuberDriver { Id = 3, Name = "Driver Three" }
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now.AddHours(-5),
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = DateTime.Now
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Now.AddHours(-3),
        CustomerId = 2,
        TuberDriverId = null,
        DeliveredOnDate = null
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Now.AddHours(-2),
        CustomerId = 3,
        TuberDriverId = 2,
        DeliveredOnDate = DateTime.Now
    }
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 3 },
    new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 2 },
    new TuberTopping { Id = 4, TuberOrderId = 2, ToppingId = 4 }
};

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here

app.MapGet("/tuberorders", () =>
{
    return tuberOrders.Select(tuberOrder =>
    {
        Customer customer = customers.FirstOrDefault(customer => customer.Id == tuberOrder.CustomerId);
        TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(tuberDriver => tuberDriver.Id == tuberOrder.TuberDriverId);

        List<TuberTopping> toppingsTables = tuberToppings.Where(tt => tt.TuberOrderId == tuberOrder.Id).ToList();
        List<Topping> orderToppings = toppingsTables.Select(tt => toppings.FirstOrDefault(t => t.Id == tt.ToppingId)).ToList();

        return new TuberOrderDTO
        {
            Id = tuberOrder.Id,
            OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
            CustomerId = tuberOrder.CustomerId,
            TuberDriverId = tuberOrder.TuberDriverId,
            DeliveredOnDate = tuberOrder.DeliveredOnDate,
            Customer = new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address
            },
            TuberDriver = tuberDriver == null ? null : new TuberDriverDTO
            {
                Id = tuberDriver.Id,
                Name = tuberDriver.Name
            },
            Toppings = orderToppings.Select(ot => new ToppingDTO
            {
                Id = ot.Id,
                Name = ot.Name
            }).ToList()
        };
    });
});

app.MapGet("/tuberorders/{id}", (int id) =>
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(tuberOrder => tuberOrder.Id == id);

    if (tuberOrder == null)
    {
        return Results.NotFound();
    }

    Customer customer = customers.FirstOrDefault(customer => customer.Id == tuberOrder.CustomerId);
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(tuberDriver => tuberDriver.Id == tuberOrder.TuberDriverId);

    List<TuberTopping> toppingsTables = tuberToppings.Where(tt => tt.TuberOrderId == tuberOrder.Id).ToList();
    List<Topping> orderToppings = toppingsTables.Select(tt => toppings.FirstOrDefault(t => t.Id == tt.ToppingId)).ToList();

    return Results.Ok(
        new TuberOrderDTO
        {
            Id = tuberOrder.Id,
            OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
            CustomerId = tuberOrder.CustomerId,
            TuberDriverId = tuberOrder.TuberDriverId,
            DeliveredOnDate = tuberOrder.DeliveredOnDate,
            Customer = new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address
            },
            TuberDriver = tuberDriver == null ? null : new TuberDriverDTO
            {
                Id = tuberDriver.Id,
                Name = tuberDriver.Name
            },
            Toppings = orderToppings.Select(ot => new ToppingDTO
            {
                Id = ot.Id,
                Name = ot.Name
            }).ToList()
        });
});

app.MapPost("/tuberorders", (TuberOrder tuberOrder) =>
{
    Customer customer = customers.FirstOrDefault(customer => customer.Id == tuberOrder.CustomerId);

    if (tuberOrder.CustomerId == null || customer == null)
    {
        return Results.BadRequest();
    }

    tuberOrder.Id = tuberOrders.Max(to => to.Id) + 1;
    tuberOrder.OrderPlacedOnDate = DateTime.Now;

    tuberOrders.Add(tuberOrder);

    return Results.Created(
        $"/tuberorders/{tuberOrder.Id}",

        new TuberOrderDTO
        {
            Id = tuberOrder.Id,
            OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
            CustomerId = tuberOrder.CustomerId,
            TuberDriverId = tuberOrder.TuberDriverId,
            DeliveredOnDate = tuberOrder.DeliveredOnDate,
            Customer = new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address
            },
            TuberDriver = null,
            Toppings = null
        }
    );

});

app.MapPut("/tuberorders/{id}", (int id, TuberOrder tuberOrder) =>
{
    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(tuberOrder => tuberOrder.Id == id);

    if (orderToUpdate == null)
    {
        return Results.NotFound();
    }

    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(tuberDriver => tuberDriver.Id == tuberOrder.TuberDriverId);

    if (tuberDriver == null)
    {
        return Results.BadRequest($"Could not find TuberDriver. Invalid TuberDriverId: {tuberOrder.TuberDriverId}");
    }
    orderToUpdate.TuberDriverId = tuberOrder.TuberDriverId;

    Customer customer = customers.FirstOrDefault(customer => customer.Id == orderToUpdate.CustomerId);
    List<TuberTopping> toppingsTables = tuberToppings.Where(tt => tt.TuberOrderId == orderToUpdate.Id).ToList();
    List<Topping> orderToppings = toppingsTables.Select(tt => toppings.FirstOrDefault(t => t.Id == tt.ToppingId)).ToList();

    return Results.Ok(
        new TuberOrderDTO
        {
            Id = orderToUpdate.Id,
            OrderPlacedOnDate = orderToUpdate.OrderPlacedOnDate,
            CustomerId = orderToUpdate.CustomerId,
            TuberDriverId = orderToUpdate.TuberDriverId,
            DeliveredOnDate = orderToUpdate.DeliveredOnDate,
            Customer = new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address
            },
            TuberDriver = tuberDriver == null ? null : new TuberDriverDTO
            {
                Id = tuberDriver.Id,
                Name = tuberDriver.Name
            },
            Toppings = orderToppings.Select(ot => new ToppingDTO
            {
                Id = ot.Id,
                Name = ot.Name
            }).ToList()
        }
    );
});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(tuberOrder => tuberOrder.Id == id);

    if (tuberOrder == null)
    {
        return Results.NotFound();
    }

    tuberOrder.DeliveredOnDate = DateTime.Now;

    Customer customer = customers.FirstOrDefault(customer => customer.Id == tuberOrder.CustomerId);
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(tuberDriver => tuberDriver.Id == tuberOrder.TuberDriverId);

    List<TuberTopping> toppingsTables = tuberToppings.Where(tt => tt.TuberOrderId == tuberOrder.Id).ToList();
    List<Topping> orderToppings = toppingsTables.Select(tt => toppings.FirstOrDefault(t => t.Id == tt.ToppingId)).ToList();

    return Results.Ok(
        new TuberOrderDTO
        {
            Id = tuberOrder.Id,
            OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
            CustomerId = tuberOrder.CustomerId,
            TuberDriverId = tuberOrder.TuberDriverId,
            DeliveredOnDate = tuberOrder.DeliveredOnDate,
            Customer = new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address
            },
            TuberDriver = tuberDriver == null ? null : new TuberDriverDTO
            {
                Id = tuberDriver.Id,
                Name = tuberDriver.Name
            },
            Toppings = orderToppings.Select(ot => new ToppingDTO
            {
                Id = ot.Id,
                Name = ot.Name
            }).ToList()
        });
});

app.MapGet("/toppings", () =>
{
    return toppings.Select(topping => new ToppingDTO
    {
        Id = topping.Id,
        Name = topping.Name
    });
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(topping => topping.Id == id);

    if (topping == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(
        new ToppingDTO
        {
            Id = topping.Id,
            Name = topping.Name
        }
    );
});

app.MapGet("/tubertoppings", () =>
{
    return tuberToppings.Select(tuberTopping => new TuberToppingDTO
    {
        Id = tuberTopping.Id,
        TuberOrderId = tuberTopping.TuberOrderId,
        ToppingId = tuberTopping.ToppingId
    });
});

app.MapPost("/tubertoppings", (TuberTopping tuberTopping) =>
{
    TuberTopping doesExist = tuberToppings.FirstOrDefault(tt => tt.TuberOrderId == tuberTopping.TuberOrderId && tt.ToppingId == tuberTopping.ToppingId);

    if (doesExist != null)
    {
        return Results.BadRequest("JoinTable Already Exists");
    }

    tuberTopping.Id = tuberToppings.Max(tt => tt.Id) + 1;

    tuberToppings.Add(tuberTopping);

    return Results.Created(
        $"/toppings/{tuberTopping.Id}",
        new TuberToppingDTO
        {
            Id = tuberTopping.Id,
            ToppingId = tuberTopping.ToppingId,
            TuberOrderId = tuberTopping.TuberOrderId
        }
    );
});

app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    TuberTopping tuberTopping = tuberToppings.FirstOrDefault(tuberTopping => tuberTopping.Id == id);

    if (tuberTopping == null)
    {
        return Results.NotFound();
    }

    tuberToppings.Remove(tuberTopping);

    return Results.NoContent();
});

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address,
        TuberOrders = tuberOrders.Where(to => to.CustomerId == c.Id).ToList()
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer c = customers.FirstOrDefault(c => c.Id == id);

    if (c == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(
        new CustomerDTO
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            TuberOrders = tuberOrders.Where(to => to.CustomerId == c.Id).ToList()
        }
    );
});

app.MapPost("/customers", (Customer customer) =>
{
    if (customer.Name == null || customer.Address == null)
    {
        return Results.BadRequest();
    }

    customer.Id = customers.Max(c => c.Id) + 1;

    customers.Add(customer);

    return Results.Created(
        $"/customers/{customer.Id}",
        new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        }
    );
});

app.MapDelete("/customers/{id}", (int id) => {
    
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    if (customer == null) {
        return Results.NotFound();
    }

    customers.Remove(customer);

    return Results.NoContent();
});

app.MapGet("/tuberdrivers", () => {
    return tuberDrivers.Select(td => new TuberDriverDTO {
        Id = td.Id,
        Name = td.Name,
        TuberDeliveries = tuberOrders.Where(to => to.TuberDriverId == td.Id).ToList()
    });
});

app.MapGet("/tuberdrivers/{id}", (int id) => {
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == id);

    if (tuberDriver == null) {
        return Results.NotFound();
    }

    return Results.Ok(
        new TuberDriverDTO {
        Id = tuberDriver.Id,
        Name = tuberDriver.Name,
        TuberDeliveries = tuberOrders.Where(to => to.TuberDriverId == tuberDriver.Id).ToList()
    }
    );
});

app.Run();
//don't touch or move this!
public partial class Program { }