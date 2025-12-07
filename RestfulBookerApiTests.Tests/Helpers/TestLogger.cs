using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RestfulBookerApiTests.Tests.Helpers
{
    public enum WarningSeverity
    {
        Info,
        Warning,
        Critical
    }

    public class TestWarning
    {
        public WarningSeverity Severity { get; set; }
        public string Message { get; set; }
        public string TestName { get; set; }
    }

    public static class TestLogger
    {
        private static readonly List<TestWarning> _warnings = new List<TestWarning>();

        public static void Info(string message)
        {
            LogWarning(WarningSeverity.Info, message);
        }

        public static void Warning(string message)
        {
            LogWarning(WarningSeverity.Warning, message);
        }

        public static void Critical(string message)
        {
            LogWarning(WarningSeverity.Critical, message);
        }

        private static void LogWarning(WarningSeverity severity, string message)
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var warning = new TestWarning
            {
                Severity = severity,
                Message = message,
                TestName = testName
            };

            _warnings.Add(warning);

            var icon = GetSeverityIcon(severity);
            var severityText = severity.ToString().ToUpper();
            var logMessage = $"{icon} {severityText} [{severityText}] {message}";

            Console.WriteLine(logMessage);
            TestContext.WriteLine(logMessage);
        }

        private static string GetSeverityIcon(WarningSeverity severity)
        {
            return severity switch
            {
                WarningSeverity.Info => "ℹ️",
                WarningSeverity.Warning => "⚠️",
                WarningSeverity.Critical => "🔴",
                _ => ""
            };
        }

        public static List<TestWarning> GetAllWarnings()
        {
            return _warnings.ToList();
        }

        public static List<TestWarning> GetWarningsBySeverity(WarningSeverity severity)
        {
            return _warnings.Where(w => w.Severity == severity).ToList();
        }

        public static void ClearWarnings()
        {
            _warnings.Clear();
        }

        public static string GetSummary()
        {
            var criticalCount = _warnings.Count(w => w.Severity == WarningSeverity.Critical);
            var warningCount = _warnings.Count(w => w.Severity == WarningSeverity.Warning);
            var infoCount = _warnings.Count(w => w.Severity == WarningSeverity.Info);

            return $"Test Warnings Summary: {criticalCount} Critical, {warningCount} Warnings, {infoCount} Info";
        }
    }
}
