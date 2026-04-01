

global using System;
global using System.IO;
global using System.Linq.Expressions;
global using System.Net;
global using System.Security.Claims;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.ComponentModel.DataAnnotations;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Abstractions;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.Data.SqlClient;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Identity.Web;
global using Microsoft.Extensions.Http.Resilience;

global using Asp.Versioning;
global using Azure.Identity;
global using Azure.Monitor.OpenTelemetry.AspNetCore;
global using OpenTelemetry.Trace;
global using Polly;
global using Polly.Retry;

global using Common.Rest.Shared.Models;
global using Common.Rest.Shared.Extensions;
global using Common.Rest.Shared.Exceptions;
global using Common.Rest.Shared.Repository;
global using Common.Rest.Shared.Middleware;
global using Common.Rest.Shared.Health;
global using Common.Rest.Shared.Resilience;
global using Common.Rest.Shared.Domain;
global using Common.Rest.Shared.Persistence;

