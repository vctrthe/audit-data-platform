using System.Text.Json.Serialization;
using AuditDataPlatform.Api.Features.RuleSets;
using AuditDataPlatform.Application.FileParsing;
using AuditDataPlatform.Application.Reporting;
using AuditDataPlatform.Application.RuleEngine;
using AuditDataPlatform.Domain.RuleEngine;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using AuditDataPlatform.Infrastructure.FileParsing;
using AuditDataPlatform.Infrastructure.Persistence;
using AuditDataPlatform.Infrastructure.Reporting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IRuleEvaluator, RequiredFieldEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, DataFormatEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, DuplicateEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, RangeThresholdEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, CrossFieldConsistencyEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, PatternMatchEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, StatisticalOutlierEvaluator>();
builder.Services.AddScoped<IRuleEngineOrchestrator, RuleEngineOrchestrator>();

builder.Services.AddSingleton<IFileParser, CsvFileParser>();
builder.Services.AddSingleton<IFileParser, XlsxFileParser>();

builder.Services.AddSingleton<IXlsxReportGenerator, XlsxReportGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCreateRuleSet();
app.MapGetRuleSets();
app.MapGetRuleSetById();
app.MapAddRule();
app.MapUpdateRule();
app.MapDeleteRule();

app.Run();