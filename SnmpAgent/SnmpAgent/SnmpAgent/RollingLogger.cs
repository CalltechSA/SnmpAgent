using SharpSnmpLib;
using SharpSnmpLib.Messaging;
using SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SnmpAgent
{
    internal class RollingLogger : ILogger
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string Empty = "-";

        public RollingLogger()
        {
            if (!Logger.IsInfoEnabled)
                return;

            Logger.Info($"#Software: #SNMP Calltech Agent {System.Reflection.Assembly.GetEntryAssembly().GetName().Version}");
            Logger.Info($"#Version: {System.Reflection.Assembly.GetEntryAssembly().GetName().Version}");
            Logger.Info($"#Date: {DateTime.UtcNow}");
            Logger.Info("#Fields: date time s-ip cs-method cs-uri-stem s-port cs-username c-ip sc-status cs-version time-taken");
        }

        public void Log(ISnmpContext context)
        {
            if (Logger.IsInfoEnabled)
                Logger.Info(GetLogEntry(context));
        }

        private static string GetLogEntry(ISnmpContext context) =>
            $"{DateTime.UtcNow} " +
            $"{context.Binding.Endpoint.Address} " +
            $"{(context.Request.TypeCode() == SnmpType.Unknown ? Empty : context.Request.TypeCode().ToString())} " +
            $"{GetStem(context.Request.Pdu().Variables)} " +
            $"{context.Binding.Endpoint.Port} " +
            $"{context.Request.Parameters.UserName} " +
            $"{context.Sender.Address} " +
            $"{((context.Response == null) ? Empty : context.Response.Pdu().ErrorStatus.ToErrorCode().ToString())} " +
            $"{context.Request.Version} " +
            $"{DateTime.Now.Subtract(context.CreatedTime).TotalMilliseconds}";

        private static string GetStem(ICollection<Variable> variables)
        {
            if (variables.Count == 0)
                return Empty;

            StringBuilder result = new StringBuilder();
            foreach (Variable v in variables)
                result.AppendFormat("{0};", v.Id);

            if (result.Length > 0)
                result.Length--;

            return result.ToString();
        }
    }
}