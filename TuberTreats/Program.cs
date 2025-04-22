using TuberTreats.Models;

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

List<TuberDriver> drivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "Driver One" },
    new TuberDriver { Id = 2, Name = "Driver Two" },
    new TuberDriver { Id = 3, Name = "Driver Three" }
};

List<TuberOrder> orders = new List<TuberOrder>
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

app.Run();
//don't touch or move this!
public partial class Program { }