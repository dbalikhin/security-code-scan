// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using Analyzer.Utilities.PooledObjects;

namespace Analyzer.Utilities.FlowAnalysis.Analysis.TaintedDataAnalysis
{
    internal static class RedirectSanitizers
    {
        /// <summary>
        /// <see cref="SanitizerInfo"/>s for Open Redirect injection sanitizers.
        /// </summary>
        public static ImmutableHashSet<SanitizerInfo> SanitizerInfos { get; }

        static RedirectSanitizers()
        {
            var builder = PooledHashSet<SanitizerInfo>.GetInstance();

            builder.AddSanitizerInfo(
                WellKnownTypeNames.SystemWebMvcUrlHelper,
                isInterface: false,
                isConstructorSanitizing: false,
                sanitizingMethods: new (MethodMatcher, (string taintedArgument, string sanitizedArgument)[])[] {
                    (
                        (methodName, arguments) => methodName == "RouteUrl",
                        new[] { ("routeContext", TaintedTargetValue.Return) }
                    ),
                });
            builder.AddSanitizerInfo(
                WellKnownTypeNames.MicrosoftAspNetCoreMvcUrlHelperExtensions,
                isInterface: false,
                isConstructorSanitizing: false,
                sanitizingMethods: new (MethodMatcher, (string taintedArgument, string sanitizedArgument)[])[] {
                    (
                        (methodName, arguments) => methodName == "RouteUrl",
                        new[] { ("routeContext", TaintedTargetValue.Return) }
                    ),
                });
            builder.AddSanitizerInfo(
                "Microsoft.AspNetCore.Mvc.IUrlHelper",
                isInterface: true,
                isConstructorSanitizing: false,
                sanitizingMethods: new (MethodMatcher, (string taintedArgument, string sanitizedArgument)[])[] {
                    (
                        (methodName, arguments) => methodName == "IsLocalUrl",
                        new[] { ("url", "url") }
                    ),
                });

            SanitizerInfos = builder.ToImmutableAndFree();
        }
    }
}
