// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using NodaTime.Benchmarks;
using NodaTime.Web.Models;
using NodaTime.Web.ViewModels;
using System.Linq;

namespace NodaTime.Web.Controllers
{
    public class BenchmarksController : Controller
    {
        private readonly IBenchmarkRepository repository;

        public BenchmarksController(IBenchmarkRepository repository)
        {
            this.repository = repository;
        }

        [Route("/benchmarks")]
        public IActionResult Index()
        {
            var environments = repository.ListEnvironments();
            return View(environments);
        }

        [Route("/benchmarks/environments/{id}")]
        public IActionResult ViewEnvironment(string id)
        {
            var environment = repository.GetEnvironment(id);
            return View(environment);
        }

        [Route("/benchmarks/runs/{runId}")]
        public IActionResult ViewRun(string runId)
        {
            var run = repository.GetRun(runId);
            var env = repository.GetEnvironment(run.BenchmarkEnvironmentId);
            return View((env, run));
        }

        [Route("/benchmarks/types/{typeId}")]
        public IActionResult ViewType(string typeId)
        {
            var tuple = LookupTypeAndAncestors(typeId);
            var previousRun = GetPreviousRun(tuple.run);
            var previousRunType = previousRun?.Types_.FirstOrDefault(t => t.FullTypeName == tuple.type.FullTypeName);
            return View((tuple.environment, tuple.run, tuple.type, previousRunType));
        }

        [Route("/benchmarks/types/{leftTypeId}/compare/{rightTypeId}")]
        public IActionResult CompareTypes(string leftTypeId, string rightTypeId)
        {
            var left = LookupTypeAndAncestors(leftTypeId);
            var right = LookupTypeAndAncestors(rightTypeId);
            return View(new CompareTypesViewModel(left, right));
        }

        [Route("/benchmarks/benchmarks/{benchmarkId}")]
        public IActionResult ViewBenchmark(string benchmarkId)
        {
            var benchmark = repository.GetBenchmark(benchmarkId);
            var type = repository.GetType(benchmark.BenchmarkTypeId);
            var run = repository.GetRun(type.BenchmarkRunId);
            var env = repository.GetEnvironment(run.BenchmarkEnvironmentId);
            return View((env, run, type, benchmark));
        }

        private (BenchmarkEnvironment environment, BenchmarkRun run, BenchmarkType type) LookupTypeAndAncestors(string typeId)
        {
            var type = repository.GetType(typeId);
            var run = repository.GetRun(type.BenchmarkRunId);
            var environment = repository.GetEnvironment(run.BenchmarkEnvironmentId);
            return (environment, run, type);
        }

        private BenchmarkRun GetPreviousRun(BenchmarkRun run) =>
            repository.GetEnvironment(run.BenchmarkEnvironmentId).Runs
                .SkipWhile(r => r.BenchmarkRunId != run.BenchmarkRunId)
                .Skip(1)
                .FirstOrDefault();            
    }
}
