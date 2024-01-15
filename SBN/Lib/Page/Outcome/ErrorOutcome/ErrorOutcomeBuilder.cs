using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SBN.Lib.Page.Outcome.ErrorOutcome
{
    public class ErrorOutcomeBuilder : IErrorOutcomeBuilder
    {
        private readonly IEnumerable<IErrorOutcomeBuilderStrategy> _errorOutcomeBuilderStrategies;

        public ErrorOutcomeBuilder(IEnumerable<IErrorOutcomeBuilderStrategy> errorOutcomeBuilderStrategies)
        {
            _errorOutcomeBuilderStrategies = errorOutcomeBuilderStrategies;
        }

        public IActionResult Build(HttpStatusCode statusCode, HttpContext context)
        {
            return _errorOutcomeBuilderStrategies
                .Single(x => x.Matches(statusCode))
                .Build(context, statusCode);
        }
    }
}
