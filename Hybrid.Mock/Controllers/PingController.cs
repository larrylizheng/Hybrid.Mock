using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace Hybrid.Mock.Controllers;
[ExcludeFromCodeCoverage]
[Route("api/[controller]")]
public class PingController : Controller
{
    public string Get() => "pong";
}
