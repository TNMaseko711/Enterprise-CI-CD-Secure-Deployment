using System.Collections.Concurrent;
using EmployeeApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

var app = builder.Build();

var employees = new ConcurrentDictionary<int, Employee>();
var idCounter = 0;

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck");

app.MapGet("/employees", () => Results.Ok(employees.Values.OrderBy(e => e.Id)))
    .WithName("GetEmployees");

app.MapGet("/employees/{id:int}", (int id, ILogger<Program> logger) =>
    {
        if (employees.TryGetValue(id, out var employee))
        {
            logger.LogInformation("Employee {EmployeeId} retrieved", id);
            return Results.Ok(employee);
        }

        logger.LogWarning("Employee {EmployeeId} not found", id);
        return Results.NotFound();
    })
    .WithName("GetEmployeeById");

app.MapPost("/employees", (EmployeeCreateRequest request, ILogger<Program> logger) =>
    {
        var id = Interlocked.Increment(ref idCounter);
        var employee = new Employee(id, request.Name, request.Role, request.Department);
        employees[id] = employee;
        logger.LogInformation("Employee {EmployeeId} created", id);
        return Results.Created($"/employees/{id}", employee);
    })
    .WithName("CreateEmployee");

app.MapPut("/employees/{id:int}", (int id, EmployeeUpdateRequest request, ILogger<Program> logger) =>
    {
        if (!employees.ContainsKey(id))
        {
            logger.LogWarning("Employee {EmployeeId} not found for update", id);
            return Results.NotFound();
        }

        var updated = new Employee(id, request.Name, request.Role, request.Department);
        employees[id] = updated;
        logger.LogInformation("Employee {EmployeeId} updated", id);
        return Results.Ok(updated);
    })
    .WithName("UpdateEmployee");

app.MapDelete("/employees/{id:int}", (int id, ILogger<Program> logger) =>
    {
        if (employees.TryRemove(id, out _))
        {
            logger.LogInformation("Employee {EmployeeId} deleted", id);
            return Results.NoContent();
        }

        logger.LogWarning("Employee {EmployeeId} not found for deletion", id);
        return Results.NotFound();
    })
    .WithName("DeleteEmployee");

app.Run();

public partial class Program { }

namespace EmployeeApi.Models
{
    public record Employee(int Id, string Name, string Role, string Department);

    public record EmployeeCreateRequest(string Name, string Role, string Department);

    public record EmployeeUpdateRequest(string Name, string Role, string Department);
}
