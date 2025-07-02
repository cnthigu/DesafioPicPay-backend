using DesafioPicPayBackEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using PicPayClone.Data;
using PicPayClone.Models;
using PicPayClone.Services;
using PicPayClone.Services.External;
using PicPayClone.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IValidator<CreateTransactionDTO>, CreateTransactionDTOValidator>();

builder.Services.AddHttpClient<IAuthorizationService, AuthorizationService>();
builder.Services.AddHttpClient<INotificationService, NotificationService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
